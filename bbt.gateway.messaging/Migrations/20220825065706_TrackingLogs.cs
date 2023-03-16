using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class TrackingLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("8b47cda1-36a8-46f3-995e-664539915480"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("ec0db1ca-b6ba-49cd-b633-7c8e8129dcab"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PushNotificationResponseLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusQueryId",
                table: "PushNotificationResponseLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MailResponseLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailTrackingLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BounceCode = table.Column<int>(type: "int", nullable: false),
                    BounceText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueriedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MailResponseLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTrackingLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailTrackingLog_MailResponseLog_MailResponseLogId",
                        column: x => x.MailResponseLogId,
                        principalTable: "MailResponseLog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PushTrackingLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueriedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PushResponseLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushTrackingLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushTrackingLog_PushNotificationResponseLogs_PushResponseLogId",
                        column: x => x.PushResponseLogId,
                        principalTable: "PushNotificationResponseLogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SmsTrackingLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueriedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SmsResponseLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTrackingLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsTrackingLog_SmsResponseLog_SmsResponseLogId",
                        column: x => x.SmsResponseLogId,
                        principalTable: "SmsResponseLog",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("7199257c-d3ec-4064-9fca-e5f344df0c44"), null, null, 4, null, null, "", 1, "", null, null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("bb5bf16b-3eaa-4b9d-933a-1bf4c42dc631"), 2000, null, 1, null, null, "", 2, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingLog_MailResponseLogId",
                table: "MailTrackingLog",
                column: "MailResponseLogId");

            migrationBuilder.CreateIndex(
                name: "IX_PushTrackingLog_PushResponseLogId",
                table: "PushTrackingLog",
                column: "PushResponseLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTrackingLog_SmsResponseLogId",
                table: "SmsTrackingLog",
                column: "SmsResponseLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailTrackingLog");

            migrationBuilder.DropTable(
                name: "PushTrackingLog");

            migrationBuilder.DropTable(
                name: "SmsTrackingLog");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("7199257c-d3ec-4064-9fca-e5f344df0c44"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("bb5bf16b-3eaa-4b9d-933a-1bf4c42dc631"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PushNotificationResponseLogs");

            migrationBuilder.DropColumn(
                name: "StatusQueryId",
                table: "PushNotificationResponseLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MailResponseLog");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("8b47cda1-36a8-46f3-995e-664539915480"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("ec0db1ca-b6ba-49cd-b633-7c8e8129dcab"), null, null, 0, "", null, "", 1, "", "", null });
        }
    }
}
