﻿using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.BroadPhaseEntries.MobileCollidables
{
    ///<summary>
    /// Collidable with a convex shape.
    ///</summary>
    public abstract class ConvexCollidable : EntityCollidable
    {

        protected ConvexCollidable(ConvexShape shape)
            : base(shape)
        {
            Events = new ContactEventManager<EntityCollidable>();
        }

        ///<summary>
        /// Gets the shape of the collidable.
        ///</summary>
        public new ConvexShape Shape
        {
            get
            {
                return (ConvexShape)shape;
            }
        }


        public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref FPVector3 sweep, out FPRayHit hit)
        {
            return MPRToolbox.Sweep(castShape, Shape, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref worldTransform, out hit);
        }

    }

    ///<summary>
    /// Collidable with a convex shape of a particular type.
    ///</summary>
    ///<typeparam name="T">ConvexShape type.</typeparam>
    public class ConvexCollidable<T> : ConvexCollidable where T : ConvexShape
    {
        ///<summary>
        /// Gets the shape of the collidable.
        ///</summary>
        public new T Shape
        {
            get
            {
                return (T)shape;
            }
        }

        ///<summary>
        /// Constructs a new convex collidable.
        ///</summary>
        ///<param name="shape">Shape to use in the collidable.</param>
        public ConvexCollidable(T shape)
            : base(shape)
        {

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
            return Shape.RayTest(ref fpRay, ref worldTransform, maximumLength, out fpRayHit);
        }



        protected internal override void UpdateBoundingBoxInternal(Fix64 dt)
        {
            Shape.GetBoundingBox(ref worldTransform, out boundingBox);

            ExpandBoundingBox(ref boundingBox, dt);
        }





    }
}
