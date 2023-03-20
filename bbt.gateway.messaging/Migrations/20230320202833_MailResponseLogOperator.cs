using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class MailResponseLogOperator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailTrackingLog_MailResponseLog_MailResponseLogId",
                table: "MailTrackingLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7a18bd5e-7ce4-452f-b829-cd0fa0fc691d"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ea6a94cf-95bc-445a-a825-82b4c2688749"));

            migrationBuilder.AlterColumn<Guid>(
                name: "MailResponseLogId",
                table: "MailTrackingLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Operator",
                table: "MailResponseLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("1ab54fda-598e-4ac3-a853-221f730a33be"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("d9c14f66-0af0-4109-9265-ade6568cf0f0"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_MailTrackingLog_MailResponseLog_MailResponseLogId",
                table: "MailTrackingLog",
                column: "MailResponseLogId",
                principalTable: "MailResponseLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailTrackingLog_MailResponseLog_MailResponseLogId",
                table: "MailTrackingLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("1ab54fda-598e-4ac3-a853-221f730a33be"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("d9c14f66-0af0-4109-9265-ade6568cf0f0"));

            migrationBuilder.DropColumn(
                name: "Operator",
                table: "MailResponseLog");

            migrationBuilder.AlterColumn<Guid>(
                name: "MailResponseLogId",
                table: "MailTrackingLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7a18bd5e-7ce4-452f-b829-cd0fa0fc691d"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ea6a94cf-95bc-445a-a825-82b4c2688749"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_MailTrackingLog_MailResponseLog_MailResponseLogId",
                table: "MailTrackingLog",
                column: "MailResponseLogId",
                principalTable: "MailResponseLog",
                principalColumn: "Id");
        }
    }
}
