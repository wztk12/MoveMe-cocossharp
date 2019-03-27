using CocosSharp;

namespace MoveMe.Entities
{
    class ButtonLeft : AnimatedEntity
    {
        public ButtonLeft()
        {
            sprite = new CCSprite("images/arrowLeft");

        }
        public bool IsTouched(CCTouch touch)
        {
            return this.BoundingBoxWorld.ContainsPoint(touch.Location);
        }

        public void HandlePress(CCTouch touch, Player player)
        {
            if (IsTouched(touch))
            {
                player.direction = "left";
                player.velocityX = -20;
            }
        }
    }
}