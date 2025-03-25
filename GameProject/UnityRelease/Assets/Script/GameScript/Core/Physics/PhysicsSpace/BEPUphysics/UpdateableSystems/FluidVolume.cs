using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities;
using FixedMath;
using FixedMath.ResourceManagement;
using FixedMath.Threading;
using FixMath.NET;

namespace BEPUphysics.UpdateableSystems
{

    /// <summary>
    /// Volume in which physically simulated objects have a buoyancy force applied to them based on their density and volume.
    /// </summary>
    public class FluidVolume : Updateable, IDuringForcesUpdateable, ICollisionRulesOwner
    {
        //TODO: The current FluidVolume implementation is awfully awful.
        //It would be really nice if it was a bit more flexible and less clunktastic.
        //(A mesh volume, maybe?)

        private RigidTransform surfaceTransform;
        private FPMatrix3x3 toSurfaceRotationMatrix;
        FPVector3 upVector;
        ///<summary>
        /// Gets or sets the up vector of the fluid volume.
        ///</summary>
        public FPVector3 UpVector
        {
            get
            {
                return upVector;
            }
            set
            {
                value.Normalize();
                upVector = value;

                RecalculateBoundingBox();

            }
        }

        /// <summary>
        /// Gets or sets the dictionary storing density multipliers for the fluid volume.  If a value is specified for an entity, the density of the object is effectively scaled to match.
        /// Higher values make entities sink more, lower values make entities Fix64 more.
        /// </summary>
        public Dictionary<Entity, Fix64> DensityMultipliers { get; set; }

        BoundingBox boundingBox;
        /// <summary>
        /// Bounding box surrounding the surface triangles and entire depth of the object.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

		Fix64 maxDepth;
        /// <summary>
        /// Maximum depth of the fluid from the surface.
        /// </summary>
        public Fix64 MaxDepth
        {
            get
            {
                return maxDepth;
            }
            set
            {
                maxDepth = value;
                RecalculateBoundingBox();
            }
        }

        /// <summary>
        /// Density of the fluid represented in the volume.
        /// </summary>
        public Fix64 Density { get; set; }

		int samplePointsPerDimension = 8;
        /// <summary>
        /// Number of locations along each of the horizontal axes from which to sample the shape.
        /// Defaults to 8.
        /// </summary>
        public int SamplePointsPerDimension
        {
            get
            {
                return samplePointsPerDimension;
            }
            set
            {
                samplePointsPerDimension = value;
            }
        }

        /// <summary>
        /// Fraction by which to reduce the linear momentum of Fix64ing objects each update.
        /// </summary>
        public Fix64 LinearDamping { get; set; }

        /// <summary>
        /// Fraction by which to reduce the angular momentum of Fix64ing objects each update.
        /// </summary>
        public Fix64 AngularDamping { get; set; }



        private FPVector3 flowDirection;
        /// <summary>
        /// Direction in which to exert force on objects within the fluid.
        /// flowForce and maxFlowSpeed must have valid values as well for the flow to work.
        /// </summary>
        public FPVector3 FlowDirection
        {
            get
            {
                return flowDirection;
            }
            set
            {
				Fix64 length = value.Length();
                if (length > F64.C0)
                {
                    flowDirection = value / length;
                }
                else
                    flowDirection = FPVector3.Zero;
                //TODO: Activate bodies in water
            }
        }

        private Fix64 flowForce;

        /// <summary>
        /// Magnitude of the flow's force, in units of flow direction.
        /// flowDirection and maxFlowSpeed must have valid values as well for the flow to work.
        /// </summary>
        public Fix64 FlowForce
        {
            get
            {
                return flowForce;
            }
            set
            {
                flowForce = value;
                //TODO: Activate bodies in water
            }
        }

        Fix64 maxFlowSpeed;
        /// <summary>
        /// Maximum speed of the flow; objects will not be accelerated by the flow force beyond this speed.
        /// flowForce and flowDirection must have valid values as well for the flow to work.
        /// </summary>
        public Fix64 MaxFlowSpeed
        {
            get
            {
                return maxFlowSpeed;
            }
            set
            {
                maxFlowSpeed = value;
            }

        }

        IQueryAccelerator QueryAccelerator { get; set; }

        ///<summary>
        /// Gets or sets the parallel loop provider used by the fluid volume.
        ///</summary>
        public IParallelLooper ParallelLooper { get; set; }

        private List<FPVector3[]> surfaceTriangles;
        /// <summary>
        /// List of coplanar triangles composing the surface of the fluid.
        /// </summary>
        public List<FPVector3[]> SurfaceTriangles
        {
            get
            {
                return surfaceTriangles;
            }
            set
            {
                surfaceTriangles = value;
                RecalculateBoundingBox();
            }
        }

		Fix64 gravity;
        ///<summary>
        /// Gets or sets the gravity used by the fluid volume.
        ///</summary>
        public Fix64 Gravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }



        /// <summary>
        /// Creates a fluid volume.
        /// </summary>
        /// <param name="upVector">Up vector of the fluid volume.</param>
        /// <param name="gravity">Strength of gravity for the purposes of the fluid volume.</param>
        /// <param name="surfaceTriangles">List of triangles composing the surface of the fluid.  Set up as a list of length 3 arrays of Vector3's.</param>
        /// <param name="depth">Depth of the fluid back along the surface normal.</param>
        /// <param name="fluidDensity">Density of the fluid represented in the volume.</param>
        /// <param name="linearDamping">Fraction by which to reduce the linear momentum of Fix64ing objects each update, in addition to any of the body's own damping.</param>
        /// <param name="angularDamping">Fraction by which to reduce the angular momentum of Fix64ing objects each update, in addition to any of the body's own damping.</param>
        public FluidVolume(FPVector3 upVector, Fix64 gravity, List<FPVector3[]> surfaceTriangles, Fix64 depth, Fix64 fluidDensity, Fix64 linearDamping, Fix64 angularDamping)
        {
            Gravity = gravity;
            SurfaceTriangles = surfaceTriangles;
            MaxDepth = depth;
            Density = fluidDensity;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;

            UpVector = upVector;

            analyzeCollisionEntryDelegate = AnalyzeEntry;

            DensityMultipliers = new Dictionary<Entity, Fix64>();
        }

        /// <summary>
        /// Recalculates the bounding box of the fluid based on its depth, surface normal, and surface triangles.
        /// </summary>
        public void RecalculateBoundingBox()
        {
            var points = CommonResources.GetVectorList();
            foreach (var tri in SurfaceTriangles)
            {
                points.Add(tri[0]);
                points.Add(tri[1]);
                points.Add(tri[2]);
                points.Add(tri[0] - upVector * MaxDepth);
                points.Add(tri[1] - upVector * MaxDepth);
                points.Add(tri[2] - upVector * MaxDepth);
            }
            boundingBox = BoundingBox.CreateFromPoints(points);
            CommonResources.GiveBack(points);

            //Compute the transforms used to pull objects into fluid local space.
            FPQuaternion.GetQuaternionBetweenNormalizedVectors(ref Toolbox.UpVector, ref upVector, out surfaceTransform.Orientation);
            FPMatrix3x3.CreateFromQuaternion(ref surfaceTransform.Orientation, out toSurfaceRotationMatrix);
            surfaceTransform.Position = surfaceTriangles[0][0];
        }

        List<BroadPhaseEntry> broadPhaseEntries = new List<BroadPhaseEntry>();

        /// <summary>
        /// Applies buoyancy forces to appropriate objects.
        /// Called automatically when needed by the owning Space.
        /// </summary>
        /// <param name="dt">Time since last frame in physical logic.</param>
        void IDuringForcesUpdateable.Update(Fix64 dt)
        {
            QueryAccelerator.GetEntries(boundingBox, broadPhaseEntries);
            //TODO: Could integrate the entire thing into the collision detection pipeline.  Applying forces
            //in the collision detection pipeline isn't allowed, so there'd still need to be an Updateable involved.
            //However, the broadphase query would be eliminated and the raycasting work would be automatically multithreaded.

            this.dt = dt;

            //Don't always multithread.  For small numbers of objects, the overhead of using multithreading isn't worth it.
            //Could tune this value depending on platform for better performance.
            if (broadPhaseEntries.Count > 30 && ParallelLooper != null && ParallelLooper.ThreadCount > 1)
                ParallelLooper.ForLoop(0, broadPhaseEntries.Count, analyzeCollisionEntryDelegate);
            else
                for (int i = 0; i < broadPhaseEntries.Count; i++)
                {
                    AnalyzeEntry(i);
                }

            broadPhaseEntries.Clear();




        }

		Fix64 dt;
        Action<int> analyzeCollisionEntryDelegate;

        void AnalyzeEntry(int i)
        {
            var entityCollidable = broadPhaseEntries[i] as EntityCollidable;
            if (entityCollidable != null && entityCollidable.IsActive && entityCollidable.entity.isDynamic && CollisionRules.collisionRuleCalculator(this, entityCollidable) <= CollisionRule.Normal)
            {
                bool keepGoing = false;
                foreach (var tri in surfaceTriangles)
                {
                    //Don't need to do anything if the entity is outside of the water.
                    if (Toolbox.IsPointInsideTriangle(ref tri[0], ref tri[1], ref tri[2], ref entityCollidable.worldTransform.Position))
                    {
                        keepGoing = true;
                        break;
                    }
                }
                if (!keepGoing)
                    return;

				//The entity is submerged, apply buoyancy forces.
				Fix64 submergedVolume;
                FPVector3 submergedCenter;
                GetBuoyancyInformation(entityCollidable, out submergedVolume, out submergedCenter);

                if (submergedVolume > F64.C0)
                {

                    //The approximation can sometimes output a volume greater than the shape itself. Don't let that error seep into usage.
                    Fix64 fractionSubmerged = MathHelper.Min(F64.C1, submergedVolume / entityCollidable.entity.CollisionInformation.Shape.Volume);

					//Divide the volume by the density multiplier if present.
					Fix64 densityMultiplier;
                    if (DensityMultipliers.TryGetValue(entityCollidable.entity, out densityMultiplier))
                    {
                        submergedVolume /= densityMultiplier;
                    }
                    FPVector3 force;
                    FPVector3.Multiply(ref upVector, -gravity * Density * dt * submergedVolume, out force);
                    entityCollidable.entity.ApplyImpulseWithoutActivating(ref submergedCenter, ref force);

                    //Flow
                    if (FlowForce != F64.C0)
                    {
                        Fix64 dot = MathHelper.Max(FPVector3.Dot(entityCollidable.entity.linearVelocity, flowDirection), F64.C0);
                        if (dot < MaxFlowSpeed)
                        {
                            force = MathHelper.Min(FlowForce, (MaxFlowSpeed - dot) * entityCollidable.entity.mass) * dt * fractionSubmerged * FlowDirection;
                            entityCollidable.entity.ApplyLinearImpulse(ref force);
                        }
                    }
                    //Damping
                    entityCollidable.entity.ModifyLinearDamping(fractionSubmerged * LinearDamping);
                    entityCollidable.entity.ModifyAngularDamping(fractionSubmerged * AngularDamping);

                }
            }
        }

        void GetBuoyancyInformation(EntityCollidable collidable, out Fix64 submergedVolume, out FPVector3 submergedCenter)
        {
            BoundingBox entityBoundingBox;

            RigidTransform localTransform;
            RigidTransform.MultiplyByInverse(ref collidable.worldTransform, ref surfaceTransform, out localTransform);
            collidable.Shape.GetBoundingBox(ref localTransform, out entityBoundingBox);
            if (entityBoundingBox.Min.Y > F64.C0)
            {
                //Fish out of the water.  Don't need to do raycast tests on objects not at the boundary.
                submergedVolume = F64.C0;
                submergedCenter = collidable.worldTransform.Position;
                return;
            }
            if (entityBoundingBox.Max.Y < F64.C0)
            {
                submergedVolume = collidable.entity.CollisionInformation.Shape.Volume;
                submergedCenter = collidable.worldTransform.Position;
                return;
            }

            FPVector3 origin, xSpacing, zSpacing;
			Fix64 perColumnArea;
            GetSamplingOrigin(ref entityBoundingBox, out xSpacing, out zSpacing, out perColumnArea, out origin);

			Fix64 boundingBoxHeight = entityBoundingBox.Max.Y - entityBoundingBox.Min.Y;
			Fix64 maxLength = -entityBoundingBox.Min.Y;
            submergedCenter = new FPVector3();
            submergedVolume = F64.C0;
            for (int i = 0; i < samplePointsPerDimension; i++)
            {
                for (int j = 0; j < samplePointsPerDimension; j++)
                {
                    FPVector3 columnVolumeCenter;
					Fix64 submergedHeight;
                    if ((submergedHeight = GetSubmergedHeight(collidable, maxLength, boundingBoxHeight, ref origin, ref xSpacing, ref zSpacing, i, j, out columnVolumeCenter)) > F64.C0)
                    {
						Fix64 columnVolume = submergedHeight * perColumnArea;
                        FPVector3.Multiply(ref columnVolumeCenter, columnVolume, out columnVolumeCenter);
                        FPVector3.Add(ref columnVolumeCenter, ref submergedCenter, out submergedCenter);
                        submergedVolume += columnVolume;
                    }
                }
            }
            FPVector3.Divide(ref submergedCenter, submergedVolume, out submergedCenter);
            //Pull the submerged center into world space before applying the force.
            RigidTransform.Transform(ref submergedCenter, ref surfaceTransform, out submergedCenter);

        }

        void GetSamplingOrigin(ref BoundingBox entityBoundingBox, out FPVector3 xSpacing, out FPVector3 zSpacing, out Fix64 perColumnArea, out FPVector3 origin)
        {
			//Compute spacing and increment informaiton.
			Fix64 samplePointsPerDimensionFix64 = (Fix64)samplePointsPerDimension;
			Fix64 widthIncrement = (entityBoundingBox.Max.X - entityBoundingBox.Min.X) / samplePointsPerDimensionFix64;
			Fix64 lengthIncrement = (entityBoundingBox.Max.Z - entityBoundingBox.Min.Z) / samplePointsPerDimensionFix64;
            xSpacing = new FPVector3(widthIncrement, F64.C0, F64.C0);
            zSpacing = new FPVector3(F64.C0, F64.C0, lengthIncrement);
            FPQuaternion.Transform(ref xSpacing, ref surfaceTransform.Orientation, out xSpacing);
            FPQuaternion.Transform(ref zSpacing, ref surfaceTransform.Orientation, out zSpacing);
            perColumnArea = widthIncrement * lengthIncrement;


            //Compute the origin.
            FPVector3 minimum;
            RigidTransform.Transform(ref entityBoundingBox.Min, ref surfaceTransform, out minimum);
            //Matrix3X3.TransformTranspose(ref entityBoundingBox.Min, ref surfaceOrientationTranspose, out minimum);
            FPVector3 offset;
            FPVector3.Multiply(ref xSpacing, F64.C0p5, out offset);
            FPVector3.Add(ref minimum, ref offset, out origin);
            FPVector3.Multiply(ref zSpacing, F64.C0p5, out offset);
            FPVector3.Add(ref origin, ref offset, out origin);


            //TODO: Could adjust the grid origin such that a ray always hits the deepest point.
            //The below code is a prototype of the idea, but has bugs.
            //var convexInfo = collidable as ConvexCollisionInformation;
            //if (convexInfo != null)
            //{
            //    Vector3 dir;
            //    Vector3.Negate(ref upVector, out dir);
            //    Vector3 extremePoint;
            //    convexInfo.Shape.GetExtremePoint(dir, ref convexInfo.worldTransform, out extremePoint);
            //    //Use extreme point to snap to grid.
            //    Vector3.Subtract(ref extremePoint, ref origin, out offset);
            //    Fix64 offsetX, offsetZ;
            //    Vector3.Dot(ref offset, ref right, out offsetX);
            //    Vector3.Dot(ref offset, ref backward, out offsetZ);
            //    offsetX %= widthIncrement;
            //    offsetZ %= lengthIncrement;

            //    if (offsetX > .5f * widthIncrement)
            //    {
            //        Vector3.Multiply(ref right, 1 - offsetX, out offset);
            //    }
            //    else
            //    {
            //        Vector3.Multiply(ref right, -offsetX, out offset);
            //    }

            //    if (offsetZ > .5f * lengthIncrement)
            //    {
            //        Vector3 temp;
            //        Vector3.Multiply(ref right, 1 - offsetZ, out temp);
            //        Vector3.Add(ref temp, ref offset, out offset);
            //    }
            //    else
            //    {
            //        Vector3 temp;
            //        Vector3.Multiply(ref right, -offsetZ, out temp);
            //        Vector3.Add(ref temp, ref offset, out offset);
            //    }

            //    Vector3.Add(ref origin, ref offset, out origin);


            //}
        }

		Fix64 GetSubmergedHeight(EntityCollidable collidable, Fix64 maxLength, Fix64 boundingBoxHeight, ref FPVector3 rayOrigin, ref FPVector3 xSpacing, ref FPVector3 zSpacing, int i, int j, out FPVector3 volumeCenter)
        {
            FPRay fpRay;
            FPVector3.Multiply(ref xSpacing, (Fix64)i, out fpRay.origin);
            FPVector3.Multiply(ref zSpacing, (Fix64)j, out fpRay.direction);
            FPVector3.Add(ref fpRay.origin, ref fpRay.direction, out fpRay.origin);
            FPVector3.Add(ref fpRay.origin, ref rayOrigin, out fpRay.origin);

            fpRay.direction = upVector;
            //do a bottom-up raycast.
            FPRayHit fpRayHit;
            //Only go up to maxLength.  If it's further away than maxLength, then it's above the water and it doesn't contribute anything.
            if (collidable.RayCast(fpRay, maxLength, out fpRayHit))
            {
                //Position the ray to point from the other side.
                FPVector3.Multiply(ref fpRay.direction, boundingBoxHeight, out fpRay.direction);
                FPVector3.Add(ref fpRay.origin, ref fpRay.direction, out fpRay.origin);
                FPVector3.Negate(ref upVector, out fpRay.direction);

                //Transform the hit into local space.
                RigidTransform.TransformByInverse(ref fpRayHit.Location, ref surfaceTransform, out fpRayHit.Location);
				Fix64 bottomY = fpRayHit.Location.Y;
				Fix64 bottom = fpRayHit.T;
                FPVector3 bottomPosition = fpRayHit.Location;
                if (collidable.RayCast(fpRay, boundingBoxHeight - fpRayHit.T, out fpRayHit))
                {
                    //Transform the hit into local space.
                    RigidTransform.TransformByInverse(ref fpRayHit.Location, ref surfaceTransform, out fpRayHit.Location);
                    FPVector3.Add(ref fpRayHit.Location, ref bottomPosition, out volumeCenter);
                    FPVector3.Multiply(ref volumeCenter, F64.C0p5, out volumeCenter);
                    return MathHelper.Min(-bottomY, boundingBoxHeight - fpRayHit.T - bottom);
                }
                //This inner raycast should always hit, but just in case it doesn't due to some numerical problem, give it a graceful way out.
                volumeCenter = FPVector3.Zero;
                return F64.C0;
            }
            volumeCenter = FPVector3.Zero;
            return F64.C0;
        }

        public override void OnAdditionToSpace(BEPUphysicsSpace newBepUphysicsSpace)
        {
            base.OnAdditionToSpace(newBepUphysicsSpace);
            ParallelLooper = newBepUphysicsSpace.ParallelLooper;
            QueryAccelerator = newBepUphysicsSpace.BroadPhase.QueryAccelerator;
        }

        public override void OnRemovalFromSpace(BEPUphysicsSpace oldBepUphysicsSpace)
        {
            base.OnRemovalFromSpace(oldBepUphysicsSpace);
            ParallelLooper = null;
            QueryAccelerator = null;
        }

        private CollisionRules collisionRules = new CollisionRules();
        /// <summary>
        /// Gets or sets the collision rules associated with the fluid volume.
        /// </summary>
        public CollisionRules CollisionRules
        {
            get
            {
                return collisionRules;
            }
            set
            {
                collisionRules = value;
            }
        }
    }
}