﻿using System;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.CollisionShapes;
using FixedMath;
using FixedMath.ResourceManagement;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.OtherSpaceStages;
using RigidTransform = FixedMath.RigidTransform;
using FixMath.NET;

namespace BEPUphysics.BroadPhaseEntries
{
    ///<summary>
    /// Collidable mesh which can be created from a reusable InstancedMeshShape.
    /// Very little data is needed for each individual InstancedMesh object, allowing
    /// a complicated mesh to be repeated many times.  Since the hierarchy used to accelerate
    /// collisions is purely local, it may be marginally slower than an individual StaticMesh.
    ///</summary>
    public class InstancedMesh : StaticCollidable
    {

        internal AffineTransform worldTransform;
        ///<summary>
        /// Gets or sets the world transform of the mesh.
        ///</summary>
        public AffineTransform WorldTransform
        {
            get
            {
                return worldTransform;
            }
            set
            {
                worldTransform = value;
                Shape.ComputeBoundingBox(ref value, out boundingBox);
            }
        }

        /// <summary>
        /// Updates the bounding box to the current state of the entry.
        /// </summary>
        public override void UpdateBoundingBox()
        {
            Shape.ComputeBoundingBox(ref worldTransform, out boundingBox);
        }


        ///<summary>
        /// Constructs a new InstancedMesh.
        ///</summary>
        ///<param name="meshShape">Shape to use for the instance.</param>
        public InstancedMesh(InstancedMeshShape meshShape)
            : this(meshShape, AffineTransform.Identity)
        {
        }

        ///<summary>
        /// Constructs a new InstancedMesh.
        ///</summary>
        ///<param name="meshShape">Shape to use for the instance.</param>
        ///<param name="worldTransform">Transform to use for the instance.</param>
        public InstancedMesh(InstancedMeshShape meshShape, AffineTransform worldTransform)
        {
            this.worldTransform = worldTransform;
            base.Shape = meshShape;
            Events = new ContactEventManager<InstancedMesh>();


        }

        ///<summary>
        /// Gets the shape used by the instanced mesh.
        ///</summary>
        public new InstancedMeshShape Shape
        {
            get
            {
                return (InstancedMeshShape)shape;
            }
        }

        internal TriangleSidedness sidedness = TriangleSidedness.DoubleSided;
        ///<summary>
        /// Gets or sets the sidedness of the mesh.  This can be used to ignore collisions and rays coming from a direction relative to the winding of the triangle.
        ///</summary>
        public TriangleSidedness Sidedness
        {
            get
            {
                return sidedness;
            }
            set
            {
                sidedness = value;
            }
        }

        internal bool improveBoundaryBehavior = true;
        /// <summary>
        /// Gets or sets whether or not the collision system should attempt to improve contact behavior at the boundaries between triangles.
        /// This has a slight performance cost, but prevents objects sliding across a triangle boundary from 'bumping,' and otherwise improves
        /// the robustness of contacts at edges and vertices.
        /// </summary>
        public bool ImproveBoundaryBehavior
        {
            get
            {
                return improveBoundaryBehavior;
            }
            set
            {
                improveBoundaryBehavior = value;
            }
        }


        protected internal ContactEventManager<InstancedMesh> events;
        ///<summary>
        /// Gets the event manager of the mesh.
        ///</summary>
        public ContactEventManager<InstancedMesh> Events
        {
            get
            {
                return events;
            }
            set
            {
                if (value.Owner != null && //Can't use a manager which is owned by a different entity.
                    value != events) //Stay quiet if for some reason the same event manager is being set.
                    throw new ArgumentException("Event manager is already owned by a mesh; event managers cannot be shared.");
                if (events != null)
                    events.Owner = null;
                events = value;
                if (events != null)
                    events.Owner = this;
            }
        }
        protected internal override IContactEventTriggerer EventTriggerer
        {
            get { return events; }
        }

        protected override IDeferredEventCreator EventCreator
        {
            get
            {
                return events;
            }
        }


        /// <summary>
        /// Tests a ray against the entry.
        /// </summary>
        /// <param name="fpRay">Ray to test.</param>
        /// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
        /// <param name="fpRayHit">Hit location of the ray on the entry, if any.</param>
        /// <returns>Whether or not the ray hit the entry.</returns>
        public override bool RayCast(FPRay fpRay, Fix64 maximumLength, out FPRayHit fpRayHit)
        {
            return RayCast(fpRay, maximumLength, sidedness, out fpRayHit);
        }

        ///<summary>
        /// Tests a ray against the instance.
        ///</summary>
        ///<param name="fpRay">Ray to test.</param>
        ///<param name="maximumLength">Maximum length of the ray to test; in units of the ray's direction's length.</param>
        ///<param name="sidedness">Sidedness to use during the ray cast.  This does not have to be the same as the mesh's sidedness.</param>
        ///<param name="fpRayHit">The hit location of the ray on the mesh, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(FPRay fpRay, Fix64 maximumLength, TriangleSidedness sidedness, out FPRayHit fpRayHit)
        {
            //Put the ray into local space.
            FPRay localFpRay;
            AffineTransform inverse;
            AffineTransform.Invert(ref worldTransform, out inverse);
            FPMatrix3x3.Transform(ref fpRay.direction, ref inverse.LinearTransform, out localFpRay.direction);
            AffineTransform.Transform(ref fpRay.origin, ref inverse, out localFpRay.origin);

            if (Shape.TriangleMesh.RayCast(localFpRay, maximumLength, sidedness, out fpRayHit))
            {
                //Transform the hit into world space.
                FPVector3.Multiply(ref fpRay.direction, fpRayHit.T, out fpRayHit.Location);
                FPVector3.Add(ref fpRayHit.Location, ref fpRay.origin, out fpRayHit.Location);
                FPMatrix3x3.TransformTranspose(ref fpRayHit.Normal, ref inverse.LinearTransform, out fpRayHit.Normal);
                return true;
            }
            fpRayHit = new FPRayHit();
            return false;
        }

        /// <summary>
        /// Casts a convex shape against the collidable.
        /// </summary>
        /// <param name="castShape">Shape to cast.</param>
        /// <param name="startingTransform">Initial transform of the shape.</param>
        /// <param name="sweep">Sweep to apply to the shape.</param>
        /// <param name="hit">Hit data, if any.</param>
        /// <returns>Whether or not the cast hit anything.</returns>
        public override bool ConvexCast(CollisionShapes.ConvexShapes.ConvexShape castShape, ref RigidTransform startingTransform, ref FPVector3 sweep, out FPRayHit hit)
        {
            hit = new FPRayHit();
            BoundingBox boundingBox;
            castShape.GetSweptLocalBoundingBox(ref startingTransform, ref worldTransform, ref sweep, out boundingBox);
            var tri = PhysicsThreadResources.GetTriangle();
            var hitElements = CommonResources.GetIntList();
            if (this.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, hitElements))
            {
                hit.T = Fix64.MaxValue;
                for (int i = 0; i < hitElements.Count; i++)
                {
                    Shape.TriangleMesh.Data.GetTriangle(hitElements[i], out tri.vA, out tri.vB, out tri.vC);
                    AffineTransform.Transform(ref tri.vA, ref worldTransform, out tri.vA);
                    AffineTransform.Transform(ref tri.vB, ref worldTransform, out tri.vB);
                    AffineTransform.Transform(ref tri.vC, ref worldTransform, out tri.vC);
                    FPVector3 center;
                    FPVector3.Add(ref tri.vA, ref tri.vB, out center);
                    FPVector3.Add(ref center, ref tri.vC, out center);
                    FPVector3.Multiply(ref center, F64.OneThird, out center);
                    FPVector3.Subtract(ref tri.vA, ref center, out tri.vA);
                    FPVector3.Subtract(ref tri.vB, ref center, out tri.vB);
                    FPVector3.Subtract(ref tri.vC, ref center, out tri.vC);
                    tri.MaximumRadius = tri.vA.LengthSquared();
					Fix64 radius = tri.vB.LengthSquared();
                    if (tri.MaximumRadius < radius)
                        tri.MaximumRadius = radius;
                    radius = tri.vC.LengthSquared();
                    if (tri.MaximumRadius < radius)
                        tri.MaximumRadius = radius;
                    tri.MaximumRadius = Fix64.Sqrt(tri.MaximumRadius);
                    tri.collisionMargin = F64.C0;
                    var triangleTransform = new RigidTransform { Orientation = FPQuaternion.Identity, Position = center };
                    FPRayHit tempHit;
                    if (MPRToolbox.Sweep(castShape, tri, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref triangleTransform, out tempHit) && tempHit.T < hit.T)
                    {
                        hit = tempHit;
                    }
                }
                tri.MaximumRadius = F64.C0;
                PhysicsThreadResources.GiveBack(tri);
                CommonResources.GiveBack(hitElements);
                return hit.T != Fix64.MaxValue;
            }
            PhysicsThreadResources.GiveBack(tri);
            CommonResources.GiveBack(hitElements);
            return false;
        }


    }
}
