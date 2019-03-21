using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public CCSprite jump;
        public Animation fall;

        public Player()
        {
            

            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");

            jump = new CCSprite("images/playerJump.png");
            fall = GetAnimation("fall");
            defaultSprite = new CCSprite(idleRight.Frames[0]);
            sprite = defaultSprite;
            this.velocityY = -8;

        }

        public void JumpingAnimations(float seconds)
        {
            if (velocityY < 0)
            {
                currentAnimation = fall;
                this.sprite.StopAllActions();
                this.sprite.AddAction(currentAnimation.Action);
            }
        }

    }
}