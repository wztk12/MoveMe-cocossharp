﻿using CocosSharp;
using System;
using System.Collections.Generic;
using MoveMe.TilemapClasses;
using System.Linq;

namespace MoveMe.Entities
{
    class PhysicsEngine
    {
        List<RectWithDirection> collisions = new List<RectWithDirection>();
        List<RectWithDirection> finishTiles = new List<RectWithDirection>();
        int tileDimension;
        private float gravity = 140;

        public CCTileMap Tilemap
        {
            get;
            private set;
        }

        public PhysicsEngine(string mapname)
        {
            this.Tilemap = new CCTileMap("maps/" + mapname + ".tmx");
            this.PopulateFrom(this.Tilemap);
        }



        public void Gravity(float seconds, AnimatedEntity entity)
        {
            if (!entity.isStanding) entity.velocityY = Math.Max(entity.velocityY + (seconds * -gravity), -30);
            else entity.velocityY = 0;
        }


        public void PopulateFrom(CCTileMap tileMap)
        {
            tileDimension = (int)(tileMap.TileTexelSize.Height + .5f);

            TileMapPropertyFinder finder = new TileMapPropertyFinder(tileMap);

            foreach (var propertyLocation in finder.GetPropertyLocations())
            {

                if (propertyLocation.Properties.ContainsKey("type"))
                {
                    float centerX = propertyLocation.WorldX;
                    float centerY = propertyLocation.WorldY;
                    float left = centerX - tileDimension / 2.0f;
                    float bottom = centerY - tileDimension / 2.0f;

                    RectWithDirection rectangle = new RectWithDirection
                    {
                        Left = left,
                        Bottom = bottom,
                        Width = tileDimension,
                        Height = tileDimension
                    };

                    collisions.Add(rectangle);
                }
                else if (propertyLocation.Properties.ContainsKey("FinishPoint"))
                {
                    float centerX = propertyLocation.WorldX;
                    float centerY = propertyLocation.WorldY;
                    float left = centerX - tileDimension / 2.0f;
                    float bottom = centerY - tileDimension / 2.0f;

                    RectWithDirection rectangle = new RectWithDirection
                    {
                        Left = left,
                        Bottom = bottom,
                        Width = tileDimension,
                        Height = tileDimension
                    };

                    finishTiles.Add(rectangle);
                }
            }

            // Sort by XAxis to speed future searches:
            collisions = collisions.OrderBy(item => item.Left).ToList();

            // now let's adjust the directions that these point
            for (int i = 0; i < collisions.Count; i++)
            {
                var rect = collisions[i];

                // By default rectangles can reposition objects in all directions:
                int valueToAssign = (int)Directions.All;

                float centerX = rect.CenterX;
                float centerY = rect.CenterY;

                // If there are collisions on the sides, then this 
                // rectangle can no longer repositon objects in that direction.
                if (HasCollisionAt(centerX - tileDimension, centerY))
                {
                    valueToAssign -= (int)Directions.Left;
                }
                if (HasCollisionAt(centerX + tileDimension, centerY))
                {
                    valueToAssign -= (int)Directions.Right;
                }
                if (HasCollisionAt(centerX, centerY + tileDimension))
                {
                    valueToAssign -= (int)Directions.Up;
                }
                if (HasCollisionAt(centerX, centerY - tileDimension))
                {
                    valueToAssign -= (int)Directions.Down;
                }

                rect.Directions = (Directions)valueToAssign;
                collisions[i] = rect;
            }

            for (int i = collisions.Count - 1; i > -1; i--)
            {
                if (collisions[i].Directions == Directions.None)
                {
                    collisions.RemoveAt(i);
                }
            }
            finishTiles = finishTiles.OrderBy(item => item.Left).ToList();

            // now let's adjust the directions that these point
            for (int i = 0; i < finishTiles.Count; i++)
            {
                var rect = finishTiles[i];

                // By default rectangles can reposition objects in all directions:
                int valueToAssign = (int)Directions.All;

                float centerX = rect.CenterX;
                float centerY = rect.CenterY;

                // If there are finishTiles on the sides, then this 
                // rectangle can no longer repositon objects in that direction.
                if (HasCollisionAtWin(centerX - tileDimension, centerY))
                {
                    valueToAssign -= (int)Directions.Left;
                }
                if (HasCollisionAtWin(centerX + tileDimension, centerY))
                {
                    valueToAssign -= (int)Directions.Right;
                }
                if (HasCollisionAtWin(centerX, centerY + tileDimension))
                {
                    valueToAssign -= (int)Directions.Up;
                }
                if (HasCollisionAtWin(centerX, centerY - tileDimension))
                {
                    valueToAssign -= (int)Directions.Down;
                }

                rect.Directions = (Directions)valueToAssign;
                finishTiles[i] = rect;
            }

            for (int i = finishTiles.Count - 1; i > -1; i--)
            {
                if (finishTiles[i].Directions == Directions.None)
                {
                    finishTiles.RemoveAt(i);
                }
            }
        }

        int GetFirstAfter(float value)
        {
            int lowBoundIndex = 0;
            int highBoundIndex = collisions.Count;

            if (lowBoundIndex == highBoundIndex)
            {
                return lowBoundIndex;
            }

            // We want it inclusive
            highBoundIndex -= 1;
            int current = 0;


            while (true)
            {
                current = (lowBoundIndex + highBoundIndex) >> 1;
                if (highBoundIndex - lowBoundIndex < 2)
                {
                    if (collisions[highBoundIndex].Left <= value)
                    {
                        return highBoundIndex + 1;
                    }
                    else if (collisions[lowBoundIndex].Left <= value)
                    {
                        return lowBoundIndex + 1;
                    }
                    else if (collisions[lowBoundIndex].Left > value)
                    {
                        return lowBoundIndex;
                    }
                }

                if (collisions[current].Left >= value)
                {
                    highBoundIndex = current;
                }
                else if (collisions[current].Left < value)
                {
                    lowBoundIndex = current;
                }
            }
        }

        bool HasCollisionAt(float worldX, float worldY)
        {
            int leftIndex;
            int rightIndex;

            GetIndicesBetween(worldX - tileDimension, worldX + tileDimension, out leftIndex, out rightIndex);

            for (int i = leftIndex; i < rightIndex; i++)
            {
                if (collisions[i].ContainsPoint(worldX, worldY))
                {
                    return true;
                }
            }
            return false;
        }

        void GetIndicesBetween(float leftX, float rightX, out int leftIndex, out int rightIndex)
        {
            float leftAdjusted = tileDimension * (((int)leftX) / tileDimension) - tileDimension / 2;
            float rightAdjusted = tileDimension * (((int)rightX) / tileDimension) + tileDimension / 2;

            leftIndex = GetFirstAfter(leftAdjusted);
            rightIndex = GetFirstAfter(rightAdjusted);
        }

        public bool PerformCollisionAgainst(AnimatedEntity entity)
        {
            bool didCollisionOccur = false;

            int leftIndex;
            int rightIndex;

            GetIndicesBetween(
                entity.BoundingBoxWorld.LowerLeft.X, entity.BoundingBoxWorld.UpperRight.X, out leftIndex, out rightIndex);

            var boundingBoxWorld = entity.BoundingBoxWorld;

            for (int i = leftIndex; i < rightIndex; i++)
            {
                var separatingVector = GetSeparatingVector(boundingBoxWorld, collisions[i]);

                if (separatingVector != CCVector2.Zero)
                {
                    entity.PositionX += separatingVector.X;
                    entity.PositionY += separatingVector.Y;
                    // refresh boundingBoxWorld:
                    boundingBoxWorld = entity.BoundingBoxWorld;

                    didCollisionOccur = true;
                }
            }

            return didCollisionOccur;
        }


        CCVector2 GetSeparatingVector(CCRect first, RectWithDirection second)
        {
            // Default to no separation
            CCVector2 separation = CCVector2.Zero;

            // Only calculate separation if the rectangles intersect
            if (Intersects(first, second))
            {
                // The intersectionRect returns the rectangle produced
                // by overlapping the two rectangles.
                // This is protected by partitioning and deep collision, so it
                // won't happen too often - it's okay to do a ToRect here
                var intersectionRect = first.Intersection(second.ToRect());

                float minDistance = float.PositiveInfinity;

                float firstCenterX = first.Center.X;
                float firstCenterY = first.Center.Y;

                float secondCenterX = second.Left + second.Width / 2.0f;
                float secondCenterY = second.Bottom + second.Width / 2.0f;

                bool canMoveLeft = (second.Directions & Directions.Left) == Directions.Left && firstCenterX < secondCenterX;
                bool canMoveRight = (second.Directions & Directions.Right) == Directions.Right && firstCenterX > secondCenterX;
                bool canMoveDown = (second.Directions & Directions.Down) == Directions.Down && firstCenterY < secondCenterY;
                bool canMoveUp = (second.Directions & Directions.Up) == Directions.Up && firstCenterY > secondCenterY;


                if (canMoveLeft)
                {
                    float candidate = first.UpperRight.X - second.Left;

                    if (candidate > 0)
                    {
                        minDistance = candidate;

                        separation.X = -minDistance;
                        separation.Y = 0;
                    }
                }
                if (canMoveRight)
                {
                    float candidate = (second.Left + second.Width) - first.LowerLeft.X;

                    if (candidate > 0 && candidate < minDistance)
                    {
                        minDistance = candidate;

                        separation.X = minDistance;
                        separation.Y = 0;
                    }
                }
                if (canMoveUp)
                {
                    float candidate = (second.Bottom + second.Height) - first.Origin.Y;

                    if (candidate > 0 && candidate < minDistance)
                    {
                        minDistance = candidate;

                        separation.X = 0;
                        separation.Y = minDistance;
                    }

                }
                if (canMoveDown)
                {
                    float candidate = first.UpperRight.Y - second.Bottom;

                    if (candidate > 0 && candidate < minDistance)
                    {
                        minDistance = candidate;

                        separation.X = 0;
                        separation.Y = -minDistance;
                    }
                }
            }

            return separation;
        }

        bool Intersects(CCRect first, RectWithDirection second)
        {
            return first.IntersectsRect(second.ToRect());
        }

        //***************Handling win tiles collisions*************************
        int GetFirstAfterWin(float value)
        {
            int lowBoundIndex = 0;
            int highBoundIndex = finishTiles.Count;

            if (lowBoundIndex == highBoundIndex)
            {
                return lowBoundIndex;
            }

            // We want it inclusive
            highBoundIndex -= 1;
            int current = 0;


            while (true)
            {
                current = (lowBoundIndex + highBoundIndex) >> 1;
                if (highBoundIndex - lowBoundIndex < 2)
                {
                    if (finishTiles[highBoundIndex].Left <= value)
                    {
                        return highBoundIndex + 1;
                    }
                    else if (finishTiles[lowBoundIndex].Left <= value)
                    {
                        return lowBoundIndex + 1;
                    }
                    else if (finishTiles[lowBoundIndex].Left > value)
                    {
                        return lowBoundIndex;
                    }
                }

                if (finishTiles[current].Left >= value)
                {
                    highBoundIndex = current;
                }
                else if (finishTiles[current].Left < value)
                {
                    lowBoundIndex = current;
                }
            }
        }

        bool HasCollisionAtWin(float worldX, float worldY)
        {
            int leftIndex;
            int rightIndex;

            GetIndicesBetweenWin(worldX - tileDimension, worldX + tileDimension, out leftIndex, out rightIndex);

            for (int i = leftIndex; i < rightIndex; i++)
            {
                if (finishTiles[i].ContainsPoint(worldX, worldY))
                {
                    return true;
                }
            }
            return false;
        }

        void GetIndicesBetweenWin(float leftX, float rightX, out int leftIndex, out int rightIndex)
        {
            float leftAdjusted = tileDimension * (((int)leftX) / tileDimension) - tileDimension / 2;
            float rightAdjusted = tileDimension * (((int)rightX) / tileDimension) + tileDimension / 2;

            leftIndex = GetFirstAfterWin(leftAdjusted);
            rightIndex = GetFirstAfterWin(rightAdjusted);
        }

        public bool PerformCollisionAgainstWin(AnimatedEntity entity)
        {
            bool didCollisionOccur = false;

            int leftIndex;
            int rightIndex;

            GetIndicesBetweenWin(
                entity.BoundingBoxWorld.LowerLeft.X, entity.BoundingBoxWorld.UpperRight.X, out leftIndex, out rightIndex);

            var boundingBoxWorld = entity.BoundingBoxWorld;

            for (int i = leftIndex; i < rightIndex; i++)
            {
                var separatingVector = GetSeparatingVector(boundingBoxWorld, finishTiles[i]);

                if (separatingVector != CCVector2.Zero)
                {
                    didCollisionOccur = true;
                }
            }

            return didCollisionOccur;
        }

    }
}
