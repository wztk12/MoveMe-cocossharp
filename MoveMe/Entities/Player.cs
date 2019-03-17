using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CocosSharp;

namespace MoveMe.Entities
{
    public class Player : AnimatedEntity
    {
        public Animation idleRight;
        public CCSprite sprite
        {
            get;
            set;
        }
        public Player()
        {
            idleRight = GetAnimation("idler");
            sprite = new CCSprite(idleRight.Frames[0]);

        }
        

    }
}