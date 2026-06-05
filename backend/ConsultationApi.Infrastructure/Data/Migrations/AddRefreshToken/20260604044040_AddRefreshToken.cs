using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationApi.Infrastructure.Data.Migrations.AddRefreshToken
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 4, 40, 39, 648, DateTimeKind.Utc).AddTicks(7069));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 4, 40, 39, 648, DateTimeKind.Utc).AddTicks(7071));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 4, 40, 39, 648, DateTimeKind.Utc).AddTicks(7073));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 4, 4, 40, 39, 175, DateTimeKind.Utc).AddTicks(2442), "$2a$11$tcdwMoxG.4UqVTrP/JfGge19s8auA12ssTblPwyObMUa.tyQ0yk9C" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 4, 4, 40, 39, 291, DateTimeKind.Utc).AddTicks(5734), "$2a$11$.ArmVbnTqnzKKASN85m5be.tyYDeUszPnEPPdhvlltV7o/51nBQZa" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 4, 4, 40, 39, 408, DateTimeKind.Utc).AddTicks(7743), "$2a$11$SBqk/MYkdURaToCcs807H.rpTBfFU7fbMopDljpNt0hk3C4L.d9Vy" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 4, 4, 40, 39, 529, DateTimeKind.Utc).AddTicks(1056), "$2a$11$Js0VJbZ/a.sDkStQrAZDJ./FH1FGwWqEr8M5/9nAnnyuKJJktPe16" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 4, 4, 40, 39, 648, DateTimeKind.Utc).AddTicks(6439), "$2a$11$urpk1Wzi1xV1qlgc.EjXl.OEIiHtWGHUudsMPariTPuSgCGTI4BbW" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId1",
                table: "refresh_tokens",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5453));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5459));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5462));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 3, 14, 2, 35, 769, DateTimeKind.Utc).AddTicks(307), "$2a$11$e3mb32XEXo09EqXZEDIMmupwOaXFU.cO5qxVg9c6sMED8qUjsfmW6" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 3, 14, 2, 36, 57, DateTimeKind.Utc).AddTicks(4921), "$2a$11$znVgmPomBDSJmjFc4ZNM2.4n0WoKsys38m7LgedVBuuXjOqxT41qW" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 3, 14, 2, 36, 332, DateTimeKind.Utc).AddTicks(210), "$2a$11$3a.loj3b9UCCLTjqVMSu0u.gTe9OL9aSvXqcXWw/lzCU2R51vB0Cy" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 3, 14, 2, 36, 594, DateTimeKind.Utc).AddTicks(3943), "$2a$11$lhecCAAjcu14EhNsT2wOX.w5t2TmWxmiUJMUIClz.WOtJH6kV1Bz2" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(3861), "$2a$11$giOfobtffAT1SM8PsViYwetZA.Wa/FVBIL/VSCQodRepatf1joPbK" });
        }
    }
}
