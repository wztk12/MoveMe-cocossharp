using CocosSharp;
using System.Collections.Generic;
using System;

namespace MoveMe.Entities
{
    public class AnimatedEntity : CCNode
    {
        public AnimatedEntity()
        {

        }

        protected Animation GetAnimation(string filename)
        {
            Animation animation = new Animation();
            CCSpriteSheet spritesheet = new CCSpriteSheet("animations/" + filename + ".plist");
            List<CCSpriteFrame> animationFrames = spritesheet.Frames;
            Console.Write(animationFrames.ToString());
            animation.Frames = animationFrames;
            animation.Action = new CCRepeatForever(new CCAnimate(new CCAnimation(animationFrames, 0.1f)));
            return animation;

        }
        
    }
    

                
}