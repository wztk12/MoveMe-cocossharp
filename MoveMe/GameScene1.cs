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
        ButtonJumpLarge buttonJumpLarge = new ButtonJumpLarge();
        PhysicsEngine engine = new PhysicsEngine("map1");
        CCLayer gameplayLayer, hudLayer;
        TouchScreenInput touchScreen;
        CCWindow mainWindow;
        CCDirector director;
        decimal time;
        int coinsCollected = 0;
        int deaths;
        CCLabel coinCounter;
        static string staticCoinString;
        CCLabel hintLabel;

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
            touchScreen = new TouchScreenInput(gameplayLayer, buttonJumpLarge, player);
            hudLayer = new CCLayer();
            this.AddChild(hudLayer);
            hintLabel = new CCLabel("Method 2: Swipe in a direction to move, press button to jump.", "arial", 22);
            hintLabel.Position = new CCPoint(hintLabel.ContentSize.Center.X + 10, 220);
            hintLabel.Color = CCColor3B.Black;
            hudLayer.AddChild(hintLabel);
            buttonJumpLarge.sprite.Position = new CCPoint(190, 40);
            hudLayer.AddChild(buttonJumpLarge.sprite);
            coinCounter.Position = new CCPoint(30, 200);
            hudLayer.AddChild(coinCounter);
            Schedule(UpdateTimer, 0.1f);
        }

        void WorldLogic(float seconds)
        {
            if (ContentSize.Center.X < player.PositionX) hintLabel.Text = "";
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
            player.ApplyMovement(seconds);
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
            time += (decimal)seconds;
        }

       void HandleLevelFinish()
       {
            var scene = new EndScene(mainWindow, director,"GameScene2", time, touchScreen.touchCounter, 0, deaths, player.distanceTravelled, coinCounter.Text);
            director.ReplaceScene(scene);
       }

    }
}

