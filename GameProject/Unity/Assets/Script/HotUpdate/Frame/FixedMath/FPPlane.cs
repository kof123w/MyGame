﻿using FixMath.NET;

namespace FixedMath
{
    /// <summary>
    /// Provides XNA-like plane functionality.
    /// </summary>
    public struct FPPlane
    {
        /// <summary>
        /// Normal of the plane.
        /// </summary>
        public FPVector3 Normal;
        /// <summary>
        /// Negative distance to the plane from the origin along the normal.
        /// </summary>
        public Fix64 D;


        /// <summary>
        /// Constructs a new plane.
        /// </summary>
        /// <param name="position">A point on the plane.</param>
        /// <param name="normal">The normal of the plane.</param>
        public FPPlane(ref FPVector3 position, ref FPVector3 normal)
        {
            Fix64 d;
            FPVector3.Dot(ref position, ref normal, out d);
            D = -d;
            Normal = normal;
        }


        /// <summary>
        /// Constructs a new plane.
        /// </summary>
        /// <param name="position">A point on the plane.</param>
        /// <param name="normal">The normal of the plane.</param>
        public FPPlane(FPVector3 position, FPVector3 normal)
            : this(ref position, ref normal)
        {

        }


        /// <summary>
        /// Constructs a new plane.
        /// </summary>
        /// <param name="normal">Normal of the plane.</param>
        /// <param name="d">Negative distance to the plane from the origin along the normal.</param>
        public FPPlane(FPVector3 normal, Fix64 d)
            : this(ref normal, d)
        {
        }

        /// <summary>
        /// Constructs a new plane.
        /// </summary>
        /// <param name="normal">Normal of the plane.</param>
        /// <param name="d">Negative distance to the plane from the origin along the normal.</param>
        public FPPlane(ref FPVector3 normal, Fix64 d)
        {
            this.Normal = normal;
            this.D = d;
        }

        /// <summary>
        /// Gets the dot product of the position offset from the plane along the plane's normal.
        /// </summary>
        /// <param name="v">Position to compute the dot product of.</param>
        /// <param name="dot">Dot product.</param>
        public void DotCoordinate(ref FPVector3 v, out Fix64 dot)
        {
            dot = Normal.x * v.x + Normal.y * v.y + Normal.z * v.z + D;
        }
    }
}
