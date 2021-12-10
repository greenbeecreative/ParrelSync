using UnityEditor;
using UnityEngine;
namespace ParrelSync
{
    /// <summary>
    /// For preventing assets being modified from the clone instance.
    /// </summary>
    public class ParrelSyncAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (ClonesManager.IsClone() && Preferences.AssetModPref.Value)
            {
                return new string[0] { };
            }
            return paths;
        }
    }
}