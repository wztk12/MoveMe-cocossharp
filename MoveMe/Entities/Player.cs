using CocosSharp;
using Java.Util;
using System.Collections.Generic;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Animation idleRight, idleLeft, runRight, runLeft, fallRight, fallLeft, jumpRight, jumpLeft;
        public string direction = "right";
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
        }

        public void ApplyMovement(float seconds)
        {
            this.sprite.PositionX += seconds * this.velocityX;
            this.sprite.PositionY += seconds * this.velocityY;
            this.SelectAnimation(seconds);
        }

        public void ReactToCollision(CCPoint reposition)
		{
			isStanding = reposition.Y > 0;

			ProjectVelocityOnSurface (reposition);
		}

        void SelectAnimation(float seconds)
        {
            Animation animationToAssign = new Animation();
            bool isFalling = this.velocityY < 0;
            bool isJumping = this.velocityY > 0;
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