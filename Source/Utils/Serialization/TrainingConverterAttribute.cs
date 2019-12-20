using System;

namespace Innoactive.Hub.Training.Utils.Serialization
{
    /// <summary>
    /// Every class with this attribute which also extends JsonConverter will be added as converter to the <see cref="JsonTrainingSerializer"/>
    /// </summary>
    public class TrainingConverterAttribute : Attribute
    {

    }
}
