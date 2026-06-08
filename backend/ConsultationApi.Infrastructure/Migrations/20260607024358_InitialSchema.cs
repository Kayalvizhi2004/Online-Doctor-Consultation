using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_messages_consultation_sessions_SessionId1",
                table: "chat_messages");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId1",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_UserId1",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_chat_messages_SessionId1",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "SessionId1",
                table: "chat_messages");

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8231));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8237));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8240));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 55, 794, DateTimeKind.Utc).AddTicks(8407), "$2a$11$sJH5SwZo2zlJsZ8qTOvNFOktz03FV/L8tpil9R9UuVtinLPD7sqL6" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 55, 995, DateTimeKind.Utc).AddTicks(3832), "$2a$11$.6CewTf9N9t4JPLk13dqWeWa6ceZ31BkQde5lcmHzzcQqhMA4w9wi" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 196, DateTimeKind.Utc).AddTicks(3660), "$2a$11$UOGuIqqm85hKRtmUO4c7O.q8z0vy.JbhM7S.t19Ai6tXVuLYJN7Xm" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 388, DateTimeKind.Utc).AddTicks(6031), "$2a$11$OhCIjGMevcb86erAGyL/Muo1rkhz.5GmfEYym/yXJkuFZicAmoezm" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(6952), "$2a$11$5rOg7fq7ZkVfRhlvJay2YeRBQ4zqWpGDPawnZbsrXmuKKBIPvt1La" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "refresh_tokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId1",
                table: "chat_messages",
                type: "uuid",
                nullable: true);

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
                name: "IX_refresh_tokens_UserId1",
                table: "refresh_tokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_SessionId1",
                table: "chat_messages",
                column: "SessionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_chat_messages_consultation_sessions_SessionId1",
                table: "chat_messages",
                column: "SessionId1",
                principalTable: "consultation_sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId1",
                table: "refresh_tokens",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
