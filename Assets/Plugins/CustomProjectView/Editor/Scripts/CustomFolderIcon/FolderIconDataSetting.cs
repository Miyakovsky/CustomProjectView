using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomProjectView.CustomFolderIcon
{
    public class FolderIconDataSetting : ScriptableObject
    {
        private const string RELATIVE_PATH = "Editor/Data/RainbowFoldersSettings.asset";
        private const string DEVEL_PATH = "Assets/Plugins/CustomProjectView/Editor/Data/CustomFolderIcon/CustomFolderIconSetting.asset";

        public List<FolderIconData> Folders;

        private static FolderIconDataSetting _instance;

        public static FolderIconDataSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = AssetDatabase.LoadAssetAtPath<FolderIconDataSetting>(DEVEL_PATH);

                return _instance;
            }
        }

        /// <summary>
        /// Searches for a folder config that should be applied for the specified path (regardless of
        /// the key type). Returns the last occurrence within the settings, if found; null otherwise.
        /// </summary>  
        public FolderIconData GetFolderByPath(string folderPath, bool allowRecursive = false)
        {
            if (IsNullOrEmpty(Folders)) return null;

            for (var index = Folders.Count - 1; index >= 0; index--)
            {
                var folder = Folders[index];
                switch (folder.Type)
                {
                    case FolderIconData.KeyType.Name:
                        var folderName = System.IO.Path.GetFileName(folderPath);
                        if (allowRecursive && folder.IsRecursive)
                        {
                            if (folderPath.Contains(string.Format("/{0}/", folder.Key))) return folder;
                        }
                        else
                        {
                            if (folder.Key.Equals(folderName)) return folder;
                        }
                        break;
                    case FolderIconData.KeyType.Path:
                        if (allowRecursive && folder.IsRecursive)
                        {
                            if (folderPath.StartsWith(folder.Key)) return folder;
                        }
                        else
                        {
                            if (folder.Key.Equals(folderPath)) return folder;
                        }
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            return null;
        }


        public Texture2D GetFolderIcon(string folderPath, bool small = true)
        {
            var folder = GetFolderByPath(folderPath, true);
            if (folder == null) return null;

            return small ? folder.SmallIcon : folder.LargeIcon;
        }

        private static bool IsNullOrEmpty(ICollection collection)
        {
            return collection == null || (collection.Count == 0);
        }
    }
}
