using bbt.gateway.common.Models.v2;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.messaging
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        private readonly IConfiguration _configuration;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            this.provider = provider;
            this._configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // add swagger document for every API version discovered
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }

        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Bbt.Messaging.Gateway",
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += "This API version has been deprecated.";
            }

            return info;
        }
    }

    public class OrderTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = swaggerDoc.Tags
                 .OrderBy(x => x.Name).ToList();
        }
    }

    public class PathDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Prod")
            {
                swaggerDoc.Paths.Where(p => p.Key.Contains("Administration")).ToList().ForEach(p => swaggerDoc.Paths.Remove(p.Key));
            }
        }
    }

    public class TemplatedSmsRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public TemplatedSmsRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new TemplatedSmsRequest
            {
                Sender = SenderType.AutoDetect,
                Phone = new Phone() 
                { 
                    CountryCode = _configuration.GetValue<int>("Swagger:Examples:Sms:CountryCode"),
                    Prefix = _configuration.GetValue<int>("Swagger:Examples:Sms:Prefix"), 
                    Number = _configuration.GetValue<int>("Swagger:Examples:Sms:Number")
                },
                Template = _configuration["Swagger:Examples:Sms:Template"],
                TemplateParams = _configuration["Swagger:Examples:Sms:TemplateParams"],
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                CitizenshipNo = "",
                CustomerNo = 0,
                Process = new Process() 
                { 
                    Name = _configuration["Swagger:Examples:Process:Name"], 
                    Identity = _configuration["Swagger:Examples:Process:Identity"] 
                }
            };

        }
    }

    public class TemplatedSmsRequestStringExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public TemplatedSmsRequestStringExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new TemplatedSmsRequestString
            {
                Sender = SenderType.AutoDetect,
                Phone = new PhoneString()
                {
                    CountryCode = _configuration.GetValue<string>("Swagger:Examples:Sms:CountryCode"),
                    Prefix = _configuration.GetValue<string>("Swagger:Examples:Sms:Prefix"),
                    Number = _configuration.GetValue<string>("Swagger:Examples:Sms:Number")
                },
                Template = _configuration["Swagger:Examples:Sms:Template"],
                TemplateParams = _configuration["Swagger:Examples:Sms:TemplateParams"],
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                CitizenshipNo = "",
                CustomerNo = 0,
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class SmsRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public SmsRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new SmsRequest
            {
                Sender = SenderType.AutoDetect,
                Phone = new Phone()
                {
                    CountryCode = _configuration.GetValue<int>("Swagger:Examples:Sms:CountryCode"),
                    Prefix = _configuration.GetValue<int>("Swagger:Examples:Sms:Prefix"),
                    Number = _configuration.GetValue<int>("Swagger:Examples:Sms:Number")
                },
                Content = _configuration["Swagger:Examples:Sms:Content"],
                SmsType = SmsTypes.Fast,
                CitizenshipNo = "",
                CustomerNo = 0,
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class SmsRequestStringExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public SmsRequestStringExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new SmsRequestString
            {
                Sender = SenderType.AutoDetect,
                Phone = new PhoneString()
                {
                    CountryCode = _configuration.GetValue<string>("Swagger:Examples:Sms:CountryCode"),
                    Prefix = _configuration.GetValue<string>("Swagger:Examples:Sms:Prefix"),
                    Number = _configuration.GetValue<string>("Swagger:Examples:Sms:Number")
                },
                Content = _configuration["Swagger:Examples:Sms:Content"],
                SmsType = SmsTypes.Fast,
                CitizenshipNo = "",
                CustomerNo = 0,
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class TemplatedMailRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public TemplatedMailRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new TemplatedMailRequest
            {
                Sender = SenderType.AutoDetect,
                Email = _configuration["Swagger:Examples:Mail:To"],
                Attachments = new List<Attachment>(),
                Bcc = "",
                Cc = "",
                CitizenshipNo = "",
                CustomerNo = 0,
                Template = _configuration["Swagger:Examples:Mail:Template"],
                TemplateParams = _configuration["Swagger:Examples:Mail:TemplateParams"],
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class MailRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public MailRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new MailRequest
            {
                Sender = SenderType.AutoDetect,
                Email = _configuration["Swagger:Examples:Mail:To"],
                From = _configuration["Swagger:Examples:Mail:From"],
                Subject = _configuration["Swagger:Examples:Mail:Subject"],
                Content = _configuration["Swagger:Examples:Mail:Content"],
                Attachments = new List<Attachment>(),
                Bcc = "",
                Cc = "",
                CitizenshipNo = "",
                CustomerNo = 0,
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class TemplatedPushRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public TemplatedPushRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new TemplatedPushRequest
            {
                Sender = SenderType.AutoDetect,
                CitizenshipNo = "",
                CustomerNo = 0,
                CustomParameters = "",
                Template = _configuration["Swagger:Examples:Push:Template"],
                TemplateParams = _configuration["Swagger:Examples:Push:TemplateParams"],
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class PushRequestExampleFilter : IExamplesProvider<object>
    {
        private readonly IConfiguration _configuration;
        public PushRequestExampleFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object GetExamples()
        {
            return new PushRequest
            {
                Sender = SenderType.AutoDetect,
                CitizenshipNo = "",
                CustomerNo = 0,
                Content = _configuration["Swagger:Examples:Push:Content"],
                Tags = _configuration.GetValue<string[]>("Swagger:Examples:Tags"),
                Process = new Process()
                {
                    Name = _configuration["Swagger:Examples:Process:Name"],
                    Identity = _configuration["Swagger:Examples:Process:Identity"]
                }
            };

        }
    }

    public class AddHeaderRequestExampleFilter : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new HeaderRequest
            {
                Sender = SenderType.AutoDetect,
                Branch = null,
                BusinessLine = "X",
                SmsType = SmsTypes.Otp,
                SmsPrefix = "Prefix",
                SmsSuffix = "Suffix"
            };

        }
    }


    public class AddSchemaExamples : Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
    {

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(SmsRequest))
            {
                schema.Properties["phone"].Description = "Telefon numarası";

            }
        }
    }
}