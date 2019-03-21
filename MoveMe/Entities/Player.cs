using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        
        public CCSprite jump, fall;

        public Player()
        {

            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");

            jump = new CCSprite("images/playerJump.png");
            fall = new CCSprite("images/playerFall.png");
            defaultSprite = new CCSprite(idleRight.Frames[0]);
            currentAnimation = runRight;
            sprite = defaultSprite;
            this.velocityY = -8;

        }
        

    }
}