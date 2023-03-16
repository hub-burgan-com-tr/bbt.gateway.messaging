using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class RemoveCreatedAtFromSmsResponse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("4c69e186-8624-4267-ba64-b8a1d1b93cfa"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("e4ef9b49-1290-4959-9d03-f1180a3a01c2"));

            migrationBuilder.DropColumn(
                name: "CreatedBy_Action",
                table: "SmsResponseLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy_Identity",
                table: "SmsResponseLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy_ItemId",
                table: "SmsResponseLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy_Name",
                table: "SmsResponseLog");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("08886f17-49b3-420f-b29c-e9fb4234b18c"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ea32ca86-e784-47b7-a6b9-f6bb61cb5cdc"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("08886f17-49b3-420f-b29c-e9fb4234b18c"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ea32ca86-e784-47b7-a6b9-f6bb61cb5cdc"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy_Action",
                table: "SmsResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy_Identity",
                table: "SmsResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy_ItemId",
                table: "SmsResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy_Name",
                table: "SmsResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("4c69e186-8624-4267-ba64-b8a1d1b93cfa"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("e4ef9b49-1290-4959-9d03-f1180a3a01c2"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }
    }
}
