using System.Collections.Generic;
using CocosSharp;
using MoveMe.Entities;
using MoveMe;
using CocosDenshion;
using System;
using MoveMe.Entities.Swipe;
using CocosSharp.Extensions.SneakyJoystick;

namespace MoveMe
{
    public class GameScene : CCScene
    {
        SneakyJoystickControlSkinnedBase joystick;

        Player player = new Player();
        PhysicsEngine engine = new PhysicsEngine("map1");
        CCLayer gameplayLayer, hudLayer;
        CCWindow mainWindow;
        CCDirector director;
        CCDrawNode drawNode = new CCDrawNode();
        decimal time;
        int coinsCollected = 0;
        int deaths;
        CCLabel coinCounter;
        static string staticCoinString;
        CCLabel hintLabel;

        public GameScene(CCWindow mainWindow, CCDirector director) : base(mainWindow)
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
            joystick = new SneakyJoystickControlSkinnedBase(drawNode);
            joystick.AddListener();
            hudLayer.AddChild(joystick);
            hintLabel = new CCLabel(player.velocityX.ToString() + " , " + player.velocityY.ToString(), "arial", 22);
            hintLabel.Position = new CCPoint(gameplayLayer.ContentSize.Center.X, 220);
            hintLabel.Color = CCColor3B.Black;
            hudLayer.AddChild(hintLabel);
            joystick.Position = new CCPoint(190, 40);
            coinCounter.Position = new CCPoint(30, 200);
            hudLayer.AddChild(coinCounter);
            Schedule(UpdateTimer, 0.1f);
        }

        void WorldLogic(float seconds)
        {
            hintLabel.Text = player.velocityX.ToString() + " , " + player.velocityY.ToString();
            hintLabel.Update(seconds);
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
            ReactToJoystickInput();
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

        void ReactToJoystickInput()
        {
            if (joystick.IsRight)
            {
                player.velocityX = 30;
                player.direction = "right";
            }
            if (joystick.IsLeft)
            {
                player.velocityX = -30;
                player.direction = "left";
            }
            if (joystick.IsUp && joystick.dycache > 12 && player.isStanding)
            {
                player.isStanding = false;
                player.velocityY = 110;
            }
            if (!joystick.HasAnyDirection)
            {
                player.velocityX = 0;
            }
        }

        void UpdateTimer(float seconds)
        {
            time += (decimal)seconds;
        }

        void HandleLevelFinish()
        {
            var scene = new EndScene(mainWindow);
            director.ReplaceScene(scene);
        }

    }
}

