using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouponService.API.Migrations
{
    /// <inheritdoc />
    public partial class rewardCoupon2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RewardPointsNeeded",
                table: "RewardCoupons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "RewardCoupons",
                keyColumn: "CouponId",
                keyValue: 1,
                column: "RewardPointsNeeded",
                value: 1000.0);

            migrationBuilder.UpdateData(
                table: "RewardCoupons",
                keyColumn: "CouponId",
                keyValue: 2,
                column: "RewardPointsNeeded",
                value: 2000.0);

            migrationBuilder.InsertData(
                table: "RewardCoupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount", "MinAmount", "RewardPointsNeeded" },
                values: new object[] { 3, "300FF", 30.0, 180.0, 3500.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RewardCoupons",
                keyColumn: "CouponId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "RewardPointsNeeded",
                table: "RewardCoupons");
        }
    }
}
