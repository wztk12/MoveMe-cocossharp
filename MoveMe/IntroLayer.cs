using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using MoveMe.Entities;

namespace MoveMe
{
    public class IntroLayer : CCLayer
    {
        Player player = new Player();
        ButtonJump buttonJump = new ButtonJump();
        ButtonLeft buttonLeft = new ButtonLeft();
        ButtonRight buttonRight = new ButtonRight();
        PhysicsEngine engine = new PhysicsEngine("map1");

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
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
            engine.Tilemap.Antialiased = false;
            this.AddChild(engine.Tilemap);

            player.sprite.Position = new CCPoint(20, 120);
            this.AddChild(player.sprite);

            buttonLeft.sprite.Position = new CCPoint(40, 40);
            this.AddChild(buttonLeft.sprite);

            buttonJump.sprite.Position = new CCPoint(70, 40);
            this.AddChild(buttonJump.sprite);

            buttonRight.sprite.Position = new CCPoint(100, 40);
            this.AddChild(buttonRight.sprite);

            Schedule(ApplyPhysics, 0.1f);

        }

        void ApplyPhysics(float seconds)
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
    }
}

