﻿using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Symmetrical shape with a circular base and a point at the top.
    ///</summary>
    public class ConeShape : ConvexShape
    {

        Fix64 height;
        ///<summary>
        /// Gets or sets the height of the cone.
        ///</summary>
        public Fix64 Height
        {
            get { return height; }
            set
            {
                height = value;
                OnShapeChanged();
            }
        }

        Fix64 radius;
        ///<summary>
        /// Gets or sets the radius of the cone base.
        ///</summary>
        public Fix64 Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnShapeChanged();
            }
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        public ConeShape(Fix64 height, Fix64 radius)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public ConeShape(Fix64 height, Fix64 radius, ConvexShapeDescription description)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(description);
        }


        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
            base.OnShapeChanged();
        }


        /// <summary>
        /// Computes a convex shape description for a ConeShape.
        /// </summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        ///<param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(Fix64 height, Fix64 radius, Fix64 collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = F64.OneThird * MathHelper.Pi * radius * radius * height;

            description.EntityShapeVolume.VolumeDistribution = new FPMatrix3x3();
            Fix64 diagValue = (F64.C0p1 * height * height + F64.C0p15 * radius * radius);
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = F64.C0p3 * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            description.MaximumRadius = collisionMargin + MathHelper.Max(F64.C0p75 * height, Fix64.Sqrt(F64.C0p0625 * height * height + radius * radius));

            Fix64 denominator = radius / height;
            denominator = denominator / Fix64.Sqrt(denominator * denominator + F64.C1);
            description.MinimumRadius = collisionMargin + MathHelper.Min(F64.C0p25 * height, denominator * F64.C0p75 * height);

            description.CollisionMargin = collisionMargin;
            return description;
        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref FPVector3 direction, out FPVector3 extremePoint)
        {
            //Is it the tip of the cone?
            Fix64 sinThetaSquared = radius * radius / (radius * radius + height * height);
            //If d.Y * d.Y / d.LengthSquared >= sinthetaSquared
            if (direction.y > F64.C0 && direction.y * direction.y >= direction.LengthSquared() * sinThetaSquared)
            {
                extremePoint = new FPVector3(F64.C0, F64.C0p75 * height, F64.C0);
                return;
            }
            //Is it a bottom edge of the cone?
            Fix64 horizontalLengthSquared = direction.x * direction.x + direction.z * direction.z;
            if (horizontalLengthSquared > Toolbox.Epsilon)
            {
                var radOverSigma = radius / Fix64.Sqrt(horizontalLengthSquared);
                extremePoint = new FPVector3((Fix64)(radOverSigma * direction.x), F64.Cm0p25 * height, (Fix64)(radOverSigma * direction.z));
            }
            else // It's pointing almost straight down...
                extremePoint = new FPVector3(F64.C0, F64.Cm0p25 * height, F64.C0);


        }


        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<ConeShape>(this);
        }

    }
}
