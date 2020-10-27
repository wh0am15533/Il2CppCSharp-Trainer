
using System;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// Attach to Empty GameObject.

namespace Trainer.UI
{
    public class TooltipGUI : MonoBehaviour
    {
        public static TooltipGUI instance = null;
        public static bool EnableTooltip = true;
        public static string Tooltip = "";
        private static GUIStyle tooltipStyle;

        public TooltipGUI(IntPtr ptr) : base(ptr)
        {
            BepInExLoader.log.LogMessage("TooltipGUI Loaded");
            instance = this;
        }

        [HarmonyPostfix]
        public static void OnGUI()
        {
            BepInExLoader.log.LogMessage("OnGUI Fired!");
            
            if (Tooltip != "" && EnableTooltip == true)
            {
                GUI.backgroundColor = Color.black;
                GUIContent content = new GUIContent(Tooltip);

                tooltipStyle = new GUIStyle(GUI.skin.box);
                tooltipStyle.normal.textColor = Color.white;

                float width = tooltipStyle.CalcSize(content).x;
                float height = tooltipStyle.CalcSize(content).y;

                var mousepos = UnityEngine.Input.mousePosition;
                //var mousepos = EventSystem.current.currentInputModule.input.mousePosition; // Instead of Input.mousePosition
                GUI.Box(new Rect(mousepos.x + 15, Screen.height - mousepos.y + 15, width, 25), content, tooltipStyle); // The +15 are cursor offsets
                
            }
            
        }
    }
}
