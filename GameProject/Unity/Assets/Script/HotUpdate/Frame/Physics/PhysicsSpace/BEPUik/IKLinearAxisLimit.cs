﻿using FixedMath;
using FixMath.NET;

namespace BEPUik
{
    /// <summary>
    /// Keeps an anchor point on one bone between planes defined by another bone.
    /// </summary>
    public class IKLinearAxisLimit : IKLimit
    {
        /// <summary>
        /// Gets or sets the offset in connection A's local space from the center of mass to the anchor point of the line.
        /// </summary>
        public FPVector3 LocalLineAnchor;

        /// <summary>
        /// Gets or sets the direction of the line in connection A's local space.
        /// Must be unit length.
        /// </summary>
        public FPVector3 LocalLineDirection;

        /// <summary>
        /// Gets or sets the offset in connection B's local space from the center of mass to the anchor point which will be kept on the line.
        /// </summary>
        public FPVector3 LocalAnchorB;

        /// <summary>
        /// Gets or sets the world space location of the line anchor attached to connection A.
        /// </summary>
        public FPVector3 LineAnchor
        {
            get { return ConnectionA.Position + FPQuaternion.Transform(LocalLineAnchor, ConnectionA.Orientation); }
            set { LocalLineAnchor = FPQuaternion.Transform(value - ConnectionA.Position, FPQuaternion.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the world space direction of the line attached to connection A.
        /// Must be unit length.
        /// </summary>
        public FPVector3 LineDirection
        {
            get { return FPQuaternion.Transform(LocalLineDirection, ConnectionA.Orientation); }
            set { LocalLineDirection = FPQuaternion.Transform(value, FPQuaternion.Conjugate(ConnectionA.Orientation)); }
        }

        /// <summary>
        /// Gets or sets the offset in world space from the center of mass of connection B to the anchor point.
        /// </summary>
        public FPVector3 AnchorB
        {
            get { return ConnectionB.Position + FPQuaternion.Transform(LocalAnchorB, ConnectionB.Orientation); }
            set { LocalAnchorB = FPQuaternion.Transform(value - ConnectionB.Position, FPQuaternion.Conjugate(ConnectionB.Orientation)); }
        }

        private Fix64 minimumDistance;
        /// <summary>
        /// Gets or sets the minimum distance that the joint connections should be kept from each other.
        /// </summary>
        public Fix64 MinimumDistance
        {
            get { return minimumDistance; }
            set { minimumDistance = value; }
        }

         private Fix64 maximumDistance;
        /// <summary>
        /// Gets or sets the maximum distance that the joint connections should be kept from each other.
        /// </summary>
        public Fix64 MaximumDistance
        {
            get { return maximumDistance; }
            set { maximumDistance = value; }
        }

        /// <summary>
        /// Constructs a new axis limit.
        /// </summary>
        /// <param name="connectionA">First bone connected by the joint.</param>
        /// <param name="connectionB">Second bone connected by the joint.</param>
        /// <param name="lineAnchor">Anchor point of the line attached to the first bone in world space.</param>
        /// <param name="lineDirection">Direction of the line attached to the first bone in world space. Must be unit length.</param>
        /// <param name="anchorB">Anchor point on the second bone in world space which is measured against the other connection's anchor.</param>
        /// <param name="minimumDistance">Minimum distance that the joint connections should be kept from each other along the axis.</param>
        /// <param name="maximumDistance">Maximum distance that the joint connections should be kept from each other along the axis.</param>
        public IKLinearAxisLimit(Bone connectionA, Bone connectionB, FPVector3 lineAnchor, FPVector3 lineDirection, FPVector3 anchorB, Fix64 minimumDistance, Fix64 maximumDistance)
            : base(connectionA, connectionB)
        {
            LineAnchor = lineAnchor;
            LineDirection = lineDirection;
            AnchorB = anchorB;
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            //Transform the anchors and offsets into world space.
            FPVector3 offsetA, offsetB, lineDirection;
            FPQuaternion.Transform(ref LocalLineAnchor, ref ConnectionA.Orientation, out offsetA);
            FPQuaternion.Transform(ref LocalLineDirection, ref ConnectionA.Orientation, out lineDirection);
            FPQuaternion.Transform(ref LocalAnchorB, ref ConnectionB.Orientation, out offsetB);
            FPVector3 anchorA, anchorB;
            FPVector3.Add(ref ConnectionA.Position, ref offsetA, out anchorA);
            FPVector3.Add(ref ConnectionB.Position, ref offsetB, out anchorB);

            //Compute the distance.
            FPVector3 separation;
            FPVector3.Subtract(ref anchorB, ref anchorA, out separation);
            //This entire constraint is very similar to the IKDistanceLimit, except the current distance is along an axis.
            Fix64 currentDistance;
            FPVector3.Dot(ref separation, ref lineDirection, out currentDistance);

            //Compute jacobians
            if (currentDistance > maximumDistance)
            {
                //We are exceeding the maximum limit.
                velocityBias = new FPVector3(errorCorrectionFactor * (currentDistance - maximumDistance), F64.C0, F64.C0);
            }
            else if (currentDistance < minimumDistance)
            {
                //We are exceeding the minimum limit.
                velocityBias = new FPVector3(errorCorrectionFactor * (minimumDistance - currentDistance), F64.C0, F64.C0);
                //The limit can only push in one direction. Flip the jacobian!
                FPVector3.Negate(ref lineDirection, out lineDirection);
            }
            else if (currentDistance - minimumDistance > (maximumDistance - minimumDistance) * F64.C0p5)
            {
                //The objects are closer to hitting the maximum limit.
                velocityBias = new FPVector3(currentDistance - maximumDistance, F64.C0, F64.C0);
            }
            else
            {
                //The objects are closer to hitting the minimum limit.
                velocityBias = new FPVector3(minimumDistance - currentDistance, F64.C0, F64.C0);
                //The limit can only push in one direction. Flip the jacobian!
                FPVector3.Negate(ref lineDirection, out lineDirection);
            }

            FPVector3 angularA, angularB;
            //We can't just use the offset to anchor for A's jacobian- the 'collision' location is way out there at anchorB!
            FPVector3 rA;
            FPVector3.Subtract(ref anchorB, ref ConnectionA.Position, out rA);
            FPVector3.Cross(ref rA, ref lineDirection, out angularA);
            //linearB = -linearA, so just swap the cross product order.
            FPVector3.Cross(ref lineDirection, ref offsetB, out angularB);

            //Put all the 1x3 jacobians into a 3x3 matrix representation.
            linearJacobianA = new FPMatrix3x3 { M11 = lineDirection.x, M12 = lineDirection.y, M13 = lineDirection.z };
            linearJacobianB = new FPMatrix3x3 { M11 = -lineDirection.x, M12 = -lineDirection.y, M13 = -lineDirection.z };
            angularJacobianA = new FPMatrix3x3 { M11 = angularA.x, M12 = angularA.y, M13 = angularA.z };
            angularJacobianB = new FPMatrix3x3 { M11 = angularB.x, M12 = angularB.y, M13 = angularB.z };

        }
    }
}
