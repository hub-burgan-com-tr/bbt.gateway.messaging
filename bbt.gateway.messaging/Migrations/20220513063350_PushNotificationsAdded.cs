using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class PushNotificationsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("2a1780ec-529d-4d5d-9969-cc6873a5cf5a"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("3eed3f91-4078-47e1-a729-be25f0ce5c56"));

            migrationBuilder.AddColumn<Guid>(
                name: "PushNotificationRequestLogId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PushNotificationRequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationRequestLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationResponseLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PushNotificationRequestLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationResponseLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotificationResponseLogs_PushNotificationRequestLogs_PushNotificationRequestLogId",
                        column: x => x.PushNotificationRequestLogId,
                        principalTable: "PushNotificationRequestLogs",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("383eaf0d-5f03-47e5-8bec-09a341db571c"), 2000, null, 0, "", null, "", 2, null, "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("3ba1c13b-1249-49b5-bc04-fbb1e5992dad"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PushNotificationRequestLogId",
                table: "Transactions",
                column: "PushNotificationRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationResponseLogs_PushNotificationRequestLogId",
                table: "PushNotificationResponseLogs",
                column: "PushNotificationRequestLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_PushNotificationRequestLogs_PushNotificationRequestLogId",
                table: "Transactions",
                column: "PushNotificationRequestLogId",
                principalTable: "PushNotificationRequestLogs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PushNotificationRequestLogs_PushNotificationRequestLogId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PushNotificationResponseLogs");

            migrationBuilder.DropTable(
                name: "PushNotificationRequestLogs");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PushNotificationRequestLogId",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("383eaf0d-5f03-47e5-8bec-09a341db571c"));

            migrationBuilder.DeleteData(
                table: "Headers",
                keyColumn: "Id",
                keyValue: new Guid("3ba1c13b-1249-49b5-bc04-fbb1e5992dad"));

            migrationBuilder.DropColumn(
                name: "PushNotificationRequestLogId",
                table: "Transactions");

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("2a1780ec-529d-4d5d-9969-cc6873a5cf5a"), null, null, 0, "", null, "", 1, "", "", null });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[] { new Guid("3eed3f91-4078-47e1-a729-be25f0ce5c56"), 2000, null, 0, "", null, "", 2, null, "", null });
        }
    }
}
