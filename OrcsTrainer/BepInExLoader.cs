
using System;
using BepInEx;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Trainer.UI;

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
        public static Action t = null;

        #endregion

        public BepInExLoader()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;
            Application.runInBackground = true;
            log = Log;
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e) => log.LogError("\r\n\r\nUnhandled Exception:" + (e.ExceptionObject as Exception).ToString());

        public override void Load()
        {
            #region[Register TrainerComponent in Il2Cpp]

            log.LogMessage("Registering C# Type's in Il2Cpp");

            try
            {
                // Trainer
                ClassInjector.RegisterTypeInIl2Cpp<Bootstrapper>();
                ClassInjector.RegisterTypeInIl2Cpp<TrainerComponent>();
                
                // UI
                ClassInjector.RegisterTypeInIl2Cpp<UIControls>();
                ClassInjector.RegisterTypeInIl2Cpp<WindowDragHandler>();
                ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();

                // Debugging
                ClassInjector.RegisterTypeInIl2Cpp<Trainer.Tools.EventTestComponent>();

            }
            catch
            {
                log.LogError("FAILED to Register Il2Cpp Type!");
            }

            #endregion

            #region[Harmony Patching]

            try
            {
                log.LogMessage(" ");
                log.LogMessage("Inserting Harmony Hooks...");

                var harmony = new Harmony("wh0am15533.trainer.il2cpp");

                #region[Enable/Disable Harmony Debug Log]
                //Harmony.DEBUG = true; (Old)
                //HarmonyFileLog.Enabled = true;
                #endregion

                #region[Update() Hook - Only Needed for Bootstrapper]

                var originalUpdate = AccessTools.Method(typeof(UnityEngine.UI.CanvasScaler), "Update");
                log.LogMessage("   Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
                var postUpdate = AccessTools.Method(typeof(Trainer.Bootstrapper), "Update");
                log.LogMessage("   Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
                harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));

                #endregion

                #region[IBeginDragHandler, IDragHandler, IEndDragHandler Hooks]
                
                // These are required since UnHollower doesn't support Interfaces yet

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

                log.LogMessage("Runtime Hooks's Applied");
                log.LogMessage(" ");
            }
            catch { log.LogError("FAILED to Apply Hooks's!"); }

            #endregion

            log.LogMessage("Initializing Il2CppTypeSupport..."); // Helps with AssetBundles
            Il2CppTypeSupport.Initialize();

            #region[Bootstrap The Main Trainer GameObject]

            #region[DevNote]
            // If you create your main object here, only Awake(), OnEnabled() get fired. But if you try to create the trainer in either of 
            // those it doesn't get created properly as the object get's destroyed right away. Bootstrapping the GameObject like this allows
            // for it to inherit Unity MonoBehavior Events like OnGUI(), Update(), etc without a Harmony Patch. The only patch needed 
            // is for the Bootstrapper. You'll see. The Trainer has an EventTest function, Press 'Tab', and watch the BepInEx Console.
            #endregion
            Bootstrapper.Create("BootStrapperGO");

            #endregion

        }        
    }
}
