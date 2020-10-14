using BepInEx;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Trainer
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class BepInExLoader : BepInEx.IL2CPP.BasePlugin
    {
        #region[Declarations]
        
        public const string
            MODNAME = "Trainer",
            AUTHOR = "wh0am15533",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0.0";
        
        public static BepInEx.Logging.ManualLogSource log;

        #endregion

        public BepInExLoader()
        {
            log = Log;
        }

        public override void Load()
        {
            #region[Register TrainerComponent in Il2Cpp]

            log.LogMessage("[Trainer] Registering TrainerComponent in Il2Cpp");

            try
            {
                // Register our custom Types in Il2Cpp
                ClassInjector.RegisterTypeInIl2Cpp<WindowDragHandler>();
                ClassInjector.RegisterTypeInIl2Cpp<TrainerComponent>();

                var go = new GameObject("TrainerObject");                
                go.AddComponent<TrainerComponent>();
                Object.DontDestroyOnLoad(go);
            }
            catch
            {
                log.LogError("[Trainer] FAILED to Register Il2Cpp Type: TrainerComponent!");
            }

            #endregion

            #region[Harmony Patching]

            try
            {
                var harmony = new Harmony("wh0am15533.trainer.il2cpp");

                #region[Primary Entry Hook]

                /// Use USplashScreen for Testing Update() Hook (Orc's Civil War) just to make sure it works,
                /// then look for a Update() that always exist in the game. Here are some for Orc's:
                ///     USplashScreen loads right away, then stops after splash screen video
                ///     ObjRotation loads when you load a level
                ///     DigitalRubyShared.FingersScript loads when you load a level
                ///     mGameStudio.RTS.CameraController loads when you load a level
                ///     UnityEngine.UI.CanvasScaler loads right away and stays loaded as long as the game has a Canvas object
                ///

                var original = AccessTools.Method(typeof(UnityEngine.UI.CanvasScaler), "Update");
                log.LogMessage("[Trainer] Harmony - Original Method: " + original.DeclaringType.Name + "." + original.Name);
                var post = AccessTools.Method(typeof(TrainerComponent), "Update");
                log.LogMessage("[Trainer] Harmony - Postfix Method: " + post.DeclaringType.Name + "." + post.Name);
                harmony.Patch(original, postfix: new HarmonyMethod(post));

                #endregion

                #region[IBeginDragHandler, IDragHandler, IEndDragHandler Hooks]

                // IBeginDragHandler
                var original2 = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnBeginDrag");
                log.LogMessage("[Trainer] Harmony - Original2 Method: " + original2.DeclaringType.Name + "." + original2.Name);
                var post2 = AccessTools.Method(typeof(WindowDragHandler), "OnBeginDrag");
                log.LogMessage("[Trainer] Harmony - Postfix2 Method: " + post2.DeclaringType.Name + "." + post2.Name);
                harmony.Patch(original2, postfix: new HarmonyMethod(post2));

                // IDragHandler
                var original3 = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnDrag");
                log.LogMessage("[Trainer] Harmony - Original3 Method: " + original3.DeclaringType.Name + "." + original3.Name);
                var post3 = AccessTools.Method(typeof(WindowDragHandler), "OnDrag");
                log.LogMessage("[Trainer] Harmony - Postfix3 Method: " + post3.DeclaringType.Name + "." + post3.Name);
                harmony.Patch(original3, postfix: new HarmonyMethod(post3));

                // IEndDragHandler
                var original4 = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnEndDrag");
                log.LogMessage("[Trainer] Harmony - Original4 Method: " + original4.DeclaringType.Name + "." + original4.Name);
                var post4 = AccessTools.Method(typeof(WindowDragHandler), "OnEndDrag");
                log.LogMessage("[Trainer] Harmony - Postfix4 Method: " + post4.DeclaringType.Name + "." + post4.Name);
                harmony.Patch(original4, postfix: new HarmonyMethod(post4));

                #endregion

                log.LogMessage("[Trainer] Harmony - Runtime Patch's Applied");
            }
            catch
            {
                log.LogError("[Trainer] Harmony - FAILED to Apply Patch's!");
            }

            #endregion
        }

    }
}


#region[Harmony Patches]
/*
public static class HarmonyPatches
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        bool isFiring = false;

        //BepInExLoader.log.LogMessage("[Trainer] Entered Hooked Update()");

        if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.Backspace) && !isFiring)
        {
            isFiring = true;

            Console.WriteLine("[Trainer] TestComponent - Update() Keypress");

            File.WriteAllText("C:\\Games\\Orcs Civil War\\TEST.txt", TrainerComponent.GetGameObjects());

            isFiring = false;
        }
    }
}
*/
#endregion