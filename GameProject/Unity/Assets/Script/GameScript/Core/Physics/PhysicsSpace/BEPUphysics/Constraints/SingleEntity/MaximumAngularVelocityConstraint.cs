using System;
using BEPUphysics.Entities;
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Prevents the target entity from moving faster than the specified speeds.
    /// </summary>
    public class MaximumAngularSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
    {
        private FPMatrix3x3 effectiveMassMatrix;
        private Fix64 maxForceDt = Fix64.MaxValue;
        private Fix64 maxForceDtSquared = Fix64.MaxValue;
        private FPVector3 accumulatedImpulse;
        private Fix64 maximumForce = Fix64.MaxValue;
        private Fix64 maximumSpeed;
        private Fix64 maximumSpeedSquared;

        private Fix64 softness = (Fix64).00001m;
        private Fix64 usedSoftness;

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// Set its Entity and MaximumSpeed to complete the configuration.
        /// IsActive also starts as false with this constructor.
        /// </summary>
        public MaximumAngularSpeedConstraint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// </summary>
        /// <param name="e">Affected entity.</param>
        /// <param name="maxSpeed">Maximum angular speed allowed.</param>
        public MaximumAngularSpeedConstraint(Entity e, Fix64 maxSpeed)
        {
            Entity = e;
            MaximumSpeed = maxSpeed;
        }

        /// <summary>
        /// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
        /// This field can be used to simulate friction in a constraint.
        /// </summary>
        public Fix64 MaximumForce
        {
            get
            {
                if (maximumForce > F64.C0)
                {
                    return maximumForce;
                }
                return F64.C0;
            }
            set { maximumForce = value >= F64.C0 ? value : F64.C0; }
        }

        /// <summary>
        /// Gets or sets the maximum angular speed that this constraint allows.
        /// </summary>
        public Fix64 MaximumSpeed
        {
            get { return maximumSpeed; }
            set
            {
                maximumSpeed = MathHelper.Max(F64.C0, value);
                maximumSpeedSquared = maximumSpeed * maximumSpeed;
            }
        }


        /// <summary>
        /// Gets and sets the softness of this constraint.
        /// Higher values of softness allow the constraint to be violated more.
        /// Must be greater than zero.
        /// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
        /// For motors, softness can be used to implement damping.  For a damping constant k, the appropriate softness is 1/k.
        /// </summary>
        public Fix64 Softness
        {
            get { return softness; }
            set { softness = MathHelper.Max(F64.C0, value); }
        }

        #region I3DImpulseConstraint Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        FPVector3 I3DImpulseConstraint.RelativeVelocity
        {
            get { return entity.angularVelocity; }
        }

        /// <summary>
        /// Gets the total impulse applied by the constraint.
        /// </summary>
        public FPVector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        #endregion

        /// <summary>
        /// Calculates and applies corrective impulses.
        /// Called automatically by space.
        /// </summary>
        public override Fix64 SolveIteration()
        {
            Fix64 angularSpeed = entity.angularVelocity.LengthSquared();
            if (angularSpeed > maximumSpeedSquared)
            {
                angularSpeed = Fix64.Sqrt(angularSpeed);
                FPVector3 impulse;
                //divide by angularSpeed to normalize the velocity.
                //Multiply by angularSpeed - maximumSpeed to get the 'velocity change vector.'
                FPVector3.Multiply(ref entity.angularVelocity, -(angularSpeed - maximumSpeed) / angularSpeed, out impulse);

                //incorporate softness
                FPVector3 softnessImpulse;
                FPVector3.Multiply(ref accumulatedImpulse, usedSoftness, out softnessImpulse);
                FPVector3.Subtract(ref impulse, ref softnessImpulse, out impulse);

                //Transform into impulse
                FPMatrix3x3.Transform(ref impulse, ref effectiveMassMatrix, out impulse);


                //Accumulate
                FPVector3 previousAccumulatedImpulse = accumulatedImpulse;
                FPVector3.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
                Fix64 forceMagnitude = accumulatedImpulse.LengthSquared();
                if (forceMagnitude > maxForceDtSquared)
                {
                    //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                    Fix64 multiplier = maxForceDt / Fix64.Sqrt(forceMagnitude);
                    accumulatedImpulse.x *= multiplier;
                    accumulatedImpulse.y *= multiplier;
                    accumulatedImpulse.z *= multiplier;

                    //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                    impulse.x = accumulatedImpulse.x - previousAccumulatedImpulse.x;
                    impulse.y = accumulatedImpulse.y - previousAccumulatedImpulse.y;
                    impulse.z = accumulatedImpulse.z - previousAccumulatedImpulse.z;
                }

                entity.ApplyAngularImpulse(ref impulse);


                return (Fix64.Abs(impulse.x) + Fix64.Abs(impulse.y) + Fix64.Abs(impulse.z));
            }

            return F64.C0;
        }

        /// <summary>
        /// Calculates necessary information for velocity solving.
        /// Called automatically by space.
        /// </summary>
        /// <param name="dt">Time in seconds since the last update.</param>
        public override void Update(Fix64 dt)
        {
            usedSoftness = softness / dt;

            effectiveMassMatrix = entity.inertiaTensorInverse;

            effectiveMassMatrix.M11 += usedSoftness;
            effectiveMassMatrix.M22 += usedSoftness;
            effectiveMassMatrix.M33 += usedSoftness;

            FPMatrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);

            //Determine maximum force
            if (maximumForce < Fix64.MaxValue)
            {
                maxForceDt = maximumForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = Fix64.MaxValue;
                maxForceDtSquared = Fix64.MaxValue;
            }

        }


        public override void ExclusiveUpdate()
        {

            //Can't do warmstarting due to the strangeness of this constraint (not based on a position error, nor is it really a motor).
            accumulatedImpulse = Toolbox.ZeroVector;
        }
    }
}