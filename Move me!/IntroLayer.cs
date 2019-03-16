using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace MoveMe
{
    public class IntroLayer : CCLayer
    {
        CCTileMap tilemap;
        CCSprite guy;
        CCAnimation walkAnim;
        CCRepeatForever walkRepeat;




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
            var spritesheet = new CCSpriteSheet("animations/runl.plist");
            var animationFrames = spritesheet.Frames.FindAll((x) => x.TextureFilename.StartsWith("runl"));

            walkAnim = new CCAnimation(animationFrames, 0.1f);
            walkRepeat = new CCRepeatForever(new CCAnimate(walkAnim));
            guy = new CCSprite(animationFrames[0]);
            
            this.AddChild(tilemap);
            this.AddChild(guy);
            guy.PositionX = 20;
            guy.PositionY = 100;
            
            
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
                guy.AddAction(walkRepeat);
            }
        }
    }
}

