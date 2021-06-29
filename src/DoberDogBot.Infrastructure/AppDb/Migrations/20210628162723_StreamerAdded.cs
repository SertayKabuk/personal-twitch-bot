using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DoberDogBot.Infrastructure.AppDb.Migrations
{
    public partial class StreamerAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "streamers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    Channel = table.Column<string>(type: "text", nullable: false),
                    ChannelId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streamers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "streamersessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<int>(type: "integer", nullable: true),
                    PlayDelay = table.Column<int>(type: "integer", nullable: false),
                    StreamEnd = table.Column<string>(type: "text", nullable: true),
                    StreamStart = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streamersessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_streamersessions_streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalTable: "streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_streamersessions_StreamerId",
                table: "streamersessions",
                column: "StreamerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "streamersessions");

            migrationBuilder.DropTable(
                name: "streamers");
        }
    }
}
