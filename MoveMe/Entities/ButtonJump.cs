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

        public void HandlePress(CCTouch touch, Player player)
        {
            if (this.BoundingBoxWorld.ContainsPoint(touch.Location) && player.isStanding)
                {
                    player.isStanding = false;
                    player.velocityY = 10;
                    this.AssignAnimation(buttonPress);
                    currentAnimation = new Animation();

                }
        }

    }
}