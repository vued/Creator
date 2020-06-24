namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Base factory class for <see cref="IEntity"/> objects.
    /// </summary>
    public abstract class EntityFactory
    {
        /// <summary>
        /// Creates a new <see cref="IEntity"/> based object.
        /// </summary>
        public abstract IEntity Create();
    }
}
