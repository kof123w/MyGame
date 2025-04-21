﻿using System;
using FixedMath;
using FixMath.NET;

namespace BEPUik
{
    public abstract class SingleBoneConstraint : IKConstraint
    {
        /// <summary>
        /// Gets or sets the bone associated with the single bone constraint.
        /// </summary>
        public Bone TargetBone { get; set; }


        internal FPVector3 velocityBias;
        internal FPMatrix3x3 linearJacobian;
        internal FPMatrix3x3 angularJacobian;
        internal FPMatrix3x3 effectiveMass;

        internal FPVector3 accumulatedImpulse;




        protected internal override void ComputeEffectiveMass()
        {
            //For all constraints, the effective mass matrix is 1 / (J * M^-1 * JT).
            //For single bone constraints, J has 2 3x3 matrices. M^-1 (W below) is a 6x6 matrix with 2 3x3 block diagonal matrices.
            //To compute the whole denominator,
            FPMatrix3x3 linearW;
            FPMatrix3x3.CreateScale(TargetBone.inverseMass, out linearW);
            FPMatrix3x3 linear;
            FPMatrix3x3.Multiply(ref linearJacobian, ref linearW, out linear); //Compute J * M^-1 for linear component
            FPMatrix3x3.MultiplyByTransposed(ref linear, ref linearJacobian, out linear); //Compute (J * M^-1) * JT for linear component

            FPMatrix3x3 angular;
            FPMatrix3x3.Multiply(ref angularJacobian, ref TargetBone.inertiaTensorInverse, out angular); //Compute J * M^-1 for angular component
            FPMatrix3x3.MultiplyByTransposed(ref angular, ref angularJacobian, out angular); //Compute (J * M^-1) * JT for angular component

            //A nice side effect of the block diagonal nature of M^-1 is that the above separated components are now combined into the complete denominator matrix by addition!
            FPMatrix3x3.Add(ref linear, ref angular, out effectiveMass);

            //Incorporate the constraint softness into the effective mass denominator. This pushes the matrix away from singularity.
            //Softness will also be incorporated into the velocity solve iterations to complete the implementation.
            if (effectiveMass.M11 != F64.C0)
                effectiveMass.M11 += softness;
            if (effectiveMass.M22 != F64.C0)
                effectiveMass.M22 += softness;
            if (effectiveMass.M33 != F64.C0)
                effectiveMass.M33 += softness;

            //Invert! Takes us from J * M^-1 * JT to 1 / (J * M^-1 * JT).
            FPMatrix3x3.AdaptiveInvert(ref effectiveMass, out effectiveMass);

        }

        protected internal override void WarmStart()
        {
            //Take the accumulated impulse and transform it into world space impulses using the jacobians by P = JT * lambda
            //(where P is the impulse, JT is the transposed jacobian matrix, and lambda is the accumulated impulse).
            //Recall the jacobian takes impulses from world space into constraint space, and transpose takes them from constraint space into world space.
            //Compute and apply linear impulse.
            FPVector3 impulse;
            FPMatrix3x3.Transform(ref accumulatedImpulse, ref linearJacobian, out impulse);
            TargetBone.ApplyLinearImpulse(ref impulse);

            //Compute and apply angular impulse.
            FPMatrix3x3.Transform(ref accumulatedImpulse, ref angularJacobian, out impulse);
            TargetBone.ApplyAngularImpulse(ref impulse);
        }

        protected internal override void SolveVelocityIteration()
        {
            //Compute the 'relative' linear and angular velocities. For single bone constraints, it's based entirely on the one bone's velocities!
            //They have to be pulled into constraint space first to compute the necessary impulse, though.
            FPVector3 linearContribution;
            FPMatrix3x3.TransformTranspose(ref TargetBone.linearVelocity, ref linearJacobian, out linearContribution);
            FPVector3 angularContribution;
            FPMatrix3x3.TransformTranspose(ref TargetBone.angularVelocity, ref angularJacobian, out angularContribution);

            //The constraint velocity error will be the velocity we try to remove.
            FPVector3 constraintVelocityError;
            FPVector3.Add(ref linearContribution, ref angularContribution, out constraintVelocityError);
            //However, we need to take into account two extra sources of velocities which modify our target velocity away from zero.
            //First, the velocity bias from position correction:
            FPVector3.Subtract(ref constraintVelocityError, ref velocityBias, out constraintVelocityError);
            //And second, the bias from softness:
            FPVector3 softnessBias;
            FPVector3.Multiply(ref accumulatedImpulse, -softness, out softnessBias);
            FPVector3.Subtract(ref constraintVelocityError, ref softnessBias, out constraintVelocityError);

            //By now, the constraint velocity error contains all the velocity we want to get rid of.
            //Convert it into an impulse using the effective mass matrix.
            FPVector3 constraintSpaceImpulse;
            FPMatrix3x3.Transform(ref constraintVelocityError, ref effectiveMass, out constraintSpaceImpulse);

            FPVector3.Negate(ref constraintSpaceImpulse, out constraintSpaceImpulse);

            //Add the constraint space impulse to the accumulated impulse so that warm starting and softness work properly.
            FPVector3 preadd = accumulatedImpulse;
            FPVector3.Add(ref constraintSpaceImpulse, ref accumulatedImpulse, out accumulatedImpulse);
			//But wait! The accumulated impulse may exceed this constraint's capacity! Check to make sure!
			Fix64 impulseSquared = accumulatedImpulse.LengthSquared();
            if (impulseSquared > maximumImpulseSquared)
            {
                //Oops! Clamp that down.
                FPVector3.Multiply(ref accumulatedImpulse, maximumImpulse / Fix64.Sqrt(impulseSquared), out accumulatedImpulse);
                //Update the impulse based upon the clamped accumulated impulse and the original, pre-add accumulated impulse.
                FPVector3.Subtract(ref accumulatedImpulse, ref preadd, out constraintSpaceImpulse);
            }

            //The constraint space impulse now represents the impulse we want to apply to the bone... but in constraint space.
            //Bring it out to world space using the transposed jacobian.
            FPVector3 linearImpulse;
            FPMatrix3x3.Transform(ref constraintSpaceImpulse, ref linearJacobian, out linearImpulse);
            FPVector3 angularImpulse;
            FPMatrix3x3.Transform(ref constraintSpaceImpulse, ref angularJacobian, out angularImpulse);

            //Apply them!
            TargetBone.ApplyLinearImpulse(ref linearImpulse);
            TargetBone.ApplyAngularImpulse(ref angularImpulse);
        }

        protected internal override void ClearAccumulatedImpulses()
        {
            accumulatedImpulse = new FPVector3();
        }

    }
}
