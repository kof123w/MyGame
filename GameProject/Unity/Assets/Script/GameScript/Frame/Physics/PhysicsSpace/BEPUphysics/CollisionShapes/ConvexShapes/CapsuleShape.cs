﻿using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Sphere-expanded line segment.  Another way of looking at it is a cylinder with half-spheres on each end.
    ///</summary>
    public class CapsuleShape : ConvexShape
    {
        Fix64 halfLength;
        ///<summary>
        /// Gets or sets the length of the capsule's inner line segment.
        ///</summary>
        public Fix64 Length
        {
            get
            {
                return halfLength * F64.C2;
            }
            set
            {
                halfLength = value * F64.C0p5;
                OnShapeChanged();
            }
        }

        //This is a convenience method.  People expect to see a 'radius' of some kind.
        ///<summary>
        /// Gets or sets the radius of the capsule.
        ///</summary>
        public Fix64 Radius { get { return collisionMargin; } set { CollisionMargin = value; } }


        ///<summary>
        /// Constructs a new capsule shape.
        ///</summary>
        ///<param name="length">Length of the capsule's inner line segment.</param>
        ///<param name="radius">Radius to expand the line segment width.</param>
        public CapsuleShape(Fix64 length, Fix64 radius)
        {
            halfLength = length * F64.C0p5;

            UpdateConvexShapeInfo(ComputeDescription(length, radius));
        }

        ///<summary>
        /// Constructs a new capsule shape from cached information.
        ///</summary>
        ///<param name="length">Length of the capsule's inner line segment.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public CapsuleShape(Fix64 length, ConvexShapeDescription description)
        {
            halfLength = length * F64.C0p5;

            UpdateConvexShapeInfo(description);
        }



        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(halfLength * F64.C2, Radius));
            base.OnShapeChanged();
        }


        /// <summary>
        /// Computes a convex shape description for a CapsuleShape.
        /// </summary>
        ///<param name="length">Length of the capsule's inner line segment.</param>
        ///<param name="radius">Radius to expand the line segment width.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(Fix64 length, Fix64 radius)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = MathHelper.Pi * radius * radius * length + F64.FourThirds * MathHelper.Pi * radius * radius * radius;

            description.EntityShapeVolume.VolumeDistribution = new FPMatrix3x3();
            Fix64 effectiveLength = length + radius / F64.C2; //This is a cylindrical inertia tensor. Approximate.
            Fix64 diagValue = F64.C0p0833333333 * effectiveLength * effectiveLength + F64.C0p25 * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = F64.C0p5 * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            description.MaximumRadius = length * F64.C0p5 + radius;
            description.MinimumRadius = radius;

            description.CollisionMargin = radius;
            return description;
        }

        public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif
            FPVector3 upExtreme;
            FPQuaternion.TransformY(halfLength, ref shapeTransform.Orientation, out upExtreme);

            if (upExtreme.x > F64.C0)
            {
                boundingBox.Max.x = upExtreme.x + collisionMargin;
                boundingBox.Min.x = -upExtreme.x - collisionMargin;
            }
            else
            {
                boundingBox.Max.x = -upExtreme.x + collisionMargin;
                boundingBox.Min.x = upExtreme.x - collisionMargin;
            }

            if (upExtreme.y > F64.C0)
            {
                boundingBox.Max.y = upExtreme.y + collisionMargin;
                boundingBox.Min.y = -upExtreme.y - collisionMargin;
            }
            else
            {
                boundingBox.Max.y = -upExtreme.y + collisionMargin;
                boundingBox.Min.y = upExtreme.y - collisionMargin;
            }

            if (upExtreme.z > F64.C0)
            {
                boundingBox.Max.z = upExtreme.z + collisionMargin;
                boundingBox.Min.z = -upExtreme.z - collisionMargin;
            }
            else
            {
                boundingBox.Max.z = -upExtreme.z + collisionMargin;
                boundingBox.Min.z = upExtreme.z - collisionMargin;
            }

            FPVector3.Add(ref shapeTransform.Position, ref boundingBox.Min, out boundingBox.Min);
            FPVector3.Add(ref shapeTransform.Position, ref boundingBox.Max, out boundingBox.Max);
        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref FPVector3 direction, out FPVector3 extremePoint)
        {
            if (direction.y > F64.C0)
                extremePoint = new FPVector3(F64.C0, halfLength, F64.C0);
            else if (direction.y < F64.C0)
                extremePoint = new FPVector3(F64.C0, -halfLength, F64.C0);
            else
                extremePoint = Toolbox.ZeroVector;
        }




        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<CapsuleShape>(this);
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

            //Check for containment in the cylindrical portion of the capsule.
            if (localFpRay.origin.y >= -halfLength && localFpRay.origin.y <= halfLength && localFpRay.origin.x * localFpRay.origin.x + localFpRay.origin.z * localFpRay.origin.z <= collisionMargin * collisionMargin)
            {
                //It's inside!
                hit.T = F64.C0;
                hit.Location = localFpRay.origin;
                hit.Normal = new FPVector3(hit.Location.x, F64.C0, hit.Location.z);
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
            FPVector2 planeDirection = new FPVector2(localFpRay.direction.x, localFpRay.direction.z);
            Fix64 planeDirectionLengthSquared = planeDirection.LengthSquared();

            if (planeDirectionLengthSquared < Toolbox.Epsilon)
            {
                //The ray is nearly parallel with the axis.
                //Skip the cylinder-sides test.  We're either inside the cylinder and won't hit the sides, or we're outside
                //and won't hit the sides.  
                if (localFpRay.origin.y > halfLength)
                    goto upperSphereTest;
                if (localFpRay.origin.y < -halfLength)
                    goto lowerSphereTest;


                hit = new FPRayHit();
                return false;

            }
            FPVector2 planeOrigin = new FPVector2(localFpRay.origin.x, localFpRay.origin.z);
            Fix64 dot;
            FPVector2.Dot(ref planeDirection, ref planeOrigin, out dot);
            Fix64 closestToCenterT = -dot / planeDirectionLengthSquared;

            FPVector2 closestPoint;
            FPVector2.Multiply(ref planeDirection, closestToCenterT, out closestPoint);
            FPVector2.Add(ref planeOrigin, ref closestPoint, out closestPoint);
            //How close does the ray come to the circle?
            Fix64 squaredDistance = closestPoint.LengthSquared();
            if (squaredDistance > collisionMargin * collisionMargin)
            {
                //It's too far!  The ray cannot possibly hit the capsule.
                hit = new FPRayHit();
                return false;
            }



            //With the squared distance, compute the distance backward along the ray from the closest point on the ray to the axis.
            Fix64 backwardsDistance = collisionMargin * Fix64.Sqrt(F64.C1 - squaredDistance / (collisionMargin * collisionMargin));
            Fix64 tOffset = backwardsDistance / Fix64.Sqrt(planeDirectionLengthSquared);

            hit.T = closestToCenterT - tOffset;

            //Compute the impact point on the infinite cylinder in 3d local space.
            FPVector3.Multiply(ref localFpRay.direction, hit.T, out hit.Location);
            FPVector3.Add(ref hit.Location, ref localFpRay.origin, out hit.Location);

            //Is it intersecting the cylindrical portion of the capsule?
            if (hit.Location.y <= halfLength && hit.Location.y >= -halfLength && hit.T < maximumLength)
            {
                //Yup!
                hit.Normal = new FPVector3(hit.Location.x, F64.C0, hit.Location.z);
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

            if (hit.Location.y < halfLength)
                goto lowerSphereTest;
        upperSphereTest:
            //Nope! It may be intersecting the ends of the capsule though.
            //We're above the capsule, so cast a ray against the upper sphere.
            //We don't have to worry about it hitting the bottom of the sphere since it would have hit the cylinder portion first.
            var spherePosition = new FPVector3(F64.C0, halfLength, F64.C0);
            if (Toolbox.RayCastSphere(ref localFpRay, ref spherePosition, collisionMargin, maximumLength, out hit))
            {
                //Pull the hit into world space.
                FPQuaternion.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref hit.Location, ref transform, out hit.Location);
                return true;
            }
            //No intersection! We can't be hitting the other sphere, so it's over!
            hit = new FPRayHit();
            return false;

        lowerSphereTest:
            //Okay, what about the bottom sphere?
            //We're above the capsule, so cast a ray against the upper sphere.
            //We don't have to worry about it hitting the bottom of the sphere since it would have hit the cylinder portion first.
            spherePosition = new FPVector3(F64.C0, -halfLength, F64.C0);
            if (Toolbox.RayCastSphere(ref localFpRay, ref spherePosition, collisionMargin, maximumLength, out hit))
            {
                //Pull the hit into world space.
                FPQuaternion.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
                RigidTransform.Transform(ref hit.Location, ref transform, out hit.Location);
                return true;
            }
            //No intersection! We can't be hitting the other sphere, so it's over!
            hit = new FPRayHit();
            return false;

        }

    }
}
