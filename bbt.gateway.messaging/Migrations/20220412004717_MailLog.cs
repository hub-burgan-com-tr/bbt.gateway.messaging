using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class MailLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("8143fc27-43f8-406a-adfe-7f136dfab5de"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("9b13fde1-2dbe-4b15-bfa7-f0e02769cc9c"));

            migrationBuilder.AddColumn<Guid>(
                name: "TxnId",
                table: "OtpRequestLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "MailConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNo = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    MailConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailConfigurations_MailConfigurations_MailConfigurationId",
                        column: x => x.MailConfigurationId,
                        principalTable: "MailConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MailConfigurationLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailConfigurationLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailConfigurationLog_MailConfigurations_PhoneId",
                        column: x => x.PhoneId,
                        principalTable: "MailConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MailRequestLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TxnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MailConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRequestLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailRequestLog_MailConfigurations_MailConfigurationId",
                        column: x => x.MailConfigurationId,
                        principalTable: "MailConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MailResponseLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusQueryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailRequestLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailResponseLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailResponseLog_MailRequestLog_MailRequestLogId",
                        column: x => x.MailRequestLogId,
                        principalTable: "MailRequestLog",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("03aa7abb-7275-400b-aa25-aef17a34fb3c"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("71f678ca-b0a1-4080-8d7e-62b7484cea3e"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.CreateIndex(
                name: "IX_MailConfigurationLog_PhoneId",
                table: "MailConfigurationLog",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_MailConfigurations_MailConfigurationId",
                table: "MailConfigurations",
                column: "MailConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRequestLog_MailConfigurationId",
                table: "MailRequestLog",
                column: "MailConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MailResponseLog_MailRequestLogId",
                table: "MailResponseLog",
                column: "MailRequestLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailConfigurationLog");

            migrationBuilder.DropTable(
                name: "MailResponseLog");

            migrationBuilder.DropTable(
                name: "MailRequestLog");

            migrationBuilder.DropTable(
                name: "MailConfigurations");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("03aa7abb-7275-400b-aa25-aef17a34fb3c"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("71f678ca-b0a1-4080-8d7e-62b7484cea3e"));

            migrationBuilder.DropColumn(
                name: "TxnId",
                table: "OtpRequestLogs");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("8143fc27-43f8-406a-adfe-7f136dfab5de"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("9b13fde1-2dbe-4b15-bfa7-f0e02769cc9c"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null });
        }
    }
}
