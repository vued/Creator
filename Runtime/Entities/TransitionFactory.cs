namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Factory implementation for <see cref="ITransition"/> objects.
    /// </summary>
    public class TransitionFactory : EntityFactory
    {
        /// <summary>
        /// Creates a new <see cref="ITransition"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        public override IEntity Create()
        {
            return new Transition();
        }

        /// <inheritdoc/>
        public override IEntity Create(string name)
        {
            return Create();
        }
    }
}
