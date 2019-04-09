using CocosSharp;
using System;
using System.Collections.Generic;

namespace MoveMe.Entities
{
    public class ButtonJumpLarge : AnimatedEntity
    {
        CCFiniteTimeAction asc, desc;
        Animation buttonPress;
        public ButtonJumpLarge()
        {
            asc = new CCMoveBy(0.3f, new CCPoint(0, 5));
            desc = asc.Reverse();
            buttonPress.Action = new CCSequence(desc, asc);
            sprite = new CCSprite("images/jumpButtonLarge");
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
                player.velocityY = 110;
                this.AssignAnimation(buttonPress);
                currentAnimation = new Animation();
            }
        }
    }
}