﻿using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;

using BEPUphysics.CollisionRuleManagement;
using FixedMath;
using BEPUphysics.Materials;
using System;
using FixMath.NET;

namespace BEPUphysics.Vehicle
{
    /// <summary>
    /// Uses a raycast as the shape of a wheel.
    /// </summary>
    public class RaycastWheelShape : WheelShape
    {
        private Fix64 graphicalRadius;

        /// <summary>
        /// Creates a new raycast based wheel shape.
        /// </summary>
        /// <param name="graphicalRadius">Graphical radius of the wheel.
        /// This is not used for simulation.  It is only used in
        /// determining aesthetic properties of a vehicle wheel,
        /// like position and orientation.</param>
        /// <param name="localGraphicTransform">Local graphic transform of the wheel shape.
        /// This transform is applied first when creating the shape's worldTransform.</param>
        public RaycastWheelShape(Fix64 graphicalRadius, FPMatrix localGraphicTransform)
        {
            Radius = graphicalRadius;
            LocalGraphicTransform = localGraphicTransform;
        }

        /// <summary>
        /// Gets or sets the graphical radius of the wheel.
        /// This is not used for simulation.  It is only used in
        /// determining aesthetic properties of a vehicle wheel,
        /// like position and orientation.
        /// </summary>
        public override sealed Fix64 Radius
        {
            get { return graphicalRadius; }
            set
            {
                graphicalRadius = MathHelper.Max(value, F64.C0);
                Initialize();
            }
        }

        /// <summary>
        /// Updates the wheel's world transform for graphics.
        /// Called automatically by the owning wheel at the end of each frame.
        /// If the engine is updating asynchronously, you can call this inside of a space read buffer lock
        /// and update the wheel transforms safely.
        /// </summary>
        public override void UpdateWorldTransform()
        {
#if !WINDOWS
            FPVector3 newPosition = new FPVector3();
#else
            Vector3 newPosition;
#endif
            FPVector3 worldAttachmentPoint;
            FPVector3 localAttach;
            FPVector3.Add(ref wheel.suspension.localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.localPosition, out localAttach);
            worldTransform = FPMatrix3x3.ToMatrix4X4(wheel.vehicle.Body.BufferedStates.InterpolatedStates.OrientationMatrix);

            FPMatrix.TransformNormal(ref localAttach, ref worldTransform, out worldAttachmentPoint);
            worldAttachmentPoint += wheel.vehicle.Body.BufferedStates.InterpolatedStates.Position;

            FPVector3 worldDirection;
            FPMatrix.Transform(ref wheel.suspension.localDirection, ref worldTransform, out worldDirection);

            Fix64 length = wheel.suspension.currentLength - graphicalRadius;
            newPosition.X = worldAttachmentPoint.X + worldDirection.X * length;
            newPosition.Y = worldAttachmentPoint.Y + worldDirection.Y * length;
            newPosition.Z = worldAttachmentPoint.Z + worldDirection.Z * length;

            FPMatrix spinTransform;

            FPVector3 localSpinAxis;
            FPVector3.Cross(ref wheel.localForwardDirection, ref wheel.suspension.localDirection, out localSpinAxis);
            FPMatrix.CreateFromAxisAngle(ref localSpinAxis, spinAngle, out spinTransform);


            FPMatrix localTurnTransform;
            FPMatrix.Multiply(ref localGraphicTransform, ref spinTransform, out localTurnTransform);
            FPMatrix.Multiply(ref localTurnTransform, ref steeringTransform, out localTurnTransform);
            //Matrix.Multiply(ref localTurnTransform, ref spinTransform, out localTurnTransform);
            FPMatrix.Multiply(ref localTurnTransform, ref worldTransform, out worldTransform);
            worldTransform.Translation += newPosition;
        }

        /// <summary>
        /// Finds a supporting entity, the contact location, and the contact normal.
        /// </summary>
        /// <param name="location">Contact point between the wheel and the support.</param>
        /// <param name="normal">Contact normal between the wheel and the support.</param>
        /// <param name="suspensionLength">Length of the suspension at the contact.</param>
        /// <param name="supportingCollidable">Collidable supporting the wheel, if any.</param>
        /// <param name="entity">Supporting object.</param>
        /// <param name="material">Material of the wheel.</param>
        /// <returns>Whether or not any support was found.</returns>
        protected internal override bool FindSupport(out FPVector3 location, out FPVector3 normal, out Fix64 suspensionLength, out Collidable supportingCollidable, out Entity entity, out Material material)
        {
            suspensionLength = Fix64.MaxValue;
            location = Toolbox.NoVector;
            supportingCollidable = null;
            entity = null;
            normal = Toolbox.NoVector;
            material = null;

            Collidable testCollidable;
            FPRayHit fpRayHit;

            bool hit = false;

            for (int i = 0; i < detector.CollisionInformation.pairs.Count; i++)
            {
                var pair = detector.CollisionInformation.pairs[i];
                testCollidable = (pair.BroadPhaseOverlap.entryA == detector.CollisionInformation ? pair.BroadPhaseOverlap.entryB : pair.BroadPhaseOverlap.entryA) as Collidable;
                if (testCollidable != null)
                {
                    if (CollisionRules.CollisionRuleCalculator(this, testCollidable) == CollisionRule.Normal &&
                        testCollidable.RayCast(new FPRay(wheel.suspension.worldAttachmentPoint, wheel.suspension.worldDirection), wheel.suspension.restLength, out fpRayHit) &&
                        fpRayHit.T < suspensionLength)
                    {
                        suspensionLength = fpRayHit.T;
                        EntityCollidable entityCollidable;
                        if ((entityCollidable = testCollidable as EntityCollidable) != null)
                        {
                            entity = entityCollidable.Entity;
                            material = entityCollidable.Entity.Material;
                        }
                        else
                        {
                            entity = null;
                            supportingCollidable = testCollidable;
                            var materialOwner = testCollidable as IMaterialOwner;
                            if (materialOwner != null)
                                material = materialOwner.Material;
                        }
                        location = fpRayHit.Location;
                        normal = fpRayHit.Normal;
                        hit = true;
                    }
                }
            }
            if (hit)
            {
                if (suspensionLength > F64.C0)
                    normal.Normalize();
                else
                    FPVector3.Negate(ref wheel.suspension.worldDirection, out normal);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Initializes the detector entity and any other necessary logic.
        /// </summary>
        protected internal override void Initialize()
        {
            //Setup the dimensions of the detector.
            FPVector3 startpoint = wheel.suspension.localAttachmentPoint;
            FPVector3 endpoint = startpoint + wheel.suspension.localDirection * wheel.suspension.restLength;
            FPVector3 min, max;
            FPVector3.Min(ref startpoint, ref endpoint, out min);
            FPVector3.Max(ref startpoint, ref endpoint, out max);

            detector.Width = max.X - min.X;
            detector.Height = max.Y - min.Y;
            detector.Length = max.Z - min.Z;
        }

        /// <summary>
        /// Updates the position of the detector before each step.
        /// </summary>
        protected internal override void UpdateDetectorPosition()
        {
#if !WINDOWS
            FPVector3 newPosition = new FPVector3();
#else
            Vector3 newPosition;
#endif

            newPosition.X = wheel.suspension.worldAttachmentPoint.X + wheel.suspension.worldDirection.X * wheel.suspension.restLength * F64.C0p5;
            newPosition.Y = wheel.suspension.worldAttachmentPoint.Y + wheel.suspension.worldDirection.Y * wheel.suspension.restLength * F64.C0p5;
            newPosition.Z = wheel.suspension.worldAttachmentPoint.Z + wheel.suspension.worldDirection.Z * wheel.suspension.restLength * F64.C0p5;

            detector.Position = newPosition;
            detector.OrientationMatrix = wheel.Vehicle.Body.orientationMatrix;
            FPVector3 linearVelocity;
            FPVector3.Subtract(ref newPosition, ref wheel.vehicle.Body.position, out linearVelocity);
            FPVector3.Cross(ref linearVelocity, ref wheel.vehicle.Body.angularVelocity, out linearVelocity);
            FPVector3.Add(ref linearVelocity, ref wheel.vehicle.Body.linearVelocity, out linearVelocity);
            detector.LinearVelocity = linearVelocity;
            detector.AngularVelocity = wheel.vehicle.Body.angularVelocity;
        }
    }
}