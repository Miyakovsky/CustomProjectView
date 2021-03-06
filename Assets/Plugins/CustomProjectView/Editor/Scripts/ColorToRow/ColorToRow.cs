﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomProjectView.ColorToRow
{
    public class ColorToRow
    {
        public static void OnGUI(string guid, Rect selectionRect)
        {
            var index = (int)(selectionRect.y) / 16;

            if (index % 2 == 0)
            {
                return;
            }

            var pos = selectionRect;
            pos.x = 0;
            pos.xMax = selectionRect.xMax;

            var color = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.Box(pos, string.Empty);
            GUI.color = color;
        }
    }
}