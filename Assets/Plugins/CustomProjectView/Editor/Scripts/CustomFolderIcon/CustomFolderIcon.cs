﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace CustomProjectView.CustomFolderIcon
{
    [InitializeOnLoad]
    public class CustomFolderIcon
    {
        private const float LARGE_ICON_SIZE = 64f;

        private static Func<bool> _isCollabEnabled;
        private static Func<bool> _isVcsEnabled;
        private static EditorApplication.ProjectWindowItemCallback _drawCollabOverlay;
        private static EditorApplication.ProjectWindowItemCallback _drawVcsOverlay;
        private static bool _multiSelection;

        static CustomFolderIcon()
        {
            var assembly = typeof(EditorApplication).Assembly;
            InitCollabDelegates(assembly);
            InitVcsDelegates(assembly);
        }

        public static void OnGUI(string guid, Rect selectionRect)
        {

            ReplaceFolderIcon(guid, selectionRect);
            //DrawEditIcon(guid, selectionRect);


        }

        private static void ReplaceFolderIcon(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;


            var isSmall = IsIconSmall(ref rect);

            var setting = FolderIconDataSetting.Instance;
            if (setting == null) return;
            var texture = setting.GetFolderIcon(path, isSmall);
            if (texture == null) return;

            DrawCustomIcon(guid, rect, texture, isSmall);
        }

        private static void DrawEditIcon(string guid, Rect rect)
        {
            //if ((Event.current.modifiers & RainbowFoldersPreferences.ModifierKey) == EventModifiers.None)
            //{
            //    _multiSelection = false;
            //    return;
            //}

            //var isSmall = IsIconSmall(ref rect);
            //var isMouseOver = rect.Contains(Event.current.mousePosition);
            //_multiSelection = (IsSelected(guid)) ? isMouseOver || _multiSelection : !isMouseOver && _multiSelection;

            //// if mouse is not over current folder icon or selected group
            //if (!isMouseOver && (!IsSelected(guid) || !_multiSelection)) return;

            //var path = AssetDatabase.GUIDToAssetPath(guid);
            //if (!AssetDatabase.IsValidFolder(path)) return;

            //var editIcon = RainbowFoldersEditorUtility.GetEditFolderIcon(isSmall);
            //DrawCustomIcon(guid, rect, editIcon, isSmall);

            //if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            //{
            //    ShowPopupWindow(rect, path);
            //}

            //EditorApplication.RepaintProjectWindow();
        }

        private static void InitVcsDelegates(Assembly assembly)
        {
            try
            {
                _isVcsEnabled = () => Provider.isActive;

                var vcsHookType = assembly.GetType("UnityEditorInternal.VersionControl.ProjectHooks");
                var vcsHook = vcsHookType.GetMethod("OnProjectWindowItem", BindingFlags.Static | BindingFlags.Public);
                _drawVcsOverlay = (EditorApplication.ProjectWindowItemCallback)Delegate.CreateDelegate(typeof(EditorApplication.ProjectWindowItemCallback), vcsHook);
            }
            catch (SystemException ex)
            {
                if (!(ex is NullReferenceException) && !(ex is ArgumentNullException)) throw;
                _isVcsEnabled = () => false;

#if RAINBOW_FOLDERS_DEVEL
                    Debug.LogException(ex);
#endif
            }
        }

        private static void InitCollabDelegates(Assembly assembly)
        {
            try
            {
                var collabAccessType = assembly.GetType("UnityEditor.Web.CollabAccess");
                var collabAccessInstance = collabAccessType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
                var collabAccessMethod = collabAccessInstance.GetType().GetMethod("IsServiceEnabled", BindingFlags.Instance | BindingFlags.Public);
                _isCollabEnabled = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), collabAccessInstance, collabAccessMethod);

                var collabHookType = assembly.GetType("UnityEditor.Collaboration.CollabProjectHook");
                var collabHook = collabHookType.GetMethod("OnProjectWindowItemIconOverlay", BindingFlags.Static | BindingFlags.Public);
                _drawCollabOverlay = (EditorApplication.ProjectWindowItemCallback)Delegate.CreateDelegate(typeof(EditorApplication.ProjectWindowItemCallback), collabHook);
            }
            catch (SystemException ex)
            {
                if (!(ex is NullReferenceException) && !(ex is ArgumentNullException)) throw;
                _isCollabEnabled = () => false;

#if RAINBOW_FOLDERS_DEVEL
                    Debug.LogException(ex);
#endif
            }
        }

        private static void ShowPopupWindow(Rect rect, string path)
        {
            //var window = RainbowFoldersPopup.GetDraggableWindow();
            //var position = GUIUtility.GUIToScreenPoint(rect.position + new Vector2(0, rect.height + 2));

            //if (_multiSelection)
            //{
            //    // ReSharper disable once RedundantTypeArgumentsOfMethod
            //    var paths = Selection.assetGUIDs
            //        .Select<string, string>(AssetDatabase.GUIDToAssetPath)
            //        .Where(AssetDatabase.IsValidFolder).ToList();

            //    var index = paths.IndexOf(path);
            //    window.ShowWithParams(position, paths, index);
            //}
            //else
            //{
            //    window.ShowWithParams(position, new List<string> { path }, 0);
            //}
        }


        private static void DrawCustomIcon(string guid, Rect rect, Texture texture, bool isSmall)
        {
            if (rect.width > LARGE_ICON_SIZE)
            {
                // center the icon if it is zoomed
                var offset = (rect.width - LARGE_ICON_SIZE) / 2f;
                rect = new Rect(rect.x + offset, rect.y + offset, LARGE_ICON_SIZE, LARGE_ICON_SIZE);
            }
            else
            {
                // unity shifted small icons a bit in 5.5
#if UNITY_5_5
                    if (isSmall) rect = new Rect(rect.x + 3, rect.y, rect.width, rect.height);
#elif UNITY_5_6_OR_NEWER
                if (isSmall && !IsTreeView(rect))
                    rect = new Rect(rect.x + 3, rect.y, rect.width, rect.height);
#endif
            }

            if (_isCollabEnabled())
            {
                //var background = RainbowFoldersEditorUtility.GetCollabBackground(isSmall, EditorGUIUtility.isProSkin);
                //GUI.DrawTexture(rect, background);
                GUI.DrawTexture(rect, texture);
                _drawCollabOverlay(guid, rect);
            }
            else if (_isVcsEnabled())
            {
                var iconRect = (!isSmall) ? rect : new Rect(rect.x + 7, rect.y, rect.width, rect.height);
                GUI.DrawTexture(iconRect, texture);
                _drawVcsOverlay(guid, rect);
            }
            {
                GUI.DrawTexture(rect, texture);
            }
        }


        private static bool IsIconSmall(ref Rect rect)
        {
            var isSmall = rect.width > rect.height;

            if (isSmall)
                rect.width = rect.height;
            else
                rect.height = rect.width;

            return isSmall;
        }

        private static bool IsTreeView(Rect rect)
        {
            return (rect.x - 16) % 14 == 0;
        }

        private static bool IsSelected(string guid)
        {
            return Selection.assetGUIDs.Contains(guid);
        }
    }
}
