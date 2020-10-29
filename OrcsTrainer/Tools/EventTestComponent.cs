
using System;
using UnityEngine;

namespace Trainer.Tools
{
    public class EventTestComponent : MonoBehaviour
    {
        public static GameObject obj = null;
        public static EventTestComponent component = null;

        public static bool eventsFired = false;
        private bool update = false;
        private bool fixedupdate = false;
        private bool lateupdate = false;
        private bool ongui = false;

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);
            component = new EventTestComponent(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<EventTestComponent>()).Pointer);
            return obj;
        }

        public EventTestComponent(IntPtr intPtr) : base(intPtr) { }

        public void Awake()
        {
            BepInExLoader.log.LogMessage("   EventTestComponent Awake() Fired!");
        }

        public void Start()
        {
            BepInExLoader.log.LogMessage("   EventTestComponent Start() Fired!");           
        }

        public void OnDestroy()
        {
            BepInExLoader.log.LogMessage("   EventTestComponent OnDestroy() Fired!");
            BepInExLoader.log.LogMessage("Complete!");
            BepInExLoader.log.LogMessage(" ");
        }

        public void Update()
        {
            if (!update) { BepInExLoader.log.LogMessage("   EventTestComponent Update() Fired!"); update = true; }
            
            if (update && fixedupdate && lateupdate && ongui) { eventsFired = true; }
        }

        public void FixedUpdate()
        {
            if (!fixedupdate) { BepInExLoader.log.LogMessage("   EventTestComponent FixedUpdate() Fired!"); fixedupdate = true; }            
        }

        public void LateUpdate()
        {
            if (!lateupdate) { BepInExLoader.log.LogMessage("   EventTestComponent LateUpdate() Fired!"); lateupdate = true; }            
        }

        public void OnGUI()
        {
            if (!ongui) { BepInExLoader.log.LogMessage("   EventTestComponent OnGUI Fired!"); ongui = true; }            
        }

        public void OnDisable()
        {
            BepInExLoader.log.LogMessage("   EventTestComponent OnDisable() Fired!");         
        }

        public void OnEnable()
        {
            BepInExLoader.log.LogMessage("   EventTestComponent OnEnable() Fired!");         
        }
    }
}
