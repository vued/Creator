using System;
using UnityEngine;
using Innoactive.Creator.Unity;

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

        /// <summary>
        /// Builds a new <see cref="IStep"/> object.
        /// </summary>
        /// <param name="position">Graphical default position for this <see cref="IStep"/> on the 'Workflow Editor'.</param>
        /// <typeparam name="T">Type of <see cref="EntityFactory"/> to be used to construct the <see cref="IStep"/>.</typeparam>
        public IStep BuildStep<T>(Vector2 position = default) where T : EntityFactory, new()
        {
            T stepFactory = new T();
            IStep step = (IStep) stepFactory.Create();
            step.StepMetadata.Position = position;

            EntityCreated?.Invoke(this, new EntityCreationEventArgs(step));

            return step;
        }

        /// <summary>
        /// Builds a new <see cref="ITransition"/> object.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="EntityFactory"/> to be used to construct the <see cref="ITransition"/>.</typeparam>
        public ITransition BuildTransition<T>() where T : EntityFactory, new()
        {
            T transitionFactory = new T();
            return (ITransition) transitionFactory.Create();
        }
    }
}
