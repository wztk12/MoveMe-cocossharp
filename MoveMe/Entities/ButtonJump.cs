using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CocosSharp;

namespace MoveMe.Entities
{
    class ButtonJump : AnimatedEntity
    {
        Animation buttonPress;
        public ButtonJump()
        {
            buttonPress = GetAnimation("buttonPress", false);
            sprite = new CCSprite(buttonPress.Frames[0]);

        }
        public bool IsTouched(CCTouch touch)
        {
            return this.BoundingBoxWorld.ContainsPoint(touch.Location);
        }

        public void HandlePress(CCTouch touch, Player player)
        {
            if (IsTouched(touch) && player.isStanding)
            {
                player.isStanding = false;
                player.velocityY = 80;
                this.AssignAnimation(buttonPress);
                currentAnimation = new Animation();
            }
        }

    }
}