using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Animation idleRight, idleLeft, runRight, runLeft;

        public Player()
        {

            idleRight = GetAnimation("idler");
            idleLeft = GetAnimation("idlel");
            runRight = GetAnimation("runr");
            runLeft = GetAnimation("runl");
            
            sprite = new CCSprite(idleRight.Frames[0]);

        }
        

    }
}