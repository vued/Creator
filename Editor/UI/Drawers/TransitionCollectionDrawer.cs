﻿using System;
using System.Reflection;
using Innoactive.Creator.Core;
using Innoactive.CreatorEditor.UI.Drawers;
using UnityEngine;

namespace Innoactive.CreatorEditor.UI.Drawers
{
    [DefaultTrainingDrawer(typeof(TransitionCollection))]
    public class TransitionCollectionDrawer : DataOwnerDrawer
    {
        public override GUIContent GetLabel(MemberInfo memberInfo, object memberOwner)
        {
            return null;
        }

        public override GUIContent GetLabel(object value, Type declaredType)
        {
            return null;
        }
    }
}