using System;
using System.Collections.Generic;
using System.Linq;
using Innoactive.Hub.Training.Utils.Serialization;
using UnityEditor;
using UnityEngine;

namespace Innoactive.Hub.Training.Editors.InnoactiveMenu
{
    /// <summary>
    /// Allows user to select which serializer they want to use.
    /// </summary>
    public class ChooseSerializerPopup : EditorWindow
    {
        private static ChooseSerializerPopup instance;

        private List<ITrainingSerializer> serializer;
        private string[] names;

        private int selected = 0;
        private Action<ITrainingSerializer> closeAction;

        /// <summary>
        /// Show the popup.
        /// </summary>
        /// <param name="serializer">Selectable serializer</param>
        /// <param name="closeAction">Action which will be invoked when closed successfully.</param>
        public static void Show(List<ITrainingSerializer> serializer, Action<ITrainingSerializer> closeAction)
        {
            if (instance != null)
            {
                instance.Close();
            }

            instance = CreateInstance<ChooseSerializerPopup>();
            instance.serializer = serializer;
            instance.closeAction = closeAction;

            Rect position = new Rect(0, 0, 320, 92);
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            instance.position = position;

            instance.ShowPopup();
            instance.Focus();
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
            {
                Close();
                instance = null;
            }

            if (names == null)
            {
                names = serializer.Select(t => t.Name).ToArray();
            }

            EditorGUILayout.LabelField("Select the serializer you want to use:");
            selected = EditorGUILayout.Popup("", selected, names);

            if (GUILayout.Button("Import"))
            {
                closeAction.Invoke(serializer[selected]);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }
}
