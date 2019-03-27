using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Animation idleRight, idleLeft, runRight, runLeft, fallRight, fallLeft, jumpRight, jumpLeft;
        public string direction = "right";
        
        public Player()
        {
            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");
            fallRight = GetAnimation("fallRight");
            fallLeft = GetAnimation("fallLeft");
            jumpRight = GetAnimation("jumpRight");
            jumpLeft = GetAnimation("jumpLeft");
            
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
            bool isMovingRight = this.velocityX > 0;
            bool isMovingLeft = this.velocityX < 0;
            bool isIdle = this.velocityX == 0;
            if (isStanding && isIdle && !currentAnimation.Equals(idleRight))
            {
                animationToAssign = idleRight;
            }
            else if (isFalling && !currentAnimation.Equals(fallRight))
            {
                animationToAssign = fallRight;
            }
            else if (isJumping && !currentAnimation.Equals(jumpRight))
            {
                animationToAssign = jumpRight;
            }
            if (!animationToAssign.Equals(new Animation()))
            {
                AssignAnimation(animationToAssign);
            }

        }


    }
}