using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Headers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    BusinessLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Branch = table.Column<int>(type: "int", nullable: true),
                    SmsSender = table.Column<int>(type: "int", nullable: false),
                    SmsPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsSuffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTemplatePrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTemplateSuffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsTemplatePrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsTemplateSuffix = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ControlDaysForOtp = table.Column<int>(type: "int", nullable: false),
                    AuthanticationService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseIvnWhenDeactive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SupportDeskMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportDeskPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone_CountryCode = table.Column<int>(type: "int", nullable: true),
                    Phone_Prefix = table.Column<int>(type: "int", nullable: true),
                    Phone_Number = table.Column<int>(type: "int", nullable: true),
                    CustomerNo = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    Operator = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlackListEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlackListEntries_PhoneConfigurations_PhoneConfigurationId",
                        column: x => x.PhoneConfigurationId,
                        principalTable: "PhoneConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtpRequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Phone_CountryCode = table.Column<int>(type: "int", nullable: true),
                    Phone_Prefix = table.Column<int>(type: "int", nullable: true),
                    Phone_Number = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpRequestLogs_PhoneConfigurations_PhoneConfigurationId",
                        column: x => x.PhoneConfigurationId,
                        principalTable: "PhoneConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhoneConfigurationLog",
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
                    table.PrimaryKey("PK_PhoneConfigurationLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneConfigurationLog_PhoneConfigurations_PhoneId",
                        column: x => x.PhoneId,
                        principalTable: "PhoneConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SmsLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    OperatorResponseCode = table.Column<int>(type: "int", nullable: false),
                    OperatorResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsLogs_PhoneConfigurations_PhoneConfigurationId",
                        column: x => x.PhoneConfigurationId,
                        principalTable: "PhoneConfigurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlackListEntryLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlackListEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterMaster = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterSlave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_ItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy_Identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackListEntryLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlackListEntryLog_BlackListEntries_BlackListEntryId",
                        column: x => x.BlackListEntryId,
                        principalTable: "BlackListEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtpResponseLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<int>(type: "int", nullable: false),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusQueryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtpRequestLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpResponseLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpResponseLog_OtpRequestLogs_OtpRequestLogId",
                        column: x => x.OtpRequestLogId,
                        principalTable: "OtpRequestLogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtpTrackingLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueriedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpTrackingLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpTrackingLog_OtpResponseLog_LogId",
                        column: x => x.LogId,
                        principalTable: "OtpResponseLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Headers",
                columns: new[] { "Id", "Branch", "BusinessLine", "ContentType", "EmailTemplatePrefix", "EmailTemplateSuffix", "SmsPrefix", "SmsSender", "SmsSuffix", "SmsTemplatePrefix", "SmsTemplateSuffix" },
                values: new object[,]
                {
                    { new Guid("771050a2-7b4b-4d3e-bacc-8f54cf7cb474"), null, null, 0, "generic", null, "Dear Honey,", 1, ":)", "generic", null },
                    { new Guid("fe295167-a1ba-4e75-8521-4af1886d0558"), 2000, null, 0, "on", null, "OBEY:", 2, null, "on", null }
                });

            migrationBuilder.InsertData(
                table: "Operators",
                columns: new[] { "Id", "AuthToken", "AuthanticationService", "ControlDaysForOtp", "Password", "QueryService", "SendService", "Status", "SupportDeskMail", "SupportDeskPhone", "TokenCreatedAt", "TokenExpiredAt", "Type", "UseIvnWhenDeactive", "User" },
                values: new object[,]
                {
                    { 1, null, null, 60, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, null },
                    { 2, null, null, 60, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, false, null },
                    { 3, null, null, 60, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, false, null },
                    { 4, null, null, 60, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, false, null },
                    { 5, null, null, 60, null, null, null, 1, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, false, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlackListEntries_PhoneConfigurationId",
                table: "BlackListEntries",
                column: "PhoneConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackListEntryLog_BlackListEntryId",
                table: "BlackListEntryLog",
                column: "BlackListEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpRequestLogs_PhoneConfigurationId",
                table: "OtpRequestLogs",
                column: "PhoneConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpResponseLog_OtpRequestLogId",
                table: "OtpResponseLog",
                column: "OtpRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpTrackingLog_LogId",
                table: "OtpTrackingLog",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneConfigurationLog_PhoneId",
                table: "PhoneConfigurationLog",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneConfigurations_Id",
                table: "PhoneConfigurations",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_SmsLogs_PhoneConfigurationId",
                table: "SmsLogs",
                column: "PhoneConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackListEntryLog");

            migrationBuilder.DropTable(
                name: "Headers");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "OtpTrackingLog");

            migrationBuilder.DropTable(
                name: "PhoneConfigurationLog");

            migrationBuilder.DropTable(
                name: "SmsLogs");

            migrationBuilder.DropTable(
                name: "BlackListEntries");

            migrationBuilder.DropTable(
                name: "OtpResponseLog");

            migrationBuilder.DropTable(
                name: "OtpRequestLogs");

            migrationBuilder.DropTable(
                name: "PhoneConfigurations");
        }
    }
}
