﻿using System;
using System.Diagnostics;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using FixedMath;
using FixedMath.DataStructures;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using FixMath.NET;

namespace BEPUphysics.Character
{
    /// <summary>
    /// Manages the horizontal movement of a character.
    /// </summary>
    public class HorizontalMotionConstraint : SolverUpdateable
    {
        Entity characterBody;
        private SupportFinder supportFinder;


        SupportData supportData;

        FPVector2 movementDirection;
        /// <summary>
        /// Gets or sets the goal movement direction.
        /// The movement direction is based on the view direction.
        /// Values of X are applied to the axis perpendicular to the HorizontalViewDirection and Down direction.
        /// Values of Y are applied to the HorizontalViewDirection.
        /// </summary>
        public FPVector2 MovementDirection
        {
            get { return movementDirection; }
            set
            {
                if (movementDirection.x != value.x || movementDirection.y != value.y) //Fix64ing point comparison is perfectly fine here. Any bitwise variation should go through.
                {
                    characterBody.ActivityInformation.Activate();

                    Fix64 lengthSquared = value.LengthSquared();
                    if (lengthSquared > Toolbox.Epsilon)
                    {
                        FPVector2.Divide(ref value, Fix64.Sqrt(lengthSquared), out movementDirection);
                    }
                    else
                    {
                        movementDirection = new FPVector2();
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the target speed of the character in its current state.
        /// </summary>
        public Fix64 TargetSpeed { get; set; }
        /// <summary>
        /// Gets or sets the maximum force the character can apply to move horizontally in its current state.
        /// </summary>
        public Fix64 MaximumForce { get; set; }
        /// <summary>
        /// Gets or sets the maximum force the character can apply to accelerate. 
        /// This will not let the character apply more force than the MaximumForce; the actual applied force is constrained by both this and the MaximumForce property.
        /// </summary>
        public Fix64 MaximumAccelerationForce { get; set; }
        Fix64 maxForceDt;
        Fix64 maxAccelerationForceDt;

        private Fix64 timeUntilPositionAnchor = (Fix64).2m;

        /// <summary>
        /// <para>Gets or sets the time it takes for the character to achieve stable footing after trying to stop moving.
        /// When a character has stable footing, it will resist position drift relative to its support. For example,
        /// if the player was on a rotating platform, integrating the velocity repeatedly would otherwise make the character gradually shift
        /// relative to the support.</para>
        /// <para>This time should be longer than the time it takes the player to decelerate from normal movement while it has traction. Otherwise, the character 
        /// will seem to 'rubber band' back to a previous location after the character tries to stop.</para>
        /// </summary>
        public Fix64 TimeUntilPositionAnchor
        {
            get { return timeUntilPositionAnchor; }
            set { timeUntilPositionAnchor = value; }
        }

        /// <summary>
        /// Gets or sets the distance beyond which the character will reset its goal position.
        /// When a character is standing still (as defined by TimeUntilStableFooting), a shove smaller than this threshold will result in an attempt to return to the previous anchor.
        /// A shove which pushes the character more than this threshold will cause a new anchor to be created.
        /// </summary>
        public Fix64 PositionAnchorDistanceThreshold { get; set; }

        /// <summary>
        /// <para>Gets whether the character currently has stable footing. If true, the character will resist position drift relative to its support. For example,
        /// if the character was on a rotating platform while trying to stand still relative to the platform, integrating the velocity repeatedly would make the character gradually shift
        /// relative to the platform. The anchoring effect of stable footing keeps the character near the same relative location.</para>
        /// <para>Can only occur when the character has traction and is not trying to move while standing on an entity.</para>
        /// </summary>
        public bool HasPositionAnchor
        {
            get { return timeSinceTransition < F64.C0; }
        }

        /// <summary>
        /// Forces a recomputation of the position anchor during the next update if a position anchor is currently active.
        /// The new position anchor will be dropped at the character's location as of the next update.
        /// </summary>
        public void ResetPositionAnchor()
        {
            if (HasPositionAnchor)
                timeSinceTransition = timeUntilPositionAnchor;
        }


        /// <summary>
        /// Gets or sets the current movement style used by the character.
        /// </summary>
        public MovementMode MovementMode { get; set; }


        public FPVector3 movementDirection3d;

        /// <summary>
        /// Gets the 3d movement direction, as updated in the previous call to UpdateMovementBasis.
        /// Note that this will not change when MovementDirection is set. It only changes on a call to UpdateMovementBasis.
        /// So, getting this value externally will get the previous frame's snapshot.
        /// </summary>
        public FPVector3 MovementDirection3d
        {
            get { return movementDirection3d; }
        }

        FPVector3 strafeDirection;
        /// <summary>
        /// Gets the strafe direction as updated in the previous call to UpdateMovementBasis.
        /// </summary>
        public FPVector3 StrafeDirection
        {
            get
            {
                return strafeDirection;
            }
        }

        FPVector3 horizontalForwardDirection;
        /// <summary>
        /// Gets the horizontal forward direction as updated in the previous call to UpdateMovementBasis.
        /// </summary>
        public FPVector3 ForwardDirection
        {
            get
            {
                return horizontalForwardDirection;
            }
        }

        /// <summary>
        /// Updates the movement basis of the horizontal motion constraint.
        /// Should be updated automatically by the character on each time step; other code should not need to call this.
        /// </summary>
        /// <param name="forward">Forward facing direction of the character.</param>
        public void UpdateMovementBasis(ref FPVector3 forward)
        {
            FPVector3 down = characterBody.orientationMatrix.Down;
            horizontalForwardDirection = forward - down * FPVector3.Dot(down, forward);
            Fix64 forwardLengthSquared = horizontalForwardDirection.LengthSquared();

            if (forwardLengthSquared < Toolbox.Epsilon)
            {
                //Use an arbitrary direction to complete the basis.
                horizontalForwardDirection = characterBody.orientationMatrix.Forward;
                strafeDirection = characterBody.orientationMatrix.Right;
            }
            else
            {
                FPVector3.Divide(ref horizontalForwardDirection, Fix64.Sqrt(forwardLengthSquared), out horizontalForwardDirection);
                FPVector3.Cross(ref down, ref horizontalForwardDirection, out strafeDirection);
                //Don't need to normalize the strafe direction; it's the cross product of two normalized perpendicular vectors.
            }


            FPVector3.Multiply(ref horizontalForwardDirection, movementDirection.y, out movementDirection3d);
            FPVector3 strafeComponent;
            FPVector3.Multiply(ref strafeDirection, movementDirection.x, out strafeComponent);
            FPVector3.Add(ref strafeComponent, ref movementDirection3d, out movementDirection3d);

        }

        /// <summary>
        /// Updates the constraint's view of the character's support data.
        /// </summary>
        public void UpdateSupportData()
        {
            //Check if the support has changed, and perform the necessary bookkeeping to keep the connections up to date.
            var oldSupport = supportData.SupportObject;
            supportData = supportFinder.SupportData;
            if (oldSupport != supportData.SupportObject)
            {
                OnInvolvedEntitiesChanged();
                var supportEntityCollidable = supportData.SupportObject as EntityCollidable;
                if (supportEntityCollidable != null)
                {
                    supportEntity = supportEntityCollidable.Entity;
                }
                else
                {
                    //We aren't on an entity, so clear out the support entity.
                    supportEntity = null;
                }
            }
        }

        Fix64 supportForceFactor = F64.C1;
        /// <summary>
        /// Gets or sets the scaling factor of forces applied to the supporting object if it is a dynamic entity.
        /// Low values (below 1) reduce the amount of motion imparted to the support object; it acts 'heavier' as far as horizontal motion is concerned.
        /// High values (above 1) increase the force applied to support objects, making them appear lighter.
        /// Be careful when changing this- it can create impossible situations!
        /// </summary>
        public Fix64 SupportForceFactor
        {
            get
            {
                return supportForceFactor;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                supportForceFactor = value;
            }
        }





        FPMatrix2x2 massMatrix;
        Entity supportEntity;
        FPVector3 linearJacobianA1;
        FPVector3 linearJacobianA2;
        FPVector3 linearJacobianB1;
        FPVector3 linearJacobianB2;
        FPVector3 angularJacobianB1;
        FPVector3 angularJacobianB2;

        FPVector2 accumulatedImpulse;
        FPVector2 targetVelocity;

        FPVector2 positionCorrectionBias;

        FPVector3 positionLocalOffset;
        bool wasTryingToMove;
        bool hadTraction;
        Entity previousSupportEntity;
        Fix64 timeSinceTransition;
        bool isTryingToMove;

        /// <summary>
        /// Constructs a new horizontal motion constraint.
        /// </summary>
        /// <param name="characterBody">Character body to be governed by this constraint.</param>
        /// <param name="supportFinder">Helper used to find supports for the character.</param>
        public HorizontalMotionConstraint(Entity characterBody, SupportFinder supportFinder)
        {
            this.characterBody = characterBody;
            this.supportFinder = supportFinder;
            CollectInvolvedEntities();
            MaximumAccelerationForce = Fix64.MaxValue;
        }



        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            var entityCollidable = supportData.SupportObject as EntityCollidable;
            if (entityCollidable != null)
                outputInvolvedEntities.Add(entityCollidable.Entity);
            outputInvolvedEntities.Add(characterBody);

        }


        /// <summary>
        /// Computes per-frame information necessary for the constraint.
        /// </summary>
        /// <param name="dt">Time step duration.</param>
        public override void Update(Fix64 dt)
        {

            isTryingToMove = movementDirection3d.LengthSquared() > F64.C0;

            maxForceDt = MaximumForce * dt;
            maxAccelerationForceDt = MaximumAccelerationForce * dt;


            //Compute the jacobians.  This is basically a PointOnLineJoint with motorized degrees of freedom.
            FPVector3 downDirection = characterBody.orientationMatrix.Down;

            if (MovementMode != MovementMode.Floating)
            {
                //Compute the linear jacobians first.
                if (isTryingToMove)
                {
                    FPVector3 velocityDirection;
                    FPVector3 offVelocityDirection;
                    //Project the movement direction onto the support plane defined by the support normal.
                    //This projection is NOT along the support normal to the plane; that would cause the character to veer off course when moving on slopes.
                    //Instead, project along the sweep direction to the plane.
                    //For a 6DOF character controller, the lineStart would be different; it must be perpendicular to the local up.
                    FPVector3 lineStart = movementDirection3d;

                    FPVector3 lineEnd;
                    FPVector3.Add(ref lineStart, ref downDirection, out lineEnd);
                    FPPlane fpPlane = new FPPlane(supportData.Normal, F64.C0);
                    Fix64 t;
                    //This method can return false when the line is parallel to the plane, but previous tests and the slope limit guarantee that it won't happen.
                    Toolbox.GetLinePlaneIntersection(ref lineStart, ref lineEnd, ref fpPlane, out t, out velocityDirection);

                    //The origin->intersection line direction defines the horizontal velocity direction in 3d space.
                    velocityDirection.Normalize();


                    //The normal and velocity direction are perpendicular and normal, so the off velocity direction doesn't need to be normalized.
                    FPVector3.Cross(ref velocityDirection, ref supportData.Normal, out offVelocityDirection);

                    linearJacobianA1 = velocityDirection;
                    linearJacobianA2 = offVelocityDirection;
                    linearJacobianB1 = -velocityDirection;
                    linearJacobianB2 = -offVelocityDirection;

                }
                else
                {
                    //If the character isn't trying to move, then the velocity directions are not well defined.
                    //Instead, pick two arbitrary vectors on the support plane.
                    //First guess will be based on the previous jacobian.
                    //Project the old linear jacobian onto the support normal plane.
                    Fix64 dot;
                    FPVector3.Dot(ref linearJacobianA1, ref supportData.Normal, out dot);
                    FPVector3 toRemove;
                    FPVector3.Multiply(ref supportData.Normal, dot, out toRemove);
                    FPVector3.Subtract(ref linearJacobianA1, ref toRemove, out linearJacobianA1);

                    //Vector3.Cross(ref linearJacobianA2, ref supportData.Normal, out linearJacobianA1);
                    Fix64 length = linearJacobianA1.LengthSquared();
                    if (length < Toolbox.Epsilon)
                    {
                        //First guess failed.  Try the right vector.
                        FPVector3.Cross(ref Toolbox.RightVector, ref supportData.Normal, out linearJacobianA1);
                        length = linearJacobianA1.LengthSquared();
                        if (length < Toolbox.Epsilon)
                        {
                            //Okay that failed too! try the forward vector.
                            FPVector3.Cross(ref Toolbox.ForwardVector, ref supportData.Normal, out linearJacobianA1);
                            length = linearJacobianA1.LengthSquared();
                            //Unless something really weird is happening, we do not need to test any more axes.
                        }

                    }
                    FPVector3.Divide(ref linearJacobianA1, Fix64.Sqrt(length), out linearJacobianA1);
                    //Pick another perpendicular vector.  Don't need to normalize it since the normal and A1 are already normalized and perpendicular.
                    FPVector3.Cross(ref linearJacobianA1, ref supportData.Normal, out linearJacobianA2);

                    //B's linear jacobians are just -A's.
                    linearJacobianB1 = -linearJacobianA1;
                    linearJacobianB2 = -linearJacobianA2;

                }

                if (supportEntity != null)
                {
                    //Compute the angular jacobians.
                    FPVector3 supportToContact = supportData.Position - supportEntity.Position;
                    //Since we treat the character to have infinite inertia, we're only concerned with the support's angular jacobians.
                    //Note the order of the cross product- it is reversed to negate the result.
                    FPVector3.Cross(ref linearJacobianA1, ref supportToContact, out angularJacobianB1);
                    FPVector3.Cross(ref linearJacobianA2, ref supportToContact, out angularJacobianB2);

                }
                else
                {
                    //If we're not standing on an entity, there are no angular jacobians.
                    angularJacobianB1 = new FPVector3();
                    angularJacobianB2 = new FPVector3();
                }
            }
            else
            {
                //If the character is Fix64ing, then the jacobians are simply the 3d movement direction and the perpendicular direction on the character's horizontal plane.
                linearJacobianA1 = movementDirection3d;
                linearJacobianA2 = FPVector3.Cross(linearJacobianA1, characterBody.orientationMatrix.Down);


            }


            //Compute the target velocity (in constraint space) for this frame.  The hard work has already been done.
            targetVelocity.x = isTryingToMove ? TargetSpeed : F64.C0;
            targetVelocity.y = F64.C0;

            //Compute the effective mass matrix.
            if (supportEntity != null && supportEntity.IsDynamic)
            {
                Fix64 m11, m22, m1221 = F64.C0;
                Fix64 inverseMass;
                FPVector3 intermediate;

                inverseMass = characterBody.InverseMass;
                m11 = inverseMass;
                m22 = inverseMass;


                //Scale the inertia and mass of the support.  This will make the solver view the object as 'heavier' with respect to horizontal motion.
                FPMatrix3x3 inertiaInverse = supportEntity.InertiaTensorInverse;
                FPMatrix3x3.Multiply(ref inertiaInverse, supportForceFactor, out inertiaInverse);
                Fix64 extra;
                inverseMass = supportForceFactor * supportEntity.InverseMass;
                FPMatrix3x3.Transform(ref angularJacobianB1, ref inertiaInverse, out intermediate);
                FPVector3.Dot(ref intermediate, ref angularJacobianB1, out extra);
                m11 += inverseMass + extra;
                FPVector3.Dot(ref intermediate, ref angularJacobianB2, out extra);
                m1221 += extra;
                FPMatrix3x3.Transform(ref angularJacobianB2, ref inertiaInverse, out intermediate);
                FPVector3.Dot(ref intermediate, ref angularJacobianB2, out extra);
                m22 += inverseMass + extra;


                massMatrix.M11 = m11;
                massMatrix.M12 = m1221;
                massMatrix.M21 = m1221;
                massMatrix.M22 = m22;
                FPMatrix2x2.Invert(ref massMatrix, out massMatrix);


            }
            else
            {
                //If we're not standing on a dynamic entity, then the mass matrix is defined entirely by the character.
                FPMatrix2x2.CreateScale(characterBody.Mass, out massMatrix);
            }

            //If we're trying to stand still on an object that's moving, use a position correction term to keep the character
            //from drifting due to accelerations. 
            //First thing to do is to check to see if we're moving into a traction/trying to stand still state from a 
            //non-traction || trying to move state.  Either that, or we've switched supports and need to update the offset.
            if (supportEntity != null && ((wasTryingToMove && !isTryingToMove) || (!hadTraction && supportFinder.HasTraction) || supportEntity != previousSupportEntity))
            {
                //We're transitioning into a new 'use position correction' state.
                //Force a recomputation of the local offset.
                //The time since transition is used as a flag.
                timeSinceTransition = F64.C0;
            }

            //The state is now up to date.  Compute an error and velocity bias, if needed.
            if (!isTryingToMove && MovementMode == MovementMode.Traction && supportEntity != null)
            {

                var distanceToBottomOfCharacter = supportFinder.BottomDistance;

                if (timeSinceTransition >= F64.C0 && timeSinceTransition < timeUntilPositionAnchor)
                    timeSinceTransition += dt;
                if (timeSinceTransition >= timeUntilPositionAnchor)
                {
                    FPVector3.Multiply(ref downDirection, distanceToBottomOfCharacter, out positionLocalOffset);
                    positionLocalOffset = (positionLocalOffset + characterBody.Position) - supportEntity.Position;
                    positionLocalOffset = FPMatrix3x3.TransformTranspose(positionLocalOffset, supportEntity.OrientationMatrix);
                    timeSinceTransition = -1; //Negative 1 means that the offset has been computed.
                }
                if (timeSinceTransition < F64.C0)
                {
                    FPVector3 targetPosition;
                    FPVector3.Multiply(ref downDirection, distanceToBottomOfCharacter, out targetPosition);
                    targetPosition += characterBody.Position;
                    FPVector3 worldSupportLocation = FPMatrix3x3.Transform(positionLocalOffset, supportEntity.OrientationMatrix) + supportEntity.Position;
                    FPVector3 error;
                    FPVector3.Subtract(ref targetPosition, ref worldSupportLocation, out error);
                    //If the error is too large, then recompute the offset.  We don't want the character rubber banding around.
                    if (error.LengthSquared() > PositionAnchorDistanceThreshold * PositionAnchorDistanceThreshold)
                    {
                        FPVector3.Multiply(ref downDirection, distanceToBottomOfCharacter, out positionLocalOffset);
                        positionLocalOffset = (positionLocalOffset + characterBody.Position) - supportEntity.Position;
                        positionLocalOffset = FPMatrix3x3.TransformTranspose(positionLocalOffset, supportEntity.OrientationMatrix);
                        positionCorrectionBias = new FPVector2();
                    }
                    else
                    {
                        //The error in world space is now available.  We can't use this error to directly create a velocity bias, though.
                        //It needs to be transformed into constraint space where the constraint operates.
                        //Use the jacobians!
                        FPVector3.Dot(ref error, ref linearJacobianA1, out positionCorrectionBias.x);
                        FPVector3.Dot(ref error, ref linearJacobianA2, out positionCorrectionBias.y);
                        //Scale the error so that a portion of the error is resolved each frame.
                        FPVector2.Multiply(ref positionCorrectionBias, F64.C0p2 / dt, out positionCorrectionBias);
                    }
                }
            }
            else
            {
                timeSinceTransition = F64.C0;
                positionCorrectionBias = new FPVector2();
            }

            wasTryingToMove = isTryingToMove;
            hadTraction = supportFinder.HasTraction;
            previousSupportEntity = supportEntity;

        }


        /// <summary>
        /// Performs any per-frame initialization needed by the constraint that must be done with exclusive access
        /// to the connected objects.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Warm start the constraint using the previous impulses and the new jacobians!
#if !WINDOWS
            FPVector3 impulse = new FPVector3();
            FPVector3 torque= new FPVector3();
#else
            Vector3 impulse;
            Vector3 torque;
#endif
            Fix64 x = accumulatedImpulse.x;
            Fix64 y = accumulatedImpulse.y;
            impulse.x = linearJacobianA1.x * x + linearJacobianA2.x * y;
            impulse.y = linearJacobianA1.y * x + linearJacobianA2.y * y;
            impulse.z = linearJacobianA1.z * x + linearJacobianA2.z * y;

            characterBody.ApplyLinearImpulse(ref impulse);

            if (supportEntity != null && supportEntity.IsDynamic)
            {
                FPVector3.Multiply(ref impulse, -supportForceFactor, out impulse);

                x *= supportForceFactor;
                y *= supportForceFactor;
                torque.x = x * angularJacobianB1.x + y * angularJacobianB2.x;
                torque.y = x * angularJacobianB1.y + y * angularJacobianB2.y;
                torque.z = x * angularJacobianB1.z + y * angularJacobianB2.z;


                supportEntity.ApplyLinearImpulse(ref impulse);
                supportEntity.ApplyAngularImpulse(ref torque);
            }
        }

        /// <summary>
        /// Computes a solution to the constraint.
        /// </summary>
        /// <returns>Impulse magnitude computed by the iteration.</returns>
        public override Fix64 SolveIteration()
        {

            FPVector2 relativeVelocity = RelativeVelocity;

            FPVector2.Add(ref relativeVelocity, ref positionCorrectionBias, out relativeVelocity);


            //Create the full velocity change, and convert it to an impulse in constraint space.
            FPVector2 lambda;
            FPVector2.Subtract(ref targetVelocity, ref relativeVelocity, out lambda);
            FPMatrix2x2.Transform(ref lambda, ref massMatrix, out lambda);

            //Add and clamp the impulse.

            FPVector2 previousAccumulatedImpulse = accumulatedImpulse;
            if (MovementMode == MovementMode.Floating)
            {
                //If it's Fix64ing, clamping rules are different.
                //The constraint is not permitted to slow down the character; only speed it up.
                //This offers a hole for an exploit; by jumping and curving just right,
                //the character can accelerate beyond its maximum speed.  A bit like an HL2 speed run.
                accumulatedImpulse.x = MathHelper.Clamp(accumulatedImpulse.x + lambda.x, F64.C0, maxForceDt);
                accumulatedImpulse.y = F64.C0;
            }
            else
            {

                FPVector2.Add(ref lambda, ref accumulatedImpulse, out accumulatedImpulse);
                Fix64 length = accumulatedImpulse.LengthSquared();
                if (length > maxForceDt * maxForceDt)
                {
                    FPVector2.Multiply(ref accumulatedImpulse, maxForceDt / Fix64.Sqrt(length), out accumulatedImpulse);
                }
                if (isTryingToMove && accumulatedImpulse.x > maxAccelerationForceDt)
                {
                    accumulatedImpulse.x = maxAccelerationForceDt;
                }
            }
            FPVector2.Subtract(ref accumulatedImpulse, ref previousAccumulatedImpulse, out lambda);


            //Use the jacobians to put the impulse into world space.

#if !WINDOWS
            FPVector3 impulse = new FPVector3();
            FPVector3 torque= new FPVector3();
#else
            Vector3 impulse;
            Vector3 torque;
#endif
            Fix64 x = lambda.x;
            Fix64 y = lambda.y;
            impulse.x = linearJacobianA1.x * x + linearJacobianA2.x * y;
            impulse.y = linearJacobianA1.y * x + linearJacobianA2.y * y;
            impulse.z = linearJacobianA1.z * x + linearJacobianA2.z * y;

            characterBody.ApplyLinearImpulse(ref impulse);

            if (supportEntity != null && supportEntity.IsDynamic)
            {
                FPVector3.Multiply(ref impulse, -supportForceFactor, out impulse);

                x *= supportForceFactor;
                y *= supportForceFactor;
                torque.x = x * angularJacobianB1.x + y * angularJacobianB2.x;
                torque.y = x * angularJacobianB1.y + y * angularJacobianB2.y;
                torque.z = x * angularJacobianB1.z + y * angularJacobianB2.z;

                supportEntity.ApplyLinearImpulse(ref impulse);
                supportEntity.ApplyAngularImpulse(ref torque);
            }

            return (Fix64.Abs(lambda.x) + Fix64.Abs(lambda.y));


        }


        /// <summary>
        /// Gets the current velocity between the character and its support in constraint space.
        /// The X component corresponds to velocity along the movement direction.
        /// The Y component corresponds to velocity perpendicular to the movement direction and support normal.
        /// </summary>
        public FPVector2 RelativeVelocity
        {
            get
            {
                //The relative velocity's x component is in the movement direction.
                //y is the perpendicular direction.
#if !WINDOWS
                FPVector2 relativeVelocity = new FPVector2();
#else
                Vector2 relativeVelocity;
#endif

                FPVector3.Dot(ref linearJacobianA1, ref characterBody.linearVelocity, out relativeVelocity.x);
                FPVector3.Dot(ref linearJacobianA2, ref characterBody.linearVelocity, out relativeVelocity.y);

                Fix64 x, y;
                if (supportEntity != null)
                {
                    FPVector3.Dot(ref linearJacobianB1, ref supportEntity.linearVelocity, out x);
                    FPVector3.Dot(ref linearJacobianB2, ref supportEntity.linearVelocity, out y);
                    relativeVelocity.x += x;
                    relativeVelocity.y += y;
                    FPVector3.Dot(ref angularJacobianB1, ref supportEntity.angularVelocity, out x);
                    FPVector3.Dot(ref angularJacobianB2, ref supportEntity.angularVelocity, out y);
                    relativeVelocity.x += x;
                    relativeVelocity.y += y;

                }
                return relativeVelocity;
            }
        }

        /// <summary>
        /// Gets the current velocity between the character and its support.
        /// </summary>
        public FPVector3 RelativeWorldVelocity
        {
            get
            {
                FPVector3 bodyVelocity = characterBody.LinearVelocity;
                if (supportEntity != null)
                    return bodyVelocity - Toolbox.GetVelocityOfPoint(supportData.Position, supportEntity.Position, supportEntity.LinearVelocity, supportEntity.AngularVelocity);
                return bodyVelocity;
            }
        }

        /// <summary>
        /// Gets the velocity of the support at the support point.
        /// </summary>
        public FPVector3 SupportVelocity
        {
            get
            {
                return supportEntity == null ? new FPVector3() : Toolbox.GetVelocityOfPoint(supportData.Position, supportEntity.Position, supportEntity.LinearVelocity, supportEntity.AngularVelocity);
            }
        }


        /// <summary>
        /// Gets the accumulated impulse in world space applied to the character.
        /// </summary>
        public FPVector3 CharacterAccumulatedImpulse
        {
            get
            {

                FPVector3 impulse;
                impulse.x = accumulatedImpulse.x * linearJacobianA1.x + accumulatedImpulse.y * linearJacobianA2.x;
                impulse.y = accumulatedImpulse.x * linearJacobianA1.y + accumulatedImpulse.y * linearJacobianA2.y;
                impulse.z = accumulatedImpulse.x * linearJacobianA1.z + accumulatedImpulse.y * linearJacobianA2.z;
                return impulse;
            }
        }

        /// <summary>
        /// Gets the accumulated impulse in constraint space.
        /// The X component corresponds to impulse along the movement direction.
        /// The Y component corresponds to impulse perpendicular to the movement direction and support normal.
        /// </summary>
        public FPVector2 AccumulatedImpulse
        {
            get
            {
                return accumulatedImpulse;
            }
        }

    }


}
