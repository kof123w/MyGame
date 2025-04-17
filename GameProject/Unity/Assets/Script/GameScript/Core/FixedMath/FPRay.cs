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

        public static implicit operator FPRay(UnityEngine.Ray ray)
        {
            return new FPRay(ray.origin, ray.direction);
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
            if (Fix64.Abs(direction.x) < Toolbox.Epsilon)
            {
                if (origin.x < boundingBox.Min.x || origin.x > boundingBox.Max.x)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.x;
                var t1 = (boundingBox.Min.x - origin.x) * inverseDirection;
                var t2 = (boundingBox.Max.x - origin.x) * inverseDirection;
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
            if (Fix64.Abs(direction.y) < Toolbox.Epsilon)
            {
                if (origin.y < boundingBox.Min.y || origin.y > boundingBox.Max.y)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.y;
                var t1 = (boundingBox.Min.y - origin.y) * inverseDirection;
                var t2 = (boundingBox.Max.y - origin.y) * inverseDirection;
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
            if (Fix64.Abs(direction.z) < Toolbox.Epsilon)
            {
                if (origin.z < boundingBox.Min.z || origin.z > boundingBox.Max.z)
                {
                    //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it
                    //can't be intersecting.
                    t = F64.C0;
                    return false;
                }
            }
            else
            {
                var inverseDirection = F64.C1 / direction.z;
                var t1 = (boundingBox.Min.z - origin.z) * inverseDirection;
                var t2 = (boundingBox.Max.z - origin.z) * inverseDirection;
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
