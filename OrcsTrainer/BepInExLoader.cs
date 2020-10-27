
using System;
using BepInEx;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Trainer.UI;
using UnityEngine.EventSystems;

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

        public static TooltipGUI tooltip = null;

        public static Action t = null;

        #endregion

        public BepInExLoader()
        {
            Application.runInBackground = true;

            log = Log;
        }

        public override void Load()
        {
            #region[Register TrainerComponent in Il2Cpp]

            log.LogMessage("Registering Custom MonoBehaviors in Il2Cpp");

            try
            {
                // Register our custom Types in Il2Cpp
                ClassInjector.RegisterTypeInIl2Cpp<TrainerComponent>();
                ClassInjector.RegisterTypeInIl2Cpp<UIControls>();
                ClassInjector.RegisterTypeInIl2Cpp<WindowDragHandler>();
                ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            }
            catch
            {
                log.LogError("FAILED to Register Il2Cpp Type: TrainerComponent!");
            }

            #endregion

            #region[MonoMod Detour Testing]
            //Invalid IL code error because of Unhollower. Try with Mono.Cecil Reflection instead of System.Reflection. I know Unhollower creates runable, but 
            // executable assemblies, but if it can read it's own shit, why the error about it now??
            /*
            // Get Some MethodInfo's First
            var originalOnGUId = AccessTools.Method(typeof(I2.Loc.RealTimeTranslation), "OnGUI");
            var postOnGUId = AccessTools.Method(typeof(Trainer.UI.TooltipGUI), "OnGUI");

            // Create Detour
            log.LogMessage("Trying to Create OnGUI Detour...");
            try
            {
                MonoMod.RuntimeDetour.Detour d = new MonoMod.RuntimeDetour.Detour(originalOnGUId, postOnGUId);
                d.Apply();
                t = d.GenerateTrampoline<Action>(); // ERROR: MonoMod Detour - Invalid IL
            }
            catch(Exception ex)
            {
                log.LogError("ERROR Applying Detour: " + ex.Message);
            }
            */
            #endregion

            #region[Harmony Patching]

            try
            {
                log.LogMessage("Inserting Harmony Hooks...");

                var harmony = new Harmony("wh0am15533.trainer.il2cpp");

                #region[Primary Entry Hooks]

                /// Use USplashScreen for Testing Update() Hook (Orc's Civil War) just to make sure it works,
                /// then look for a Update() that always exist in the game. Here are some for Orc's:
                ///     USplashScreen loads right away, then stops after splash screen video
                ///     ObjRotation loads when you load a level
                ///     DigitalRubyShared.FingersScript loads when you load a level
                ///     mGameStudio.RTS.CameraController loads when you load a level
                ///     UnityEngine.UI.CanvasScaler loads right away and stays loaded as long as the game has a Canvas object
                ///

                var originalUpdate = AccessTools.Method(typeof(UnityEngine.UI.CanvasScaler), "Update");
                log.LogMessage("   Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
                var postUpdate = AccessTools.Method(typeof(TrainerComponent), "Update");
                log.LogMessage("   Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
                harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));

                #endregion

                #region[IBeginDragHandler, IDragHandler, IEndDragHandler Hooks]

                // IBeginDragHandler
                var originalOnBeginDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnBeginDrag");
                log.LogMessage("   Original Method: " + originalOnBeginDrag.DeclaringType.Name + "." + originalOnBeginDrag.Name);
                var postOnBeginDrag = AccessTools.Method(typeof(WindowDragHandler), "OnBeginDrag");
                log.LogMessage("   Postfix Method: " + postOnBeginDrag.DeclaringType.Name + "." + postOnBeginDrag.Name);
                harmony.Patch(originalOnBeginDrag, postfix: new HarmonyMethod(postOnBeginDrag));

                // IDragHandler
                var originalOnDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnDrag");
                log.LogMessage("   Original Method: " + originalOnDrag.DeclaringType.Name + "." + originalOnDrag.Name);
                var postOnDrag = AccessTools.Method(typeof(WindowDragHandler), "OnDrag");
                log.LogMessage("   Postfix Method: " + postOnDrag.DeclaringType.Name + "." + postOnDrag.Name);
                harmony.Patch(originalOnDrag, postfix: new HarmonyMethod(postOnDrag));

                // IEndDragHandler
                var originalOnEndDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnEndDrag");
                log.LogMessage("   Original Method: " + originalOnEndDrag.DeclaringType.Name + "." + originalOnEndDrag.Name);
                var postOnEndDrag = AccessTools.Method(typeof(WindowDragHandler), "OnEndDrag");
                log.LogMessage("   Postfix Method: " + postOnEndDrag.DeclaringType.Name + "." + postOnEndDrag.Name);
                harmony.Patch(originalOnEndDrag, postfix: new HarmonyMethod(postOnEndDrag));

                #endregion

                #region[OnGUI Hook]

                // OnGUI can be the hardest event to find, it get's stripped often, after most of the OnGUI's you find will crash the game.
                // In Orc's this is the only one that work's. See TrainerComponent->Initialize() for how we have to use it since the 
                // game doesn't neve loads it. Another issue is that the one you hook will often have it's own OnGUI stuff, so you have to
                // hide it or replace the method instead of just hooking it.
                /*
                var originalOnGUI = AccessTools.Method(typeof(I2.Loc.RealTimeTranslation), "OnGUI");
                log.LogMessage("   Original Method: " + originalOnGUI.DeclaringType.Name + "." + originalOnGUI.Name);
                var postOnGUI = AccessTools.Method(typeof(Trainer.UI.TooltipGUI), "OnGUI");
                log.LogMessage("   Postfix Method: " + postOnGUI.DeclaringType.Name + "." + postOnGUI.Name);
                harmony.Patch(originalOnGUI, postfix: new HarmonyMethod(postOnGUI));
                */
                #endregion

                log.LogMessage("Runtime Hooks's Applied");

            }
            catch
            {
                log.LogError("FAILED to Apply Hooks's!");
            }

            #endregion

            log.LogMessage("Initializing Il2CppTypeSupport..."); // Helps with AssetBundles
            Il2CppTypeSupport.Initialize();

            #region[Create our Object and Add our Trainer Component]

            var go = new GameObject(
                "Trainer",
                new Il2CppSystem.Type[]
                {
                    Il2CppType.Of<TrainerComponent>(),
                    Il2CppType.Of<Trainer.UI.TooltipGUI>()
                }
            ); 
            Object.DontDestroyOnLoad(go);

            #endregion

        }
    }
}
