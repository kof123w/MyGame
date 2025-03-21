using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
 
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Symmetrical object with a circular bottom and top.
    ///</summary>
    public class CylinderShape : ConvexShape
    {
        private Fix64 radius;
        ///<summary>
        /// Gets or sets the radius of the cylinder.
        ///</summary>
        public Fix64 Radius { get { return radius; } set { radius = value; OnShapeChanged(); } }

        private Fix64 halfHeight;
        ///<summary>
        /// Gets or sets the height of the cylinder.
        ///</summary>
        public Fix64 Height { get { return halfHeight * F64.C2; } set { halfHeight = value * F64.C0p5; OnShapeChanged(); } }

        ///<summary>
        /// Constructs a new cylinder shape.
        ///</summary>
        ///<param name="height">Height of the cylinder.</param>
        ///<param name="radius">Radius of the cylinder.</param>
        public CylinderShape(Fix64 height, Fix64 radius)
        {
            halfHeight = height * F64.C0p5;
            this.radius = radius;
            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
        }

        ///<summary>
        /// Constructs a new cylinder shape from cached data.
        ///</summary>
        ///<param name="height">Height of the cylinder.</param>
        ///<param name="radius">Radius of the cylinder.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public CylinderShape(Fix64 height, Fix64 radius, ConvexShapeDescription description)
        {
            halfHeight = height * F64.C0p5;
            this.radius = radius;
            UpdateConvexShapeInfo(description);
        }

        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(Height, radius, collisionMargin));
            base.OnShapeChanged();
        }
        /// <summary>
        /// Computes a convex shape description for a CylinderShape.
        /// </summary>
        ///<param name="height">Height of the cylinder.</param>
        ///<param name="radius">Radius of the cylinder.</param>
        ///<param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(Fix64 height, Fix64 radius, Fix64 collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = MathHelper.Pi * radius * radius * height;

            description.EntityShapeVolume.VolumeDistribution = new FPMatrix3x3();
            Fix64 diagValue = (F64.C0p0833333333 * height * height + F64.C0p25 * radius * radius);
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = F64.C0p5 * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            Fix64 halfHeight = height * F64.C0p5;
            description.MinimumRadius = MathHelper.Min(radius, halfHeight);
            description.MaximumRadius = Fix64.Sqrt(radius * radius + halfHeight * halfHeight);
            description.CollisionMargin = collisionMargin;
            return description;
        }


        /// <summary>
        /// Gets the bounding box of the shape given a transform.
        /// </summary>
        /// <param name="shapeTransform">Transform to use.</param>
        /// <param name="boundingBox">Bounding box of the transformed shape.</param>
        public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif


            FPMatrix3x3 o;
            FPMatrix3x3.CreateFromQuaternion(ref shapeTransform.Orientation, out o);
            //Sample the local directions from the orientation matrix, implicitly transposed.
            //Notice only three directions are used.  Due to cylinder symmetry, 'left' is just -right.
            var direction = new FPVector3(o.M11, o.M21, o.M31);
            FPVector3 right;
            GetLocalExtremePointWithoutMargin(ref direction, out right);

            direction = new FPVector3(o.M12, o.M22, o.M32);
            FPVector3 up;
            GetLocalExtremePointWithoutMargin(ref direction, out up);

            direction = new FPVector3(o.M13, o.M23, o.M33);
            FPVector3 backward;
            GetLocalExtremePointWithoutMargin(ref direction, out backward);

            //Rather than transforming each axis independently (and doing three times as many operations as required), just get the 3 required values directly.
            FPVector3 positive;
            TransformLocalExtremePoints(ref right, ref up, ref backward, ref o, out positive);

            //The positive and negative vectors represent the X, Y and Z coordinates of the extreme points in world space along the world space axes.
            boundingBox.Max.X = shapeTransform.Position.X + positive.X + collisionMargin;
            boundingBox.Max.Y = shapeTransform.Position.Y + positive.Y + collisionMargin;
            boundingBox.Max.Z = shapeTransform.Position.Z + positive.Z + collisionMargin;

            boundingBox.Min.X = shapeTransform.Position.X - positive.X - collisionMargin;
            boundingBox.Min.Y = shapeTransform.Position.Y - positive.Y - collisionMargin;
            boundingBox.Min.Z = shapeTransform.Position.Z - positive.Z - collisionMargin;
        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref FPVector3 direction, out FPVector3 extremePoint)
        {
            Fix64 horizontalLengthSquared = direction.X * direction.X + direction.Z * direction.Z;
            if (horizontalLengthSquared > Toolbox.Epsilon)
            {
                Fix64 multiplier = (radius - collisionMargin) / Fix64.Sqrt(horizontalLengthSquared);
                extremePoint = new FPVector3(direction.X * multiplier, Fix64.Sign(direction.Y) * (halfHeight - collisionMargin), direction.Z * multiplier);
            }
            else
            {
                extremePoint = new FPVector3(F64.C0, Fix64.Sign(direction.Y) * (halfHeight - collisionMargin), F64.C0);
            }

        }

        
        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<CylinderShape>(this);
        }

        /// <summary>
        /// Gets the intersection between the convex shape and the ray.
        /// </summary>
        /// <param name="fpRay">Ray to test.</param>
        /// <param name="transform">Transform of the convex shape.</param>
        /// <param name="maximumLength">Maximum distance to travel in units of the ray direction's length.</param>
        /// <param name="hit">Ray hit data, if any.</param>
        /// <returns>Whether or not the ray hit the target.</returns>
        public override bool RayTest(ref FPRay fpRay, ref RigidTransform transform, Fix64 maximumLength, out FPRayHit hit)
        {
            //Put the ray into local space.
            FPQuaternion conjugate;
            FPQuaternion.Conjugate(ref transform.Orientation, out conjugate);
            FPRay localFpRay;
            FPVector3.Subtract(ref fpRay.origin, ref transform.Position, out localFpRay.origin);
            FPQuaternion.Transform(ref localFpRay.origin, ref conjugate, out localFpRay.origin);
            FPQuaternion.Transform(ref fpRay.direction, ref conjugate, out localFpRay.direction);

            //Check for containment.
            if (localFpRay.origin.Y >= -halfHeight && localFpRay.origin.Y <= halfHeight && localFpRay.origin.X * localFpRay.origin.X + localFpRay.origin.Z * localFpRay.origin.Z <= radius * radius)
            {
                //It's inside!
                hit.T = F64.C0;
                hit.Location = localFpRay.origin;
                hit.Normal = new FPVector3(hit.Location.X, F64.C0, hit.Location.Z);
                Fix64 normalLengthSquared = hit.Normal.LengthSquared();
                if (normalLengthSquared > F64.C1em9)
                    FPVector3.Divide(ref hit.Normal, Fix64.Sqrt(normalLengthSquared), out hit.Normal);
                else
                    hit.Normal = new FPVector3();
                //Pull the hit into world space.
                FPQuaternion.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref hit.Location, ref transform, out hit.Location);
                return true;
            }

            //Project the ray direction onto the plane where the cylinder is a circle.
            //The projected ray is then tested against the circle to compute the time of impact.
            //That time of impact is used to compute the 3d hit location.
            FPVector2 planeDirection = new FPVector2(localFpRay.direction.X, localFpRay.direction.Z);
            Fix64 planeDirectionLengthSquared = planeDirection.LengthSquared();

            if (planeDirectionLengthSquared < Toolbox.Epsilon)
            {
                //The ray is nearly parallel with the axis.
                //Skip the cylinder-sides test.  We're either inside the cylinder and won't hit the sides, or we're outside
                //and won't hit the sides.  
                if (localFpRay.origin.Y > halfHeight)
                    goto upperTest;
                if (localFpRay.origin.Y < -halfHeight)
                    goto lowerTest;


                hit = new FPRayHit();
                return false;

            }
            FPVector2 planeOrigin = new FPVector2(localFpRay.origin.X, localFpRay.origin.Z);
            Fix64 dot;
            FPVector2.Dot(ref planeDirection, ref planeOrigin, out dot);
            Fix64 closestToCenterT = -dot / planeDirectionLengthSquared;

            FPVector2 closestPoint;
            FPVector2.Multiply(ref planeDirection, closestToCenterT, out closestPoint);
            FPVector2.Add(ref planeOrigin, ref closestPoint, out closestPoint);
            //How close does the ray come to the circle?
            Fix64 squaredDistance = closestPoint.LengthSquared();
            if (squaredDistance > radius * radius)
            {
                //It's too far!  The ray cannot possibly hit the capsule.
                hit = new FPRayHit();
                return false;
            }



            //With the squared distance, compute the distance backward along the ray from the closest point on the ray to the axis.
            Fix64 backwardsDistance = radius * Fix64.Sqrt(F64.C1 - squaredDistance / (radius * radius));
            Fix64 tOffset = backwardsDistance / Fix64.Sqrt(planeDirectionLengthSquared);

            hit.T = closestToCenterT - tOffset;

            //Compute the impact point on the infinite cylinder in 3d local space.
            FPVector3.Multiply(ref localFpRay.direction, hit.T, out hit.Location);
            FPVector3.Add(ref hit.Location, ref localFpRay.origin, out hit.Location);

            //Is it intersecting the cylindrical portion of the capsule?
            if (hit.Location.Y <= halfHeight && hit.Location.Y >= -halfHeight && hit.T < maximumLength)
            {
                //Yup!
                hit.Normal = new FPVector3(hit.Location.X, F64.C0, hit.Location.Z);
                Fix64 normalLengthSquared = hit.Normal.LengthSquared();
                if (normalLengthSquared > F64.C1em9)
                    FPVector3.Divide(ref hit.Normal, Fix64.Sqrt(normalLengthSquared), out hit.Normal);
                else
                    hit.Normal = new FPVector3();
                //Pull the hit into world space.
                FPQuaternion.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref hit.Location, ref transform, out hit.Location);
                return true;
            }

            if (hit.Location.Y < halfHeight)
                goto lowerTest;
        upperTest:
            //Nope! It may be intersecting the ends of the cylinder though.
            //We're above the cylinder, so cast a ray against the upper cap.
            if (localFpRay.direction.Y > F64.Cm1em9)
            {
                //Can't hit the upper cap if the ray isn't pointing down.
                hit = new FPRayHit();
                return false;
            }
            Fix64 t = (halfHeight - localFpRay.origin.Y) / localFpRay.direction.Y;
            FPVector3 planeIntersection;
            FPVector3.Multiply(ref localFpRay.direction, t, out planeIntersection);
            FPVector3.Add(ref localFpRay.origin, ref planeIntersection, out planeIntersection);
            if(planeIntersection.X * planeIntersection.X + planeIntersection.Z * planeIntersection.Z < radius * radius + F64.C1em9 && t < maximumLength)
            {
                //Pull the hit into world space.
                FPQuaternion.Transform(ref Toolbox.UpVector, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref planeIntersection, ref transform, out hit.Location);
                hit.T = t;
                return true;
            }
            //No intersection! We can't be hitting the other sphere, so it's over!
            hit = new FPRayHit();
            return false;

        lowerTest:
            //Is it intersecting the bottom cap?
            if (localFpRay.direction.Y < F64.C1em9)
            {
                //Can't hit the bottom cap if the ray isn't pointing up.
                hit = new FPRayHit();
                return false;
            }
            t = (-halfHeight - localFpRay.origin.Y) / localFpRay.direction.Y;
            FPVector3.Multiply(ref localFpRay.direction, t, out planeIntersection);
            FPVector3.Add(ref localFpRay.origin, ref planeIntersection, out planeIntersection);
            if (planeIntersection.X * planeIntersection.X + planeIntersection.Z * planeIntersection.Z < radius * radius + F64.C1em9 && t < maximumLength)
            {
                //Pull the hit into world space.
                FPQuaternion.Transform(ref Toolbox.DownVector, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref planeIntersection, ref transform, out hit.Location);
                hit.T = t;
                return true;
            }
            //No intersection! We can't be hitting the other sphere, so it's over!
            hit = new FPRayHit();
            return false;

        }


    }
}
