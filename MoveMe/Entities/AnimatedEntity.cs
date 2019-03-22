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
        public Animation idleRight, idleLeft, runRight, runLeft, fallRight, fallLeft, jump;

        public AnimatedEntity()
        {
        }

        protected Animation GetAnimation(string filename)
        {
            Animation animation = new Animation();
            CCSpriteSheet spritesheet = new CCSpriteSheet("animations/" + filename + ".plist");
            List<CCSpriteFrame> animationFrames = spritesheet.Frames;
            animation.Frames = animationFrames;
            animation.Action = new CCRepeatForever(new CCAnimate(new CCAnimation(animationFrames, 0.1f)));
            return animation;

        }

        public CCRect BoundingBoxWorld
        {
            get
            {
                return this.sprite.BoundingBoxTransformedToWorld;
            }
        }

        
        public void ApplyPhysics(float seconds)
        {
            this.sprite.PositionX += seconds * this.velocityX;
            this.sprite.PositionY += seconds * this.velocityY;
            this.SelectAnimation(seconds);
        }

        void SelectAnimation(float seconds)
        {
            Animation animationToAssign = new Animation();
            bool isFalling = this.velocityY < 0;
            bool isStanding = this.velocityY == 0;
            bool isJumping = this.velocityY > 0;
            bool isMovingRight = this.velocityX > 0;
            bool isMovingLeft = this.velocityX < 0;
            bool isIdle = this.velocityX == 0;
            if (isStanding && isIdle && !currentAnimation.Equals(idleRight))
            {
                animationToAssign = idleRight;
            }
            else if(isFalling && !currentAnimation.Equals(fallRight))
            {
                animationToAssign = fallRight;
            }
            if (!animationToAssign.Equals(new Animation()))
            {
                AssignAnimation(animationToAssign);
            }

        }

        void AssignAnimation(Animation animationToAssign)
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