using System;

namespace Innoactive.CreatorEditor.UI.Drawers
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class InstantiatorTrainingDrawerAttribute : Attribute
    {
        public Type Type { get; private set; }

        public InstantiatorTrainingDrawerAttribute(Type type)
        {
            Type = type;
        }
    }
}
