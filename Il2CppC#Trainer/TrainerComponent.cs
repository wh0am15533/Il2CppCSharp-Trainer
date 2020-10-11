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
#if DEBUG
    internal delegate void getRootSceneObjects(int handle, IntPtr list);
#endif

    public class TrainerComponent : MonoBehaviour
    {
        #region[Declarations]

        private static GameObject levelManagerGO;
        private static mico.game.LevelManager levelManager = null;

#if DEBUG
        private static List<GameObject> _unitButtons = new List<GameObject>();
        private static UnitButtonManager unitButtonManager = null;
#endif

        #endregion

        public TrainerComponent(IntPtr ptr) : base(ptr)
        {
#if DEBUG
            BepInExLoader.log.LogMessage("[Trainer] TestComponent - Entered Constructor");
#endif
        }

        public static void Awake()
        {
#if DEBUG
            BepInExLoader.log.LogMessage("[Trainer] TestComponent - Entered Awake()");
#endif
        }

        public static void Start()
        {
#if DEBUG
            BepInExLoader.log.LogMessage("[Trainer] TestComponent - Entered Start()");
#endif
        }

        [HarmonyPostfix]
        public static void Update()
        {
#if DEBUG
            // Dump GameObjects
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.Backspace) && Event.current.type == EventType.KeyDown)
            {
                Event.current.Use();

                string dbg = "";
                foreach(GameObject obj in TrainerComponent.GetGameObjects())
                {
                    BepInExLoader.log.LogMessage("[GameObject]: " + obj.name);
                    dbg += "[GameObject]: " + obj.name + "\r\n";

                    #region[Get GameObject Components]

                    BepInExLoader.log.LogMessage("\t[Components]:");
                    dbg += "\t[Components]:\r\n";

                    var comps = obj.GetGameObjectComponents();
                    foreach(var comp in comps)
                    {
                        BepInExLoader.log.LogMessage("\t   " + comp.Name);
                        dbg += "\t\t" + comp.Name + "\r\n";
                    }

                    dbg += "\r\n";

                    #endregion

                    #region[Get Child Objects]
                    
                    BepInExLoader.log.LogMessage("\t[Children]:");
                    dbg += "\t[Children]:\r\n";

                    // Get Children
                    foreach (var child in obj.GetParentsChildren())
                    {
                        BepInExLoader.log.LogMessage("\t   " + child.name + ":");
                        dbg += "\t\t" + child.name + "\r\n";
                        
                        BepInExLoader.log.LogMessage("\t      [Components]:");
                        dbg += "\t\t\t[Components]:\r\n";
                        var childcomps = child.GetGameObjectComponents();
                        foreach(var childcomp in childcomps)
                        {
                            BepInExLoader.log.LogMessage("\t         " + childcomp.Name);
                            dbg += "\t\t\t\t" + childcomp.Name + "\r\n";
                        }

                        dbg += "\r\n";

                        // Get Grand Children
                        BepInExLoader.log.LogMessage("\t      [Grand Children]:");
                        dbg += "\t\t\t[Grand Children]:\r\n";

                        foreach (var grandchild in child.GetParentsChildren())
                        {
                            BepInExLoader.log.LogMessage("\t         " + grandchild.name + ":");
                            dbg += "\t\t\t\t" + grandchild.name + "\r\n";

                            BepInExLoader.log.LogMessage("\t            [Components]:");
                            dbg += "\t\t\t\t\t[Components]:\r\n";
                            var grandchildcomps = grandchild.GetGameObjectComponents();
                            foreach (var grandchildcomp in grandchildcomps)
                            {
                                BepInExLoader.log.LogMessage("\t               " + grandchildcomp.Name);
                                dbg += "\t\t\t\t\t\t" + grandchildcomp.Name + "\r\n";
                            }
                        }
                    }

                    dbg += "\r\n";
                    
                    #endregion
                }

                File.WriteAllText("C:\\Games\\Orcs Civil War\\TEST.txt", dbg);
            }
#endif

            #region[Orc's Cheats]

            if (levelManager == null)
            {
                levelManagerGO = GameObject.Find("LevelManager");
                if (levelManagerGO != null)
                {
                    levelManager = levelManagerGO.GetComponent<mico.game.LevelManager>();
                    //levelManager.levelData.unitButtonVisible = true;
                }
            }
            else
            {
                // Give Resources
                if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F12) && Event.current.type == EventType.KeyDown)
                {
                    levelManager.addResources(5000);
                    Event.current.Use();
                }

#if DEBUG
                levelManager.levelData.startResources = 1000;
                levelManager.levelData.enableResurrection = true;
                levelManager.levelData.resetResurrectionTime = 100.0f;
                levelManager.levelData.resetMeteorTime = 100.0f;
                levelManager.levelData.meteorIsReady = true;
                levelManager.levelData.resurrectionIsReady = true;
#endif
            }

            // Kill All
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F11) && Event.current.type == EventType.KeyDown)
            {
                GameObject UnitManagerGO = GameObject.Find("UnitManager");

                if(UnitManagerGO != null)
                {
                    var comp = UnitManagerGO.GetComponent<UnitManager>();
                    comp.killAll();
                }

                Event.current.Use();
            }


            #region[Enable all Unit Types (It enables the objects but they still aren't clickable)]
            /*
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F11) && Event.current.type == EventType.KeyDown)
            {
                GameObject unitButtonsGO = GameObject.Find("UnitButton");
                if (unitButtonsGO != null)
                {
                    unitButtonManager = unitButtonsGO.GetComponent<UnitButtonManager>();

                    for (int idx = 0; idx < unitButtonsGO.transform.childCount; idx++)
                    {
                        var child = unitButtonsGO.transform.GetChild(idx);
                        _unitButtons.Add(child.gameObject);
                        BepInExLoader.log.LogMessage("[GameObject]: " + child.name + " IS_ACTIVE: " + child.gameObject.activeSelf.ToString());
                    }
                }

                Event.current.Use();
            }

            if (unitButtonManager != null)
            {
                foreach (var unitButton in _unitButtons)
                {
                    unitButtonManager.enableClick = true;

                    if (unitButton.name == "Button_Orc_Shaman" || unitButton.name == "Button_Orc_Catapult")
                    {
                        var comp = unitButton.GetComponent<UnitButton>();

                        comp.btn.enabled = true;
                        comp.btn.interactable = true;
                        comp.igonreCheck = true;
                        comp.isEnabled = true;
                        comp.show(0f);
                        unitButton.SetActive(true);
                    }
                }
            }
            */
            #endregion

            #endregion
        }

        #region[Unity Testing]
#if DEBUG
        private static List<GameObject> AllLoadedGameObjects = new List<GameObject>();
        internal static getRootSceneObjects getRootSceneObjects_iCall = IL2CPP.ResolveICall<getRootSceneObjects>("UnityEngine.SceneManagement.Scene::GetRootGameObjectsInternal");
        public static void GetRootGameObjects_Internal(Scene scene, IntPtr list)
        {
            getRootSceneObjects_iCall(scene.handle, list);
        }
        public static Il2CppSystem.Collections.Generic.List<GameObject> TestGetRootSceneGameObjects()
        {
            var scene = SceneManager.GetActiveScene();
            var list = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);

            GetRootGameObjects_Internal(scene, list.Pointer);

            return list;
        }

        public static List<GameObject> GetGameObjects()
        {
            foreach (var gobject in TestGetRootSceneGameObjects())
            {
                AllLoadedGameObjects.Add(gobject);
            }
            return AllLoadedGameObjects;
        }
#endif
    #endregion

    }

    #region[GameObjects Extensions]
#if DEBUG
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject flag) where T : Component
        {
            if (flag == null)
                return false;
            return flag.GetComponent<T>() != null;
        }

        public static List<GameObject> GetParentsChildren(this GameObject parent)
        {
            if (parent == null)
                return null;
            List<GameObject> tmp = new List<GameObject>();

            for (int idx = 0; idx < parent.transform.childCount; idx++)
            {
                tmp.Add(parent.transform.GetChild(idx).gameObject);
            }

            return tmp;
        }

        public static List<Il2CppSystem.Type> GetGameObjectComponents(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;
            List<Il2CppSystem.Type> tmp = new List<Il2CppSystem.Type>();

            var comps = gameObject.GetComponents<Component>();
            foreach (var comp in comps)
            {
                tmp.Add(comp.GetIl2CppType());
            }

            return tmp;
        }
    }
#endif
    #endregion
}

/*
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F11) && Event.current.type == EventType.KeyDown)
            {
                GameObject unitButtonsGO = GameObject.Find("UnitButton");
                if (unitButtonsGO != null)
                {
                    unitButtonManager = unitButtonsGO.GetComponent<UnitButtonManager>();

                    for (int idx = 0; idx < unitButtonsGO.transform.childCount; idx++)
                    {
                        var child = unitButtonsGO.transform.GetChild(idx);

                        if (child.name == "Button_Orc_Shaman")
                        {
                            var comp = child.gameObject.GetComponent<UnitButton>();

                            comp.btn.enabled = true;
                            comp.btn.interactable = true;
                            //comp.igonreCheck = true;
                            comp.isEnabled = true;
                            comp.show(0f);
                            child.gameObject.SetActive(true);

                            //unitButtonManager._currentUnitIndex = 6;
                            //unitButtonManager.activeBtnByIndex(6);
                            unitButtonManager.activeButton(comp, true);
                            unitButtonManager._isReadyToBuildUnit = true;
                            unitButtonManager.enableClick = true;
                        }

                        //BepInExLoader.log.LogMessage("[GameObject]: " + child.name + " IS_ACTIVE: " + child.gameObject.activeSelf.ToString());
                    }
                }

                Event.current.Use();
            }

*/
