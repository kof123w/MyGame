namespace BEPUphysics
{
    ///<summary>
    /// Defines an object which can be managed by an Space.
    ///</summary>
    public interface ISpaceObject
    {
        /// <summary>
        /// Gets the Space to which the object belongs.
        /// </summary>
        BEPUphysicsSpace BepUphysicsSpace { get; set; }
        /// <summary>
        /// Called after the object is added to a space.
        /// </summary>
        /// <param name="newBepUphysicsSpace">Space to which the object was added.</param>
        void OnAdditionToSpace(BEPUphysicsSpace newBepUphysicsSpace);
        /// <summary>
        /// Called before an object is removed from its space.
        /// </summary>
        /// <param name="oldBepUphysicsSpace">Space from which the object was removed.</param>
        void OnRemovalFromSpace(BEPUphysicsSpace oldBepUphysicsSpace);
        /// <summary>
        /// Gets or sets the user data associated with this object.
        /// </summary>
        object Tag { get; set; }
    }
}
