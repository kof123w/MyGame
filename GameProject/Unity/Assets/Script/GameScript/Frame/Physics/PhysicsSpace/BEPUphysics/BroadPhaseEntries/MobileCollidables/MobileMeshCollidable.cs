using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.CollisionShapes;
using FixedMath;
using FixedMath.ResourceManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using System;
using FixMath.NET;

namespace BEPUphysics.BroadPhaseEntries.MobileCollidables
{
    ///<summary>
    /// Collidable used by compound shapes.
    ///</summary>
    public class MobileMeshCollidable : EntityCollidable
    {
        ///<summary>
        /// Gets the shape of the collidable.
        ///</summary>
        public new MobileMeshShape Shape
        {
            get
            {
                return (MobileMeshShape)shape;
            }
        }

        /// <summary>
        /// Constructs a new mobile mesh collidable.
        /// </summary>
        /// <param name="shape">Shape to use in the collidable.</param>
        public MobileMeshCollidable(MobileMeshShape shape)
            : base(shape)
        {
            Events = new ContactEventManager<EntityCollidable>();
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

        protected internal override void UpdateBoundingBoxInternal(Fix64 dt)
        {
            Shape.GetBoundingBox(ref worldTransform, out boundingBox);

            //This DOES NOT EXPAND the local hierarchy.
            //The bounding boxes of queries against the local hierarchy
            //should be expanded using the relative velocity.
            ExpandBoundingBox(ref boundingBox, dt);
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
            //Put the ray into local space.
            FPRay localFpRay;
            FPMatrix3x3 orientation;
            FPMatrix3x3.CreateFromQuaternion(ref worldTransform.Orientation, out orientation);
            FPMatrix3x3.TransformTranspose(ref fpRay.direction, ref orientation, out localFpRay.direction);
            FPVector3.Subtract(ref fpRay.origin, ref worldTransform.Position, out localFpRay.origin);
            FPMatrix3x3.TransformTranspose(ref localFpRay.origin, ref orientation, out localFpRay.origin);


            if (Shape.solidity == MobileMeshSolidity.Solid)
            {
                //Find all hits.  Use the count to determine the ray started inside or outside.
                //If it starts inside and we're in 'solid' mode, then return the ray start.
                //The raycast must be of infinite length at first.  This allows it to determine
                //if it is inside or outside.
                if (Shape.IsLocalRayOriginInMesh(ref localFpRay, out fpRayHit))
                {
                    //It was inside!
                    fpRayHit = new FPRayHit() { Location = fpRay.origin, Normal = FPVector3.Zero, T = F64.C0 };
                    return true;

                }
                else
                {
                    if (fpRayHit.T < maximumLength)
                    {
                        //Transform the hit into world space.
                        FPVector3.Multiply(ref fpRay.direction, fpRayHit.T, out fpRayHit.Location);
                        FPVector3.Add(ref fpRayHit.Location, ref fpRay.origin, out fpRayHit.Location);
                        FPMatrix3x3.Transform(ref fpRayHit.Normal, ref orientation, out fpRayHit.Normal);
                    }
                    else
                    {
                        //The hit was too far away, or there was no hit (in which case T would be Fix64.MaxValue).
                        return false;
                    }
                    return true;
                }
            }
            else
            {
                //Just do a normal raycast since the object isn't solid.
                TriangleSidedness sidedness;
                switch (Shape.solidity)
                {
                    case MobileMeshSolidity.Clockwise:
                        sidedness = TriangleSidedness.Clockwise;
                        break;
                    case MobileMeshSolidity.Counterclockwise:
                        sidedness = TriangleSidedness.Counterclockwise;
                        break;
                    default:
                        sidedness = TriangleSidedness.DoubleSided;
                        break;
                }
                if (Shape.TriangleMesh.RayCast(localFpRay, maximumLength, sidedness, out fpRayHit))
                {
                    //Transform the hit into world space.
                    FPVector3.Multiply(ref fpRay.direction, fpRayHit.T, out fpRayHit.Location);
                    FPVector3.Add(ref fpRayHit.Location, ref fpRay.origin, out fpRayHit.Location);
                    FPMatrix3x3.Transform(ref fpRayHit.Normal, ref orientation, out fpRayHit.Normal);
                    return true;
                }
            }
            fpRayHit = new FPRayHit();
            return false;
        }

        ///<summary>
        /// Tests a ray against the surface of the mesh.  This does not take into account solidity.
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
            FPMatrix3x3 orientation;
            FPMatrix3x3.CreateFromQuaternion(ref worldTransform.Orientation, out orientation);
            FPMatrix3x3.TransformTranspose(ref fpRay.direction, ref orientation, out localFpRay.direction);
            FPVector3.Subtract(ref fpRay.origin, ref worldTransform.Position, out localFpRay.origin);
            FPMatrix3x3.TransformTranspose(ref localFpRay.origin, ref orientation, out localFpRay.origin);

            if (Shape.TriangleMesh.RayCast(localFpRay, maximumLength, sidedness, out fpRayHit))
            {
                //Transform the hit into world space.
                FPVector3.Multiply(ref fpRay.direction, fpRayHit.T, out fpRayHit.Location);
                FPVector3.Add(ref fpRayHit.Location, ref fpRay.origin, out fpRayHit.Location);
                FPMatrix3x3.Transform(ref fpRayHit.Normal, ref orientation, out fpRayHit.Normal);
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
        public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref FPVector3 sweep, out FPRayHit hit)
        {
            if (Shape.solidity == MobileMeshSolidity.Solid)
            {
                //If the convex cast is inside the mesh and the mesh is solid, it should return t = 0.
                var ray = new FPRay() { origin = startingTransform.Position, direction = Toolbox.UpVector };
                if (Shape.IsLocalRayOriginInMesh(ref ray, out hit))
                {

                    hit = new FPRayHit() { Location = startingTransform.Position, Normal = new FPVector3(), T = F64.C0 };
                    return true;
                }
            }
            hit = new FPRayHit();
            BoundingBox boundingBox;
            var transform = new AffineTransform {Translation = worldTransform.Position};
            FPMatrix3x3.CreateFromQuaternion(ref worldTransform.Orientation, out transform.LinearTransform);
            castShape.GetSweptLocalBoundingBox(ref startingTransform, ref transform, ref sweep, out boundingBox);
            var tri = PhysicsThreadResources.GetTriangle();
            var hitElements = CommonResources.GetIntList();
            if (this.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, hitElements))
            {
                hit.T = Fix64.MaxValue;
                for (int i = 0; i < hitElements.Count; i++)
                {
                    Shape.TriangleMesh.Data.GetTriangle(hitElements[i], out tri.vA, out tri.vB, out tri.vC);
                    AffineTransform.Transform(ref tri.vA, ref transform, out tri.vA);
                    AffineTransform.Transform(ref tri.vB, ref transform, out tri.vB);
                    AffineTransform.Transform(ref tri.vC, ref transform, out tri.vC);
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
                    var triangleTransform = new RigidTransform {Orientation = FPQuaternion.Identity, Position = center};
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
