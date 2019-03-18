using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using MoveMe.Entities;

namespace MoveMe
{
    public class IntroLayer : CCLayer
    {
        CCTileMap tilemap;
        Player player = new Player();

        public IntroLayer() 
        {
            
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
            tilemap = new CCTileMap("maps/map1.tmx");
            tilemap.Antialiased = false;
            this.AddChild(tilemap);
            
            player.sprite.Position = new CCPoint(20, 74);
            this.AddChild(player.sprite);
            player.sprite.AddAction(player.idleRight.Action);
            bool intersects = false;
            foreach(CCRect rect in GetGroundTiles(tilemap)){
                if (HandleWorldCollision(rect))
                {
                    intersects = true;
                }
            }
            
            CCLabel label = new CCLabel(intersects.ToString(), "fonts/MarkerFelt", 22, CCLabelFormat.SpriteFont);
            label.Position = new CCPoint(40, 100);
            this.AddChild(label);
            
        }
        bool HandleWorldCollision(CCRect tile)
        {
            return tile.IntersectsRect(player.BoundingBoxWorld);
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

                        var tile = HandleCustomTilePropertyAt(worldX, worldY, layer);
                        rectangles.Add(tile);

                    }
                }
            }
            return rectangles;
        }

        CCRect HandleCustomTilePropertyAt(int worldX, int worldY, CCTileMapLayer layer)
        {
            CCTileMapCoordinates tileAtXy = layer.ClosestTileCoordAtNodePosition(new CCPoint(worldX, worldY));

            CCTileGidAndFlags info = layer.TileGIDAndFlags(tileAtXy.Column, tileAtXy.Row);

            if (info != null)
            {
                Dictionary<string, string> properties = null;

                try
                {
                    properties = tilemap.TilePropertiesForGID(info.Gid);
                }
                catch{}

                if (properties != null && properties.ContainsKey("type") && properties["type"] == "ground")
                {
                    return new CCRect(worldX-8, worldY -8, 16, 16);
                }
                return new CCRect();
            }
            return new CCRect();
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
            }
        }
    }
}

