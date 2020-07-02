namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Factory implementation for <see cref="IChapter"/> objects.
    /// </summary>
    public class ChapterFactory : EntityFactory
    {
        /// <summary>
        /// Creates a new <see cref="IChapter"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        public override IEntity Create()
        {
            return Create("New Chapter");
        }

        /// <summary>
        /// Creates a new <see cref="IChapter"/> and returns it as <see cref="IEntity"/>.
        /// </summary>
        /// <param name="name"><see cref="IChapter"/>'s name.</param>
        public override IEntity Create(string name)
        {
            return new Chapter(name, null);
        }
    }
}
