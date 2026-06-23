using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "attachment_url",
                table: "chat_messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "message_type",
                table: "chat_messages",
                type: "text",
                nullable: false,
                defaultValue: "text");

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "created_at",
                value: new DateTime(2026, 6, 20, 11, 1, 51, 26, DateTimeKind.Utc).AddTicks(9894));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "created_at",
                value: new DateTime(2026, 6, 20, 11, 1, 51, 26, DateTimeKind.Utc).AddTicks(9994));

            migrationBuilder.UpdateData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "created_at",
                value: new DateTime(2026, 6, 20, 11, 1, 51, 26, DateTimeKind.Utc).AddTicks(9996));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 20, 11, 1, 50, 308, DateTimeKind.Utc).AddTicks(5854), "$2a$11$xYIScWCiD0qOYmptsQQmJOwAW0rbTGOnvhR7upa4PFFOPW2BHqb3G" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 20, 11, 1, 50, 487, DateTimeKind.Utc).AddTicks(3936), "$2a$11$P2UXMbGc8skzpAIWs2Ll..8SvATL0fUjEanrUTAup.plC/osEw/k2" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 20, 11, 1, 50, 674, DateTimeKind.Utc).AddTicks(2474), "$2a$11$Da.Q4R31BDvosMz2mcJBk.iHLXTkFh.vSAfUHS3Ld7P7E4eF4YKYS" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 20, 11, 1, 50, 852, DateTimeKind.Utc).AddTicks(4870), "$2a$11$tqxoDxHhSELbc87fofAMhO2YqM7lIApBw6iuNJNlZG0s6DIKkneke" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2026, 6, 20, 11, 1, 51, 26, DateTimeKind.Utc).AddTicks(8894), "$2a$11$/cG/DXoenKWktMc2iWBtg.R1uckne9D/uqx14yfL6frBV7MjDm3E2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attachment_url",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "message_type",
                table: "chat_messages");

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
    }
}
