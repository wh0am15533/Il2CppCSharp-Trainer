#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BepInEx;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using BepInEx.IL2CPP.UnityEngine; //For UnityEngine.Input

namespace Trainer
{
    public class Menu : MonoBehaviour
    {
        #region[Declarations]

        private string baseDirectory = Environment.CurrentDirectory;
        private Rect MainWindow;
        private bool MainWindowVisible = true;

        #endregion

        private void Start()
        {
            using (TextWriter textWriter = File.AppendText("TestTrainer.txt"))
            {
                textWriter.WriteLine("[Trainer] Start()");
                textWriter.Flush();
            }

            #region[Window Definitions - Don't Touch]
            MainWindow = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 250, 250f, 50f);
            #endregion

        }
        /*
        public void Update()
        {
            using (TextWriter textWriter = File.AppendText("TestTrainer.txt"))
            {
                textWriter.WriteLine("[Trainer] Update()");
                textWriter.Flush();
            }

            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                this.MainWindowVisible = !this.MainWindowVisible;
            }

        }

        private void OnGUI()
        {
            using (TextWriter textWriter = File.AppendText("TestTrainer.txt"))
            {
                textWriter.WriteLine("[Trainer] OnGUI()");
                textWriter.Flush();
            }

            if (!MainWindowVisible)
                return;

            if (Event.current.type == EventType.Layout)
            {
                GUI.backgroundColor = Color.black;
                GUIStyle titleStyle = new GUIStyle(GUI.skin.window);
                titleStyle.normal.textColor = Color.green;

                //MAIN WINDOW
                MainWindow = new Rect(MainWindow.x, MainWindow.y, 250f, 50f);
                MainWindow = GUILayout.Window(0, MainWindow, new GUI.WindowFunction(RenderUI), "TestTrainer v1", titleStyle, new GUILayoutOption[0]);
            }
        }

        private void RenderUI(int id)
        {
            using (TextWriter textWriter = File.AppendText("TestTrainer.txt"))
            {
                textWriter.WriteLine("[Trainer] RenderUI()");
                textWriter.Flush();
            }

            #region[Styles]
            GUIStyle labelStyleGreen = new GUIStyle();
            labelStyleGreen.normal.textColor = Color.green;
            labelStyleGreen.alignment = TextAnchor.MiddleCenter;

            GUIStyle labelStyleCyan = new GUIStyle();
            labelStyleCyan.normal.textColor = Color.cyan;
            labelStyleCyan.alignment = TextAnchor.MiddleCenter;

            GUIStyle labelStyleWhite = new GUIStyle();
            labelStyleWhite.normal.textColor = Color.white;
            labelStyleWhite.alignment = TextAnchor.MiddleCenter;
            #endregion

            switch (id)
            {
                #region[Main Window]

                case 0:
                    #region[Windows Header]
                    GUI.color = Color.green;
                    GUILayout.Label("Show/Hide: Backslash", labelStyleGreen, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    #endregion

                    GUI.color = Color.white;
                    if (GUILayout.Button("Test", new GUILayoutOption[0]))
                    {
                        
                    }

                    break;

                #endregion

                #region[Default - Nothing]
                default:
                    break;
                #endregion
            }

            GUI.DragWindow();
        }
        */
    }
}
#endif