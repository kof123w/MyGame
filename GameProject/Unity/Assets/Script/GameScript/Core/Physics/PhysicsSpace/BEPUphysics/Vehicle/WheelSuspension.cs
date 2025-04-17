﻿using BEPUphysics.Constraints;
using BEPUphysics.Entities;

using FixedMath;
using FixMath.NET;

namespace BEPUphysics.Vehicle
{
    /// <summary>
    /// Allows the connected wheel and vehicle to smoothly absorb bumps.
    /// </summary>
    public class WheelSuspension : ISpringSettings, ISolverSettings
    {
        private readonly SpringSettings springSettings = new SpringSettings();


        internal Fix64 accumulatedImpulse;

        //Fix64 linearBX, linearBY, linearBZ;
        private Fix64 angularAX, angularAY, angularAZ;
        private Fix64 angularBX, angularBY, angularBZ;
        private Fix64 bias;

        internal bool isActive = true;
        private Fix64 linearAX, linearAY, linearAZ;
        private Fix64 allowedCompression = (Fix64).01m;
        internal Fix64 currentLength;
        internal FPVector3 localAttachmentPoint;
        internal FPVector3 localDirection;
        private Fix64 maximumSpringCorrectionSpeed = Fix64.MaxValue;
        private Fix64 maximumSpringForce = Fix64.MaxValue;
        internal Fix64 restLength;
        internal SolverSettings solverSettings = new SolverSettings();
        private Wheel wheel;
        internal FPVector3 worldAttachmentPoint;
        internal FPVector3 worldDirection;
        internal int numIterationsAtZeroImpulse;
        private Entity vehicleEntity, supportEntity;
        private Fix64 softness;

        //Inverse effective mass matrix
        private Fix64 velocityToImpulse;
        private bool supportIsDynamic;

        /// <summary>
        /// Constructs a new suspension for a wheel.
        /// </summary>
        /// <param name="stiffnessConstant">Strength of the spring.  Higher values resist compression more.</param>
        /// <param name="dampingConstant">Damping constant of the spring.  Higher values remove more momentum.</param>
        /// <param name="localDirection">Direction of the suspension in the vehicle's local space.  For a normal, straight down suspension, this would be (0, -1, 0).</param>
        /// <param name="restLength">Length of the suspension when uncompressed.</param>
        /// <param name="localAttachmentPoint">Place where the suspension hooks up to the body of the vehicle.</param>
        public WheelSuspension(Fix64 stiffnessConstant, Fix64 dampingConstant, FPVector3 localDirection, Fix64 restLength, FPVector3 localAttachmentPoint)
        {
            SpringSettings.Stiffness = stiffnessConstant;
            SpringSettings.Damping = dampingConstant;
            LocalDirection = localDirection;
            RestLength = restLength;
            LocalAttachmentPoint = localAttachmentPoint;
        }

        internal WheelSuspension(Wheel wheel)
        {
            Wheel = wheel;
        }

        /// <summary>
        /// Gets or sets the allowed compression of the suspension before suspension forces take effect.
        /// Usually a very small number.  Used to prevent 'jitter' where the wheel leaves the ground due to spring forces repeatedly.
        /// </summary>
        public Fix64 AllowedCompression
        {
            get { return allowedCompression; }
            set { allowedCompression = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Gets the the current length of the suspension.
        /// This will be less than the RestLength if the suspension is compressed.
        /// </summary>
        public Fix64 CurrentLength
        {
            get { return currentLength; }
        }

        /// <summary>
        /// Gets or sets the attachment point of the suspension to the vehicle body in the body's local space.
        /// </summary>
        public FPVector3 LocalAttachmentPoint
        {
            get { return localAttachmentPoint; }
            set
            {
                localAttachmentPoint = value;
                if (wheel != null && wheel.vehicle != null)
                {
                    RigidTransform.Transform(ref localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);

                }
                else
                    worldAttachmentPoint = localAttachmentPoint;
            }
        }



        /// <summary>
        /// Gets or sets the maximum speed at which the suspension will try to return the suspension to rest length.
        /// </summary>
        public Fix64 MaximumSpringCorrectionSpeed
        {
            get { return maximumSpringCorrectionSpeed; }
            set { maximumSpringCorrectionSpeed = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Gets or sets the maximum force that can be applied by this suspension.
        /// </summary>
        public Fix64 MaximumSpringForce
        {
            get { return maximumSpringForce; }
            set { maximumSpringForce = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Gets or sets the length of the uncompressed suspension.
        /// </summary>
        public Fix64 RestLength
        {
            get { return restLength; }
            set
            {
                restLength = value;
                if (wheel != null)
                    wheel.shape.Initialize();
            }
        }

        /// <summary>
        /// Gets the force that the suspension is applying to support the vehicle.
        /// </summary>
        public Fix64 TotalImpulse
        {
            get { return -accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the wheel that this suspension applies to.
        /// </summary>
        public Wheel Wheel
        {
            get { return wheel; }
            internal set { wheel = value; }
        }

        /// <summary>
        /// Gets or sets the attachment point of the suspension to the vehicle body in world space.
        /// </summary>
        public FPVector3 WorldAttachmentPoint
        {
            get { return worldAttachmentPoint; }
            set
            {
                worldAttachmentPoint = value;
                if (wheel != null && wheel.vehicle != null)
                {
                    RigidTransform.TransformByInverse(ref worldAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out localAttachmentPoint);
                }
                else
                    localAttachmentPoint = worldAttachmentPoint;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the wheel suspension in the local space of the vehicle body.
        /// A normal, straight suspension would be (0,-1,0).
        /// </summary>
        public FPVector3 LocalDirection
        {
            get { return localDirection; }
            set
            {
                localDirection = FPVector3.Normalize(value);
                if (wheel != null)
                    wheel.shape.Initialize();
                if (wheel != null && wheel.vehicle != null)
                    FPMatrix3x3.Transform(ref localDirection, ref wheel.vehicle.Body.orientationMatrix, out worldDirection);
                else
                    worldDirection = localDirection;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the wheel suspension in the world space of the vehicle body.
        /// </summary>
        public FPVector3 WorldDirection
        {
            get { return worldDirection; }
            set
            {
                worldDirection = FPVector3.Normalize(value);
                if (wheel != null)
                    wheel.shape.Initialize();
                if (wheel != null && wheel.vehicle != null)
                    FPMatrix3x3.TransformTranspose(ref worldDirection, ref wheel.Vehicle.Body.orientationMatrix, out localDirection);
                else
                    localDirection = worldDirection;
            }
        }

        #region ISolverSettings Members

        /// <summary>
        /// Gets the solver settings used by this wheel constraint.
        /// </summary>
        public SolverSettings SolverSettings
        {
            get { return solverSettings; }
        }

        #endregion

        #region ISpringSettings Members

        /// <summary>
        /// Gets the spring settings that define the behavior of the suspension.
        /// </summary>
        public SpringSettings SpringSettings
        {
            get { return springSettings; }
        }

        #endregion


        ///<summary>
        /// Gets the relative velocity along the support normal at the contact point.
        ///</summary>
        public Fix64 RelativeVelocity
        {
            get
            {
                Fix64 velocity = vehicleEntity.linearVelocity.x * linearAX + vehicleEntity.linearVelocity.y * linearAY + vehicleEntity.linearVelocity.z * linearAZ +
                                 vehicleEntity.angularVelocity.x * angularAX + vehicleEntity.angularVelocity.y * angularAY + vehicleEntity.angularVelocity.z * angularAZ;
                if (supportEntity != null)
                    velocity += -supportEntity.linearVelocity.x * linearAX - supportEntity.linearVelocity.y * linearAY - supportEntity.linearVelocity.z * linearAZ +
                                supportEntity.angularVelocity.x * angularBX + supportEntity.angularVelocity.y * angularBY + supportEntity.angularVelocity.z * angularBZ;
                return velocity;
            }
        }

        internal Fix64 ApplyImpulse()
        {
            //Compute relative velocity
            Fix64 lambda = (RelativeVelocity
                            + bias //Add in position correction
                            + softness * accumulatedImpulse) //Add in squishiness
                           * velocityToImpulse; //convert to impulse


            //Clamp accumulated impulse
            Fix64 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + lambda, -maximumSpringForce, F64.C0);
            lambda = accumulatedImpulse - previousAccumulatedImpulse;

            //Apply the impulse
#if !WINDOWS
            FPVector3 linear = new FPVector3();
            FPVector3 angular = new FPVector3();
#else
            Vector3 linear, angular;
#endif
            linear.x = lambda * linearAX;
            linear.y = lambda * linearAY;
            linear.z = lambda * linearAZ;
            if (vehicleEntity.isDynamic)
            {
                angular.x = lambda * angularAX;
                angular.y = lambda * angularAY;
                angular.z = lambda * angularAZ;
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }
            if (supportIsDynamic)
            {
                linear.x = -linear.x;
                linear.y = -linear.y;
                linear.z = -linear.z;
                angular.x = lambda * angularBX;
                angular.y = lambda * angularBY;
                angular.z = lambda * angularBZ;
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }

            return lambda;
        }

        internal void ComputeWorldSpaceData()
        {
            //Transform local space vectors to world space.
            RigidTransform.Transform(ref localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);
            FPMatrix3x3.Transform(ref localDirection, ref wheel.vehicle.Body.orientationMatrix, out worldDirection);
        }

        internal void OnAdditionToVehicle()
        {
            //This looks weird, but it's just re-setting the world locations.
            //If the wheel doesn't belong to a vehicle (or this doesn't belong to a wheel)
            //then the world space location can't be set.
            LocalDirection = LocalDirection;
            LocalAttachmentPoint = LocalAttachmentPoint;
        }

        internal void PreStep(Fix64 dt)
        {
            vehicleEntity = wheel.vehicle.Body;
            supportEntity = wheel.supportingEntity;
            supportIsDynamic = supportEntity != null && supportEntity.isDynamic;

            //The next line is commented out because the world direction is computed by the wheelshape.  Weird, but necessary.
            //Vector3.TransformNormal(ref myLocalDirection, ref parentA.myInternalOrientationMatrix, out myWorldDirection);

            //Set up the jacobians.
            linearAX = -wheel.normal.x; //myWorldDirection.X;
            linearAY = -wheel.normal.y; //myWorldDirection.Y;
            linearAZ = -wheel.normal.z; // myWorldDirection.Z;
            //linearBX = -linearAX;
            //linearBY = -linearAY;
            //linearBZ = -linearAZ;

            //angular A = Ra x N
            angularAX = (wheel.ra.y * linearAZ) - (wheel.ra.z * linearAY);
            angularAY = (wheel.ra.z * linearAX) - (wheel.ra.x * linearAZ);
            angularAZ = (wheel.ra.x * linearAY) - (wheel.ra.y * linearAX);

            //Angular B = N x Rb
            angularBX = (linearAY * wheel.rb.z) - (linearAZ * wheel.rb.y);
            angularBY = (linearAZ * wheel.rb.x) - (linearAX * wheel.rb.z);
            angularBZ = (linearAX * wheel.rb.y) - (linearAY * wheel.rb.x);

            //Compute inverse effective mass matrix
            Fix64 entryA, entryB;

            //these are the transformed coordinates
            Fix64 tX, tY, tZ;
            if (vehicleEntity.isDynamic)
            {
                tX = angularAX * vehicleEntity.inertiaTensorInverse.M11 + angularAY * vehicleEntity.inertiaTensorInverse.M21 + angularAZ * vehicleEntity.inertiaTensorInverse.M31;
                tY = angularAX * vehicleEntity.inertiaTensorInverse.M12 + angularAY * vehicleEntity.inertiaTensorInverse.M22 + angularAZ * vehicleEntity.inertiaTensorInverse.M32;
                tZ = angularAX * vehicleEntity.inertiaTensorInverse.M13 + angularAY * vehicleEntity.inertiaTensorInverse.M23 + angularAZ * vehicleEntity.inertiaTensorInverse.M33;
                entryA = tX * angularAX + tY * angularAY + tZ * angularAZ + vehicleEntity.inverseMass;
            }
            else
                entryA = F64.C0;

            if (supportIsDynamic)
            {
                tX = angularBX * supportEntity.inertiaTensorInverse.M11 + angularBY * supportEntity.inertiaTensorInverse.M21 + angularBZ * supportEntity.inertiaTensorInverse.M31;
                tY = angularBX * supportEntity.inertiaTensorInverse.M12 + angularBY * supportEntity.inertiaTensorInverse.M22 + angularBZ * supportEntity.inertiaTensorInverse.M32;
                tZ = angularBX * supportEntity.inertiaTensorInverse.M13 + angularBY * supportEntity.inertiaTensorInverse.M23 + angularBZ * supportEntity.inertiaTensorInverse.M33;
                entryB = tX * angularBX + tY * angularBY + tZ * angularBZ + supportEntity.inverseMass;
            }
            else
                entryB = F64.C0;

            //Convert spring constant and damping constant into ERP and CFM.
            Fix64 biasFactor;
            springSettings.ComputeErrorReductionAndSoftness(dt, F64.C1 / dt, out biasFactor, out softness);

            velocityToImpulse = -1 / (entryA + entryB + softness);

            //Correction velocity
            bias = MathHelper.Min(MathHelper.Max(F64.C0, (restLength - currentLength) - allowedCompression) * biasFactor, maximumSpringCorrectionSpeed);


        }

        internal void ExclusiveUpdate()
        {
            //Warm starting
#if !WINDOWS
            FPVector3 linear = new FPVector3();
            FPVector3 angular = new FPVector3();
#else
            Vector3 linear, angular;
#endif
            linear.x = accumulatedImpulse * linearAX;
            linear.y = accumulatedImpulse * linearAY;
            linear.z = accumulatedImpulse * linearAZ;
            if (vehicleEntity.isDynamic)
            {
                angular.x = accumulatedImpulse * angularAX;
                angular.y = accumulatedImpulse * angularAY;
                angular.z = accumulatedImpulse * angularAZ;
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }
            if (supportIsDynamic)
            {
                linear.x = -linear.x;
                linear.y = -linear.y;
                linear.z = -linear.z;
                angular.x = accumulatedImpulse * angularBX;
                angular.y = accumulatedImpulse * angularBY;
                angular.z = accumulatedImpulse * angularBZ;
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }
        }
    }
}