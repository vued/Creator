using System;
using Innoactive.Creator.Unity;
using UnityEngine;

namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Builder for <see cref="IEntity"/> objects.
    /// </summary>
    public class EntityBuilder : Singleton<EntityBuilder>
    {
        /// <summary>
        /// Event that is fired when a new <see cref="IEntity"/> is created.
        /// </summary>
        public class EntityCreationEventArgs : EventArgs
        {
            /// <summary>
            /// Recently created <see cref="IEntity"/>.
            /// </summary>
            public readonly IEntity Entity;

            public EntityCreationEventArgs(IEntity entity)
            {
                Entity = entity;
            }
        }

        /// <summary>
        /// Event that is fired when a new <see cref="IEntity"/> is created.
        /// </summary>
        public event EventHandler<EntityCreationEventArgs> EntityCreated;

        private StepFactory stepFactory;
        private TransitionFactory transitionFactory;

        public EntityBuilder()
        {
            stepFactory = new StepFactory();
            transitionFactory = new TransitionFactory();
        }

        /// <summary>
        /// Builds a new <see cref="IStep"/> object.
        /// </summary>
        /// <param name="position">Graphical default position for this <see cref="IStep"/> on the 'Workflow Editor'.</param>
        public IStep BuildStep(Vector2 position = default)
        {
            IStep step = (IStep) stepFactory.Create();
            ITransition transition = (ITransition) transitionFactory.Create();

            step.Data.Transitions.Data.Transitions.Add(transition);
            step.StepMetadata.Position = position;

            EntityCreated?.Invoke(this, new EntityCreationEventArgs(step));

            return step;
        }
    }
}
