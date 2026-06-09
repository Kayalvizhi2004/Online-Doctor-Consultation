using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInfinityAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "reviews",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "notifications",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "doctor_profiles",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "doctor_profiles",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "consultation_sessions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "consultation_sessions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "chat_messages",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "chat_messages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "availability_slots",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "availability_slots",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "appointments",
                newName: "updated_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "started_at",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "sent_at",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "availability_slots",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "availability_slots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "created_at",
                value: new DateTime(2026, 6, 9, 1, 58, 51, 609, DateTimeKind.Utc).AddTicks(7085));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "created_at",
                value: new DateTime(2026, 6, 9, 1, 58, 51, 609, DateTimeKind.Utc).AddTicks(7089));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "created_at",
                value: new DateTime(2026, 6, 9, 1, 58, 51, 609, DateTimeKind.Utc).AddTicks(7092));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 9, 1, 58, 50, 845, DateTimeKind.Utc).AddTicks(1393), "$2a$11$cHBGQprjK2dK2IB4M5xVuuUySaDbvW.TefTjKHZ/bJFv8lSK0jS0S" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 9, 1, 58, 51, 31, DateTimeKind.Utc).AddTicks(8728), "$2a$11$UWyNSQyj3B8NvHxbLd2RZuRC1CES/7Ddz/Mw3W/YwifLs.3zecPEK" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 9, 1, 58, 51, 228, DateTimeKind.Utc).AddTicks(8546), "$2a$11$wF4j07vcXWh/MLuDN/ktJuLtNmkmFLcbWgIMtdto7qjle5bNabMI." });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 9, 1, 58, 51, 424, DateTimeKind.Utc).AddTicks(9692), "$2a$11$GeZsSB30QIPFh25ZkeUuu.L8KDjAjMY6CEF48NoyHpQGd6B5xJ2U2" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 9, 1, 58, 51, 609, DateTimeKind.Utc).AddTicks(5658), "$2a$11$utORswrYLQfTQcK5FkOPXuiPNj3he86TIywtZuNafAwIGdUNKSWu6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "reviews",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "notifications",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "doctor_profiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "doctor_profiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "consultation_sessions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "consultation_sessions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "chat_messages",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "chat_messages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "availability_slots",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "availability_slots",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "appointments",
                newName: "UpdatedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "started_at",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "consultation_sessions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "sent_at",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "availability_slots",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "availability_slots",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.UpdateData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000004"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000005"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8231), null });

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8237), null });

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(8240), null });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 55, 794, DateTimeKind.Utc).AddTicks(8407), "$2a$11$sJH5SwZo2zlJsZ8qTOvNFOktz03FV/L8tpil9R9UuVtinLPD7sqL6", null });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 55, 995, DateTimeKind.Utc).AddTicks(3832), "$2a$11$.6CewTf9N9t4JPLk13dqWeWa6ceZ31BkQde5lcmHzzcQqhMA4w9wi", null });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 196, DateTimeKind.Utc).AddTicks(3660), "$2a$11$UOGuIqqm85hKRtmUO4c7O.q8z0vy.JbhM7S.t19Ai6tXVuLYJN7Xm", null });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 388, DateTimeKind.Utc).AddTicks(6031), "$2a$11$OhCIjGMevcb86erAGyL/Muo1rkhz.5GmfEYym/yXJkuFZicAmoezm", null });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 7, 2, 43, 56, 586, DateTimeKind.Utc).AddTicks(6952), "$2a$11$5rOg7fq7ZkVfRhlvJay2YeRBQ4zqWpGDPawnZbsrXmuKKBIPvt1La", null });
        }
    }
}
