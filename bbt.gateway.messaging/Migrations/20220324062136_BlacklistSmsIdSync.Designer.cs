﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using bbt.gateway.common;

#nullable disable

namespace bbt.gateway.messaging.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220324062136_BlacklistSmsIdSync")]
    partial class BlacklistSmsIdSync
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("bbt.gateway.common.Models.BlackListEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PhoneConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResolvedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("SmsId")
                        .HasColumnType("bigint");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PhoneConfigurationId");

                    b.ToTable("BlackListEntries");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.BlackListEntryLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("BlackListEntryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ParameterMaster")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParameterSlave")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlackListEntryId");

                    b.ToTable("BlackListEntryLog");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.Header", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Branch")
                        .HasColumnType("int");

                    b.Property<string>("BusinessLine")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ContentType")
                        .HasColumnType("int");

                    b.Property<string>("EmailTemplatePrefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailTemplateSuffix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmsPrefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SmsSender")
                        .HasColumnType("int");

                    b.Property<string>("SmsSuffix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmsTemplatePrefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmsTemplateSuffix")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Headers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("8143fc27-43f8-406a-adfe-7f136dfab5de"),
                            ContentType = 0,
                            EmailTemplatePrefix = "generic",
                            SmsPrefix = "Dear Honey,",
                            SmsSender = 1,
                            SmsSuffix = ":)",
                            SmsTemplatePrefix = "generic"
                        },
                        new
                        {
                            Id = new Guid("9b13fde1-2dbe-4b15-bfa7-f0e02769cc9c"),
                            Branch = 2000,
                            ContentType = 0,
                            EmailTemplatePrefix = "on",
                            SmsPrefix = "OBEY:",
                            SmsSender = 2,
                            SmsTemplatePrefix = "on"
                        });
                });

            modelBuilder.Entity("bbt.gateway.common.Models.Operator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AuthToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthanticationService")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ControlDaysForOtp")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QueryService")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SendService")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("SupportDeskMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupportDeskPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TokenCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TokenExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<bool>("UseIvnWhenDeactive")
                        .HasColumnType("bit");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Operators");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ControlDaysForOtp = 60,
                            Status = 1,
                            TokenCreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TokenExpiredAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = 1,
                            UseIvnWhenDeactive = false
                        },
                        new
                        {
                            Id = 2,
                            ControlDaysForOtp = 60,
                            Status = 1,
                            TokenCreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TokenExpiredAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = 2,
                            UseIvnWhenDeactive = false
                        },
                        new
                        {
                            Id = 3,
                            ControlDaysForOtp = 60,
                            Status = 1,
                            TokenCreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TokenExpiredAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = 3,
                            UseIvnWhenDeactive = false
                        },
                        new
                        {
                            Id = 4,
                            ControlDaysForOtp = 60,
                            Status = 1,
                            TokenCreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TokenExpiredAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = 4,
                            UseIvnWhenDeactive = false
                        },
                        new
                        {
                            Id = 5,
                            ControlDaysForOtp = 60,
                            Status = 1,
                            TokenCreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TokenExpiredAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Type = 5,
                            UseIvnWhenDeactive = false
                        });
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpRequestLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PhoneConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PhoneConfigurationId");

                    b.ToTable("OtpRequestLogs");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpResponseLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Operator")
                        .HasColumnType("int");

                    b.Property<Guid?>("OtpRequestLogId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RequestBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResponseBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResponseCode")
                        .HasColumnType("int");

                    b.Property<string>("ResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusQueryId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Topic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrackingStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OtpRequestLogId");

                    b.ToTable("OtpResponseLog");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpTrackingLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Detail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LogId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("QueriedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LogId");

                    b.ToTable("OtpTrackingLog");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.PhoneConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("CustomerNo")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("Operator")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("Id"), false);

                    b.ToTable("PhoneConfigurations");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.PhoneConfigurationLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PhoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RelatedId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PhoneId");

                    b.ToTable("PhoneConfigurationLog");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.SmsLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Operator")
                        .HasColumnType("int");

                    b.Property<int>("OperatorResponseCode")
                        .HasColumnType("int");

                    b.Property<string>("OperatorResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PhoneConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PhoneConfigurationId");

                    b.ToTable("SmsLogs");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.BlackListEntry", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.PhoneConfiguration", "PhoneConfiguration")
                        .WithMany("BlacklistEntries")
                        .HasForeignKey("PhoneConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("bbt.gateway.common.Models.Process", "CreatedBy", b1 =>
                        {
                            b1.Property<Guid>("BlackListEntryId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("BlackListEntryId");

                            b1.ToTable("BlackListEntries");

                            b1.WithOwner()
                                .HasForeignKey("BlackListEntryId");
                        });

                    b.OwnsOne("bbt.gateway.common.Models.Process", "ResolvedBy", b1 =>
                        {
                            b1.Property<Guid>("BlackListEntryId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("BlackListEntryId");

                            b1.ToTable("BlackListEntries");

                            b1.WithOwner()
                                .HasForeignKey("BlackListEntryId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("PhoneConfiguration");

                    b.Navigation("ResolvedBy");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.BlackListEntryLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.BlackListEntry", "BlackListEntry")
                        .WithMany("Logs")
                        .HasForeignKey("BlackListEntryId");

                    b.OwnsOne("bbt.gateway.common.Models.Process", "CreatedBy", b1 =>
                        {
                            b1.Property<Guid>("BlackListEntryLogId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("BlackListEntryLogId");

                            b1.ToTable("BlackListEntryLog");

                            b1.WithOwner()
                                .HasForeignKey("BlackListEntryLogId");
                        });

                    b.Navigation("BlackListEntry");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpRequestLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.PhoneConfiguration", "PhoneConfiguration")
                        .WithMany("OtpLogs")
                        .HasForeignKey("PhoneConfigurationId");

                    b.OwnsOne("bbt.gateway.common.Models.Process", "CreatedBy", b1 =>
                        {
                            b1.Property<Guid>("OtpRequestLogId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("OtpRequestLogId");

                            b1.ToTable("OtpRequestLogs");

                            b1.WithOwner()
                                .HasForeignKey("OtpRequestLogId");
                        });

                    b.OwnsOne("bbt.gateway.common.Models.Phone", "Phone", b1 =>
                        {
                            b1.Property<Guid>("OtpRequestLogId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("CountryCode")
                                .HasColumnType("int");

                            b1.Property<int>("Number")
                                .HasColumnType("int");

                            b1.Property<int>("Prefix")
                                .HasColumnType("int");

                            b1.HasKey("OtpRequestLogId");

                            b1.ToTable("OtpRequestLogs");

                            b1.WithOwner()
                                .HasForeignKey("OtpRequestLogId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("Phone");

                    b.Navigation("PhoneConfiguration");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpResponseLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.OtpRequestLog", null)
                        .WithMany("ResponseLogs")
                        .HasForeignKey("OtpRequestLogId");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpTrackingLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.OtpResponseLog", "OtpResponseLog")
                        .WithMany("TrackingLogs")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OtpResponseLog");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.PhoneConfiguration", b =>
                {
                    b.OwnsOne("bbt.gateway.common.Models.Phone", "Phone", b1 =>
                        {
                            b1.Property<Guid>("PhoneConfigurationId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("CountryCode")
                                .HasColumnType("int");

                            b1.Property<int>("Number")
                                .HasColumnType("int");

                            b1.Property<int>("Prefix")
                                .HasColumnType("int");

                            b1.HasKey("PhoneConfigurationId");

                            b1.ToTable("PhoneConfigurations");

                            b1.WithOwner()
                                .HasForeignKey("PhoneConfigurationId");
                        });

                    b.Navigation("Phone");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.PhoneConfigurationLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.PhoneConfiguration", "Phone")
                        .WithMany("Logs")
                        .HasForeignKey("PhoneId");

                    b.OwnsOne("bbt.gateway.common.Models.Process", "CreatedBy", b1 =>
                        {
                            b1.Property<Guid>("PhoneConfigurationLogId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PhoneConfigurationLogId");

                            b1.ToTable("PhoneConfigurationLog");

                            b1.WithOwner()
                                .HasForeignKey("PhoneConfigurationLogId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("Phone");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.SmsLog", b =>
                {
                    b.HasOne("bbt.gateway.common.Models.PhoneConfiguration", "PhoneConfiguration")
                        .WithMany("SmsLogs")
                        .HasForeignKey("PhoneConfigurationId");

                    b.OwnsOne("bbt.gateway.common.Models.Process", "CreatedBy", b1 =>
                        {
                            b1.Property<Guid>("SmsLogId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Action")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Identity")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ItemId")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("SmsLogId");

                            b1.ToTable("SmsLogs");

                            b1.WithOwner()
                                .HasForeignKey("SmsLogId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("PhoneConfiguration");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.BlackListEntry", b =>
                {
                    b.Navigation("Logs");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpRequestLog", b =>
                {
                    b.Navigation("ResponseLogs");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.OtpResponseLog", b =>
                {
                    b.Navigation("TrackingLogs");
                });

            modelBuilder.Entity("bbt.gateway.common.Models.PhoneConfiguration", b =>
                {
                    b.Navigation("BlacklistEntries");

                    b.Navigation("Logs");

                    b.Navigation("OtpLogs");

                    b.Navigation("SmsLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
