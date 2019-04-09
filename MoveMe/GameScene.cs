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
        CCWindow mainWindow;
        CCDirector director;
        int time;
        int coinsCollected = 0;
        decimal touchCounter;
        decimal missedCounter;
        int deaths;
        CCLabel coinCounter;
        static string staticCoinString;

        public GameScene(CCWindow mainWindow, CCDirector director) : base(mainWindow)
        {
            this.director = director;
            this.mainWindow = mainWindow;
            staticCoinString = "/" + engine.coins;
            coinCounter = new CCLabel("Coins: " + coinsCollected + staticCoinString, "arial", 22);
            coinCounter.Color = CCColor3B.Orange;
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
            
            buttonLeft.sprite.Position = new CCPoint(155, 40);
            hudLayer.AddChild(buttonLeft.sprite);

            buttonJump.sprite.Position = new CCPoint(190, 40);
            hudLayer.AddChild(buttonJump.sprite);

            buttonRight.sprite.Position = new CCPoint(225, 40);
            hudLayer.AddChild(buttonRight.sprite);

            coinCounter.Position = new CCPoint(30, 200);
            hudLayer.AddChild(coinCounter);

            AddEventListener(touchListener, hudLayer);
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

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                touchCounter++;
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
                else missedCounter++;
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
            var scene = new EndScene(mainWindow, director, "GameScene1", time, touchCounter, missedCounter, deaths, player.distanceTravelled, coinCounter.Text);
            director.ReplaceScene(scene);
       }

    }
}

