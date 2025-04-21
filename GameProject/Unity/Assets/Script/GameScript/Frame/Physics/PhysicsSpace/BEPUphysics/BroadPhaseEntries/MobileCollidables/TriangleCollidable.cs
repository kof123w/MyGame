﻿using BEPUphysics.CollisionShapes.ConvexShapes;
using FixedMath;
 

namespace BEPUphysics.BroadPhaseEntries.MobileCollidables
{
    ///<summary>
    /// Special case collidable for reuseable triangles.
    ///</summary>
    public class TriangleCollidable : ConvexCollidable<TriangleShape>
    {
        ///<summary>
        /// Constructs a new shapeless collidable.
        ///</summary>
        public TriangleCollidable()
            : base(new TriangleShape())
        {
        }

        ///<summary>
        /// Constructs the triangle collidable using the given shape.
        ///</summary>
        ///<param name="shape">TriangleShape to use in the collidable.</param>
        public TriangleCollidable(TriangleShape shape)
            : base(shape)
        {
        }

        ///<summary>
        /// Initializes the collidable using the new triangle shape, but does NOT
        /// fire any shape-changed events.
        ///</summary>
        ///<param name="a">First vertex in the triangle.</param>
        ///<param name="b">Second vertex in the triangle. </param>
        ///<param name="c">Third vertex in the triangle. </param>
        public void Initialize(ref FPVector3 a, ref FPVector3 b, ref FPVector3 c)
        {
            var shape = Shape;
            shape.collisionMargin = F64.C0;
            shape.sidedness = TriangleSidedness.DoubleSided;
            shape.vA = a;
            shape.vB = b;
            shape.vC = c;
        }

        ///<summary>
        /// Cleans up the collidable by removing all events.
        ///</summary>
        public void CleanUp()
        {
            events.RemoveAllEvents();
        }
    }
}
