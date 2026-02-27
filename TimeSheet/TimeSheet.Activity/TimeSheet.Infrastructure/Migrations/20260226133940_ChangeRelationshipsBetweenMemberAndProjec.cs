using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSheet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipsBetweenMemberAndProjec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLeads_Members_MemberId1",
                table: "ProjectLeads");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLeads_Projects_ProjectId1",
                table: "ProjectLeads");

            migrationBuilder.DropIndex(
                name: "IX_ProjectLeads_MemberId1",
                table: "ProjectLeads");

            migrationBuilder.DropIndex(
                name: "IX_ProjectLeads_ProjectId1",
                table: "ProjectLeads");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "ProjectLeads");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "ProjectLeads");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CurrentLeadId",
                table: "Projects",
                column: "CurrentLeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Members_CurrentLeadId",
                table: "Projects",
                column: "CurrentLeadId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Members_CurrentLeadId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CurrentLeadId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberId1",
                table: "ProjectLeads",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId1",
                table: "ProjectLeads",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_MemberId1",
                table: "ProjectLeads",
                column: "MemberId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_ProjectId1",
                table: "ProjectLeads",
                column: "ProjectId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLeads_Members_MemberId1",
                table: "ProjectLeads",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLeads_Projects_ProjectId1",
                table: "ProjectLeads",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
