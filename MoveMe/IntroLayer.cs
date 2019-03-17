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
            tilemap = new CCTileMap("maps/map1.tmx");
            this.AddChild(tilemap);
            player.sprite.Position = new CCPoint(20, 72);
            this.AddChild(player.sprite);
            player.sprite.AddAction(player.idleRight.Action);
            
            
            
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

