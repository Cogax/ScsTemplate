﻿// <auto-generated />
using System;

using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Migrations
{
    [DbContext(typeof(WriteModelDbContext))]
    [Migration("20220620172606_AddTodoItem")]
    partial class AddTodoItem
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates.TodoItem", b =>
                {
                    b.Property<Guid>("id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<bool>("completed")
                        .HasColumnType("bit")
                        .HasColumnName("Completed");

                    b.Property<string>("label")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)")
                        .HasColumnName("Label");

                    b.HasKey("id");

                    b.ToTable("TodoItems", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
