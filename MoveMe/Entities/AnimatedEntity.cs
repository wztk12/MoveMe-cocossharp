using CocosSharp;
using System.Collections.Generic;
using System;

namespace MoveMe.Entities
{
    public class AnimatedEntity : CCNode
    {
        public Animation currentAnimation = new Animation();
        public CCSprite sprite, defaultSprite;
        public float velocityX, velocityY = 0;
        public bool isStanding = false;

        public AnimatedEntity()
        {
        }

        protected Animation GetAnimation(string filename, bool repeating = true)
        {
            Animation animation = new Animation();
            CCSpriteSheet spritesheet = new CCSpriteSheet("animations/" + filename + ".plist");
            List<CCSpriteFrame> animationFrames = spritesheet.Frames;
            animation.Frames = animationFrames;
            CCAnimate action = new CCAnimate(new CCAnimation(animationFrames, 0.1f));
            if (repeating) animation.Action = new CCRepeatForever(action);
            else animation.Action = action;
            return animation;

        }

        public CCRect BoundingBoxWorld
        {
            get
            {
                return this.sprite.BoundingBoxTransformedToWorld;
            }
        }

        
        public void AssignAnimation(Animation animationToAssign)
        {
            if (!currentAnimation.Equals(animationToAssign))
            {
                this.currentAnimation = animationToAssign;
                this.sprite.StopAllActions();
                this.sprite.AddAction(currentAnimation.Action);
            }
        }
    }

    

                
}