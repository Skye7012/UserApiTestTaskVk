using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApiTestTaskVk.Infrastructure.Persistence.Migrations
{
    public partial class fix_initial_const_entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_groups",
                keyColumn: "id",
                keyValue: new Guid("8e76c7ec-4df1-4546-b39d-870a2f0cc126"),
                column: "created_date",
                value: new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "user_groups",
                keyColumn: "id",
                keyValue: new Guid("fbc4e285-0375-4e05-af2b-0f9fe061c886"),
                column: "created_date",
                value: new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "user_states",
                keyColumn: "id",
                keyValue: new Guid("16643212-60d8-416c-b9fd-41ef7ed2721b"),
                column: "created_date",
                value: new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "user_states",
                keyColumn: "id",
                keyValue: new Guid("da8b202c-21ef-4b44-b358-83cb1a5c9ca3"),
                column: "created_date",
                value: new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_groups",
                keyColumn: "id",
                keyValue: new Guid("8e76c7ec-4df1-4546-b39d-870a2f0cc126"),
                column: "created_date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "user_groups",
                keyColumn: "id",
                keyValue: new Guid("fbc4e285-0375-4e05-af2b-0f9fe061c886"),
                column: "created_date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "user_states",
                keyColumn: "id",
                keyValue: new Guid("16643212-60d8-416c-b9fd-41ef7ed2721b"),
                column: "created_date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "user_states",
                keyColumn: "id",
                keyValue: new Guid("da8b202c-21ef-4b44-b358-83cb1a5c9ca3"),
                column: "created_date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
