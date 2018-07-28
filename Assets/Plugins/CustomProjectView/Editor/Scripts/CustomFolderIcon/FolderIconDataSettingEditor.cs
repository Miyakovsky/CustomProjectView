using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
//using Borodar.ReorderableList;

namespace CustomProjectView.CustomFolderIcon
{
    [CustomEditor(typeof(FolderIconDataSetting))]
    public class FolderIconDataSettingEditor : Editor
    {
        private const string PROP_NAME_FOLDERS = "Folders";
        private const int HeightSize = 90;

        ReorderableList _reorderableList;

        protected void OnEnable()
        {
            //リストの表示設定
            var prop = serializedObject.FindProperty(PROP_NAME_FOLDERS);
            _reorderableList = new ReorderableList(serializedObject, prop);

            _reorderableList.elementHeight = HeightSize;
            _reorderableList.drawElementCallback =
              (rect, index, isActive, isFocused) => {
                  var element = prop.GetArrayElementAtIndex(index);
                  EditorGUI.PropertyField(rect, element);
              };

            var defaultColor = GUI.backgroundColor;

            _reorderableList.drawHeaderCallback = (rect) =>
              EditorGUI.LabelField(rect, prop.displayName);
        }

        public override void OnInspectorGUI()
        {
            //表示更新
            serializedObject.Update();
            _reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
