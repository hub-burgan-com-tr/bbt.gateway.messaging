using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class PushContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsTrackingLog_SmsResponseLog_SmsResponseLogId",
                table: "SmsTrackingLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7199257c-d3ec-4064-9fca-e5f344df0c44"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("bb5bf16b-3eaa-4b9d-933a-1bf4c42dc631"));

            migrationBuilder.AlterColumn<Guid>(
                name: "SmsResponseLogId",
                table: "SmsTrackingLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "PushNotificationRequestLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7a18bd5e-7ce4-452f-b829-cd0fa0fc691d"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ea6a94cf-95bc-445a-a825-82b4c2688749"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTrackingLog_SmsResponseLog_SmsResponseLogId",
                table: "SmsTrackingLog",
                column: "SmsResponseLogId",
                principalTable: "SmsResponseLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsTrackingLog_SmsResponseLog_SmsResponseLogId",
                table: "SmsTrackingLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7a18bd5e-7ce4-452f-b829-cd0fa0fc691d"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ea6a94cf-95bc-445a-a825-82b4c2688749"));

            migrationBuilder.DropColumn(
                name: "Content",
                table: "PushNotificationRequestLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "SmsResponseLogId",
                table: "SmsTrackingLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7199257c-d3ec-4064-9fca-e5f344df0c44"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("bb5bf16b-3eaa-4b9d-933a-1bf4c42dc631"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTrackingLog_SmsResponseLog_SmsResponseLogId",
                table: "SmsTrackingLog",
                column: "SmsResponseLogId",
                principalTable: "SmsResponseLog",
                principalColumn: "Id");
        }
    }
}
