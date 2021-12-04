using UnityEngine;
using UnityEditor;

namespace ParrelSync
{
    /// <summary>
    /// To add value caching for <see cref="EditorPrefs"/> functions
    /// </summary>
    public class BoolPreference
    {
        public string key { get; private set; }
        public bool defaultValue { get; private set; }
        public BoolPreference(string key, bool defaultValue)
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }

        private bool? valueCache = null;

        public bool Value
        {
            get
            {
                if (valueCache == null)
                    valueCache = EditorPrefs.GetBool(key, defaultValue);

                return (bool)valueCache;
            }
            set
            {
                if (valueCache == value)
                    return;

                EditorPrefs.SetBool(key, value);
                valueCache = value;
            }
        }

        public void ClearValue()
        {
            EditorPrefs.DeleteKey(key);
            valueCache = null;
        }
    }

    public class StringPreference {
        public string key { get; private set; }
        public string defaultValue { get; private set; }
        public StringPreference(string key, string defaultValue) {
            this.key = key;
            this.defaultValue = defaultValue;
        }

        private string valueCache = null;

        public string Value {
            get {
                if ( valueCache == null )
                    valueCache = EditorPrefs.GetString(key, defaultValue);

                return valueCache;
            }
            set {
                if ( valueCache == value )
                    return;

                EditorPrefs.SetString(key, value);
                valueCache = value;
            }
        }

        public void ClearValue() {
            EditorPrefs.DeleteKey(key);
            valueCache = null;
        }
    }

    public class Preferences : EditorWindow
    {
        [MenuItem("Build/Cloning/Preferences", priority = 1)]
        public static void InitWindow()
        {
            Preferences window = (Preferences)EditorWindow.GetWindow(typeof(Preferences));
            window.titleContent = new GUIContent("Cloning Preferences");
            window.Show();
        }

        public static StringPreference ClonesDirPref = new StringPreference("ParrelSync_ClonesDirectory", null);

        /// <summary>
        /// Disable asset saving in clone editors?
        /// </summary>
        public static BoolPreference AssetModPref = new BoolPreference("ParrelSync_DisableClonesAssetSaving", true);

        /// <summary>
        /// In addition of checking the existence of UnityLockFile, 
        /// also check is the is the UnityLockFile being opened.
        /// </summary>
        public static BoolPreference AlsoCheckUnityLockFileStaPref = new BoolPreference("ParrelSync_CheckUnityLockFileOpenStatus", true);

        /// <summary>
        /// Whether project settings should be linked in clones. If false, they will be copied instead.
        /// </summary>
        public static BoolPreference LinkProjectSettingsPref = new BoolPreference("ParrelSync_LinkProjectSettings", false);

        private void OnGUI()
        {
            if (ClonesManager.IsClone())
            {
                EditorGUILayout.HelpBox(
                        "This is a clone project. Please use the original project editor to change preferences.",
                        MessageType.Info);
                return;
            }

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label("Preferences");
            GUILayout.BeginVertical("GroupBox");

            ClonesDirPref.Value = EditorGUILayout.TextField(
                new GUIContent(
                    "Clones Directory",
                    "Path to root directory where clones will be created. If empty, clones are created in the project's parent directory."
                ),
                ClonesDirPref.Value
            );

            AssetModPref.Value = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "(recommended) Disable asset saving in clone editors- require re-open clone editors",
                    "Disable asset saving in clone editors so all assets can only be modified from the original project editor"
                ),
                AssetModPref.Value
            );

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                AlsoCheckUnityLockFileStaPref.Value = EditorGUILayout.ToggleLeft(
                    new GUIContent(
                        "Also check UnityLockFile lock status while checking clone projects running status",
                        "Disabling this can slightly increase Clones Manager window performance, but will lead to in-correct clone project running status" +
                        "(the Clones Manager window show the clone project is still running even it's not) if the clone editor crashed"
                    ),
                    AlsoCheckUnityLockFileStaPref.Value
                );
            }

            LinkProjectSettingsPref.Value = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Link Project Settings in clones",
                    "If disabled, project settings are intead copied to cloned and therefore won't be synced after the initial clone"
                ),
                LinkProjectSettingsPref.Value
            );

            GUILayout.EndVertical();
            if (GUILayout.Button("Reset to default"))
            {
                AssetModPref.ClearValue();
                AlsoCheckUnityLockFileStaPref.ClearValue();
            }
            GUILayout.EndVertical();
        }
    }
}