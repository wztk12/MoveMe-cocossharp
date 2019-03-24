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
            AddEventListener(touchListener, this);
            tilemap = engine.Tilemap;
            tilemap.Antialiased = false;
            this.AddChild(tilemap);

            player.sprite.Position = new CCPoint(20, 120);
            this.AddChild(player.sprite);
            
            Schedule(ApplyPhysics, 0.1f);
            
        }

        void ApplyPhysics(float seconds)
        {
            engine.LevelCollision(engine.GroundTiles, player);
            player.ApplyMovement(seconds);
            engine.Gravity(seconds, player);
        }
        

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if (player.isStanding)
                {
                    player.velocityY = 10;
                    player.isStanding = false;
                }
            }
        }
    }
}

