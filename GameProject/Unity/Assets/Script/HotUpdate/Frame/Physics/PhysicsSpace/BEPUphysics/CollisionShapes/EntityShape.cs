﻿using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes
{
    ///<summary>
    /// Superclass of all collision shapes that are used by Entities.
    ///</summary>
    public abstract class EntityShape : CollisionShape
    {

        protected void UpdateEntityShapeVolume(EntityShapeVolumeDescription volume)
        {
            Volume = volume.Volume;
            volumeDistribution = volume.VolumeDistribution;
        }

        /// <summary>
        /// Gets the volume of the shape.
        /// </summary>
        public Fix64 Volume { get; set; }

        internal FPMatrix3x3 volumeDistribution;
        /// <summary>
        /// Gets the volume distribution of the shape.
        /// This can be considered the unscaled inertia tensor of the shape.
        /// By default, entities scale this distribution by Mass * InertiaHelper.InertiaTensorScale to compute their local inertia tensor.
        /// </summary>
        public FPMatrix3x3 VolumeDistribution
        {
            get { return volumeDistribution; }
        }


        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public abstract EntityCollidable GetCollidableInstance();

        /// <summary>
        /// Computes a bounding box for the shape given the specified transform.
        /// </summary>
        /// <param name="transform">Transform to apply to the shape to compute the bounding box.</param>
        /// <param name="boundingBox">Bounding box for the shape given the transform.</param>
        public abstract void GetBoundingBox(ref RigidTransform transform, out BoundingBox boundingBox);

        /// <summary>
        /// Transforms a set of local extreme points by a linear transform and stores each axis result in the appropriate component of the result vector.
        /// Requires that the input variables and output variables are separate.
        /// </summary>
        /// <param name="x">Local extreme point along the world X axis.</param>
        /// <param name="y">Local extreme point along the world Y axis.</param>
        /// <param name="z">Local extreme point along the world Z axis.</param>
        /// <param name="transform">World transform.</param>
        /// <param name="result">Contains the transformed X coordinate of input X, transformed Y coordinate of input Y, and transformed Z coordinate of input Z.</param>
        protected void TransformLocalExtremePoints(ref FPVector3 x, ref FPVector3 y, ref FPVector3 z, ref FPMatrix3x3 transform, out FPVector3 result)
        {
            result.x = x.x * transform.M11 + x.y * transform.M21 + x.z * transform.M31;
            result.y = y.x * transform.M12 + y.y * transform.M22 + y.z * transform.M32;
            result.z = z.x * transform.M13 + z.y * transform.M23 + z.z * transform.M33;
        }
    }
}
