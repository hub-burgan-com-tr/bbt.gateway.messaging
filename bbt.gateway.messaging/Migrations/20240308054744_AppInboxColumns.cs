using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    public partial class AppInboxColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PushNotificationRequestLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PushNotificationRequestLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "PushNotificationRequestLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "PushNotificationRequestLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SaveInbox",
                table: "PushNotificationRequestLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PushNotificationRequestLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PushNotificationRequestLogs");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "PushNotificationRequestLogs");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "PushNotificationRequestLogs");

            migrationBuilder.DropColumn(
                name: "SaveInbox",
                table: "PushNotificationRequestLogs");

            
        }
    }
}
