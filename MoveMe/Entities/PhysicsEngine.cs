using CocosSharp;
using System.Collections.Generic;

namespace MoveMe.Entities
{
    class PhysicsEngine
    {
        public CCTileMap Tilemap
        {
            get;
            private set;
        }

        public List<CCRect> GroundTiles
        {
            get;
            private set;
        }

        public PhysicsEngine(string mapname)
        {
            this.Tilemap = new CCTileMap("maps/" + mapname + ".tmx");
            this.GroundTiles = GetGroundTiles(this.Tilemap);
        }

        public void LevelCollision(List<CCRect> groundTiles, AnimatedEntity entity)
        {
            foreach(var tile in groundTiles)
            {
                if(Intersects(tile, entity))
                {
                    entity.velocityY = 0;
                }
            }
        }

        bool Intersects(CCRect tile, AnimatedEntity entity)
        {
            return tile.IntersectsRect(entity.BoundingBoxWorld);
        }

        List<CCRect> GetGroundTiles(CCTileMap tileMap)
        {
            // Width and Height are equal so we can use either
            List<CCRect> rectangles = new List<CCRect>();
            int tileDimension = (int)tileMap.TileTexelSize.Width;

            // Find out how many rows and columns are in our tile map
            int numberOfColumns = (int)tileMap.MapDimensions.Size.Width;
            int numberOfRows = (int)tileMap.MapDimensions.Size.Height;

            // Tile maps can have multiple layers, so let's loop through all of them:
            foreach (CCTileMapLayer layer in tileMap.TileLayersContainer.Children)
            {
                // Loop through the columns and rows to find all tiles
                for (int column = 0; column < numberOfColumns; column++)
                {
                    // We're going to add tileDimension / 2 to get the position
                    // of the center of the tile - this will help us in 
                    // positioning entities, and will eliminate the possibility
                    // of floating point error when calculating the nearest tile:
                    int worldX = tileDimension * column + tileDimension / 2;
                    for (int row = 0; row < numberOfRows; row++)
                    {
                        // See above on why we add tileDimension / 2
                        int worldY = tileDimension * row + tileDimension / 2;

                        var tile = GetGroundTileRect(worldX, worldY, layer);
                        rectangles.Add(tile);

                    }
                }
            }
            return rectangles;
        }

        CCRect GetGroundTileRect(int worldX, int worldY, CCTileMapLayer layer)
        {
            CCTileMapCoordinates tileAtXy = layer.ClosestTileCoordAtNodePosition(new CCPoint(worldX, worldY));

            CCTileGidAndFlags info = layer.TileGIDAndFlags(tileAtXy.Column, tileAtXy.Row);

            if (info != null)
            {
                Dictionary<string, string> properties = null;

                try
                {
                    properties = Tilemap.TilePropertiesForGID(info.Gid);
                }
                catch { }

                if (properties != null && properties.ContainsKey("type") && properties["type"] == "ground")
                {
                    return new CCRect(worldX - 8, worldY - 8, 16, 16);
                }
                return new CCRect();
            }
            return new CCRect();
        }
    }
}