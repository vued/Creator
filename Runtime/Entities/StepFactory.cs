namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Factory implementation for <see cref="IEntity"/> objects.
    /// </summary>
    public class StepFactory : EntityFactory
    {
        private EntityFactory transitionFactory = new TransitionFactory();

        /// <summary>
        /// Creates a new <see cref="IStep"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        public override IEntity Create()
        {
            IStep step = new Step("New Step");
            ITransition transition = (ITransition) transitionFactory.Create();

            step.Data.Transitions.Data.Transitions.Add(transition);

            return step;
        }

        /// <summary>
        /// Sets a new default <see cref="ITransition"/> provider.
        /// </summary>
        public void SetTransitionFactory(EntityFactory newFactory)
        {
            transitionFactory = newFactory;
        }
    }
}
