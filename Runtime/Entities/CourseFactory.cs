namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Factory implementation for <see cref="ICourse"/> objects.
    /// </summary>
    public class CourseFactory : EntityFactory
    {
        private EntityFactory chapterFactory = new ChapterFactory();

        /// <summary>
        /// Creates a new <see cref="ICourse"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        public override IEntity Create()
        {
            return Create("New Course");
        }

        /// <summary>
        /// Creates a new <see cref="ICourse"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        /// <param name="name"><see cref="ICourse"/>'s name.</param>
        public override IEntity Create(string name)
        {
            IChapter chapter = (IChapter) chapterFactory.Create("Chapter 1");
            return new Course(name, chapter);
        }

        /// <summary>
        /// Sets a new default <see cref="IChapter"/> provider.
        /// </summary>
        public void SetTransitionFactory(EntityFactory newFactory)
        {
            chapterFactory = newFactory;
        }
    }
}
