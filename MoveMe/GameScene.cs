using System.Collections.Generic;
using CocosSharp;
using MoveMe.Entities;
using MoveMe;
using CocosDenshion;
using System;

namespace MoveMe
{
    public class GameScene : CCScene
    {
        Player player = new Player();
        ButtonJump buttonJump = new ButtonJump();
        ButtonLeft buttonLeft = new ButtonLeft();
        ButtonRight buttonRight = new ButtonRight();
        PhysicsEngine engine = new PhysicsEngine("map1");
        CCLayer gameplayLayer, hudLayer;
        CCLabel label;
        int time;


        public GameScene(CCWindow mainWindow) : base(mainWindow)
        {
            CreateLayers();
            Schedule(WorldLogic);
        }

        private void CreateLayers()
        {
            engine.Tilemap.Antialiased = false;
            this.AddChild(engine.Tilemap);
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesEnded = OnTouchesEnded;
            
            gameplayLayer = new CCLayer();
            this.AddChild(gameplayLayer);
            player.sprite.Position = new CCPoint(20, 200);
            gameplayLayer.AddChild(player.sprite);

            hudLayer = new CCLayer();
            this.AddChild(hudLayer);
            
            Schedule(UpdateTimer, 1f);

            buttonLeft.sprite.Position = new CCPoint(35, 40);
            hudLayer.AddChild(buttonLeft.sprite);

            buttonJump.sprite.Position = new CCPoint(70, 40);
            hudLayer.AddChild(buttonJump.sprite);

            buttonRight.sprite.Position = new CCPoint(105, 40);
            hudLayer.AddChild(buttonRight.sprite);
            AddEventListener(touchListener, hudLayer);

        }

        
        void UpdateLabel(float seconds)
        {
            hudLayer.RemoveChild(label);
            label = new CCLabel(time.ToString(), "fonts/MarkerFelt-22", 22);
            label.Position = new CCPoint(20, 200);
            hudLayer.AddChild(label);
        }
        void WorldLogic(float seconds)
        {
            //engine.LevelCollision(engine.GroundTiles, player);
            player.ApplyMovement(seconds);
            engine.Gravity(seconds, player);
            CCPoint positionBeforeCollision = player.Position;
            CCPoint reposition = CCPoint.Zero;
            if (engine.PerformCollisionAgainst(player))
            {
                reposition = player.Position - positionBeforeCollision;
            }
            player.ReactToCollision(reposition);
            PerformScrolling();
        }


        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if (buttonJump.IsTouched(touches[0]))
                {
                    buttonJump.HandlePress(touches[0], player);
                }
                else if (buttonLeft.IsTouched(touches[0]))
                {
                    buttonLeft.HandlePress(touches[0], player);
                }
                else if (buttonRight.IsTouched(touches[0]))
                {
                    buttonRight.HandlePress(touches[0], player);
                }
            }
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                player.velocityX = 0;
            }
        }

        private void PerformScrolling()
        {
            float effectivePlayerX = player.sprite.PositionX;

            // Effective values limit the scorlling beyond the level's bounds
            effectivePlayerX = System.Math.Max(player.sprite.PositionX, this.ContentSize.Center.X);
            float levelWidth = engine.Tilemap.TileTexelSize.Width * engine.Tilemap.MapDimensions.Column;
            effectivePlayerX = System.Math.Min(effectivePlayerX, levelWidth - this.ContentSize.Center.X);

            float effectivePlayerY = System.Math.Max(player.sprite.PositionY, this.ContentSize.Center.Y);
            float levelHeight = engine.Tilemap.TileTexelSize.Height * engine.Tilemap.MapDimensions.Row;
            effectivePlayerY = System.Math.Min(player.sprite.PositionY, levelHeight - this.ContentSize.Center.Y);
            
            // We don't want to limit the scrolling on Y - instead levels should be large enough
            // so that the view never reaches the bottom. This allows the user to play
            // with their thumbs without them getting in the way of the game.

            float positionX = -effectivePlayerX + this.ContentSize.Center.X;
            float positionY = -effectivePlayerY + this.ContentSize.Center.Y;

            gameplayLayer.PositionX = positionX;
            gameplayLayer.PositionY = positionY;

            // We don't want the background to scroll, 
            // so we'll make it move the opposite direction of the rest of the tilemap:


            engine.Tilemap.TileLayersContainer.PositionX = positionX;
            engine.Tilemap.TileLayersContainer.PositionY = positionY;
        }

        void UpdateTimer(float seconds)
        {
            time += (int)seconds;
        }
    }
}

