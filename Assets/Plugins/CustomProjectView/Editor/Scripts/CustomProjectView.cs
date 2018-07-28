using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CustomProjectView
{
    [InitializeOnLoad]
    public class CustomProjectView
    {
        static CustomProjectView()
        {
            EditorApplication.projectWindowItemOnGUI += OnGUI;

            var assembly = typeof(EditorApplication).Assembly;

        }

        private static void OnGUI(string guid, Rect selectionRect)
        {
            CustomFolderIcon.CustomFolderIcon.OnGUI(guid, selectionRect);



        }
    }
}
