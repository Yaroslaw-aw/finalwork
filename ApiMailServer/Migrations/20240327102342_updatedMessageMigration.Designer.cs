﻿// <auto-generated />
using System;
using ApiMailServer.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiMailServer.Migrations
{
    [DbContext(typeof(MessageContext))]
    [Migration("20240327102342_updatedMessageMigration")]
    partial class updatedMessageMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ApiMailServer.Db.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConsumerId")
                        .HasColumnType("uuid")
                        .HasColumnName("consumer_id");

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("ProducerEmail")
                        .HasColumnType("text")
                        .HasColumnName("producer_email");

                    b.Property<Guid>("ProducerId")
                        .HasColumnType("uuid")
                        .HasColumnName("producer_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("message_pkey");

                    b.ToTable("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
