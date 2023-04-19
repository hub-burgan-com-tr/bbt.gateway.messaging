using bbt.gateway.messaging.Api.Pusula.Model.GetByPhone;
using bbt.gateway.messaging.Api.Pusula.Model.GetCustomer;
using bbt.gateway.messaging.Api.Pusula.Model.GetByCitizenshipNumber;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace bbt.gateway.messaging.Api.Pusula
{
    public class PusulaClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PusulaClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("Api:Pusula:BaseAddress"));
        }

        public async Task<GetByCitizenshipNumberResponse> GetCustomerByCitizenshipNumber(GetByCitizenshipNumberRequest getByCitizenshipNumberRequest)
        {
            GetByCitizenshipNumberResponse getByCitizenshipNumberResponse = new();
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"citizenshipNo", getByCitizenshipNumberRequest.CitizenshipNumber}
                };

                var httpResponse = await _httpClient.GetAsync(
                    QueryHelpers.AddQueryString(_configuration.GetValue<string>("Api:Pusula:EndPoints:GetByCitizenship"), queryParams));


                if (httpResponse.IsSuccessStatusCode)
                {
                    var response = httpResponse.Content.ReadAsStringAsync().Result;
                    var customerNo = response.
                        GetWithRegexSingle("(<ExternalClientNo[^>]*>)(.*?)(</ExternalClientNo>)", 2);
                    if (!string.IsNullOrEmpty(customerNo))
                    {
                        getByCitizenshipNumberResponse.IsSuccess = true;
                        getByCitizenshipNumberResponse.CustomerNo = (ulong)Convert.ToInt64(customerNo);
                    }
                    else
                    {
                        getByCitizenshipNumberResponse.IsSuccess = false;
                    }
                }
                else
                {
                    getByCitizenshipNumberResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                getByCitizenshipNumberResponse.IsSuccess = false;
            }

            return getByCitizenshipNumberResponse;
        }

        public async Task<GetByPhoneNumberResponse> GetCustomerByPhoneNumber(GetByPhoneNumberRequest getByPhoneNumberRequest)
        {
            GetByPhoneNumberResponse getByPhoneNumberResponse = new();
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"CountryCode", getByPhoneNumberRequest.CountryCode},
                    {"CityCode",getByPhoneNumberRequest.CityCode},
                    {"TelephoneNumber",getByPhoneNumberRequest.TelephoneNumber}
                };

                var httpResponse = await _httpClient.GetAsync(
                    QueryHelpers.AddQueryString(_configuration.GetValue<string>("Api:Pusula:EndPoints:GetByPhoneNumber"), queryParams));


                if (httpResponse.IsSuccessStatusCode)
                {
                    var response = httpResponse.Content.ReadAsStringAsync().Result.DeserializeXml<DataTable>();
                    if (response.diffgram.DocumentElement != null)
                    {
                        getByPhoneNumberResponse.IsSuccess = true;
                        getByPhoneNumberResponse.CustomerNo = response.diffgram.DocumentElement[0].CustomerNumber;
                    }
                    else
                    {
                        getByPhoneNumberResponse.IsSuccess = false;
                    }
                }
                else
                {
                    getByPhoneNumberResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                getByPhoneNumberResponse.IsSuccess = false;
            }

            return getByPhoneNumberResponse;
        }

        public async Task<GetByEmailResponse> GetCustomerByEmail(GetByEmailRequest getByEmailRequest)
        {
            GetByEmailResponse getByEmailResponse = new();
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"eMail", getByEmailRequest.Email}
                };

                var httpResponse = await _httpClient.GetAsync(
                    QueryHelpers.AddQueryString(_configuration.GetValue<string>("Api:Pusula:EndPoints:GetByEmail"), queryParams));


                if (httpResponse.IsSuccessStatusCode)
                {
                    var customerNo = (long)System.Xml.Linq.XElement.Parse(httpResponse.Content.ReadAsStringAsync().Result);
                    if (customerNo > 0 && customerNo != 55)
                    {
                        getByEmailResponse.IsSuccess = true;
                        getByEmailResponse.CustomerNo = (ulong)customerNo;
                    }
                    else
                    {
                        getByEmailResponse.IsSuccess = false;
                        //logging
                    }
                }
                else
                {
                    getByEmailResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                getByEmailResponse.IsSuccess = false;
            }

            return getByEmailResponse;
        }

        public async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest getCustomerRequest)
        {
            GetCustomerResponse getCustomerResponse = new();
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    {"custNo", getCustomerRequest.CustomerNo.ToString()},
                };
                var httpResponse = await _httpClient.GetAsync(
                    QueryHelpers.AddQueryString(_configuration.GetValue<string>("Api:Pusula:EndPoints:GetCustomer"), queryParams));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var httpContent = httpResponse.Content.ReadAsStringAsync().Result;

                    var customerIndividual = httpContent.
                        GetWithRegexSingle("(<CustomerIndividual[^>]*>)(.*?)(</CustomerIndividual>)", 2);
                    var customerPhones = httpContent.
                        GetWithRegexMultiple("(<Telephones[^>]*>)(.*?)(</Telephones>)", 2);
                    var customerMails = httpContent.
                        GetWithRegexMultiple("(<Emails[^>]*>)(.*?)(</Emails>)", 2);
                    if (!string.IsNullOrEmpty(customerIndividual))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml("<root>" + customerIndividual + "</root>");
                        var serializedJson = JsonConvert.SerializeXmlNode(xmlDocument);
                        var pusulaCustomerInfo = JsonConvert.DeserializeObject<PusulaCustomerRoot>(serializedJson);

                        if (!string.IsNullOrEmpty(pusulaCustomerInfo.root.BusinessLine))
                        {
                            getCustomerResponse.IsSuccess = true;
                            getCustomerResponse.BranchCode = pusulaCustomerInfo.root.MainBranchCode;
                            getCustomerResponse.BusinessLine = pusulaCustomerInfo.root.BusinessLine;
                            getCustomerResponse.CitizenshipNo = pusulaCustomerInfo.root.CitizenshipNumber;
                        }
                        else
                        {
                            getCustomerResponse.IsSuccess = true;
                            getCustomerResponse.BusinessLine = "B";
                            getCustomerResponse.BranchCode = pusulaCustomerInfo.root.MainBranchCode;
                            getCustomerResponse.CitizenshipNo = pusulaCustomerInfo.root.CitizenshipNumber;
                        }
                    }
                    else
                    {
                        getCustomerResponse.IsSuccess = false;
                    }

                    if (customerPhones.Count > 0)
                    {
                        foreach (var phone in customerPhones)
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml("<root>" + phone + "</root>");
                            var serializedJson = JsonConvert.SerializeXmlNode(xmlDocument);
                            var pusulaPhoneInfo = JsonConvert.DeserializeObject<PusulaPhoneRoot>(serializedJson);
                            if (pusulaPhoneInfo.root.TelephoneType == 3)
                            {
                                getCustomerResponse.MainPhone.CountryCode = pusulaPhoneInfo.root.CountryCode;
                                getCustomerResponse.MainPhone.Prefix = pusulaPhoneInfo.root.AreaCode;
                                getCustomerResponse.MainPhone.Number = pusulaPhoneInfo.root.TelephoneNumber;
                            }
                        }
                    }

                    if (customerMails.Count > 0)
                    {
                        foreach (var mail in customerMails)
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml("<root>" + mail + "</root>");
                            var serializedJson = JsonConvert.SerializeXmlNode(xmlDocument);
                            var pusulaMailInfo = JsonConvert.DeserializeObject<PusulaMailRoot>(serializedJson);
                            if (pusulaMailInfo.root.IsVerified == "Evet")
                            {
                                getCustomerResponse.VerifiedMailAdresses.Add((pusulaMailInfo.root.Email));
                            }
                            if (pusulaMailInfo.root.EmailType == 1)
                            {
                                getCustomerResponse.MainEmail = pusulaMailInfo.root.Email;
                            }
                        }
                    }
                }
                else
                {
                    getCustomerResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                getCustomerResponse.IsSuccess = false;
            }

            return getCustomerResponse;
        }
    }
}
