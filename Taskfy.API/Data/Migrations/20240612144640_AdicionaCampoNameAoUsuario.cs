﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskfy.API.Data.Migrations
{
	/// <inheritdoc />
	public partial class AdicionaCampoNameAoUsuario : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Name",
				table: "AspNetUsers",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Name",
				table: "AspNetUsers");
		}
	}
}
