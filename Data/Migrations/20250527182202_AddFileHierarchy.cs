using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileManagerApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "Files",
                newName: "PhysicalPath");

            migrationBuilder.RenameColumn(
                name: "StoragePath",
                table: "Files",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Files",
                newName: "Modified");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "Files",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectory",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Files",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ParentId",
                table: "Files",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Files_ParentId",
                table: "Files",
                column: "ParentId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Files_ParentId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ParentId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsDirectory",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "PhysicalPath",
                table: "Files",
                newName: "UploadDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Files",
                newName: "StoragePath");

            migrationBuilder.RenameColumn(
                name: "Modified",
                table: "Files",
                newName: "FileName");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
