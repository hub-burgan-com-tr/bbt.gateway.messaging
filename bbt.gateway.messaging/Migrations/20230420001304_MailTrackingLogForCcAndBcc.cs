using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class MailTrackingLogForCcAndBcc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("1ab54fda-598e-4ac3-a853-221f730a33be"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("d9c14f66-0af0-4109-9265-ade6568cf0f0"));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MailTrackingLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BccStatusQueryId",
                table: "MailResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CcStatusQueryId",
                table: "MailResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("6f91b219-317b-4c9d-95b8-8124149b8d95"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("96aeb11e-c54d-4737-9fb5-99efb7745368"), null, null, 4, null, null, "", 1, "", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("6f91b219-317b-4c9d-95b8-8124149b8d95"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("96aeb11e-c54d-4737-9fb5-99efb7745368"));

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MailTrackingLog");

            migrationBuilder.DropColumn(
                name: "BccStatusQueryId",
                table: "MailResponseLog");

            migrationBuilder.DropColumn(
                name: "CcStatusQueryId",
                table: "MailResponseLog");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("1ab54fda-598e-4ac3-a853-221f730a33be"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("d9c14f66-0af0-4109-9265-ade6568cf0f0"), 2000, null, 1, null, null, "", 2, null, null, null });
        }
    }
}
