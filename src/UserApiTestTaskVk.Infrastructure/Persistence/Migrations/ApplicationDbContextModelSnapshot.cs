﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UserApiTestTaskVk.Infrastructure.Persistence;

#nullable disable

namespace UserApiTestTaskVk.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("RevokedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("revoked_on");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_refresh_tokens");

                    b.HasIndex("Token")
                        .IsUnique()
                        .HasDatabaseName("ix_refresh_tokens_token");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_refresh_tokens_user_id");

                    b.ToTable("refresh_tokens", (string)null);

                    b.HasComment("Refresh токен");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("password_hash");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("password_salt");

                    b.Property<Guid>("UserGroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_group_id");

                    b.Property<Guid>("UserStateId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_state_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Login")
                        .IsUnique()
                        .HasDatabaseName("ix_users_login");

                    b.HasIndex("UserGroupId")
                        .HasDatabaseName("ix_users_user_group_id");

                    b.HasIndex("UserStateId")
                        .HasDatabaseName("ix_users_user_state_id");

                    b.ToTable("users", (string)null);

                    b.HasComment("Пользователи");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.UserGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<int>("Code")
                        .HasColumnType("integer")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_user_groups");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_user_groups_code");

                    b.ToTable("user_groups", (string)null);

                    b.HasComment("Группа пользователя");

                    b.HasData(
                        new
                        {
                            Id = new Guid("fbc4e285-0375-4e05-af2b-0f9fe061c886"),
                            Code = 0,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Администратор"
                        },
                        new
                        {
                            Id = new Guid("8e76c7ec-4df1-4546-b39d-870a2f0cc126"),
                            Code = 1,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Пользователь"
                        });
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.UserState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<int>("Code")
                        .HasColumnType("integer")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_user_states");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_user_states_code");

                    b.ToTable("user_states", (string)null);

                    b.HasComment("Статус пользователя");

                    b.HasData(
                        new
                        {
                            Id = new Guid("16643212-60d8-416c-b9fd-41ef7ed2721b"),
                            Code = 0,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Активный"
                        },
                        new
                        {
                            Id = new Guid("da8b202c-21ef-4b44-b358-83cb1a5c9ca3"),
                            Code = 1,
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Удаленный"
                        });
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.RefreshToken", b =>
                {
                    b.HasOne("UserApiTestTaskVk.Domain.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("fk_refresh_tokens_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.User", b =>
                {
                    b.HasOne("UserApiTestTaskVk.Domain.Entities.UserGroup", "UserGroup")
                        .WithMany("Users")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_user_groups_user_group_id");

                    b.HasOne("UserApiTestTaskVk.Domain.Entities.UserState", "UserState")
                        .WithMany("Users")
                        .HasForeignKey("UserStateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_user_states_user_state_id");

                    b.Navigation("UserGroup");

                    b.Navigation("UserState");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.UserGroup", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("UserApiTestTaskVk.Domain.Entities.UserState", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
