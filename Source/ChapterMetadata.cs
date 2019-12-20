using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training
{
    [DataContract(IsReference = true)]
    public class ChapterMetadata : IMetadata
    {
        [DataMember]
        public IStep LastSelectedStep { get; set; }

        [DataMember]
        public Vector2 EntryNodePosition { get; set; }

        [JsonConstructor]
        public ChapterMetadata()
        {
        }
    }
}
