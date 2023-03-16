using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionForeignKeys1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: new Guid("40d24780-fbcb-4a62-b427-97ab1046e1a5"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("9d083b75-8487-4e2f-b079-33fe8f91cad4"));

            migrationBuilder.DropColumn(
                name: "MailRequestLogId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OtpRequestLogId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SmsRequestLogId",
                table: "Transactions");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("4022745e-5c0a-4d92-b7af-bc0e4618709d"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("a7c32626-860b-4714-8619-c959ba675dbe"), 2000, null, 0, "", null, "", 2, null, "", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("4022745e-5c0a-4d92-b7af-bc0e4618709d"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("a7c32626-860b-4714-8619-c959ba675dbe"));

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
                values: new object[] { new Guid("40d24780-fbcb-4a62-b427-97ab1046e1a5"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("9d083b75-8487-4e2f-b079-33fe8f91cad4"), 2000, null, 0, "", null, "", 2, null, "", null });

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
    }
}
