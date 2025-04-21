﻿using System;
using BEPUphysics.Entities;
using FixedMath;
 
using BEPUphysics.Settings;
using FixedMath.DataStructures;
using FixMath.NET;

namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces to slow down and stop sliding motion between two entities when centralized friction is active.
    /// </summary>
    public class SlidingFrictionTwoAxis : SolverUpdateable
    {
        private ConvexContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the contact manifold constraint that owns this constraint.
        ///</summary>
        public ConvexContactManifoldConstraint ContactManifoldConstraint
        {
            get
            {
                return contactManifoldConstraint;
            }
        }
        internal FPVector2 accumulatedImpulse;
        internal FPMatrix2x3 angularA, angularB;
        private int contactCount;
        private Fix64 friction;
        internal FPMatrix2x3 linearA;
        private Entity entityA, entityB;
        private bool entityADynamic, entityBDynamic;
        private FPVector3 ra, rb;
        private FPMatrix2x2 velocityToImpulse;


        /// <summary>
        /// Gets the first direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public FPVector3 FrictionDirectionX
        {
            get { return new FPVector3(linearA.M11, linearA.M12, linearA.M13); }
        }

        /// <summary>
        /// Gets the second direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public FPVector3 FrictionDirectionY
        {
            get { return new FPVector3(linearA.M21, linearA.M22, linearA.M23); }
        }

        /// <summary>
        /// Gets the total impulse applied by sliding friction in the last time step.
        /// The X component of this vector is the force applied along the frictionDirectionX,
        /// while the Y component is the force applied along the frictionDirectionY.
        /// </summary>
        public FPVector2 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the tangential relative velocity between the associated entities at the contact point.
        ///</summary>
        public FPVector2 RelativeVelocity
        {
            get
            {
                //Compute relative velocity
                //Explicit version:
                //Vector2 dot;
                //Matrix2x3.Transform(ref parentA.myInternalLinearVelocity, ref linearA, out lambda);
                //Matrix2x3.Transform(ref parentB.myInternalLinearVelocity, ref linearA, out dot);
                //lambda.X -= dot.X; lambda.Y -= dot.Y;
                //Matrix2x3.Transform(ref parentA.myInternalAngularVelocity, ref angularA, out dot);
                //lambda.X += dot.X; lambda.Y += dot.Y;
                //Matrix2x3.Transform(ref parentB.myInternalAngularVelocity, ref angularB, out dot);
                //lambda.X += dot.X; lambda.Y += dot.Y;

                //Inline version:
                //lambda.X = linearA.M11 * parentA.myInternalLinearVelocity.X + linearA.M12 * parentA.myInternalLinearVelocity.Y + linearA.M13 * parentA.myInternalLinearVelocity.Z -
                //           linearA.M11 * parentB.myInternalLinearVelocity.X - linearA.M12 * parentB.myInternalLinearVelocity.Y - linearA.M13 * parentB.myInternalLinearVelocity.Z +
                //           angularA.M11 * parentA.myInternalAngularVelocity.X + angularA.M12 * parentA.myInternalAngularVelocity.Y + angularA.M13 * parentA.myInternalAngularVelocity.Z +
                //           angularB.M11 * parentB.myInternalAngularVelocity.X + angularB.M12 * parentB.myInternalAngularVelocity.Y + angularB.M13 * parentB.myInternalAngularVelocity.Z;
                //lambda.Y = linearA.M21 * parentA.myInternalLinearVelocity.X + linearA.M22 * parentA.myInternalLinearVelocity.Y + linearA.M23 * parentA.myInternalLinearVelocity.Z -
                //           linearA.M21 * parentB.myInternalLinearVelocity.X - linearA.M22 * parentB.myInternalLinearVelocity.Y - linearA.M23 * parentB.myInternalLinearVelocity.Z +
                //           angularA.M21 * parentA.myInternalAngularVelocity.X + angularA.M22 * parentA.myInternalAngularVelocity.Y + angularA.M23 * parentA.myInternalAngularVelocity.Z +
                //           angularB.M21 * parentB.myInternalAngularVelocity.X + angularB.M22 * parentB.myInternalAngularVelocity.Y + angularB.M23 * parentB.myInternalAngularVelocity.Z;

                //Re-using information version:
                //TODO: va + wa x ra - vb - wb x rb, dotted against each axis, is it faster?
                Fix64 dvx = F64.C0, dvy = F64.C0, dvz = F64.C0;
                if (entityA != null)
                {
                    dvx = entityA.linearVelocity.x + (entityA.angularVelocity.y * ra.z) - (entityA.angularVelocity.z * ra.y);
                    dvy = entityA.linearVelocity.y + (entityA.angularVelocity.z * ra.x) - (entityA.angularVelocity.x * ra.z);
                    dvz = entityA.linearVelocity.z + (entityA.angularVelocity.x * ra.y) - (entityA.angularVelocity.y * ra.x);
                }
                if (entityB != null)
                {
                    dvx += -entityB.linearVelocity.x - (entityB.angularVelocity.y * rb.z) + (entityB.angularVelocity.z * rb.y);
                    dvy += -entityB.linearVelocity.y - (entityB.angularVelocity.z * rb.x) + (entityB.angularVelocity.x * rb.z);
                    dvz += -entityB.linearVelocity.z - (entityB.angularVelocity.x * rb.y) + (entityB.angularVelocity.y * rb.x);
                }

                //Fix64 dvx = entityA.linearVelocity.X + (entityA.angularVelocity.Y * ra.Z) - (entityA.angularVelocity.Z * ra.Y)
                //            - entityB.linearVelocity.X - (entityB.angularVelocity.Y * rb.Z) + (entityB.angularVelocity.Z * rb.Y);

                //Fix64 dvy = entityA.linearVelocity.Y + (entityA.angularVelocity.Z * ra.X) - (entityA.angularVelocity.X * ra.Z)
                //            - entityB.linearVelocity.Y - (entityB.angularVelocity.Z * rb.X) + (entityB.angularVelocity.X * rb.Z);

                //Fix64 dvz = entityA.linearVelocity.Z + (entityA.angularVelocity.X * ra.Y) - (entityA.angularVelocity.Y * ra.X)
                //            - entityB.linearVelocity.Z - (entityB.angularVelocity.X * rb.Y) + (entityB.angularVelocity.Y * rb.X);

#if !WINDOWS
                FPVector2 lambda = new FPVector2();
#else
                Vector2 lambda;
#endif
                lambda.x = dvx * linearA.M11 + dvy * linearA.M12 + dvz * linearA.M13;
                lambda.y = dvx * linearA.M21 + dvy * linearA.M22 + dvz * linearA.M23;
                return lambda;

                //Using XNA Cross product instead of inline
                //Vector3 wara, wbrb;
                //Vector3.Cross(ref parentA.myInternalAngularVelocity, ref Ra, out wara);
                //Vector3.Cross(ref parentB.myInternalAngularVelocity, ref Rb, out wbrb);

                //Fix64 dvx, dvy, dvz;
                //dvx = wara.X + parentA.myInternalLinearVelocity.X - wbrb.X - parentB.myInternalLinearVelocity.X;
                //dvy = wara.Y + parentA.myInternalLinearVelocity.Y - wbrb.Y - parentB.myInternalLinearVelocity.Y;
                //dvz = wara.Z + parentA.myInternalLinearVelocity.Z - wbrb.Z - parentB.myInternalLinearVelocity.Z;

                //lambda.X = dvx * linearA.M11 + dvy * linearA.M12 + dvz * linearA.M13;
                //lambda.Y = dvx * linearA.M21 + dvy * linearA.M22 + dvz * linearA.M23;
            }
        }


        ///<summary>
        /// Constructs a new sliding friction constraint.
        ///</summary>
        public SlidingFrictionTwoAxis()
        {
            isActive = false;
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override Fix64 SolveIteration()
        {

            FPVector2 lambda = RelativeVelocity;

            //Convert to impulse
            //Matrix2x2.Transform(ref lambda, ref velocityToImpulse, out lambda);
            Fix64 x = lambda.x;
            lambda.x = x * velocityToImpulse.M11 + lambda.y * velocityToImpulse.M21;
            lambda.y = x * velocityToImpulse.M12 + lambda.y * velocityToImpulse.M22;

            //Accumulate and clamp
            FPVector2 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse.x += lambda.x;
            accumulatedImpulse.y += lambda.y;
            Fix64 length = accumulatedImpulse.LengthSquared();
            Fix64 maximumFrictionForce = F64.C0;
            for (int i = 0; i < contactCount; i++)
            {
                maximumFrictionForce += contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
            }
            maximumFrictionForce *= friction;
            if (length > maximumFrictionForce * maximumFrictionForce)
            {
                length = maximumFrictionForce / Fix64.Sqrt(length);
                accumulatedImpulse.x *= length;
                accumulatedImpulse.y *= length;
            }
            lambda.x = accumulatedImpulse.x - previousAccumulatedImpulse.x;
            lambda.y = accumulatedImpulse.y - previousAccumulatedImpulse.y;
            //Single Axis clamp
            //Fix64 maximumFrictionForce = 0;
            //for (int i = 0; i < contactCount; i++)
            //{
            //    maximumFrictionForce += pair.contacts[i].penetrationConstraint.accumulatedImpulse;
            //}
            //maximumFrictionForce *= friction;
            //Fix64 previousAccumulatedImpulse = accumulatedImpulse.X;
            //accumulatedImpulse.X = MathHelper.Clamp(accumulatedImpulse.X + lambda.X, -maximumFrictionForce, maximumFrictionForce);
            //lambda.X = accumulatedImpulse.X - previousAccumulatedImpulse;
            //previousAccumulatedImpulse = accumulatedImpulse.Y;
            //accumulatedImpulse.Y = MathHelper.Clamp(accumulatedImpulse.Y + lambda.Y, -maximumFrictionForce, maximumFrictionForce);
            //lambda.Y = accumulatedImpulse.Y - previousAccumulatedImpulse;

            //Apply impulse
#if !WINDOWS
            FPVector3 linear = new FPVector3();
            FPVector3 angular = new FPVector3();
#else
            Vector3 linear, angular;
#endif
            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.x = lambda.x * linearA.M11 + lambda.y * linearA.M21;
            linear.y = lambda.x * linearA.M12 + lambda.y * linearA.M22;
            linear.z = lambda.x * linearA.M13 + lambda.y * linearA.M23;
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.x = lambda.x * angularA.M11 + lambda.y * angularA.M21;
                angular.y = lambda.x * angularA.M12 + lambda.y * angularA.M22;
                angular.z = lambda.x * angularA.M13 + lambda.y * angularA.M23;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.x = -linear.x;
                linear.y = -linear.y;
                linear.z = -linear.z;
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.x = lambda.x * angularB.M11 + lambda.y * angularB.M21;
                angular.y = lambda.x * angularB.M12 + lambda.y * angularB.M22;
                angular.z = lambda.x * angularB.M13 + lambda.y * angularB.M23;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }


            return Fix64.Abs(lambda.x) + Fix64.Abs(lambda.y);
        }

        internal FPVector3 manifoldCenter, relativeVelocity;

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(Fix64 dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            contactCount = contactManifoldConstraint.penetrationConstraints.Count;
            switch (contactCount)
            {
                case 1:
                    manifoldCenter = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position;
                    break;
                case 2:
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    manifoldCenter.x *= F64.C0p5;
                    manifoldCenter.y *= F64.C0p5;
                    manifoldCenter.z *= F64.C0p5;
                    break;
                case 3:
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.x *= F64.OneThird;
                    manifoldCenter.y *= F64.OneThird;
                    manifoldCenter.z *= F64.OneThird;
                    break;
                case 4:
                    //This isn't actually the center of the manifold.  Is it good enough?  Sure seems like it.
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    FPVector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[3].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.x *= F64.C0p25;
                    manifoldCenter.y *= F64.C0p25;
                    manifoldCenter.z *= F64.C0p25;
                    break;
                default:
                    manifoldCenter = Toolbox.NoVector;
                    break;
            }

            //Compute the three dimensional relative velocity at the point.


            FPVector3 velocityA, velocityB;
            if (entityA != null)
            {
                FPVector3.Subtract(ref manifoldCenter, ref entityA.position, out ra);
                FPVector3.Cross(ref entityA.angularVelocity, ref ra, out velocityA);
                FPVector3.Add(ref velocityA, ref entityA.linearVelocity, out velocityA);
            }
            else
                velocityA = new FPVector3();
            if (entityB != null)
            {
                FPVector3.Subtract(ref manifoldCenter, ref entityB.position, out rb);
                FPVector3.Cross(ref entityB.angularVelocity, ref rb, out velocityB);
                FPVector3.Add(ref velocityB, ref entityB.linearVelocity, out velocityB);
            }
            else
                velocityB = new FPVector3();
            FPVector3.Subtract(ref velocityA, ref velocityB, out relativeVelocity);

            //Get rid of the normal velocity.
            FPVector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
            Fix64 normalVelocityScalar = normal.x * relativeVelocity.x + normal.y * relativeVelocity.y + normal.z * relativeVelocity.z;
            relativeVelocity.x -= normalVelocityScalar * normal.x;
            relativeVelocity.y -= normalVelocityScalar * normal.y;
            relativeVelocity.z -= normalVelocityScalar * normal.z;

            //Create the jacobian entry and decide the friction coefficient.
            Fix64 length = relativeVelocity.LengthSquared();
            if (length > Toolbox.Epsilon)
            {
                length = Fix64.Sqrt(length);
                Fix64 inverseLength = F64.C1 / length;
                linearA.M11 = relativeVelocity.x * inverseLength;
                linearA.M12 = relativeVelocity.y * inverseLength;
                linearA.M13 = relativeVelocity.z * inverseLength;


                friction = length > CollisionResponseSettings.StaticFrictionVelocityThreshold ?
                           contactManifoldConstraint.materialInteraction.KineticFriction :
                           contactManifoldConstraint.materialInteraction.StaticFriction;
            }
            else
            {
                friction = contactManifoldConstraint.materialInteraction.StaticFriction;

                //If there was no velocity, try using the previous frame's jacobian... if it exists.
                //Reusing an old one is okay since jacobians are cleared when a contact is initialized.
                if (!(linearA.M11 != F64.C0 || linearA.M12 != F64.C0 || linearA.M13 != F64.C0))
                {
                    //Otherwise, just redo it all.
                    //Create arbitrary axes.
                    FPVector3 axis1;
                    FPVector3.Cross(ref normal, ref Toolbox.RightVector, out axis1);
                    length = axis1.LengthSquared();
                    if (length > Toolbox.Epsilon)
                    {
                        length = Fix64.Sqrt(length);
                        Fix64 inverseLength = F64.C1 / length;
                        linearA.M11 = axis1.x * inverseLength;
                        linearA.M12 = axis1.y * inverseLength;
                        linearA.M13 = axis1.z * inverseLength;
                    }
                    else
                    {
                        FPVector3.Cross(ref normal, ref Toolbox.UpVector, out axis1);
                        axis1.Normalize();
                        linearA.M11 = axis1.x;
                        linearA.M12 = axis1.y;
                        linearA.M13 = axis1.z;
                    }
                }
            }

            //Second axis is first axis x normal
            linearA.M21 = (linearA.M12 * normal.z) - (linearA.M13 * normal.y);
            linearA.M22 = (linearA.M13 * normal.x) - (linearA.M11 * normal.z);
            linearA.M23 = (linearA.M11 * normal.y) - (linearA.M12 * normal.x);


            //Compute angular jacobians
            if (entityA != null)
            {
                //angularA 1 =  ra x linear axis 1
                angularA.M11 = (ra.y * linearA.M13) - (ra.z * linearA.M12);
                angularA.M12 = (ra.z * linearA.M11) - (ra.x * linearA.M13);
                angularA.M13 = (ra.x * linearA.M12) - (ra.y * linearA.M11);

                //angularA 2 =  ra x linear axis 2
                angularA.M21 = (ra.y * linearA.M23) - (ra.z * linearA.M22);
                angularA.M22 = (ra.z * linearA.M21) - (ra.x * linearA.M23);
                angularA.M23 = (ra.x * linearA.M22) - (ra.y * linearA.M21);
            }

            //angularB 1 =  linear axis 1 x rb
            if (entityB != null)
            {
                angularB.M11 = (linearA.M12 * rb.z) - (linearA.M13 * rb.y);
                angularB.M12 = (linearA.M13 * rb.x) - (linearA.M11 * rb.z);
                angularB.M13 = (linearA.M11 * rb.y) - (linearA.M12 * rb.x);

                //angularB 2 =  linear axis 2 x rb
                angularB.M21 = (linearA.M22 * rb.z) - (linearA.M23 * rb.y);
                angularB.M22 = (linearA.M23 * rb.x) - (linearA.M21 * rb.z);
                angularB.M23 = (linearA.M21 * rb.y) - (linearA.M22 * rb.x);
            }
            //Compute inverse effective mass matrix
            FPMatrix2x2 entryA, entryB;

            //these are the transformed coordinates
            FPMatrix2x3 transform;
            FPMatrix3x2 transpose;
            if (entityADynamic)
            {
                FPMatrix2x3.Multiply(ref angularA, ref entityA.inertiaTensorInverse, out transform);
                FPMatrix2x3.Transpose(ref angularA, out transpose);
                FPMatrix2x2.Multiply(ref transform, ref transpose, out entryA);
                entryA.M11 += entityA.inverseMass;
                entryA.M22 += entityA.inverseMass;
            }
            else
            {
                entryA = new FPMatrix2x2();
            }

            if (entityBDynamic)
            {
                FPMatrix2x3.Multiply(ref angularB, ref entityB.inertiaTensorInverse, out transform);
                FPMatrix2x3.Transpose(ref angularB, out transpose);
                FPMatrix2x2.Multiply(ref transform, ref transpose, out entryB);
                entryB.M11 += entityB.inverseMass;
                entryB.M22 += entityB.inverseMass;
            }
            else
            {
                entryB = new FPMatrix2x2();
            }

            velocityToImpulse.M11 = -entryA.M11 - entryB.M11;
            velocityToImpulse.M12 = -entryA.M12 - entryB.M12;
            velocityToImpulse.M21 = -entryA.M21 - entryB.M21;
            velocityToImpulse.M22 = -entryA.M22 - entryB.M22;
            FPMatrix2x2.Invert(ref velocityToImpulse, out velocityToImpulse);


        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {

            //Warm starting
#if !WINDOWS
            FPVector3 linear = new FPVector3();
            FPVector3 angular = new FPVector3();
#else
            Vector3 linear, angular;
#endif
            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.x = accumulatedImpulse.x * linearA.M11 + accumulatedImpulse.y * linearA.M21;
            linear.y = accumulatedImpulse.x * linearA.M12 + accumulatedImpulse.y * linearA.M22;
            linear.z = accumulatedImpulse.x * linearA.M13 + accumulatedImpulse.y * linearA.M23;
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.x = accumulatedImpulse.x * angularA.M11 + accumulatedImpulse.y * angularA.M21;
                angular.y = accumulatedImpulse.x * angularA.M12 + accumulatedImpulse.y * angularA.M22;
                angular.z = accumulatedImpulse.x * angularA.M13 + accumulatedImpulse.y * angularA.M23;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.x = -linear.x;
                linear.y = -linear.y;
                linear.z = -linear.z;
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.x = accumulatedImpulse.x * angularB.M11 + accumulatedImpulse.y * angularB.M21;
                angular.y = accumulatedImpulse.x * angularB.M12 + accumulatedImpulse.y * angularB.M22;
                angular.z = accumulatedImpulse.x * angularB.M13 + accumulatedImpulse.y * angularB.M23;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }
        }

        internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            isActive = true;

            linearA = new FPMatrix2x3();

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        internal void CleanUp()
        {
            accumulatedImpulse = new FPVector2();
            contactManifoldConstraint = null;
            entityA = null;
            entityB = null;
            isActive = false;
        }

        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            //This should never really have to be called.
            if (entityA != null)
                outputInvolvedEntities.Add(entityA);
            if (entityB != null)
                outputInvolvedEntities.Add(entityB);
        }
    }
}