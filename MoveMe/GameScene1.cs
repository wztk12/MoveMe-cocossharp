using System.Collections.Generic;
using CocosSharp;
using MoveMe.Entities;
using MoveMe;
using CocosDenshion;
using System;
using MoveMe.Entities.Swipe;

namespace MoveMe
{
    public class GameScene1 : CCScene
    {
        Player player = new Player();
        ButtonJump buttonJump = new ButtonJump();
        ButtonLeft buttonLeft = new ButtonLeft();
        ButtonRight buttonRight = new ButtonRight();
        PhysicsEngine engine = new PhysicsEngine("map1");
        CCLayer gameplayLayer, hudLayer;
        CCWindow mainWindow;
        CCDirector director;
        int time;
        int coinsCollected = 0;
        TouchScreenInput touchScreen;
        int deaths;
        CCLabel coinCounter;
        static string staticCoinString;

        public GameScene1(CCWindow mainWindow, CCDirector director) : base(mainWindow)
        {

            this.director = director;
            this.mainWindow = mainWindow;
            staticCoinString = "/" + engine.coins;
            coinCounter = new CCLabel("Coins: " + coinsCollected + staticCoinString, "arial", 22);
            coinCounter.Color = CCColor3B.Black;
            CreateLayers();
            Schedule(WorldLogic);
        }

        private void CreateLayers()
        {
            engine.Tilemap.Antialiased = false;
            
            this.AddChild(engine.Tilemap);

           

            gameplayLayer = new CCLayer();
            this.AddChild(gameplayLayer);
            player.Position = new CCPoint(20, 200);
            player.defaultPosition = player.Position;
            gameplayLayer.AddChild(player);
            hudLayer = new CCLayer();
            this.AddChild(hudLayer);

            touchScreen = new TouchScreenInput(gameplayLayer);

            coinCounter.Position = new CCPoint(30, 200);
            hudLayer.AddChild(coinCounter);
            Schedule(UpdateTimer, 1f);
        }

        void WorldLogic(float seconds)
        {
            coinCounter.Text = player.velocityY.ToString() + " " + player.velocityX.ToString();
            touchScreen.UpdateInputValues();
            if (touchScreen.HorizontalRatio < 0)
            {
                player.direction = "left";
            }
            else if (touchScreen.HorizontalRatio > 0)
            {
                player.direction = "right";
            }
            player.ApplySwipeInput(touchScreen.HorizontalRatio, touchScreen.WasJumpPressed);
            player.ApplyMovement(seconds);
            engine.Gravity(seconds, player);
            CCPoint positionBeforeCollision = player.Position;
            CCPoint reposition = CCPoint.Zero;
            if (engine.PerformCollisionAgainst("solid", player))
            {
                reposition = player.Position - positionBeforeCollision;
            }
            player.ReactToCollision(reposition);
            if (engine.PerformCollisionAgainst("win", player))
            {
                HandleLevelFinish();
            }
            if (engine.PerformCollisionAgainst("death", player))
            {
                player.Position = player.defaultPosition;
                deaths++;
            }
            if (engine.PerformCollisionAgainst("coin", player))
            {
                coinsCollected++;
                coinCounter.Text = "Coins: " + coinsCollected + staticCoinString;
            }
    

            PerformScrolling();
            

        }

        private void PerformScrolling()
        {
            float effectivePlayerX = player.PositionX;

            // Effective values limit the scorlling beyond the level's bounds
            effectivePlayerX = System.Math.Max(player.PositionX, this.ContentSize.Center.X);
            float levelWidth = engine.Tilemap.TileTexelSize.Width * engine.Tilemap.MapDimensions.Column;
            effectivePlayerX = System.Math.Min(effectivePlayerX, levelWidth - this.ContentSize.Center.X);

            float effectivePlayerY = System.Math.Max(player.PositionY, this.ContentSize.Center.Y);
            float levelHeight = engine.Tilemap.TileTexelSize.Height * engine.Tilemap.MapDimensions.Row;
            effectivePlayerY = System.Math.Min(player.PositionY, levelHeight - this.ContentSize.Center.Y);
           

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

       void HandleLevelFinish()
       {
            var scene = new EndScene(mainWindow, director,"GameScene", time, 2, 1, deaths, player.distanceTravelled, coinCounter.Text);
            director.ReplaceScene(scene);
       }

    }
}

