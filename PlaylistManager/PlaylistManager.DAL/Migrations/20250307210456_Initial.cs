using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultimediaBaseEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Duration = table.Column<double>(type: "REAL", nullable: true),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    Format = table.Column<int>(type: "INTEGER", nullable: true),
                    Genre = table.Column<int>(type: "INTEGER", nullable: true),
                    MusicEntity_Genre = table.Column<int>(type: "INTEGER", nullable: true),
                    VideoMediaEntity_Format = table.Column<int>(type: "INTEGER", nullable: true),
                    VideoMediaEntity_Genre = table.Column<int>(type: "INTEGER", nullable: true),
                    Resolution = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultimediaBaseEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistMultimedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlaylistId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MultimediaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistMultimedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistMultimedia_MultimediaBaseEntities_MultimediaId",
                        column: x => x.MultimediaId,
                        principalTable: "MultimediaBaseEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistMultimedia_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistMultimedia_MultimediaId",
                table: "PlaylistMultimedia",
                column: "MultimediaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistMultimedia_PlaylistId",
                table: "PlaylistMultimedia",
                column: "PlaylistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistMultimedia");

            migrationBuilder.DropTable(
                name: "MultimediaBaseEntities");

            migrationBuilder.DropTable(
                name: "Playlists");
        }
    }
}
