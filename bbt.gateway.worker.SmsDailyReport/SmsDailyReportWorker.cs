using bbt.gateway.common;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using Dapr.Client;
using Elastic.Apm.Api;
using Microsoft.EntityFrameworkCore;

namespace bbt.gateway.worker.SmsDailyReport
{
    public class SmsDailyReportWorker : BackgroundService
    {
        private readonly ITracer _tracer;
        private readonly LogManager _logManager;
        private IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private readonly DaprClient _daprClient;

        public SmsDailyReportWorker(LogManager logManager,ITracer tracer,
            IRepositoryManager repositoryManager,
            IHostApplicationLifetime hostApplicationLifetime,IConfiguration configuration,DaprClient daprClient)
        {
            _logManager = logManager;
            _tracer = tracer;
            _repositoryManager = repositoryManager;
            _hostApplicationLifetime = hostApplicationLifetime;
            _configuration = configuration;
            _daprClient = daprClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logManager.LogInformation("Sms Daily Report Triggered");
            try
            {
                await _tracer.CaptureTransaction("Sms Daily Report", ApiConstants.TypeRequest, async () =>
                {
                    try
                    {
                        DateTime startDate = DateTime.Now.AddDays(-1);
                        var isFirstRun = _configuration["isFirstRun"];
                        if (isFirstRun.Equals("E"))
                        {
                            startDate = new DateTime(DateTime.Now.Year,1,1);
                        }
                        _logManager.LogInformation("First Run : "+isFirstRun);
                        foreach (var item in GlobalConstants.reportOperators)
                        {
                            OperatorReportInfo operatorReportInfo = item.Value;
                            for (var dt = startDate.Date; dt < DateTime.Now.Date; dt = dt.AddDays(1))
                            {
                                var res = await GetOperatorInfo(dt, dt.AddDays(1), operatorReportInfo.OperatorType, operatorReportInfo.isOtp, operatorReportInfo.isFast);
                                while (operatorReportInfo.AdditionalOperatorType != null)
                                {
                                    operatorReportInfo = operatorReportInfo.AdditionalOperatorType;
                                    res += await GetOperatorInfo(dt, dt.AddDays(1), operatorReportInfo.OperatorType, operatorReportInfo.isOtp, operatorReportInfo.isFast);
                                }
                                var key = GlobalConstants.SMS_DAILY_REPORT + "_" + item.Key + "_" + dt.ToShortDateString() + "_" + dt.AddDays(1).ToShortDateString();
                                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,key , res);
                                _logManager.LogInformation($"{key} saved");
                            }
                        }
                        

                    }
                    catch (Exception ex)
                    {
                        _logManager.LogError(ex.ToString());
                        _tracer.CaptureException(ex);
                        _hostApplicationLifetime.StopApplication();
                    }

                });


            }
            catch (Exception ex)
            {
                _logManager.LogError(ex.ToString());
                _hostApplicationLifetime.StopApplication();
            }
            _logManager.LogInformation("Sms Daily Report Finished");
            _hostApplicationLifetime.StopApplication();
        }

        private async Task<OperatorReport> GetOperatorInfo(DateTime startDate, DateTime endDate, OperatorType @operator, bool isOtp, bool isFast)
        {
            var operatorReport = new OperatorReport();
            operatorReport.Operator = @operator;
            if (isOtp)
            {

                try
                {
                    operatorReport.OtpCount = await _repositoryManager.Transactions.GetSuccessfullOtpCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.ForeignOtpCount = await _repositoryManager.Transactions.GetSuccessfullForeignOtpCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullOtpRequestCount = await _repositoryManager.Transactions.GetOtpRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullOtpRequestCount = await _repositoryManager.Transactions.GetOtpRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullForeignOtpRequestCount = await _repositoryManager.Transactions.GetForeignOtpRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullForeignOtpRequestCount = await _repositoryManager.Transactions.GetForeignOtpRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            if (isFast)
            {
                try
                {
                    operatorReport.FastCount = await _repositoryManager.Transactions.GetSuccessfullSmsCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.ForeignFastCount = await _repositoryManager.Transactions.GetSuccessfullForeignSmsCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullFastRequestCount = await _repositoryManager.Transactions.GetSmsRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullFastRequestCount = await _repositoryManager.Transactions.GetSmsRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullForeignFastRequestCount = await _repositoryManager.Transactions.GetForeignSmsRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullForeignFastRequestCount = await _repositoryManager.Transactions.GetForeignSmsRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return operatorReport;
        }
    }


}