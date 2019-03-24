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
            fallLeft = GetAnimation("fallLeft");
            jumpRight = GetAnimation("jumpRight");
            jumpLeft = GetAnimation("jumpLeft");
            
            defaultSprite = new CCSprite(idleRight.Frames[0]);
            sprite = defaultSprite;
        }


    }
}