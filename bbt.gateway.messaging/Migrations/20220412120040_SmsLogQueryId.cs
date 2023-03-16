using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class SmsLogQueryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailConfigurationLog_MailConfigurations_PhoneId",
                table: "MailConfigurationLog");

            migrationBuilder.DropForeignKey(
                name: "FK_MailConfigurations_MailConfigurations_MailConfigurationId",
                table: "MailConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_MailConfigurations_MailConfigurationId",
                table: "MailConfigurations");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("538e9637-4c27-4d9f-a1bf-e5d918dc51b5"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("73f71952-ddbd-4e33-a1c6-e7115bc39e82"));

            migrationBuilder.DropColumn(
                name: "RequestBody",
                table: "MailResponseLog");

            migrationBuilder.DropColumn(
                name: "ResponseBody",
                table: "MailResponseLog");

            migrationBuilder.DropColumn(
                name: "MailConfigurationId",
                table: "MailConfigurations");

            migrationBuilder.RenameColumn(
                name: "PhoneId",
                table: "MailConfigurationLog",
                newName: "MailId");

            migrationBuilder.RenameIndex(
                name: "IX_MailConfigurationLog_PhoneId",
                table: "MailConfigurationLog",
                newName: "IX_MailConfigurationLog_MailId");

            migrationBuilder.AddColumn<string>(
                name: "StatusQueryId",
                table: "SmsLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("1b8be329-431b-4066-9f7e-0285e56a7822"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("5043baab-5a6c-47c7-b3b3-bd6f8bb001aa"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.AddForeignKey(
                name: "FK_MailConfigurationLog_MailConfigurations_MailId",
                table: "MailConfigurationLog",
                column: "MailId",
                principalTable: "MailConfigurations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailConfigurationLog_MailConfigurations_MailId",
                table: "MailConfigurationLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("1b8be329-431b-4066-9f7e-0285e56a7822"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("5043baab-5a6c-47c7-b3b3-bd6f8bb001aa"));

            migrationBuilder.DropColumn(
                name: "StatusQueryId",
                table: "SmsLogs");

            migrationBuilder.RenameColumn(
                name: "MailId",
                table: "MailConfigurationLog",
                newName: "PhoneId");

            migrationBuilder.RenameIndex(
                name: "IX_MailConfigurationLog_MailId",
                table: "MailConfigurationLog",
                newName: "IX_MailConfigurationLog_PhoneId");

            migrationBuilder.AddColumn<string>(
                name: "RequestBody",
                table: "MailResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseBody",
                table: "MailResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MailConfigurationId",
                table: "MailConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("538e9637-4c27-4d9f-a1bf-e5d918dc51b5"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("73f71952-ddbd-4e33-a1c6-e7115bc39e82"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });

            migrationBuilder.CreateIndex(
                name: "IX_MailConfigurations_MailConfigurationId",
                table: "MailConfigurations",
                column: "MailConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailConfigurationLog_MailConfigurations_PhoneId",
                table: "MailConfigurationLog",
                column: "PhoneId",
                principalTable: "MailConfigurations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MailConfigurations_MailConfigurations_MailConfigurationId",
                table: "MailConfigurations",
                column: "MailConfigurationId",
                principalTable: "MailConfigurations",
                principalColumn: "Id");
        }
    }
}
