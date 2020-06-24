namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Factory implementation for <see cref="IEntity"/> objects.
    /// </summary>
    public class StepFactory : EntityFactory
    {
        /// <summary>
        /// Creates a new <see cref="IStep"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        public override IEntity Create()
        {
            return new Step("New Step");
        }
    }
}
