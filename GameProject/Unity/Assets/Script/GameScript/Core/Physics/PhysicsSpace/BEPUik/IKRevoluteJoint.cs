﻿using System;
using FixedMath;
using FixMath.NET;

namespace BEPUik
{
    public class IKRevoluteJoint : IKJoint
    {
        private FPVector3 localFreeAxisA;
        /// <summary>
        /// Gets or sets the free axis in connection A's local space.
        /// Must be unit length.
        /// </summary>
        public FPVector3 LocalFreeAxisA
        {
            get { return localFreeAxisA; }
            set
            {
                localFreeAxisA = value;
                ComputeConstrainedAxes();
            }
        }

        private FPVector3 localFreeAxisB;
        /// <summary>
        /// Gets or sets the free axis in connection B's local space.
        /// Must be unit length.
        /// </summary>
        public FPVector3 LocalFreeAxisB
        {
            get { return localFreeAxisB; }
            set
            {
                localFreeAxisB = value;
                ComputeConstrainedAxes();
            }
        }



        /// <summary>
        /// Gets or sets the free axis attached to connection A in world space.
        /// This does not change the other connection's free axis.
        /// </summary>
        public FPVector3 WorldFreeAxisA
        {
            get { return FPQuaternion.Transform(localFreeAxisA, ConnectionA.Orientation); }
            set
            {
                LocalFreeAxisA = FPQuaternion.Transform(value, FPQuaternion.Conjugate(ConnectionA.Orientation));
            }
        }

        /// <summary>
        /// Gets or sets the free axis attached to connection B in world space.
        /// This does not change the other connection's free axis.
        /// </summary>
        public FPVector3 WorldFreeAxisB
        {
            get { return FPQuaternion.Transform(localFreeAxisB, ConnectionB.Orientation); }
            set
            {
                LocalFreeAxisB = FPQuaternion.Transform(value, FPQuaternion.Conjugate(ConnectionB.Orientation));
            }
        }

        private FPVector3 localConstrainedAxis1, localConstrainedAxis2;
        void ComputeConstrainedAxes()
        {
            FPVector3 worldAxisA = WorldFreeAxisA;
            FPVector3 error = FPVector3.Cross(worldAxisA, WorldFreeAxisB);
            Fix64 lengthSquared = error.LengthSquared();
            FPVector3 worldConstrainedAxis1, worldConstrainedAxis2;
            //Find the first constrained axis.
            if (lengthSquared > Toolbox.Epsilon)
            {
                //The error direction can be used as the first axis!
                FPVector3.Divide(ref error, Fix64.Sqrt(lengthSquared), out worldConstrainedAxis1);
            }
            else
            {
                //There's not enough error for it to be a good constrained axis.
                //We'll need to create the constrained axes arbitrarily.
                FPVector3.Cross(ref Toolbox.UpVector, ref worldAxisA, out worldConstrainedAxis1);
                lengthSquared = worldConstrainedAxis1.LengthSquared();
                if (lengthSquared > Toolbox.Epsilon)
                {
                    //The up vector worked!
                    FPVector3.Divide(ref worldConstrainedAxis1, Fix64.Sqrt(lengthSquared), out worldConstrainedAxis1);
                }
                else
                {
                    //The up vector didn't work. Just try the right vector.
                    FPVector3.Cross(ref Toolbox.RightVector, ref worldAxisA, out worldConstrainedAxis1);
                    worldConstrainedAxis1.Normalize();
                }
            }
            //Don't have to normalize the second constraint axis; it's the cross product of two perpendicular normalized vectors.
            FPVector3.Cross(ref worldAxisA, ref worldConstrainedAxis1, out worldConstrainedAxis2);

            localConstrainedAxis1 = FPQuaternion.Transform(worldConstrainedAxis1, FPQuaternion.Conjugate(ConnectionA.Orientation));
            localConstrainedAxis2 = FPQuaternion.Transform(worldConstrainedAxis2, FPQuaternion.Conjugate(ConnectionA.Orientation));
        }

        /// <summary>
        /// Constructs a new orientation joint.
        /// Orientation joints can be used to simulate the angular portion of a hinge.
        /// Orientation joints allow rotation around only a single axis.
        /// </summary>
        /// <param name="connectionA">First entity connected in the orientation joint.</param>
        /// <param name="connectionB">Second entity connected in the orientation joint.</param>
        /// <param name="freeAxis">Axis allowed to rotate freely in world space.</param>
        public IKRevoluteJoint(Bone connectionA, Bone connectionB, FPVector3 freeAxis)
            : base(connectionA, connectionB)
        {
            WorldFreeAxisA = freeAxis;
            WorldFreeAxisB = freeAxis;
        }

        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            linearJacobianA = linearJacobianB = new FPMatrix3x3();

            //We know the one free axis. We need the two restricted axes. This amounts to completing the orthonormal basis.
            //We can grab one of the restricted axes using a cross product of the two world axes. This is not guaranteed
            //to be nonzero, so the normalization requires protection.

            FPVector3 worldAxisA, worldAxisB;
            FPQuaternion.Transform(ref localFreeAxisA, ref ConnectionA.Orientation, out worldAxisA);
            FPQuaternion.Transform(ref localFreeAxisB, ref ConnectionB.Orientation, out worldAxisB);

            FPVector3 error;
            FPVector3.Cross(ref worldAxisA, ref worldAxisB, out error);

            FPVector3 worldConstrainedAxis1, worldConstrainedAxis2;
            FPQuaternion.Transform(ref localConstrainedAxis1, ref ConnectionA.Orientation, out worldConstrainedAxis1);
            FPQuaternion.Transform(ref localConstrainedAxis2, ref ConnectionA.Orientation, out worldConstrainedAxis2);


            angularJacobianA = new FPMatrix3x3
            {
                M11 = worldConstrainedAxis1.x,
                M12 = worldConstrainedAxis1.y,
                M13 = worldConstrainedAxis1.z,
                M21 = worldConstrainedAxis2.x,
                M22 = worldConstrainedAxis2.y,
                M23 = worldConstrainedAxis2.z
            };
            FPMatrix3x3.Negate(ref angularJacobianA, out angularJacobianB);


            FPVector2 constraintSpaceError;
            FPVector3.Dot(ref error, ref worldConstrainedAxis1, out constraintSpaceError.x);
            FPVector3.Dot(ref error, ref worldConstrainedAxis2, out constraintSpaceError.y);
            velocityBias.x = errorCorrectionFactor * constraintSpaceError.x;
            velocityBias.y = errorCorrectionFactor * constraintSpaceError.y;


        }
    }
}
