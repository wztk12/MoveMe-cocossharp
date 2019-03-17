using System.Collections.Generic;
using CocosSharp;

namespace MoveMe.Entities
{
    public struct Animation
    {
        public List<CCSpriteFrame> Frames
        {
            get;
            set;
        }
        public CCAction Action
        {
            get;
            set;
        }
        
    }
}