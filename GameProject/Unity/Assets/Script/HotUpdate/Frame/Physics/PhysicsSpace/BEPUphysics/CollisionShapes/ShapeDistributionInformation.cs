﻿using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes
{
    ///<summary>
    /// Contains data about the distribution of volume in a shape.
    ///</summary>
    public struct ShapeDistributionInformation
    {
        ///<summary>
        /// The distribution of volume in a shape.
        /// This can be scaled to create an inertia tensor for a shape.
        ///</summary>
        public FPMatrix3x3 VolumeDistribution;
        /// <summary>
        /// The center of a shape.
        /// </summary>
        public FPVector3 Center;
        /// <summary>
        /// The volume of a shape.
        /// </summary>
        public Fix64 Volume;

    }
}
