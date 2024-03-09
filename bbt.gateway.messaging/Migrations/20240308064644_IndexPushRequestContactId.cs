using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class IndexPushRequestContactId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationRequestLogs_ContactId",
                table: "PushNotificationRequestLogs",
                column: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PushNotificationRequestLogs_ContactId",
                table: "PushNotificationRequestLogs");

            
        }
    }
}
