using CocosSharp;
using Java.Util;
using System.Collections.Generic;
using System;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Animation idleRight, idleLeft, runRight, runLeft, fallRight, fallLeft, jumpRight, jumpLeft;
        public string direction = "right";
        public float distanceTravelled;
        private Dictionary<string, Animation> idleAnimations = new Dictionary<string, Animation>();
        private Dictionary<string, Animation> runAnimations = new Dictionary<string, Animation>();
        private Dictionary<string, Animation> fallAnimations = new Dictionary<string, Animation>();
        private Dictionary<string, Animation> jumpAnimations = new Dictionary<string, Animation>();

        public Player()
        {
            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            idleAnimations.Add("right", idleRight);
            idleAnimations.Add("left", idleLeft);
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");
            runAnimations.Add("right", runRight);
            runAnimations.Add("left", runLeft);
            fallRight = GetAnimation("fallRight");
            fallLeft = GetAnimation("fallLeft");
            fallAnimations.Add("right", fallRight);
            fallAnimations.Add("left", fallLeft);
            jumpRight = GetAnimation("jumpRight");
            jumpLeft = GetAnimation("jumpLeft");
            jumpAnimations.Add("right", jumpRight);
            jumpAnimations.Add("left", jumpLeft);

            defaultSprite = new CCSprite(idleRight.Frames[0]);
            sprite = defaultSprite;
            this.AddChild(sprite);
        }

        public void ApplySwipeInput(float horizontalMovementRatio, bool jumpPressed)
        {

            velocityX = horizontalMovementRatio * 30;

            if (jumpPressed && isStanding)
            {
                isStanding = false;
                velocityY = 110;
            }
        }

        public void ApplyMovement(float seconds)
        {

            this.PositionX += this.velocityX * seconds;
            this.PositionY += this.velocityY * seconds;
            this.distanceTravelled += seconds * Math.Abs(velocityX);
            if (!isStanding) this.distanceTravelled += seconds * Math.Abs(velocityY);
            this.SelectAnimation(seconds);
        }

        public void ReactToCollision(CCPoint reposition)
		{
			isStanding = reposition.Y > 0;

			this.ProjectVelocityOnSurface (reposition);
		}

        protected void ProjectVelocityOnSurface(CCPoint reposition)
        {
            if (reposition.X != 0 || reposition.Y != 0)
            {
                
                CCPoint velocity = new CCPoint(velocityX, velocityY);

                var dot = CCPoint.Dot(velocity, reposition);
                // falling into the collision, rather than out of
                if (dot < 0 && reposition.X<5 && reposition.Y<5 && reposition.Y>-5 && reposition.X>-5)
                {
                    velocity -= reposition * dot;

                    velocityX = velocity.X;
                    velocityY = velocity.Y;
                }
            }
        }
        void SelectAnimation(float seconds)
        {
            Animation animationToAssign = new Animation();
            bool isFalling = this.velocityY < -3;
            bool isJumping = this.velocityY > 3;
            bool isIdle = this.velocityX == 0;
            if (isStanding && isIdle && !currentAnimation.Equals(idleAnimations[direction]))
            {
                animationToAssign = idleAnimations[direction];
            }
            else if(isStanding && !isIdle && !currentAnimation.Equals(runAnimations[direction]))
            {
                animationToAssign = runAnimations[direction];
            }
            else if (isFalling && !isStanding && !currentAnimation.Equals(fallAnimations[direction]))
            {
                animationToAssign = fallAnimations[direction];
            }
            else if (isJumping && !currentAnimation.Equals(jumpAnimations[direction]))
            {
                animationToAssign = jumpAnimations[direction];
            }
            if (!animationToAssign.Equals(new Animation()))
            {
                AssignAnimation(animationToAssign);
            }

        }


    }
}