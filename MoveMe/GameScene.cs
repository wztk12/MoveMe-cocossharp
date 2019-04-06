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
        CCWindow mainWindow;
        CCDirector director;
        int time;


        public GameScene(CCWindow mainWindow, CCDirector director) : base(mainWindow)
        {
            this.director = director;
            this.mainWindow = mainWindow;
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
            
            buttonLeft.sprite.Position = new CCPoint(35, 40);
            hudLayer.AddChild(buttonLeft.sprite);

            buttonJump.sprite.Position = new CCPoint(70, 40);
            hudLayer.AddChild(buttonJump.sprite);

            buttonRight.sprite.Position = new CCPoint(105, 40);
            hudLayer.AddChild(buttonRight.sprite);
            AddEventListener(touchListener, hudLayer);
            Schedule(UpdateTimer, 1f);
        }

        void UpdateLabel(float seconds)
        {
            hudLayer.RemoveChild(label);
            label = new CCLabel(engine.PerformCollisionAgainstWin(player).ToString(), "fonts/MarkerFelt-22", 22);
            label.Position = new CCPoint(20, 200);
            hudLayer.AddChild(label);
        }

        void WorldLogic(float seconds)
        {
            
            player.ApplyMovement(seconds);
            engine.Gravity(seconds, player);
            CCPoint positionBeforeCollision = player.Position;
            CCPoint reposition = CCPoint.Zero;
            if (engine.PerformCollisionAgainst(player))
            {
                player.isStanding = true;
                reposition = player.Position - positionBeforeCollision;
            }
            player.ReactToCollision(reposition);
            PerformScrolling();
            if (engine.PerformCollisionAgainstWin(player))
            {
                HandleLevelFinish(time);
            }
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
           

            float positionX = -effectivePlayerX + this.ContentSize.Center.X;
            float positionY = -effectivePlayerY + this.ContentSize.Center.Y;

            gameplayLayer.PositionX = positionX;
            gameplayLayer.PositionY = positionY;



            engine.Tilemap.TileLayersContainer.PositionX = positionX;
            engine.Tilemap.TileLayersContainer.PositionY = positionY;
        }

        void UpdateTimer(float seconds)
        {
            time += (int)seconds;
        }

        public void HandleLevelFinish(float time)
        {
            var scene = new EndScene(mainWindow, time);
            director.ReplaceScene(scene);
        }

    }
}

