using FixedMath;
using FixedMath.DataStructures;

namespace BEPUphysics.OtherSpaceStages
{


    ///<summary>
    /// Thead-safely buffers up space objects for addition and removal.
    ///</summary>
    public class SpaceObjectBuffer : ProcessingStage
    {

        private struct SpaceObjectChange
        {
            public readonly ISpaceObject SpaceObject;
            //Could change to enumeration, or more generally, buffered 'action<ISpaceObject>' to perform on the space object.
            public readonly bool ShouldAdd;

            public SpaceObjectChange(ISpaceObject spaceObject, bool shouldAdd)
            {
                SpaceObject = spaceObject;
                ShouldAdd = shouldAdd;
            }
        }
        private ConcurrentDeque<SpaceObjectChange> objectsToChange = new ConcurrentDeque<SpaceObjectChange>();

        private BEPUphysicsSpace _bepUphysicsSpace;
        ///<summary>
        /// Gets the space which owns this buffer.
        ///</summary>
        public BEPUphysicsSpace BepUphysicsSpace
        {
            get { return _bepUphysicsSpace; }
        }

        ///<summary>
        /// Constructs the buffer.
        ///</summary>
        ///<param name="bepUphysicsSpace">Space that owns the buffer.</param>
        public SpaceObjectBuffer(BEPUphysicsSpace bepUphysicsSpace)
        {
            Enabled = true;
            this._bepUphysicsSpace = bepUphysicsSpace;
        }

        ///<summary>
        /// Adds a space object to the buffer.
        /// It will be added to the space the next time the buffer is flushed.
        ///</summary>
        ///<param name="spaceObject">Space object to add.</param>
        public void Add(ISpaceObject spaceObject)
        {
            objectsToChange.Enqueue(new SpaceObjectChange(spaceObject, true));
        }

        /// <summary>
        /// Enqueues a removal request to the buffer.
        /// It will be processed the next time the buffer is flushed.
        /// </summary>
        /// <param name="spaceObject">Space object to remove.</param>
        public void Remove(ISpaceObject spaceObject)
        {
            objectsToChange.Enqueue(new SpaceObjectChange(spaceObject, false));
        }


        protected override void UpdateStage()
        {
            SpaceObjectChange change;
            while (objectsToChange.TryDequeueFirst(out change))
            {
                if (change.ShouldAdd)
                    _bepUphysicsSpace.Add(change.SpaceObject);
                else
                    _bepUphysicsSpace.Remove(change.SpaceObject);
            }
        }



    }
}
