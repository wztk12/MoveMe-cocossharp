using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
    public class SneakyJoystickControl : CCNode
    {
        protected CCDrawNode drawNode;

		bool isDPad;
		float joystickRadius;
        float thumbRadius;
        public float dycache;
        float deadRadius; //Size of deadzone in joystick (how far you must move before input starts). Automatically set if isDpad == YES
        bool isMoving;
        public int touchCounter, missedCounter;
        public float value;
		public CCRect ControlSize { get; set; }
        public CCPoint StickPosition { get; set; }
        public CCPoint StickDirection { get; set; }
        public CCPoint Center { get; set; }
        public CCPoint StickPreviousPosition { get; set; }
        public static CCPoint Velocity { get; set; }
        public float Degrees { get; set; }
        public bool AutoCenter { get; set; }
        public float JoystickRadiusSq { get; set; } //Optimizations (keep Squared values of all radii for faster calculations) (updated internally when changing joy/thumb radii)
        public float JoystickRadius
        {
            get { return joystickRadius; }
            set
            {
                JoystickRadiusSq = value * value;
                joystickRadius = value;
            }
        }
        public float ThumbRadius
        {
            get { return thumbRadius; }
            set
            {
                ThumbRadiusSq = value * value;
                thumbRadius = value;
            }
        }
        public float ThumbRadiusSq { get; set; }
		public float DeadRadiusSq { get; set; }
		public bool IsDebug;

        public float DeadRadius
        {
            get { return deadRadius; }
            set
            {
                DeadRadiusSq = value * value;
                deadRadius = value;
            }
        }

        //DPAD =====================================================

        public bool IsDPad
        {
            get { return isDPad; }
            set
            {
                isDPad = value;
                if (isDPad)
                {
                    HasDeadzone = true;
                    deadRadius = 10.0f;
                }
            }
        }
        public bool HasDeadzone { get; set; } //Turns Deadzone on/off for joystick, always YES if ifDpad == YES
        public int NumberOfDirections { get; set; } //Used only when isDpad == YES

        //DIRECTIONS ===============================================

        public bool HasAnyDirection
        {
            get { 
				return (IsDown || IsLeft || IsUp || IsRight); 
			}
        }
        public bool IsUp
        {
            get
            {
                return StickDirection.Y == 1;
            }
        }
        public bool IsRight
        {
            get
            {
                return StickDirection.X == 1;
            }
        }
        public bool IsLeft
        {
            get
            {
                return StickDirection.X == -1;
            }
        }
        public bool IsDown
        {
            get
            {
                return StickDirection.Y == -1;
            }
        }
        public bool IsUpLeft
        {
            get
            {
                return StickDirection.Y == 1 && StickDirection.X == -1;
            }
        }
        public bool IsUpRight
        {
            get
            {
                return StickDirection.Y == 1 && StickDirection.X == 1;
            }
        }
        public bool IsDownLeft
        {
            get
            {
                return StickDirection.Y == -1 && StickDirection.X == -1;
            }
        }
        public bool IsDownRight
        {
            get
            {
                return StickDirection.Y == 1 && StickDirection.X == 1;
            }
        }

        public SneakyJoystickControl(CCRect rect, CCDrawNode drawNode) : base()
        {
            this.drawNode = drawNode;
            Degrees = 0.0f;
            Velocity = CCPoint.Zero;
            AutoCenter = true;
            HasDeadzone = false;
            NumberOfDirections = 4;
            isDPad = false;
            joystickRadius = rect.Size.Width / 2;
            ThumbRadius = 32.0f;
            DeadRadius = 0.0f;
            AnchorPoint = CCPoint.AnchorMiddle;
			ControlSize = rect;

        }
        

        public void AddListener()
        {
            var tListener = new CCEventListenerTouchAllAtOnce();
            tListener.OnTouchesBegan = OnTouchesBegan;
            tListener.OnTouchesMoved = OnTouchesMoved;
            tListener.OnTouchesCancelled = OnTouchesCancelled;
            tListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(tListener, this);
        }
		protected override void AddedToScene()
		{
            base.AddedToScene();
			var rect = ControlSize;
			ContentSize = rect.Size;
			Position = rect.Center;
			StickPosition = ContentSize.Center;
			Center = ContentSize.Center;
		}

        public virtual void UpdateVelocity(CCPoint point)
        {
            // Calculate distance and angle from the center.
            float dx = point.X - PositionX;
            float dy = point.Y - PositionY;
            dycache = dy;
            float dSq = dx * dx + dy * dy;
            if (dSq <= DeadRadiusSq)
            {
                Velocity = CCPoint.Zero;
                Degrees = 0.0f;
                StickPosition = Center;
                return;
            }

            float angle = (float)Math.Atan2(dy, dx); // in radians
            if (angle < 0)
                angle += CCMathHelper.TwoPi;

            float cosAngle;
            float sinAngle;

            if (isDPad)
            {
                float anglePerSector = 360.0f / CCMathHelper.ToRadians(NumberOfDirections); //  NumberOfDirections * ((float)Math.PI / 180.0f);
                angle = (float)Math.Round(angle / anglePerSector) * anglePerSector;
            }

            cosAngle = CCMathHelper.Cos(angle);
            sinAngle = CCMathHelper.Sin(angle);

            // NOTE: Velocity goes from -1.0 to 1.0.
            if (dSq > JoystickRadiusSq || isDPad)
            {
                dx = cosAngle * joystickRadius;
                dy = sinAngle * joystickRadius;
            }

            Velocity = new CCPoint(dx / joystickRadius, dy / joystickRadius);
            Degrees = CCMathHelper.ToDegrees(angle);

            // Update the thumb's position
			var newLoc = new CCPoint(dx + ContentSize.Width / 2, dy + ContentSize.Height / 2);
			StickPosition = newLoc;
        }

        public virtual void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCTouch touch = touches.First();

            var location = Layer.ScreenToWorldspace(touch.LocationOnScreen);
            location = WorldToParentspace(location);
            touchCounter++;
            //Do a fast rect check before doing a circle hit check:
            if (location.X - PositionX < -joystickRadius || location.X - PositionX > joystickRadius || location.Y - PositionY < -joystickRadius || location.Y - PositionY > joystickRadius)
            {
                missedCounter++;
                return;
            }
            else
            {
                float relativePosX = Math.Abs(location.X - PositionX);
                float relativePosY = Math.Abs( location.Y - PositionY);
                float dSq = (relativePosX - joystickRadius) * (relativePosX - joystickRadius) + (relativePosY - joystickRadius) * (relativePosY - joystickRadius);
                value = dSq;
                if (dSq>0)
                {
                    isMoving = true;
                    UpdateVelocity(location);;

                    // Fire off our event to notify that movement was started
                    return;
                }
            }
        }

        public virtual void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (!isMoving)
                return;

            CCTouch touch = touches.First();
            CCPoint location = Layer.ScreenToWorldspace(touch.LocationOnScreen);
            location = WorldToParentspace(location);

            //Check direction
            StickDirection = new CCPoint(
                (location.X > PositionX) ? 1 : (location.X <PositionX) ? -1 : 0,
                (location.Y > PositionY) ? 1 : (location.Y < PositionY) ? -1 : 0
                );

            StickPreviousPosition = location;
            // Fire off our event to notify that movement was started

            UpdateVelocity(location);
        }

        public virtual void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {
            OnTouchesEnded(touches, touchEvent);
        }

        public virtual void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (!isMoving)
                return;

            ResetDirections();

            isMoving = false;

            CCTouch touch = touches.First();

            CCPoint location = Position;
 
            if (!AutoCenter)
            {
                location = Layer.ScreenToWorldspace(touch.LocationOnScreen);
                location = WorldToParentspace(location);
            }

            UpdateVelocity(location);
            // Fire off our event to notify that movement has ended.
        }

        public void ResetDirections()
        {
            StickDirection = CCPoint.Zero;
        }

        public void ForceCenter()
        {
            //StickPosition = new CCPoint(Center.X, Center.Y);
        }

        public CCPoint GetNextPositionFromImage(CCNode node, float dt, CCSize wSize)
        {
            return GetPositionFromVelocity(Velocity, node, dt, wSize);
        }

        public void RefreshImagePosition(CCNode node, float dt, CCSize wSize)
        {
            if (node != null)
                node.Position = GetPositionFromVelocity(Velocity, node, dt, wSize);
        }

        #region Static methods

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCNode node, float dt, CCSize winSize)
        {
			return GetPositionFromVelocity(velocity, node.Position, node.BoundingBox.Size, winSize, dt);
        }

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCPoint actualPosition, CCSize size, CCSize winSize, float dt)
        {
            return GetPositionFromVelocity(velocity, actualPosition, size.Width, size.Height, winSize.Width, winSize.Height, dt);
        }

        public static CCPoint GetPositionFromVelocity(CCPoint velocity, CCPoint actualPosition, float Width, float Height, float maxWindowWidth, float maxWindowHeight, float dt)
        {
            CCPoint scaledVelocity = velocity * 240;
			CCPoint newPosition = new CCPoint(actualPosition.X + scaledVelocity.X * dt, actualPosition.Y + scaledVelocity.Y * dt);

            if (newPosition.Y > maxWindowHeight - Height / 2)
                newPosition.Y = maxWindowHeight - Height / 2;

            if (newPosition.Y < (0 + Height / 2))
                newPosition.Y = (0 + Height / 2);

            if (newPosition.X > maxWindowWidth - Width / 2)
                newPosition.X = maxWindowWidth - Width / 2;

            if (newPosition.X < (0 + Width / 2))
                newPosition.X = (0 + Width / 2);

            return newPosition;
        }

        public void Draw()
		{
            if (drawNode != null)
                drawNode.DrawRect(new CCRect(0, 0, this.ContentSize.Width, this.ContentSize.Height), CCColor4B.Blue);
		}

        #endregion

    }
}
