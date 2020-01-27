using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Common.Logging;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.Unity.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Innoactive.Hub.Training.Editors.Configuration
{
    /// <summary>
    /// Default editor configuration definition which is used if no other was implemented.
    /// </summary>
    public class DefaultEditorConfiguration : IEditorConfiguration
    {
        private static readonly ILog logger = LogManager.GetLogger<DefaultEditorConfiguration>();

        private AllowedMenuItemsSettings allowedMenuItemsSettings;

        public static readonly string[] PrefabsToDestroy = new[]
        {
            "[HUB-BOOTSTRAP]",
            "[HUB-BUG-LOGGING]",
            "[HUB-DEBUG-UI]",
            "[HUB-LOGIN-CHECK]",
            "[HUB-MENU-SETUP]",
            "[HUB-MULTIUSER]",
            "[HUB-PERSISTENCE]",
            "[HUB-PLAYER-SETUP-MANAGER]",
            "[HUB-MAINTHREAD-DISPATCHER]",
            "[HUB-VR-LAUNCHER-CLIENT]"
        };


        /// <inheritdoc />
        public virtual string DefaultCourseStreamingAssetsFolder
        {
            get { return "Training"; }
        }

        /// <inheritdoc />
        public virtual string AllowedMenuItemsSettingsAssetPath
        {
            get { return null; }
        }

        /// <inheritdoc />
        public virtual AllowedMenuItemsSettings AllowedMenuItemsSettings
        {
            get
            {
                if (allowedMenuItemsSettings == null)
                {
                    allowedMenuItemsSettings = AllowedMenuItemsSettings.Load();
                }

                return allowedMenuItemsSettings;
            }
            set { allowedMenuItemsSettings = value; }
        }

        protected DefaultEditorConfiguration()
        {
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<Menu.Option<IBehavior>> BehaviorsMenuContent
        {
            get { return AllowedMenuItemsSettings.GetBehaviorMenuOptions().Cast<Menu.Option<IBehavior>>().ToList().AsReadOnly(); }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<Menu.Option<ICondition>> ConditionsMenuContent
        {
            get { return AllowedMenuItemsSettings.GetConditionMenuOptions().Cast<Menu.Option<ICondition>>().ToList().AsReadOnly(); }
        }
        
        /// <inheritdoc />
        public virtual void SetupTrainingScene()
        {
            List<GameObject> sceneObjects = SceneUtils.GetActiveAndInactiveGameObjects().ToList();

            // Delete duplicated deactivated Hub SDK game objects.
            IEnumerable<GameObject> oldPrefabs = sceneObjects.Where(obj => obj != null && PrefabsToDestroy.Contains(obj.name));

            if (oldPrefabs.Any() && EditorUtility.DisplayDialog("Outdated Prefabs found", "Prefabs from the Hub-SDK detected inside of your scene, do you want to delete them?", "Delete", "Cancel"))
            {
                foreach (GameObject oldPrefab in oldPrefabs)
                {
                    Object.DestroyImmediate(oldPrefab);
                }
            }

            if (File.Exists("hub-logging-config.json") == false)
            {
                TextAsset configFile = Resources.Load<TextAsset>("hub-logging-config");
                if (configFile != null)
                {
                    File.WriteAllText("hub-logging-config.json", configFile.text);
                    logger.Info("Copied logging config");
                }
            }

            SetupVRTK();

            // Create default save folder.
            Directory.CreateDirectory(DefaultCourseStreamingAssetsFolder);

            // Initialize training configuration.
            SceneUtils.SetupTrainingConfiguration();

            logger.Info("Scene setup is complete.");
        }

        private void SetupVRTK()
        {
            GameObject camera = GameObject.Find("Main Camera");
            if (camera != null)
            {
                GameObject.DestroyImmediate(camera);
            }

            GameObject vrtkSetup = GameObject.Find("[VRTK_Setup]");
            if (vrtkSetup == null)
            {
                GameObject prefab = (GameObject)GameObject.Instantiate(Resources.Load("[VRTK_Setup]", typeof(GameObject)));
                prefab.name = prefab.name.Replace("(Clone)", "");
            }
        }
    }
}