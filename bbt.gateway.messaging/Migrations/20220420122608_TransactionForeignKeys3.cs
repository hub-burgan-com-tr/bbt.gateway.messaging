using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionForeignKeys3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmsRequestLog_TxnId",
                table: "SmsRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_OtpRequestLogs_TxnId",
                table: "OtpRequestLogs");

            migrationBuilder.DropIndex(
                name: "IX_MailRequestLog_TxnId",
                table: "MailRequestLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("0aa59255-0855-4832-b56f-548dcea64bc4"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("536473e4-fe2f-496b-850f-c0cd1140a52e"));

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7581a33a-56a2-4575-a589-3510a32c9679"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("b46090d0-299b-4baf-9c7d-81cb1ecd1247"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_SmsRequestLog_TxnId",
                table: "SmsRequestLog",
                column: "TxnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpRequestLogs_TxnId",
                table: "OtpRequestLogs",
                column: "TxnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailRequestLog_TxnId",
                table: "MailRequestLog",
                column: "TxnId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmsRequestLog_TxnId",
                table: "SmsRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_OtpRequestLogs_TxnId",
                table: "OtpRequestLogs");

            migrationBuilder.DropIndex(
                name: "IX_MailRequestLog_TxnId",
                table: "MailRequestLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7581a33a-56a2-4575-a589-3510a32c9679"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("b46090d0-299b-4baf-9c7d-81cb1ecd1247"));

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("0aa59255-0855-4832-b56f-548dcea64bc4"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("536473e4-fe2f-496b-850f-c0cd1140a52e"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_SmsRequestLog_TxnId",
                table: "SmsRequestLog",
                column: "TxnId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpRequestLogs_TxnId",
                table: "OtpRequestLogs",
                column: "TxnId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRequestLog_TxnId",
                table: "MailRequestLog",
                column: "TxnId");
        }
    }
}
