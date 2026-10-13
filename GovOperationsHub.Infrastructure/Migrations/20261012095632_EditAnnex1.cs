using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovOperationsHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditAnnex1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitizenServiceCenters_ContractAnnexes_ContractAnnexId",
                table: "CitizenServiceCenters");

            migrationBuilder.DropIndex(
                name: "IX_CitizenServiceCenters_ContractAnnexId",
                table: "CitizenServiceCenters");

            migrationBuilder.DropColumn(
                name: "ContractAnnexId",
                table: "CitizenServiceCenters");

            migrationBuilder.AddColumn<int>(
                name: "TargetCenterId",
                table: "ContractAnnexes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractAnnexes_TargetCenterId",
                table: "ContractAnnexes",
                column: "TargetCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractAnnexes_CitizenServiceCenters_TargetCenterId",
                table: "ContractAnnexes",
                column: "TargetCenterId",
                principalTable: "CitizenServiceCenters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractAnnexes_CitizenServiceCenters_TargetCenterId",
                table: "ContractAnnexes");

            migrationBuilder.DropIndex(
                name: "IX_ContractAnnexes_TargetCenterId",
                table: "ContractAnnexes");

            migrationBuilder.DropColumn(
                name: "TargetCenterId",
                table: "ContractAnnexes");

            migrationBuilder.AddColumn<int>(
                name: "ContractAnnexId",
                table: "CitizenServiceCenters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitizenServiceCenters_ContractAnnexId",
                table: "CitizenServiceCenters",
                column: "ContractAnnexId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitizenServiceCenters_ContractAnnexes_ContractAnnexId",
                table: "CitizenServiceCenters",
                column: "ContractAnnexId",
                principalTable: "ContractAnnexes",
                principalColumn: "Id");
        }
    }
}
