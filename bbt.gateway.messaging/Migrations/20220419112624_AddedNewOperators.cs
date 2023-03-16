using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class AddedNewOperators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("1103e7e0-22f8-4333-8364-262729aee40c"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("4b872904-8b23-483d-bb9d-bdb5415d4740"));

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[,]
                {
                    { new Guid("70a947b4-70c6-4741-9716-1a5220409bab"), null, null, 0, "", null, "", 1, "", "", null },
                    { new Guid("7db91891-5345-42ad-b5a6-a819b2c100dd"), 2000, null, 0, "", null, "", 2, null, "", null }
                });

            migrationBuilder.InsertData(
                table: "Operators",
                columns: new[] { "Id", "AuthToken", "AuthanticationService", "ControlDaysForOtp", "Password", "QueryService", "SendService", "Status", "SupportDeskMail", "SupportDeskPhone", "TokenCreatedAt", "TokenExpiredAt", "Type", "UseIvnWhenDeactive", "User" },
                values: new object[,]
                {
                    { 6, null, null, 0, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, false, null },
                    { 7, null, null, 0, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, false, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("70a947b4-70c6-4741-9716-1a5220409bab"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7db91891-5345-42ad-b5a6-a819b2c100dd"));

            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("1103e7e0-22f8-4333-8364-262729aee40c"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("4b872904-8b23-483d-bb9d-bdb5415d4740"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }
    }
}
