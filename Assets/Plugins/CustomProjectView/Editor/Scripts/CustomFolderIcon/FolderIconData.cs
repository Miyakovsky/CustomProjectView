using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomProjectView.CustomFolderIcon
{
    [Serializable]
    public class FolderIconData
    {
        public enum KeyType
        {
            Name,
            Path
        }

        public KeyType Type;
        public string Key;
        public bool IsRecursive;

        public Texture2D SmallIcon;
        public Texture2D LargeIcon;
    }
}
