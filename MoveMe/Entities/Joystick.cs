using System;
using System.Collections.Generic;
using CocosSharp;
using MoveMe.Entities.Swipe;
namespace MoveMe.Entities
{
    class Joystick : CCNode
    {
        CCDrawNode socket, stick;
        public TouchScreenAnalogStick analogStick;
        public Joystick(CCPoint center)
        {
            socket = new CCDrawNode();
            stick = new CCDrawNode();
            analogStick = new TouchScreenAnalogStick();
            socket.DrawCircle(center, 50, CCColor4B.Black);
            stick.DrawCircle(center, 10, CCColor4B.Black);
            AddChild(socket);
            AddChild(stick);
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesMoved = HandleTouchesMoved;
            socket.AddEventListener(touchListener);
        }

        private void HandleTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            analogStick.DetermineHorizontalRatio(touches);
            if (socket.BoundingBox.ContainsPoint(stick.Position)) stick.AddAction(new CCMoveBy(0.1f, new CCPoint(analogStick.HorizontalRatio * 100, 0)));
            else stick.StopAllActions();
        }
    }
}