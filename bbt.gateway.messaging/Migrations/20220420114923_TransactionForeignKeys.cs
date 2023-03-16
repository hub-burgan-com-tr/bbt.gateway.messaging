using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TransactionForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("70a947b4-70c6-4741-9716-1a5220409bab"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7db91891-5345-42ad-b5a6-a819b2c100dd"));

            migrationBuilder.AddColumn<Guid>(
                name: "MailConfigurationId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhoneConfigurationId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("002a7f76-e512-4c40-ba66-986ba39d9100"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ce6fbd60-2a03-4782-9bdc-297e5a755910"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MailConfigurationId",
                table: "Transactions",
                column: "MailConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PhoneConfigurationId",
                table: "Transactions",
                column: "PhoneConfigurationId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MailConfigurations_MailConfigurationId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PhoneConfigurations_PhoneConfigurationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_MailConfigurationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PhoneConfigurationId",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("002a7f76-e512-4c40-ba66-986ba39d9100"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ce6fbd60-2a03-4782-9bdc-297e5a755910"));

            migrationBuilder.DropColumn(
                name: "MailConfigurationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PhoneConfigurationId",
                table: "Transactions");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("70a947b4-70c6-4741-9716-1a5220409bab"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7db91891-5345-42ad-b5a6-a819b2c100dd"), 2000, null, 0, "", null, "", 2, null, "", null });
        }
    }
}
