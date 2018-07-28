using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Borodar.ReorderableList;

namespace CustomProjectView.CustomFolderIcon
{
    [CustomEditor(typeof(FolderIconDataSetting))]
    public class FolderIconDataSettingEditor : Editor
    {
        private const string PROP_NAME_FOLDERS = "Folders";

        private SerializedProperty _foldersProperty;

        protected void OnEnable()
        {
            _foldersProperty = serializedObject.FindProperty(PROP_NAME_FOLDERS);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ReorderableListGUI.Title("Custom Folder Icon");
            ReorderableListGUI.ListField(_foldersProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
