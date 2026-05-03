using Godot;
using System;

public static partial class TilemapToSpriteGenerator
{
    public static Sprite2D ConvertToSprite(TileMapLayer layer)
    {
        Rect2I rect = layer.GetUsedRect();
        Vector2I tileSize = layer.TileSet.TileSize;

        Image combinedImage = Image.CreateEmpty(
            rect.Size.X * tileSize.X,
            rect.Size.Y * tileSize.Y,
            false,
            Image.Format.Rgba8
        );

        foreach (Vector2I coords in layer.GetUsedCells())
        {
            // Get the source tile's texture/region
            TileData data = layer.GetCellTileData(coords);
            if (data == null) continue;

            int sourceId = layer.GetCellSourceId(coords);
            Vector2I atlasCoords = layer.GetCellAtlasCoords(coords);

            TileSetAtlasSource source = (TileSetAtlasSource)layer.TileSet.GetSource(sourceId);
            Image atlasImage = source.Texture.GetImage();
            Rect2I region = source.GetTileTextureRegion(atlasCoords);

            // Calculate where to "stamp" this tile on our combined image
            Vector2I destination = new Vector2I(
                (coords.X - rect.Position.X) * tileSize.X,
                (coords.Y - rect.Position.Y) * tileSize.Y
            );

            // 4. Blit (copy) the tile pixel data onto the main image
            combinedImage.BlitRect(atlasImage, region, destination);
        }

        ImageTexture finalTexture = ImageTexture.CreateFromImage(combinedImage);

        Sprite2D resultSprite = new Sprite2D();
        resultSprite.Texture = finalTexture;

        // Adjust the sprite position to match where the tilemap was
        resultSprite.GlobalPosition = layer.MapToLocal(rect.Position) + (Vector2)(rect.Size * tileSize) / 2.0f;

        layer.QueueFree();

        return resultSprite;
    }
}