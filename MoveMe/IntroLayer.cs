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
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
            tilemap = engine.Tilemap;
            tilemap.Antialiased = false;
            this.AddChild(tilemap);

            player.sprite.Position = new CCPoint(20, 120);
            this.AddChild(player.sprite);
            
            Schedule(ApplyGravity, 0.1f);
            
        }

        void ApplyGravity(float seconds)
        {
            engine.LevelCollision(engine.GroundTiles, player);
            player.ApplyPhysics(seconds);
            player.JumpingAnimations(seconds);
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

