﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Innoactive.Creator.Core.Conditions;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Core.Properties;
using Innoactive.Creator.Core.RestrictiveEnvironment;
using Innoactive.Creator.Core.SceneObjects;
using Innoactive.Creator.Core.Utils;
using UnityEngine;

namespace Innoactive.Creator.Core
{
    public static class PropertyReflectionHelper
    {

        public static List<LockablePropertyReference> ExtractLockablesFromStep(IStep step)
        {
            List<LockablePropertyReference> result = new List<LockablePropertyReference>();

            foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
            {
                foreach (ICondition condition in transition.Data.Conditions)
                {
                    result.AddRange(ExtractLockablePropertiesFromConditions(condition.Data));
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts all scene and property references which have extend a LockableProperty.
        /// </summary>
        public static List<LockablePropertyReference> ExtractLockablePropertiesFromConditions(IConditionData data)
        {
            List<MemberInfo> memberInfo = data.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(info =>
                    info.PropertyType.IsConstructedGenericType && info.PropertyType.GetGenericTypeDefinition() ==
                    typeof(ScenePropertyReference<>))
                .Cast<MemberInfo>()
                .ToList();

            memberInfo.AddRange(data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(info =>
                    info.FieldType.IsConstructedGenericType && info.FieldType.GetGenericTypeDefinition() ==
                    typeof(ScenePropertyReference<>)));

            List<LockablePropertyReference> result = new List<LockablePropertyReference>();
            memberInfo.ForEach(info =>
            {
                UniqueNameReference reference =
                    (UniqueNameReference) ReflectionUtils.GetValueFromPropertyOrField(data, info);

                Type refType = ReflectionUtils
                    .GetConcreteImplementationsOf(reference.GetReferenceType())
                    .Where(typeof(LockableProperty).IsAssignableFrom)
                    .FirstOrDefault(type => Enumerable.All(type.Assembly.GetReferencedAssemblies(),
                        assemblyName => assemblyName.Name != "UnityEditor" && assemblyName.Name != "nunit.framework"));

                if (refType != null)
                {
                    result.Add(new LockablePropertyReference(GetProperty(reference, refType)));
                }
            });

            return result;
        }

        private static LockableProperty GetProperty(UniqueNameReference reference, Type type)
        {
            ISceneObject sceneObject = RuntimeConfigurator.Configuration.SceneObjectRegistry.GetByName(reference.UniqueName);
            foreach (ISceneObjectProperty prop in sceneObject.Properties)
            {
                if (prop.GetType() == type)
                {
                    return (LockableProperty)prop;
                }
            }
            Debug.LogWarningFormat("Could not find fitting {0} type in SceneObject {1}", type.Name, reference.UniqueName);
            return null;
        }
    }
}
