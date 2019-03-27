using CocosSharp;

namespace MoveMe.Entities
{
    class ButtonRight : AnimatedEntity
    {
        public ButtonRight()
        {
            sprite = new CCSprite("images/arrowRight");

        }
        public bool IsTouched(CCTouch touch)
        {
            return this.BoundingBoxWorld.ContainsPoint(touch.Location);
        }

        public void HandlePress(CCTouch touch, Player player)
        {
            if (IsTouched(touch))
            {
                player.direction = "right";
                player.velocityX = 20;
            }
        }

    }
}