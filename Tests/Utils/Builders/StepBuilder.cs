namespace Innoactive.Creator.Tests.Builder
{
    public abstract class StepBuilder<TStep> : BuilderWithResourcePath<TStep>
    {
        public StepBuilder(string name) : base(name)
        {
        }
    }
}
