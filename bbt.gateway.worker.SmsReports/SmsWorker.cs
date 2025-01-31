using bbt.gateway.common;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Models;
using Elastic.Apm.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Refit;
using System.Collections.Concurrent;

namespace bbt.gateway.worker.SmsReports
{
    public class SmsWorker : BackgroundService
    {
        private const string PROCESS_NO_CACHE_KEY = "bbt_gateway_worker_sms_reports_process_no";
        private const string _reportTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly IMessagingGatewayApi _messagingGatewayApi;
        private readonly ITracer _tracer;
        private readonly LogManager _logManager;
        private readonly DatabaseContext _dbContext;
        private IHostApplicationLifetime _hostApplicationLifetime;
        private IDistributedCache _distributedCache;
        private IConfiguration _configuration;

        public SmsWorker(LogManager logManager, ITracer tracer,
            IMessagingGatewayApi messagingGatewayApi, DbContextOptions<DatabaseContext> dbContextOptions,
            IHostApplicationLifetime hostApplicationLifetime, IDistributedCache distributedCache, IConfiguration configuration
            )
        {
            _logManager = logManager;
            _tracer = tracer;
            _messagingGatewayApi = messagingGatewayApi;
            _dbContext = new DatabaseContext(dbContextOptions);
            _hostApplicationLifetime = hostApplicationLifetime;
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logManager.LogInformation("Sms Tracking Triggered");

            try
            {
                await _tracer.CaptureTransaction("Sms Tracking", ApiConstants.TypeRequest, async () =>
                {
                    try
                    {
                        var workerCount = Convert.ToInt32(_configuration["WorkerCount"]);

                        var lastDay = DateTime.Now.AddDays(-1).Date;
                        var today = DateTime.Today;

                        var cacheKey = PROCESS_NO_CACHE_KEY + "_" + lastDay.ToString("dd_MM_yyyy");

                        int currentProcessIndex;
                        var processOrderFromCache = _distributedCache.GetString(cacheKey);

                        currentProcessIndex = processOrderFromCache is null ? 1 : Convert.ToInt32(processOrderFromCache) + 1;

                        await _distributedCache.SetStringAsync(cacheKey,
                                                               currentProcessIndex.ToString(),
                                                               new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(24) }
                                                               );

                        string? reportStartDate;
                        string? reportEndDate;

                        if (currentProcessIndex <= workerCount)
                        {
                            var dateRanges = new Dictionary<int, DataDateRange>();
                            var hourRange = 24 / workerCount;

                            for (int i = 0; i < workerCount - 1; i++)
                            {
                                dateRanges.Add(i + 1, new DataDateRange()
                                {
                                    startDate = lastDay.AddHours(hourRange * i),
                                    endDate = lastDay.AddHours(hourRange * (i + 1))
                                });
                            }

                            //Add Last Part
                            dateRanges.Add(workerCount, new DataDateRange()
                            {
                                startDate = dateRanges[workerCount - 1].endDate,
                                endDate = today
                            });

                            reportStartDate = dateRanges[currentProcessIndex].startDate.ToString(_reportTimeFormat);
                            reportEndDate = dateRanges[currentProcessIndex].endDate.ToString(_reportTimeFormat);
                        }
                        else
                        {
                            reportStartDate = lastDay.ToString(_reportTimeFormat);
                            reportEndDate = today.ToString(_reportTimeFormat);
                        }

                        _logManager.LogInformation("Current Process Index : " + currentProcessIndex);
                        _logManager.LogInformation("Start Date : " + reportStartDate);
                        _logManager.LogInformation("End Date : " + reportEndDate);

                        var smsResponseLogs = await _dbContext.SmsResponseLog.
                        FromSqlRaw("Select * from SmsResponseLog (NOLOCK) " +
                                    "WHERE OperatorResponseCode = 0 " +
                                    "AND CreatedAt Between {0} " + "AND {1} " +
                                    "AND (status is null OR status = '')",
                                     reportStartDate,
                                     reportEndDate)
                        .AsNoTracking().ToListAsync();

                        _logManager.LogInformation("Sms Count : " + smsResponseLogs.Count);

                        ConcurrentBag<SmsEntitiesToBeProcessed> concurrentBag = new();

                        var dividedList = smsResponseLogs.DivideListIntoParts(50);

                        foreach (List<SmsResponseLog> smsResponseLogsParts in dividedList)
                        {
                            _logManager.LogInformation("Part Count : " + smsResponseLogsParts.Count);

                            var taskList = new List<Task>();

                            smsResponseLogsParts.ForEach(smsResponseLog =>
                            {
                                taskList.Add(GetDeliveryStatusAsync(smsResponseLog, concurrentBag));
                            });

                            await Task.WhenAll(taskList);
                        }

                        foreach (var entities in concurrentBag)
                        {
                            try
                            {
                                if (entities.smsTrackingLog != null)
                                {
                                    await _dbContext.SmsTrackingLog.AddAsync(entities.smsTrackingLog);
                                }

                                if (entities.smsResponseLog != null)
                                {
                                    _dbContext.SmsResponseLog.Update(entities.smsResponseLog);
                                }
                            }
                            catch (Exception)
                            {
                                _logManager.LogError("Messaging Gateway Worker Error | Db Context Error");
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await _dbContext.DisposeAsync();

                        _logManager.LogError(ex.ToString());
                        _tracer.CaptureException(ex);

                        _hostApplicationLifetime.StopApplication();
                    }
                });
            }
            catch (Exception ex)
            {
                _logManager.LogError(ex.ToString());

                await _dbContext.DisposeAsync();

                _hostApplicationLifetime.StopApplication();
            }

            _logManager.LogInformation("Sms Tracking Finished");
            _hostApplicationLifetime.StopApplication();
        }

        private async Task GetDeliveryStatusAsync(SmsResponseLog smsResponseLog, ConcurrentBag<SmsEntitiesToBeProcessed> concurrentBag)
        {
            try
            {
                SmsEntitiesToBeProcessed entitiesToBeProcessed = new();

                var response = await _messagingGatewayApi.CheckSmsStatus(new common.Models.v2.CheckFastSmsRequest
                {
                    Operator = smsResponseLog.Operator,
                    SmsRequestLogId = smsResponseLog.Id,
                    StatusQueryId = smsResponseLog.StatusQueryId
                });

                entitiesToBeProcessed.smsTrackingLog = response;

                if (response.Status != SmsTrackingStatus.Pending)
                {
                    smsResponseLog.Status = response.Status.ToString();
                    entitiesToBeProcessed.smsResponseLog = smsResponseLog;
                }

                concurrentBag.Add(entitiesToBeProcessed);
            }
            catch (ApiException ex)
            {
                _logManager.LogError($"Messaging Gateway Api Error | Status Code : {ex.StatusCode} | Detail : Operator => {smsResponseLog.Operator}, SmsResponseLogId => {smsResponseLog.Id}, StatusQueryId => {smsResponseLog.StatusQueryId}");
            }
            catch (Exception ex)
            {
                _logManager.LogError($"Messaging Gateway Worker Error | Error : {ex.Message}");
            }
        }
    }

    public class SmsEntitiesToBeProcessed
    {
        public SmsResponseLog smsResponseLog { get; set; }
        public SmsTrackingLog smsTrackingLog { get; set; }
    }

    public class DataDateRange
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}