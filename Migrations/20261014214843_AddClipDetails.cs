using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClipDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "HeroSlides",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            // Backfill slugs for existing video clips so public /clips/{slug}
            // links resolve immediately after deploy. Images keep a NULL slug.
            migrationBuilder.Sql(@"
DO $$
DECLARE
    rec RECORD;
    base_slug text;
    candidate text;
    suffix int;
BEGIN
    FOR rec IN
        SELECT ""Id"", ""Title""
        FROM ""HeroSlides""
        WHERE ""MediaType"" = 'Video' AND ""Slug"" IS NULL
        ORDER BY ""Id""
    LOOP
        base_slug := lower(coalesce(rec.""Title"", ''));
        base_slug := regexp_replace(base_slug, '[^a-z0-9\s-]', '', 'g');
        base_slug := regexp_replace(base_slug, '\s+', '-', 'g');
        base_slug := regexp_replace(base_slug, '-+', '-', 'g');
        base_slug := trim(both '-' from base_slug);
        IF base_slug = '' THEN
            base_slug := 'clip';
        END IF;

        candidate := base_slug;
        suffix := 1;
        WHILE EXISTS (SELECT 1 FROM ""HeroSlides"" WHERE ""Slug"" = candidate) LOOP
            suffix := suffix + 1;
            candidate := base_slug || '-' || suffix;
        END LOOP;

        UPDATE ""HeroSlides"" SET ""Slug"" = candidate WHERE ""Id"" = rec.""Id"";
    END LOOP;
END $$;
");

            migrationBuilder.CreateTable(
                name: "HeroSlideBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeroSlideId = table.Column<int>(type: "integer", nullable: false),
                    BlockType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSlideBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSlideBlocks_HeroSlides_HeroSlideId",
                        column: x => x.HeroSlideId,
                        principalTable: "HeroSlides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroSlides_Slug",
                table: "HeroSlides",
                column: "Slug",
                unique: true,
                filter: "\"Slug\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSlideBlocks_HeroSlideId",
                table: "HeroSlideBlocks",
                column: "HeroSlideId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroSlideBlocks");

            migrationBuilder.DropIndex(
                name: "IX_HeroSlides_Slug",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "HeroSlides");
        }
    }
}
