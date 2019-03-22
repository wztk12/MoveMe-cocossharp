using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Player()
        {
            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");
            fallRight = GetAnimation("fallRight");
            defaultSprite = new CCSprite(idleRight.Frames[0]);
            sprite = defaultSprite;
            this.velocityY = -8;
        }


    }
}