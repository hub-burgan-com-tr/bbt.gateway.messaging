using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionForeignKeys4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: new Guid("7581a33a-56a2-4575-a589-3510a32c9679"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("b46090d0-299b-4baf-9c7d-81cb1ecd1247"));

            migrationBuilder.DropColumn(
                name: "TxnId",
                table: "SmsRequestLog");

            migrationBuilder.DropColumn(
                name: "TxnId",
                table: "OtpRequestLogs");

            migrationBuilder.DropColumn(
                name: "TxnId",
                table: "MailRequestLog");

            migrationBuilder.AddColumn<Guid>(
                name: "MailRequestLogId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OtpRequestLogId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsRequestLogId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("53f7519e-a3c2-46fa-985b-f4dec7f7d954"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("a364b99b-092b-45d5-9c9f-a513ed1dd09f"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MailRequestLogId",
                table: "Transactions",
                column: "MailRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OtpRequestLogId",
                table: "Transactions",
                column: "OtpRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SmsRequestLogId",
                table: "Transactions",
                column: "SmsRequestLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_MailRequestLog_MailRequestLogId",
                table: "Transactions",
                column: "MailRequestLogId",
                principalTable: "MailRequestLog",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_OtpRequestLogs_OtpRequestLogId",
                table: "Transactions",
                column: "OtpRequestLogId",
                principalTable: "OtpRequestLogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SmsRequestLog_SmsRequestLogId",
                table: "Transactions",
                column: "SmsRequestLogId",
                principalTable: "SmsRequestLog",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MailRequestLog_MailRequestLogId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_OtpRequestLogs_OtpRequestLogId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SmsRequestLog_SmsRequestLogId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_MailRequestLogId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OtpRequestLogId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SmsRequestLogId",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("53f7519e-a3c2-46fa-985b-f4dec7f7d954"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("a364b99b-092b-45d5-9c9f-a513ed1dd09f"));

            migrationBuilder.DropColumn(
                name: "MailRequestLogId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OtpRequestLogId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SmsRequestLogId",
                table: "Transactions");

            migrationBuilder.AddColumn<Guid>(
                name: "TxnId",
                table: "SmsRequestLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TxnId",
                table: "OtpRequestLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TxnId",
                table: "MailRequestLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
