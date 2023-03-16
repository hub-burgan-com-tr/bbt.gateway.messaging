using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class BlacklistSmsIdSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("771050a2-7b4b-4d3e-bacc-8f54cf7cb474"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("fe295167-a1ba-4e75-8521-4af1886d0558"));

            migrationBuilder.AddColumn<long>(
                name: "SmsId",
                table: "BlackListEntries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("8143fc27-43f8-406a-adfe-7f136dfab5de"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("9b13fde1-2dbe-4b15-bfa7-f0e02769cc9c"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("8143fc27-43f8-406a-adfe-7f136dfab5de"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("9b13fde1-2dbe-4b15-bfa7-f0e02769cc9c"));

            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "BlackListEntries");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("771050a2-7b4b-4d3e-bacc-8f54cf7cb474"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("fe295167-a1ba-4e75-8521-4af1886d0558"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }
    }
}
