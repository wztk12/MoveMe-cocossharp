﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
    public class SneakyJoystickControlSkinnedBase : SneakyJoystickControl
    {

        public static string DEFAULT_IMAGE_BACKGROUND { get { return "control/joystick_background"; } }
        public static string DEFAULT_IMAGE_THUMB { get { return "control/joystick_thumb"; } }

		private CCSprite _backgroundSprite;
        private CCSprite _thumbSprite;
        private byte _opacity;
        public CCSprite ThumbSprite
        {
            get { return _thumbSprite; }
            set
            {
                if (_thumbSprite != null)
                {
                    if (_thumbSprite.Parent != null)
                        _thumbSprite.Parent.RemoveChild(_thumbSprite, true);
                }

                _thumbSprite = value;
                if (value != null)
                {
                    AddChild(_thumbSprite, 1);
                    RefreshThumbSpritePosition();
                }
                _thumbSprite.Scale = 0.5f;
            }
        }
        public CCSprite BackgroundSprite
        {
            get { return _backgroundSprite; }
            set
            {
                if (_backgroundSprite != null)
                {
                    if (_backgroundSprite.Parent != null)
                        _backgroundSprite.Parent.RemoveChild(_backgroundSprite, true);
                }

                _backgroundSprite = value;
                if (value != null)
                {
                    AddChild(_backgroundSprite, 0);

                    RefreshBackgroundSpritePosition();
                }
            }
        }
        public override byte Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                if (_backgroundSprite != null)
                    _backgroundSprite.Opacity = value;
                if (_thumbSprite != null)
                    _thumbSprite.Opacity = value;
            }
        }

        //OVERRIDE PROPERTIES =================================================
        public override CCPoint Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                //Reposicionamos todo
                RefreshAllPosition();
            }
        }

        public override CCSize ContentSize
        {
            get { return base.ContentSize; }
            set
            {
                if (_backgroundSprite != null)
                {
                    _backgroundSprite.ContentSize = value;
					_backgroundSprite.Position = value.Center; // new CCPoint(base.ContentSize.Width / 2, base.ContentSize.Height / 2);
                }

                if (_thumbSprite != null)
                {
                    _thumbSprite.ContentSize = value;
					_thumbSprite.Position = value.Center;
                }

                JoystickRadius = value.Width / 2;
                base.ContentSize = value;
            }
        }

		#region Constructors
        public SneakyJoystickControlSkinnedBase(CCDrawNode drawNode)
			: this(new CCRect(0,0, 80, 80), drawNode)
        {
        }

        public SneakyJoystickControlSkinnedBase(CCRect size, CCDrawNode drawNode)
			: base(size, drawNode)
		{
			BackgroundSprite = new CCSprite(DEFAULT_IMAGE_BACKGROUND);  //new ColoredCircleSprite( CCColor4B.Red, 100f);
			ThumbSprite = new CCSprite(DEFAULT_IMAGE_THUMB);  //new ColoredCircleSprite(CCColor4B.Blue,30f);
		}
		#endregion

        public override void UpdateVelocity(CCPoint point)
        {
            base.UpdateVelocity(point);

            if (_thumbSprite != null)
                _thumbSprite.Position = StickPosition;
        }

        public void RefreshBackgroundSpritePosition()
        {
            ContentSize = _backgroundSprite.ContentSize;
            JoystickRadius = (_backgroundSprite.ContentSize.Width / 2);
        }

        public void RefreshThumbSpritePosition()
        {
			_thumbSprite.Position = _thumbSprite.ContentSize.Center;
            ThumbRadius = _thumbSprite.ContentSize.Width / 2;
        }

        public void RefreshAllPosition()
        {
            RefreshBackgroundSpritePosition();
            RefreshThumbSpritePosition(); 
        }
    }
}
