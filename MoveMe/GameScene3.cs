using System.Collections.Generic;
using CocosSharp;
using MoveMe.Entities;
using MoveMe;
using CocosDenshion;
using System;
using MoveMe.Entities.Swipe;

namespace MoveMe
{
    public class GameScene3 : CCScene
    {
        Player player = new Player();
        ButtonJumpLarge buttonJumpLarge = new ButtonJumpLarge();
        PhysicsEngine engine = new PhysicsEngine("map1");
        CCLayer gameplayLayer, hudLayer;
        CCWindow mainWindow;
        CCDirector director;
        int time;
        int coinsCollected = 0;
        int deaths;
        int touchCounter;
        CCLabel coinCounter;
        static string staticCoinString;

        public GameScene3(CCWindow mainWindow, CCDirector director) : base(mainWindow)
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


            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesEnded = OnTouchesEnded;
            gameplayLayer = new CCLayer();
            this.AddChild(gameplayLayer);
            player.Position = new CCPoint(20, 200);
            player.defaultPosition = player.Position;
            gameplayLayer.AddChild(player);
            hudLayer = new CCLayer();
            this.AddChild(hudLayer);
            AddEventListener(touchListener, hudLayer);
            buttonJumpLarge.sprite.Position = new CCPoint(190, 40);
            hudLayer.AddChild(buttonJumpLarge.sprite);
            coinCounter.Position = new CCPoint(30, 200);
            hudLayer.AddChild(coinCounter);
            Schedule(UpdateTimer, 1f);
        }

        void WorldLogic(float seconds)
        {

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
            var scene = new EndScene(mainWindow, director, "GameScene", time, touchCounter, 0, deaths, player.distanceTravelled, coinCounter.Text);
            director.ReplaceScene(scene);
        }

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                foreach(var touch in touches)
                {
                    touchCounter++;
                    if (buttonJumpLarge.IsTouched(touch))
                    {
                        buttonJumpLarge.HandlePress(touch, player);
                    }
                    else
                    {
                        if (touch.Location.X >= player.VisibleBoundsWorldspace.Center.X)
                        {
                            player.velocityX = 30;
                            player.direction = "right";
                        }
                        else
                        {
                            player.velocityX = -30;
                            player.direction = "left";
                        }
                    }
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

    }
}

