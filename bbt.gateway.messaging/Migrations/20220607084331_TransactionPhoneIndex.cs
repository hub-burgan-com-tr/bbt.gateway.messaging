using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionPhoneIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("78d981ac-6842-4452-9bd7-1b4a13fba0e8"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("f5fa100e-fc96-4605-9e1c-2bf2c72fe8a7"));

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("8b47cda1-36a8-46f3-995e-664539915480"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ec0db1ca-b6ba-49cd-b633-7c8e8129dcab"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_WhiteList_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "WhiteList",
                columns: new[] { "Phone_CountryCode", "Phone_Prefix", "Phone_Number" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "Transactions",
                columns: new[] { "Phone_CountryCode", "Phone_Prefix", "Phone_Number" });

            migrationBuilder.CreateIndex(
                name: "IX_SmsRequestLog_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "SmsRequestLog",
                columns: new[] { "Phone_CountryCode", "Phone_Prefix", "Phone_Number" });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneConfigurations_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "PhoneConfigurations",
                columns: new[] { "Phone_CountryCode", "Phone_Prefix", "Phone_Number" });

            migrationBuilder.CreateIndex(
                name: "IX_OtpRequestLogs_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "OtpRequestLogs",
                columns: new[] { "Phone_CountryCode", "Phone_Prefix", "Phone_Number" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WhiteList_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "WhiteList");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_SmsRequestLog_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "SmsRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_PhoneConfigurations_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "PhoneConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_OtpRequestLogs_Phone_CountryCode_Phone_Prefix_Phone_Number",
                table: "OtpRequestLogs");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("8b47cda1-36a8-46f3-995e-664539915480"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ec0db1ca-b6ba-49cd-b633-7c8e8129dcab"));

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("78d981ac-6842-4452-9bd7-1b4a13fba0e8"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("f5fa100e-fc96-4605-9e1c-2bf2c72fe8a7"), 2000, null, 0, "", null, "", 2, null, "", null });
        }
    }
}
