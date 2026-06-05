using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConsultationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "created_at", "email", "full_name", "password_hash", "phone", "role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 6, 3, 14, 2, 35, 769, DateTimeKind.Utc).AddTicks(307), "patient1@test.com", "John Patient", "$2a$11$e3mb32XEXo09EqXZEDIMmupwOaXFU.cO5qxVg9c6sMED8qUjsfmW6", "9000000001", "Patient", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 6, 3, 14, 2, 36, 57, DateTimeKind.Utc).AddTicks(4921), "patient2@test.com", "Mary Patient", "$2a$11$znVgmPomBDSJmjFc4ZNM2.4n0WoKsys38m7LgedVBuuXjOqxT41qW", "9000000002", "Patient", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 6, 3, 14, 2, 36, 332, DateTimeKind.Utc).AddTicks(210), "doctor1@test.com", "Dr Arun Kumar", "$2a$11$3a.loj3b9UCCLTjqVMSu0u.gTe9OL9aSvXqcXWw/lzCU2R51vB0Cy", "9000000003", "Doctor", null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 6, 3, 14, 2, 36, 594, DateTimeKind.Utc).AddTicks(3943), "doctor2@test.com", "Dr Priya Sharma", "$2a$11$lhecCAAjcu14EhNsT2wOX.w5t2TmWxmiUJMUIClz.WOtJH6kV1Bz2", "9000000004", "Doctor", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(3861), "doctor3@test.com", "Dr Rajesh Singh", "$2a$11$giOfobtffAT1SM8PsViYwetZA.Wa/FVBIL/VSCQodRepatf1joPbK", "9000000005", "Doctor", null }
                });

            migrationBuilder.InsertData(
                table: "doctor_profiles",
                columns: new[] { "Id", "bio", "consultation_fee", "CreatedAt", "is_available", "specialization", "UpdatedAt", "user_id" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Heart specialist", 500m, new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5453), true, "Cardiology", null, new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Skin specialist", 400m, new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5459), true, "Dermatology", null, new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Senior cardiologist", 700m, new DateTime(2026, 6, 3, 14, 2, 36, 849, DateTimeKind.Utc).AddTicks(5462), true, "Cardiology", null, new Guid("55555555-5555-5555-5555-555555555555") }
                });

            migrationBuilder.InsertData(
                table: "availability_slots",
                columns: new[] { "Id", "CreatedAt", "date", "doctor_id", "end_time", "is_booked", "start_time", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 6, 10), new Guid("66666666-6666-6666-6666-666666666666"), new TimeOnly(10, 30, 0), false, new TimeOnly(10, 0, 0), null },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 6, 10), new Guid("66666666-6666-6666-6666-666666666666"), new TimeOnly(11, 30, 0), false, new TimeOnly(11, 0, 0), null },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 6, 11), new Guid("77777777-7777-7777-7777-777777777777"), new TimeOnly(9, 30, 0), false, new TimeOnly(9, 0, 0), null },
                    { new Guid("90000000-0000-0000-0000-000000000004"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 6, 11), new Guid("88888888-8888-8888-8888-888888888888"), new TimeOnly(2, 30, 0), false, new TimeOnly(2, 0, 0), null },
                    { new Guid("90000000-0000-0000-0000-000000000005"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 6, 12), new Guid("88888888-8888-8888-8888-888888888888"), new TimeOnly(4, 30, 0), false, new TimeOnly(4, 0, 0), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "availability_slots",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "doctor_profiles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
