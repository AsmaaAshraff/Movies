using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies_website_project.Migrations
{
    /// <inheritdoc />
    public partial class migrationn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovie_actors_ActorsId",
                table: "ActorMovie");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "actors",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "actors",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ActorsId",
                table: "ActorMovie",
                newName: "Actorsid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageByteArray",
                table: "Movies",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovie_actors_Actorsid",
                table: "ActorMovie",
                column: "Actorsid",
                principalTable: "actors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorMovie_actors_Actorsid",
                table: "ActorMovie");

            migrationBuilder.DropColumn(
                name: "ImageByteArray",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "actors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "actors",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Actorsid",
                table: "ActorMovie",
                newName: "ActorsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorMovie_actors_ActorsId",
                table: "ActorMovie",
                column: "ActorsId",
                principalTable: "actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
