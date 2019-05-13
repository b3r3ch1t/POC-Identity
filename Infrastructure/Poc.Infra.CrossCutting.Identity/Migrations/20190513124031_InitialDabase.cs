using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Poc.Infra.CrossCutting.Identity.Migrations
{
    public partial class InitialDabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationRole",
                columns: table => new
                {
                    role_id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRole", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "hd_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hd_users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Valid = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ProfilePicture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "hd_roleclaims",
                columns: table => new
                {
                    roleclaim_id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_roleclaims", x => x.roleclaim_id);
                    table.ForeignKey(
                        name: "FK_hd_roleclaims_hd_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "hd_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hd_userclaims",
                columns: table => new
                {
                    userclaim_id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_userclaims", x => x.userclaim_id);
                    table.ForeignKey(
                        name: "FK_hd_userclaims_hd_users_UserId",
                        column: x => x.UserId,
                        principalTable: "hd_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hd_userlogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_userlogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_hd_userlogins_hd_users_user_id",
                        column: x => x.user_id,
                        principalTable: "hd_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hd_userroles",
                columns: table => new
                {
                    userId_id = table.Column<Guid>(nullable: false),
                    userrole_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_userroles", x => new { x.userId_id, x.userrole_id });
                    table.ForeignKey(
                        name: "FK_hd_userroles_hd_roles_userrole_id",
                        column: x => x.userrole_id,
                        principalTable: "hd_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hd_userroles_hd_users_userId_id",
                        column: x => x.userId_id,
                        principalTable: "hd_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hd_usertokens",
                columns: table => new
                {
                    usertoken_id = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hd_usertokens", x => new { x.usertoken_id, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_hd_usertokens_hd_users_usertoken_id",
                        column: x => x.usertoken_id,
                        principalTable: "hd_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hd_roleclaims_RoleId",
                table: "hd_roleclaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "hd_roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hd_userclaims_UserId",
                table: "hd_userclaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_hd_userlogins_user_id",
                table: "hd_userlogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_hd_userroles_userrole_id",
                table: "hd_userroles",
                column: "userrole_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "hd_users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "hd_users",
                column: "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationRole");

            migrationBuilder.DropTable(
                name: "hd_roleclaims");

            migrationBuilder.DropTable(
                name: "hd_userclaims");

            migrationBuilder.DropTable(
                name: "hd_userlogins");

            migrationBuilder.DropTable(
                name: "hd_userroles");

            migrationBuilder.DropTable(
                name: "hd_usertokens");

            migrationBuilder.DropTable(
                name: "hd_roles");

            migrationBuilder.DropTable(
                name: "hd_users");
        }
    }
}
