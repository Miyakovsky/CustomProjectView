using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CustomProjectView
{
    [InitializeOnLoad]
    public class CustomProjectView : EditorWindow
    {
        private static bool _isCustomFolderIconFlag = true;
        private static bool _isCustomFolderIconSetting = true;

        [MenuItem("Tools/CustomProjectView")]
        static void Open()
        {
            _isCustomFolderIconSetting = _isCustomFolderIconFlag;

            GetWindow<CustomProjectView>();
        }

        void OnGUI()
        {

            EditorGUILayout.LabelField("カスタムプロジェクトビュー設定");
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                _isCustomFolderIconSetting = EditorGUILayout.Toggle("フォルダアイコンの適応", _isCustomFolderIconSetting);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("設定"))
            {
                _isCustomFolderIconFlag = _isCustomFolderIconSetting;
                Close();
            }
        }


        /// <summary>
        /// 初期化
        /// </summary>
        static CustomProjectView()
        {
            EditorApplication.projectWindowItemOnGUI += OnGUI;

            var assembly = typeof(EditorApplication).Assembly;

        }


        /// <summary>
        /// プロジェクトビューの表示追加更新
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="selectionRect"></param>
        private static void OnGUI(string guid, Rect selectionRect)
        {
            if(_isCustomFolderIconFlag)
            {
                CustomFolderIcon.CustomFolderIcon.OnGUI(guid, selectionRect);
            }
        }
    }
}
