using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedatabasedesignUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Publisher_publisher_id",
                table: "Article");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Publisher");

            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "publisher_id",
                table: "Article",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Article_publisher_id",
                table: "Article",
                newName: "IX_Article_user_id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, "Admin", "ADMIN" },
                    { 2, null, "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_user_id",
                table: "Article",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_user_id",
                table: "Article");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Article",
                newName: "publisher_id");

            migrationBuilder.RenameIndex(
                name: "IX_Article_user_id",
                table: "Article",
                newName: "IX_Article_publisher_id");

            migrationBuilder.AddColumn<string>(
                name: "RoleType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("admin_id", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_Admin_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                columns: table => new
                {
                    publisher_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    profile_pic_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("publisher_id", x => x.publisher_id);
                    table.ForeignKey(
                        name: "FK_Publisher_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_user_id",
                table: "Admin",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publisher_user_id",
                table: "Publisher",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Publisher_publisher_id",
                table: "Article",
                column: "publisher_id",
                principalTable: "Publisher",
                principalColumn: "publisher_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
