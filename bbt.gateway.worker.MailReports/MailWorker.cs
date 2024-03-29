using bbt.gateway.common;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Models;
using Elastic.Apm.Api;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Collections.Concurrent;

namespace bbt.gateway.worker.MailReports
{
    public class MailWorker : BackgroundService
    {
        private readonly IMessagingGatewayApi _messagingGatewayApi;
        private readonly ITracer _tracer;
        private readonly LogManager _logManager;
        private readonly DatabaseContext _dbContext;
        private IHostApplicationLifetime _hostApplicationLifetime;
        public MailWorker(LogManager logManager, ITracer tracer,
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
            await Task.Delay(5000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logManager.LogInformation("Mail Tracking Triggered");
            try
            {
                await _tracer.CaptureTransaction("Mail Tracking", ApiConstants.TypeRequest, async () =>
                {
                try
                {
                    var endDate = DateTime.Now.AddMinutes(-5);
                    var startDate = endDate.AddMinutes(-60);
                    var mailResponseLogs = await _dbContext.MailRequestLog.AsNoTracking().Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                            .Include(m => m.ResponseLogs.Where(r => r.ResponseCode == "0" && string.IsNullOrWhiteSpace(r.Status))).ToListAsync();
                    

                        _logManager.LogInformation("Mail Count : " + mailResponseLogs.Count);

                        ConcurrentBag<MailEntitiesToBeProcessed> concurrentBag = new();

                        var dividedList = mailResponseLogs.DivideListIntoParts(50);
                        foreach (List<MailRequestLog> mailResponseLogsParts in dividedList)
                        {
                            _logManager.LogInformation("Part Count : " + mailResponseLogsParts.Count);
                            var taskList = new List<Task>();
                            mailResponseLogsParts.ForEach(mailResponseLog =>
                            {
                                if (mailResponseLog.ResponseLogs.Count > 0)
                                {
                                    try
                                    {
                                        var rLog = mailResponseLog.ResponseLogs.FirstOrDefault();
                                        rLog.Operator = mailResponseLog.Operator;
                                        taskList.Add(GetDeliveryStatus(rLog, concurrentBag));
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    
                                }
                            });
                            await Task.WhenAll(taskList);
                        }

                        foreach (var entities in concurrentBag)
                        {
                            if (entities.mailTrackingLog != null)
                            {
                                await _dbContext.MailTrackingLog.AddAsync(entities.mailTrackingLog);
                            }
                            if (entities.mailResponseLog != null)
                            {
                                _dbContext.MailResponseLog.Update(entities.mailResponseLog);
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
            _logManager.LogInformation("Mail Tracking Finished");
            _hostApplicationLifetime.StopApplication();
        }

        private async Task GetDeliveryStatus(MailResponseLog mailResponseLog, ConcurrentBag<MailEntitiesToBeProcessed> concurrentBag)
        {
            try
            {
                MailEntitiesToBeProcessed entitiesToBeProcessed = new();
                var response = await _messagingGatewayApi.CheckMailStatus(new common.Models.v2.CheckMailStatusRequest
                {
                    Operator = mailResponseLog.Operator,
                    MailRequestLogId = mailResponseLog.Id,
                    StatusQueryId = mailResponseLog.StatusQueryId
                });

                entitiesToBeProcessed.mailTrackingLog = response;

                if (response.Status != MailTrackingStatus.Pending)
                {
                    mailResponseLog.Status = response.Status.ToString();
                    entitiesToBeProcessed.mailResponseLog = mailResponseLog;
                }

                concurrentBag.Add(entitiesToBeProcessed);
            }
            catch (ApiException ex)
            {
                _logManager.LogError($"Messaging Gateway Api Error | Status Code : {ex.StatusCode} | Detail : Operator => {mailResponseLog.Operator}, MailResponseLogId => {mailResponseLog.Id}, StatusQueryId => {mailResponseLog.StatusQueryId}");
            }
            catch (Exception ex)
            {
                _logManager.LogError($"Messaging Gateway Worker Error | Error : {ex.Message}");
            }
        }
    }

    public class MailEntitiesToBeProcessed
    {
        public MailResponseLog mailResponseLog { get; set; }
        public MailTrackingLog mailTrackingLog { get; set; }
    }

}