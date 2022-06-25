using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogViewer.Migrations
{
    public partial class AddLogStringDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedAtDateString",
                table: "LogStrings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtDateString",
                table: "LogStrings");
        }
    }
}
