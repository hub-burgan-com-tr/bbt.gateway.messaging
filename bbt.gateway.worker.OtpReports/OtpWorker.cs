using bbt.gateway.common;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Models;
using Elastic.Apm.Api;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Collections.Concurrent;

namespace bbt.gateway.worker.OtpReports
{
    public class OtpWorker : BackgroundService
    {
        private readonly IMessagingGatewayApi _messagingGatewayApi;
        private readonly ITracer _tracer;
        private readonly LogManager _logManager;
        private readonly DatabaseContext _dbContext;
        private IHostApplicationLifetime _hostApplicationLifetime;
        public OtpWorker(LogManager logManager, ITracer tracer,
            IMessagingGatewayApi messagingGatewayApi, DbContextOptions<DatabaseContext> dbContextOptions,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logManager = logManager;
            _tracer = tracer;
            _messagingGatewayApi = messagingGatewayApi;
            _dbContext = new DatabaseContext(dbContextOptions);
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(30000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logManager.LogInformation("Otp Tracking Triggered");
            try
            {
                await _tracer.CaptureTransaction("Otp Tracking", ApiConstants.TypeRequest, async () =>
                {
                    try
                    {
                        var endDate = DateTime.Now.AddMinutes(-5);
                        var startDate = endDate.AddHours(-1.5);
                        var otpResponseLogs = await _dbContext.OtpResponseLog.
                        FromSqlRaw("Select * from OtpResponseLog (NOLOCK) WHERE ResponseCode = 200 AND CreatedAt Between {0} AND {1} AND TrackingStatus = 462", startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"))
                        .AsNoTracking().ToListAsync();

                        _logManager.LogInformation("Otp Count : " + otpResponseLogs.Count);

                        ConcurrentBag<OtpEntitiesToBeProcessed> concurrentBag = new();

                        var dividedList = otpResponseLogs.DivideListIntoParts(50);
                        foreach (List<OtpResponseLog> otpResponseLogsParts in dividedList)
                        {
                            _logManager.LogInformation("Part Count : " + otpResponseLogsParts.Count);
                            var taskList = new List<Task>();
                            otpResponseLogsParts.ForEach(otpResponseLog =>
                            {
                                taskList.Add(GetDeliveryStatus(otpResponseLog, concurrentBag));
                            });
                            await Task.WhenAll(taskList);
                        }

                        foreach (var entities in concurrentBag)
                        {
                            if (entities.otpTrackingLog != null)
                            {
                                await _dbContext.OtpTrackingLog.AddAsync(entities.otpTrackingLog);
                            }
                            if (entities.otpResponseLog != null)
                            {
                                _dbContext.OtpResponseLog.Update(entities.otpResponseLog);
                            }
                            await _dbContext.SaveChangesAsync();
                        }

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
            _logManager.LogInformation("Otp Tracking Finished");
            _hostApplicationLifetime.StopApplication();
        }

        private async Task GetDeliveryStatus(OtpResponseLog otpResponseLog, ConcurrentBag<OtpEntitiesToBeProcessed> concurrentBag)
        {
            try
            {
                OtpEntitiesToBeProcessed entitiesToBeProcessed = new();
                var response = await _messagingGatewayApi.CheckOtpStatus(new CheckSmsRequest
                {
                    Operator = otpResponseLog.Operator,
                    OtpRequestLogId = otpResponseLog.Id,
                    StatusQueryId = otpResponseLog.StatusQueryId
                });

                entitiesToBeProcessed.otpTrackingLog = response;

                if (response.Status != SmsTrackingStatus.Pending)
                {
                    otpResponseLog.TrackingStatus = response.Status;
                    entitiesToBeProcessed.otpResponseLog = otpResponseLog;
                }

                concurrentBag.Add(entitiesToBeProcessed);
            }
            catch (ApiException ex)
            {
                _logManager.LogError($"Messaging Gateway Api Error | Status Code : {ex.StatusCode} | Detail : Operator => {otpResponseLog.Operator}, SmsResponseLogId => {otpResponseLog.Id}, StatusQueryId => {otpResponseLog.StatusQueryId}");
            }
            catch (Exception ex)
            {
                _logManager.LogError($"Messaging Gateway Worker Error | Error : {ex.Message}");
            }
        }
    }

    public class OtpEntitiesToBeProcessed
    {
        public OtpResponseLog otpResponseLog { get; set; }
        public OtpTrackingLog otpTrackingLog { get; set; }
    }
}