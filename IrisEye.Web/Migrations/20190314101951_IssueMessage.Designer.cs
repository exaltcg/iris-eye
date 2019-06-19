﻿// <auto-generated />
using System;
using IrisEye.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IrisEye.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190314101951_IssueMessage")]
    partial class IssueMessage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("IrisEye.Core.Entities.Folder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Run", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedOn");

                    b.Property<string>("BetaChannel");

                    b.Property<int>("Blocked");

                    b.Property<string>("Build")
                        .IsRequired();

                    b.Property<string>("Environment")
                        .IsRequired();

                    b.Property<int>("Errors");

                    b.Property<int>("Failed");

                    b.Property<string[]>("FailedTests")
                        .IsRequired();

                    b.Property<long?>("FolderId");

                    b.Property<int>("Passed");

                    b.Property<string>("ReportHash")
                        .IsRequired();

                    b.Property<DateTime>("ReportTime");

                    b.Property<int>("Skipped");

                    b.Property<int>("Total");

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Runs");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Step", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPassed");

                    b.Property<DateTime>("Issued");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Stacktrace");

                    b.Property<long?>("TestId");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.SystemUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EntityId")
                        .IsRequired();

                    b.Property<string>("GitHubAccount");

                    b.Property<string[]>("GithubAliases");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long?>("SelectedFolderId");

                    b.HasKey("Id");

                    b.HasIndex("SelectedFolderId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Test", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long?>("RunId");

                    b.Property<string>("SuiteName");

                    b.HasKey("Id");

                    b.HasIndex("RunId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestCase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("AssigneeId");

                    b.Property<DateTime>("FinishedOn");

                    b.Property<int>("GitHubId");

                    b.Property<bool>("IsIssue");

                    b.Property<DateTime?>("MergedDate");

                    b.Property<string>("Message");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long?>("ReviewerId");

                    b.Property<DateTime>("StartedOn");

                    b.Property<int>("Status");

                    b.Property<int>("TestRailId");

                    b.Property<long?>("TestSuiteId");

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("ReviewerId");

                    b.HasIndex("TestSuiteId");

                    b.ToTable("TestCases");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestCaseHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedOn");

                    b.Property<long?>("AuthorId");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<long?>("TestCaseId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TestCaseId");

                    b.ToTable("TestCaseHistoryItems");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestInfo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorGitHubId");

                    b.Property<string>("AuthorLogin")
                        .IsRequired();

                    b.Property<string>("SuiteName")
                        .IsRequired();

                    b.Property<string>("TestName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("TestInfos");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestStep", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Actual")
                        .IsRequired();

                    b.Property<string>("Expected")
                        .IsRequired();

                    b.Property<int>("SortIndex");

                    b.Property<long?>("TestCaseId");

                    b.HasKey("Id");

                    b.HasIndex("TestCaseId");

                    b.ToTable("TestSteps");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestSuite", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedOn");

                    b.Property<int>("GitHubProjectId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("TestRailId");

                    b.HasKey("Id");

                    b.ToTable("TestSuites");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Run", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.Folder", "Folder")
                        .WithMany("Runs")
                        .HasForeignKey("FolderId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Step", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.Test", "Test")
                        .WithMany("Steps")
                        .HasForeignKey("TestId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.SystemUser", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.Folder", "SelectedFolder")
                        .WithMany()
                        .HasForeignKey("SelectedFolderId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.Test", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.Run", "Run")
                        .WithMany("Tests")
                        .HasForeignKey("RunId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestCase", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.SystemUser", "Assignee")
                        .WithMany()
                        .HasForeignKey("AssigneeId");

                    b.HasOne("IrisEye.Core.Entities.SystemUser", "Reviewer")
                        .WithMany()
                        .HasForeignKey("ReviewerId");

                    b.HasOne("IrisEye.Core.Entities.TestSuite", "TestSuite")
                        .WithMany("TestCases")
                        .HasForeignKey("TestSuiteId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestCaseHistory", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.SystemUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("IrisEye.Core.Entities.TestCase")
                        .WithMany("HistoryItems")
                        .HasForeignKey("TestCaseId");
                });

            modelBuilder.Entity("IrisEye.Core.Entities.TestStep", b =>
                {
                    b.HasOne("IrisEye.Core.Entities.TestCase")
                        .WithMany("TestSteps")
                        .HasForeignKey("TestCaseId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
