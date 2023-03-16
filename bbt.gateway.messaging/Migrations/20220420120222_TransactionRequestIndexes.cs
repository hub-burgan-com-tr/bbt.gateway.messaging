using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionRequestIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MailConfigurations_MailConfigurationId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PhoneConfigurations_PhoneConfigurationId",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("002a7f76-e512-4c40-ba66-986ba39d9100"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ce6fbd60-2a03-4782-9bdc-297e5a755910"));

            migrationBuilder.RenameColumn(
                name: "PhoneConfigurationId",
                table: "Transactions",
                newName: "SmsRequestLogId");

            migrationBuilder.RenameColumn(
                name: "MailConfigurationId",
                table: "Transactions",
                newName: "OtpRequestLogId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_PhoneConfigurationId",
                table: "Transactions",
                newName: "IX_Transactions_SmsRequestLogId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_MailConfigurationId",
                table: "Transactions",
                newName: "IX_Transactions_OtpRequestLogId");

            migrationBuilder.AddColumn<Guid>(
                name: "MailRequestLogId",
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

            migrationBuilder.RenameColumn(
                name: "SmsRequestLogId",
                table: "Transactions",
                newName: "PhoneConfigurationId");

            migrationBuilder.RenameColumn(
                name: "OtpRequestLogId",
                table: "Transactions",
                newName: "MailConfigurationId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_SmsRequestLogId",
                table: "Transactions",
                newName: "IX_Transactions_PhoneConfigurationId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_OtpRequestLogId",
                table: "Transactions",
                newName: "IX_Transactions_MailConfigurationId");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("002a7f76-e512-4c40-ba66-986ba39d9100"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ce6fbd60-2a03-4782-9bdc-297e5a755910"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_MailConfigurations_MailConfigurationId",
                table: "Transactions",
                column: "MailConfigurationId",
                principalTable: "MailConfigurations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_PhoneConfigurations_PhoneConfigurationId",
                table: "Transactions",
                column: "PhoneConfigurationId",
                principalTable: "PhoneConfigurations",
                principalColumn: "Id");
        }
    }
}
