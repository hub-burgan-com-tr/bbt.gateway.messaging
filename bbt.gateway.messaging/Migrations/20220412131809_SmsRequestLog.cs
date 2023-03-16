using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class SmsRequestLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ba523768-66a1-4918-b54b-39be8a82a5b4"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("f2fad0b3-e7ca-4a87-8529-13f2673e0f7a"));

            migrationBuilder.AddColumn<Guid>(
                name: "SmsRequestLogId",
                table: "SmsResponseLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsRequestLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TxnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Phone_CountryCode = table.Column<int>(type: "int", nullable: true),
                    Phone_Prefix = table.Column<int>(type: "int", nullable: true),
                    Phone_Number = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsRequestLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsRequestLog_PhoneConfigurations_PhoneConfigurationId",
                        column: x => x.PhoneConfigurationId,
                        principalTable: "PhoneConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("2b722e09-aef3-488a-8c4e-a54e1708967d"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("bc630ef9-3379-4d52-8027-20453f27bd8d"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.CreateIndex(
                name: "IX_SmsResponseLog_SmsRequestLogId",
                table: "SmsResponseLog",
                column: "SmsRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsRequestLog_PhoneConfigurationId",
                table: "SmsRequestLog",
                column: "PhoneConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsResponseLog_SmsRequestLog_SmsRequestLogId",
                table: "SmsResponseLog",
                column: "SmsRequestLogId",
                principalTable: "SmsRequestLog",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsResponseLog_SmsRequestLog_SmsRequestLogId",
                table: "SmsResponseLog");

            migrationBuilder.DropTable(
                name: "SmsRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_SmsResponseLog_SmsRequestLogId",
                table: "SmsResponseLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("2b722e09-aef3-488a-8c4e-a54e1708967d"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("bc630ef9-3379-4d52-8027-20453f27bd8d"));

            migrationBuilder.DropColumn(
                name: "SmsRequestLogId",
                table: "SmsResponseLog");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ba523768-66a1-4918-b54b-39be8a82a5b4"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("f2fad0b3-e7ca-4a87-8529-13f2673e0f7a"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }
    }
}
