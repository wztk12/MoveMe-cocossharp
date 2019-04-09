using System;
using CocosSharp;
using System.Collections.Generic;

namespace MoveMe.Entities.Swipe
{
	public class TouchScreenInput : IDisposable
	{
		CCEventListenerTouchAllAtOnce touchListener;
        CCLayer owner;
		bool touchedOnRightSide = false;
        public int touchCounter;
        ButtonJumpLarge designatedButton;
        Player player;

		TouchScreenAnalogStick analogStick;

        public float HorizontalRatio
        {
            get
			{
				return analogStick.HorizontalRatio;
			}
        }

		public bool WasJumpPressed
		{
			get;
			private set;
		}


		public TouchScreenInput(CCLayer owner, ButtonJumpLarge designatedButton, Player player)
		{
            this.designatedButton = designatedButton;
            this.owner = owner;
            this.player = player;

			analogStick = new TouchScreenAnalogStick ();
			analogStick.Owner = owner;

			touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesMoved = HandleTouchesMoved;
			touchListener.OnTouchesBegan = HandleTouchesBegan;
			owner.AddEventListener (touchListener);

		}

		private void HandleTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			foreach (var touch in touches)
			{
                touchCounter++;
                designatedButton.HandlePress(touch, player);
			}
		}

		private void HandleTouchesMoved (List<CCTouch> touches, CCEvent touchEvent)
		{
			analogStick.DetermineHorizontalRatio (touches);
		}

		public void Dispose()
		{
			owner.RemoveEventListener (touchListener);
		}



        public void UpdateInputValues()
        {
			WasJumpPressed = touchedOnRightSide;
			touchedOnRightSide = false;
        }
    }
}

