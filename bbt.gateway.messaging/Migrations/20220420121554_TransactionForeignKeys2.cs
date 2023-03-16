using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionForeignKeys2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("4022745e-5c0a-4d92-b7af-bc0e4618709d"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("a7c32626-860b-4714-8619-c959ba675dbe"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_MailRequestLog_Transactions_TxnId",
                table: "MailRequestLog",
                column: "TxnId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OtpRequestLogs_Transactions_TxnId",
                table: "OtpRequestLogs",
                column: "TxnId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsRequestLog_Transactions_TxnId",
                table: "SmsRequestLog",
                column: "TxnId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailRequestLog_Transactions_TxnId",
                table: "MailRequestLog");

            migrationBuilder.DropForeignKey(
                name: "FK_OtpRequestLogs_Transactions_TxnId",
                table: "OtpRequestLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsRequestLog_Transactions_TxnId",
                table: "SmsRequestLog");

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
                values: new object[] { new Guid("4022745e-5c0a-4d92-b7af-bc0e4618709d"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("a7c32626-860b-4714-8619-c959ba675dbe"), 2000, null, 0, "", null, "", 2, null, "", null });
        }
    }
}
