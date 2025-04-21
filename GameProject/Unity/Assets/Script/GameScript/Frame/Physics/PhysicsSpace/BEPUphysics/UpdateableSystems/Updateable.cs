﻿using System.Collections.Generic;

namespace BEPUphysics.UpdateableSystems
{
    ///<summary>
    /// Convenience superclass of Updateables.
    /// Updateables are updated by the Space at various
    /// points during the execution of the engine
    /// to support easy extensions.
    ///</summary>
    public abstract class Updateable : ISpaceUpdateable
    {

        protected Updateable()
        {
            IsUpdating = true;
        }

        #region ISpaceUpdateable Members
        List<UpdateableManager> managers = new List<UpdateableManager>();
        List<UpdateableManager> ISpaceUpdateable.Managers
        {
            get
            {
                return managers;
            }
        }

        private bool isUpdatedSequentially = true;
        /// <summary>
        /// Gets and sets whether or not the updateable should be updated sequentially even in a multithreaded space.
        /// If this is true, the updateable can make use of the space's ParallelLooper for internal multithreading.
        /// </summary>
        public bool IsUpdatedSequentially
        {
            get { return isUpdatedSequentially; }
            set
            {
                bool oldValue = isUpdatedSequentially;
                isUpdatedSequentially = value;
                if (value != oldValue)
                    for (int i = 0; i < managers.Count; i++)
                    {
                        managers[i].SequentialUpdatingStateChanged(this);
                    }
            }
        }


        /// <summary>
        /// Gets and sets whether or not the updateable should be updated by its manager.
        /// </summary>
        public bool IsUpdating
        {
            get;
            set;
        }


        /// <summary>
        /// Called after the object is added to a space.
        /// </summary>
        /// <param name="newBepUphysicsSpace">Space to which the object was added.</param>
        public virtual void OnAdditionToSpace(BEPUphysicsSpace newBepUphysicsSpace)
        {
        }


        /// <summary>
        /// Called before an object is removed from its space.
        /// </summary>
        /// <param name="oldBepUphysicsSpace">Space from which the object was removed.</param>
        public virtual void OnRemovalFromSpace(BEPUphysicsSpace oldBepUphysicsSpace)
        {
        }

        private BEPUphysicsSpace _bepUphysicsSpace;
        BEPUphysicsSpace ISpaceObject.BepUphysicsSpace
        {
            get
            {
                return _bepUphysicsSpace;
            }
            set
            {
                _bepUphysicsSpace = value;
            }
        }

        ///<summary>
        /// Space that owns the updateable.
        ///</summary>
        public BEPUphysicsSpace BepUphysicsSpace
        {
            get
            {
                return _bepUphysicsSpace;
            }
        }

        /// <summary>
        /// Gets or sets the user data associated with this object.
        /// </summary>
        public object Tag { get; set; }
        #endregion





    }
}
