using FixMath.NET;
using System;
using UnityEngine;

namespace FixedMath
{
    /// <summary>
    /// Provides XNA-like ray functionality.
    /// </summary>
    public struct FPRay
    {  
        /// <summary>
        /// Starting position of the ray.
        /// </summary>
        public FPVector3 origin;
        /// <summary>
        /// Direction in which the ray points.
        /// </summary>
        public FPVector3 direction;


        /// <summary>
        /// Constructs a new ray.
        /// </summary>
        /// <param name="origin">Starting position of the ray.</param>
        /// <param name="direction">Direction in which the ray points.</param>
        public FPRay(FPVector3 origin, FPVector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }



        /// <summary>
        /// Determines if and when the ray intersects the bounding box.
        /// </summary>
        /// <param name="boundingBox">Bounding box to test against.</param>
        /// <param name="t">The length along the ray to the impact, if any impact occurs.</param>
        /// <returns>True if the ray intersects the target, false otherwise.</returns>
        public bool Intersects(ref BoundingBox boundingBox, out Fix64 t)
        {
			Fix64 tmin = F64.C0, tmax = Fix64.MaxValue;
            if (Fix64.Abs(direction.X) < Toolbox.Epsilon)
            {
                if (origin.X < boundingBox.Min.X || origin.X > boundingBox.Max.X)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.X;
                var t1 = (boundingBox.Min.X - origin.X) * inverseDirection;
                var t2 = (boundingBox.Max.X - origin.X) * inverseDirection;
                if (t1 > t2)
                {
					Fix64 temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                tmin = MathHelper.Max(tmin, t1);
                tmax = MathHelper.Min(tmax, t2);
                if (tmin > tmax)
                {
                    t = F64.C0;
                    return false;
                }
            }
            if (Fix64.Abs(direction.Y) < Toolbox.Epsilon)
            {
                if (origin.Y < boundingBox.Min.Y || origin.Y > boundingBox.Max.Y)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.Y;
                var t1 = (boundingBox.Min.Y - origin.Y) * inverseDirection;
                var t2 = (boundingBox.Max.Y - origin.Y) * inverseDirection;
                if (t1 > t2)
                {
					Fix64 temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                tmin = MathHelper.Max(tmin, t1);
                tmax = MathHelper.Min(tmax, t2);
                if (tmin > tmax)
                {
                    t = F64.C0;
                    return false;
                }
            }
            if (Fix64.Abs(direction.Z) < Toolbox.Epsilon)
            {
                if (origin.Z < boundingBox.Min.Z || origin.Z > boundingBox.Max.Z)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.Z;
                var t1 = (boundingBox.Min.Z - origin.Z) * inverseDirection;
                var t2 = (boundingBox.Max.Z - origin.Z) * inverseDirection;
                if (t1 > t2)
                {
					Fix64 temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                tmin = MathHelper.Max(tmin, t1);
                tmax = MathHelper.Min(tmax, t2);
                if (tmin > tmax)
                {
                    t = F64.C0;
                    return false;
                }
            }
            t = tmin;
            return true;
        }

        /// <summary>
        /// Determines if and when the ray intersects the bounding box.
        /// </summary>
        /// <param name="boundingBox">Bounding box to test against.</param>
        /// <param name="t">The length along the ray to the impact, if any impact occurs.</param>
        /// <returns>True if the ray intersects the target, false otherwise.</returns>
        public bool Intersects(BoundingBox boundingBox, out Fix64 t)
        {
            return Intersects(ref boundingBox, out t);
        }

        /// <summary>
        /// Determines if and when the ray intersects the plane.
        /// </summary>
        /// <param name="fpPlane">Plane to test against.</param>
        /// <param name="t">The length along the ray to the impact, if any impact occurs.</param>
        /// <returns>True if the ray intersects the target, false otherwise.</returns>
        public bool Intersects(ref FPPlane fpPlane, out Fix64 t)
        {
			Fix64 velocity;
            FPVector3.Dot(ref direction, ref fpPlane.Normal, out velocity);
            if (Fix64.Abs(velocity) < Toolbox.Epsilon)
            {
                t = F64.C0;
                return false;
            }
			Fix64 distanceAlongNormal;
            FPVector3.Dot(ref origin, ref fpPlane.Normal, out distanceAlongNormal);
            distanceAlongNormal += fpPlane.D;
            t = -distanceAlongNormal / velocity;
            return t >= -Toolbox.Epsilon;
        }

        /// <summary>
        /// Determines if and when the ray intersects the plane.
        /// </summary>
        /// <param name="fpPlane">Plane to test against.</param>
        /// <param name="t">The length along the ray to the impact, if any impact occurs.</param>
        /// <returns>True if the ray intersects the target, false otherwise.</returns>
        public bool Intersects(FPPlane fpPlane, out Fix64 t)
        {
            return Intersects(ref fpPlane, out t);
        }

        /// <summary>
        /// Computes a point along a ray given the length along the ray from the ray position.
        /// </summary>
        /// <param name="t">Length along the ray from the ray position in terms of the ray's direction.</param>
        /// <param name="v">Point along the ray at the given location.</param>
        public void GetPointOnRay(Fix64 t, out FPVector3 v)
        {
            FPVector3.Multiply(ref direction, t, out v);
            FPVector3.Add(ref v, ref origin, out v);
        }
    }
}
