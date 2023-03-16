using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class PhoneConfigurationsSmsRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_SmsResponseLog_PhoneConfigurations_PhoneConfigurationId",
                table: "SmsResponseLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("9127e6ea-4cc9-4918-97e1-8d7bca14894c"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("d0f2d7ba-57e5-467a-a0b1-5fdbee2a0bdd"));

            migrationBuilder.DropIndex(
                name: "IX_SmsResponseLog_PhoneConfigurationId",
                table: "SmsResponseLog");

            migrationBuilder.DropColumn(
                name: "PhoneConfigurationId",
                table: "SmsResponseLog");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("4c69e186-8624-4267-ba64-b8a1d1b93cfa"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("e4ef9b49-1290-4959-9d03-f1180a3a01c2"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("4c69e186-8624-4267-ba64-b8a1d1b93cfa"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("e4ef9b49-1290-4959-9d03-f1180a3a01c2"));

            migrationBuilder.AddColumn<Guid>(
                name: "PhoneConfigurationId",
                table: "SmsResponseLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("9127e6ea-4cc9-4918-97e1-8d7bca14894c"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("d0f2d7ba-57e5-467a-a0b1-5fdbee2a0bdd"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.AddForeignKey(
                name: "FK_SmsResponseLog_PhoneConfigurations_PhoneConfigurationId",
                table: "SmsResponseLog",
                column: "PhoneConfigurationId",
                principalTable: "PhoneConfigurations",
                principalColumn: "Id");
        }
    }
}
