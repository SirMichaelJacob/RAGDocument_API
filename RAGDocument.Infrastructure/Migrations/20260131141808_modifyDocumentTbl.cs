using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAGDocument.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifyDocumentTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContentHash",
                table: "Documents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ContentHash",
                table: "Documents",
                type: "varbinary(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
