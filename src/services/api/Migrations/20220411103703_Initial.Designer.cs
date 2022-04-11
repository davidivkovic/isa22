﻿// <auto-generated />
using System;
using System.Collections.Generic;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220411103703_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("API.Core.Model.Business", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("CancellationFee")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("NumberOfReviews")
                        .HasColumnType("integer");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UserId");

                    b.ToTable("Businesses");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Business");
                });

            modelBuilder.Entity("API.Core.Model.Loyalty", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("DiscountPercentage")
                        .HasColumnType("double precision");

                    b.Property<int>("Threshold")
                        .HasColumnType("integer");

                    b.HasKey("Name");

                    b.ToTable("LoyaltyLevels");
                });

            modelBuilder.Entity("API.Core.Model.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BusinessId")
                        .HasColumnType("uuid");

                    b.Property<double>("DiscountPercentage")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("End");

                    b.HasIndex("Start");

                    b.HasIndex("UserId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("API.Core.Model.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Approved")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("BusinessId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.Property<bool>("Rejected")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("API.Core.Model.Slot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Available")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("BusinessId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("End");

                    b.HasIndex("Start");

                    b.ToTable("Slot");
                });

            modelBuilder.Entity("API.Core.Model.Tax", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<double>("Percentage")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Tax");
                });

            modelBuilder.Entity("API.Core.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<Guid?>("BusinessId")
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("LevelName")
                        .HasColumnType("text");

                    b.Property<bool>("LockedOut")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LoyaltyPoints")
                        .HasColumnType("integer");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<List<string>>("Roles")
                        .HasColumnType("text[]");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("LevelName");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("API.Security.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("API.Core.Model.Adventure", b =>
                {
                    b.HasBaseType("API.Core.Model.Business");

                    b.Property<string>("Biography")
                        .HasColumnType("text");

                    b.Property<List<string>>("FishingEquipment")
                        .HasColumnType("text[]");

                    b.HasDiscriminator().HasValue("Adventure");
                });

            modelBuilder.Entity("API.Core.Model.Boat", b =>
                {
                    b.HasBaseType("API.Core.Model.Business");

                    b.HasDiscriminator().HasValue("Boat");
                });

            modelBuilder.Entity("API.Core.Model.Cabin", b =>
                {
                    b.HasBaseType("API.Core.Model.Business");

                    b.HasDiscriminator().HasValue("Cabin");
                });

            modelBuilder.Entity("API.Core.Model.Business", b =>
                {
                    b.HasOne("API.Core.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("API.Core.Model.User", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId");

                    b.OwnsOne("API.Core.Model.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("BusinessId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Apartment")
                                .HasColumnType("text");

                            b1.Property<string>("City")
                                .HasColumnType("text");

                            b1.Property<string>("Country")
                                .HasColumnType("text");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("text");

                            b1.Property<string>("Street")
                                .HasColumnType("text");

                            b1.HasKey("BusinessId");

                            b1.ToTable("Businesses");

                            b1.WithOwner()
                                .HasForeignKey("BusinessId");
                        });

                    b.OwnsOne("API.Core.Model.Money", "PricePerUnit", b1 =>
                        {
                            b1.Property<Guid>("BusinessId")
                                .HasColumnType("uuid");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<string>("Currency")
                                .HasColumnType("text");

                            b1.HasKey("BusinessId");

                            b1.ToTable("Businesses");

                            b1.WithOwner()
                                .HasForeignKey("BusinessId");
                        });

                    b.OwnsMany("API.Core.Model.Service", "Services", b1 =>
                        {
                            b1.Property<Guid>("BusinessId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Name")
                                .HasColumnType("text");

                            b1.HasKey("BusinessId", "Id");

                            b1.ToTable("Businesses_Services");

                            b1.WithOwner()
                                .HasForeignKey("BusinessId");

                            b1.OwnsOne("API.Core.Model.Money", "Price", b2 =>
                                {
                                    b2.Property<Guid>("ServiceBusinessId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("ServiceId")
                                        .HasColumnType("integer");

                                    b2.Property<decimal>("Amount")
                                        .HasColumnType("numeric");

                                    b2.Property<string>("Currency")
                                        .HasColumnType("text");

                                    b2.HasKey("ServiceBusinessId", "ServiceId");

                                    b2.ToTable("Businesses_Services");

                                    b2.WithOwner()
                                        .HasForeignKey("ServiceBusinessId", "ServiceId");
                                });

                            b1.Navigation("Price");
                        });

                    b.OwnsMany("API.Core.Model.Rule", "Rules", b1 =>
                        {
                            b1.Property<Guid>("BusinessId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("Allowed")
                                .HasColumnType("boolean");

                            b1.Property<string>("Name")
                                .HasColumnType("text");

                            b1.HasKey("BusinessId", "Id");

                            b1.ToTable("Rule");

                            b1.WithOwner()
                                .HasForeignKey("BusinessId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("PricePerUnit");

                    b.Navigation("Rules");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("API.Core.Model.Reservation", b =>
                {
                    b.HasOne("API.Core.Model.Business", "Business")
                        .WithMany("Reservations")
                        .HasForeignKey("BusinessId");

                    b.HasOne("API.Core.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.OwnsOne("API.Core.Model.Complaint", "Complaint", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Answer")
                                .HasColumnType("text");

                            b1.Property<bool>("Answered")
                                .HasColumnType("boolean");

                            b1.Property<string>("Reason")
                                .HasColumnType("text");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("ReservationId");

                            b1.ToTable("Complaints");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");
                        });

                    b.OwnsOne("API.Core.Model.Payment", "Payment", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uuid");

                            b1.Property<double>("DiscountPercentage")
                                .HasColumnType("double precision");

                            b1.Property<double>("TaxPercentage")
                                .HasColumnType("double precision");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("ReservationId");

                            b1.ToTable("Reservations");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");

                            b1.OwnsOne("API.Core.Model.Money", "Price", b2 =>
                                {
                                    b2.Property<Guid>("PaymentReservationId")
                                        .HasColumnType("uuid");

                                    b2.Property<decimal>("Amount")
                                        .HasColumnType("numeric");

                                    b2.Property<string>("Currency")
                                        .HasColumnType("text");

                                    b2.HasKey("PaymentReservationId");

                                    b2.ToTable("Reservations");

                                    b2.WithOwner()
                                        .HasForeignKey("PaymentReservationId");
                                });

                            b1.Navigation("Price");
                        });

                    b.OwnsOne("API.Core.Model.Report", "Report", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uuid");

                            b1.Property<bool>("IsApproved")
                                .HasColumnType("boolean");

                            b1.Property<bool>("Penalize")
                                .HasColumnType("boolean");

                            b1.Property<string>("Reason")
                                .HasColumnType("text");

                            b1.HasKey("ReservationId");

                            b1.ToTable("Reservations");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");
                        });

                    b.OwnsMany("API.Core.Model.Service", "Services", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Name")
                                .HasColumnType("text");

                            b1.HasKey("ReservationId", "Id");

                            b1.ToTable("Reservations_Services");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");

                            b1.OwnsOne("API.Core.Model.Money", "Price", b2 =>
                                {
                                    b2.Property<Guid>("ServiceReservationId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("ServiceId")
                                        .HasColumnType("integer");

                                    b2.Property<decimal>("Amount")
                                        .HasColumnType("numeric");

                                    b2.Property<string>("Currency")
                                        .HasColumnType("text");

                                    b2.HasKey("ServiceReservationId", "ServiceId");

                                    b2.ToTable("Reservations_Services");

                                    b2.WithOwner()
                                        .HasForeignKey("ServiceReservationId", "ServiceId");
                                });

                            b1.Navigation("Price");
                        });

                    b.Navigation("Business");

                    b.Navigation("Complaint");

                    b.Navigation("Payment");

                    b.Navigation("Report");

                    b.Navigation("Services");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Core.Model.Review", b =>
                {
                    b.HasOne("API.Core.Model.Business", "Business")
                        .WithMany("Reviews")
                        .HasForeignKey("BusinessId");

                    b.HasOne("API.Core.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Business");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Core.Model.Slot", b =>
                {
                    b.HasOne("API.Core.Model.Business", null)
                        .WithMany("Availability")
                        .HasForeignKey("BusinessId");
                });

            modelBuilder.Entity("API.Core.Model.User", b =>
                {
                    b.HasOne("API.Core.Model.Business", null)
                        .WithMany("Subscribers")
                        .HasForeignKey("BusinessId");

                    b.HasOne("API.Core.Model.Loyalty", "Level")
                        .WithMany()
                        .HasForeignKey("LevelName");

                    b.OwnsOne("API.Core.Model.OneTimePassword", "OneTimePassword", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<DateTimeOffset>("Expires")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Value")
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("API.Core.Model.Penalty", "Penalty", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Expires")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<int>("Points")
                                .HasColumnType("integer");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("API.Core.Model.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Apartment")
                                .HasColumnType("text");

                            b1.Property<string>("City")
                                .HasColumnType("text");

                            b1.Property<string>("Country")
                                .HasColumnType("text");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("text");

                            b1.Property<string>("Street")
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("API.Core.Model.Request", "DeletionRequest", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Answer")
                                .HasColumnType("text");

                            b1.Property<bool>("Approved")
                                .HasColumnType("boolean");

                            b1.Property<string>("Reason")
                                .HasColumnType("text");

                            b1.Property<bool>("Rejected")
                                .HasColumnType("boolean");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("API.Core.Model.Request", "JoinRequest", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Answer")
                                .HasColumnType("text");

                            b1.Property<bool>("Approved")
                                .HasColumnType("boolean");

                            b1.Property<string>("Reason")
                                .HasColumnType("text");

                            b1.Property<bool>("Rejected")
                                .HasColumnType("boolean");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("DeletionRequest");

                    b.Navigation("JoinRequest");

                    b.Navigation("Level");

                    b.Navigation("OneTimePassword");

                    b.Navigation("Penalty");
                });

            modelBuilder.Entity("API.Security.RefreshToken", b =>
                {
                    b.HasOne("API.Core.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("API.Core.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("API.Core.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Core.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API.Core.Model.Boat", b =>
                {
                    b.OwnsOne("API.Core.Model.BoatCharacteristics", "Characteristics", b1 =>
                        {
                            b1.Property<Guid>("BoatId")
                                .HasColumnType("uuid");

                            b1.Property<int>("BHP")
                                .HasColumnType("integer");

                            b1.Property<int>("Engines")
                                .HasColumnType("integer");

                            b1.Property<int>("Length")
                                .HasColumnType("integer");

                            b1.Property<int>("Seats")
                                .HasColumnType("integer");

                            b1.Property<int>("TopSpeed")
                                .HasColumnType("integer");

                            b1.HasKey("BoatId");

                            b1.ToTable("Businesses");

                            b1.WithOwner()
                                .HasForeignKey("BoatId");
                        });

                    b.OwnsOne("API.Core.Model.BoatEquipment", "Equipment", b1 =>
                        {
                            b1.Property<Guid>("BoatId")
                                .HasColumnType("uuid");

                            b1.Property<List<string>>("Fishing")
                                .HasColumnType("text[]");

                            b1.Property<List<string>>("Navigational")
                                .HasColumnType("text[]");

                            b1.HasKey("BoatId");

                            b1.ToTable("Businesses");

                            b1.WithOwner()
                                .HasForeignKey("BoatId");
                        });

                    b.Navigation("Characteristics");

                    b.Navigation("Equipment")
                        .IsRequired();
                });

            modelBuilder.Entity("API.Core.Model.Cabin", b =>
                {
                    b.OwnsMany("API.Core.Model.Room", "Rooms", b1 =>
                        {
                            b1.Property<Guid>("CabinId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("Beds")
                                .HasColumnType("integer");

                            b1.HasKey("CabinId", "Id");

                            b1.ToTable("Room");

                            b1.WithOwner()
                                .HasForeignKey("CabinId");
                        });

                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("API.Core.Model.Business", b =>
                {
                    b.Navigation("Availability");

                    b.Navigation("Reservations");

                    b.Navigation("Reviews");

                    b.Navigation("Subscribers");
                });

            modelBuilder.Entity("API.Core.Model.User", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}