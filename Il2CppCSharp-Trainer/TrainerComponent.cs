
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Input = BepInEx.IL2CPP.UnityEngine.Input; //For UnityEngine.Input
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib; // UnhollowerRuntimeLib.Il2CppType.Of<>
using UnityEngine; // This has built-in Il2CppType which conflicts with UnhollowerRuntimeLib.Il2CppType (Doesn't contain .Of<>)
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Trainer.UI;

namespace Trainer
{
    #region[Delegates]

    internal delegate void getRootSceneObjects(int handle, IntPtr list);

    #endregion

    public class TrainerComponent : MonoBehaviour
    {
        #region[Declarations]

        #region[Trainer]

        // Trainer Base
        public static GameObject obj = null;
        public static TrainerComponent instance;
        private static bool initialized = false;
        private static BepInEx.Logging.ManualLogSource log;
        public static bool optionToggle = false;
        private static string spyText = "";
        private static GameObject eventsTester = null;
        private static TooltipGUI toolTipComp = null;
        private static IntPtr renderUIPointer = IntPtr.Zero;

        // UI
        private static Il2CppAssetBundle testAssetBundle = null;
        private static GameObject canvas = null;
        private static bool isVisible = false;
        private static GameObject uiPanel = null;
        private static Sprite stompy = null; // Test Sprite from AssetBundle
        private static GameObject positionDebug = null;

        // Debugging
        private static bool onGuiFired = false;
        private static bool updateFired = false;

        #endregion



        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            var component = new TrainerComponent(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<TrainerComponent>()).Pointer);

            toolTipComp = new TooltipGUI(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<TooltipGUI>()).Pointer);
            toolTipComp.enabled = false;

            return obj;
        }

        public TrainerComponent(IntPtr ptr) : base(ptr)
        {
            log = BepInExLoader.log;
            //log.LogMessage("TrainerComponent Loaded");

            instance = this;
        }

        private static void Initialize()
        {
            #region[AssetBundle Loading - Put the AssetBundles folder in the Game root folder!]

            if (testAssetBundle == null)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\AssetBundles\\testassetbundle"))
                {
                    log.LogMessage(" ");
                    log.LogMessage("Trying to Load AssetBundle...");
                    testAssetBundle = Il2CppAssetBundleManager.LoadFromFile(AppDomain.CurrentDomain.BaseDirectory + "\\AssetBundles\\testassetbundle");
                    if (testAssetBundle == null) { log.LogMessage("AssetBundle Failed to Load!"); return; }

                    #region[Print out asset names/paths]

                    log.LogMessage("Assets:");
                    foreach (var asset in testAssetBundle.AllAssetNames())
                    {
                        log.LogMessage("   Asset Name: " + asset.ToString());
                    }

                    #endregion

                    #region[Test Loading a prefab and instantiating it]
                    /*
                
                    // NOTE: There's currently a error in Unhollower that throws a NullReference exception, but the object and components DO get instantiated
                    // but the prefab doesn't display. It and it's components are running though.

                    log.LogMessage("Trying to Load Prefab...");

                    var prefab = testAssetBundle.LoadAsset<GameObject>("SOME PREFAB ASSET");
                    if (prefab != null)
                    {
                        log.LogMessage("Asset Loaded!");

                        // Instantiate the object
                        log.LogMessage("Trying to Instantiate Prefab...");
                        var t = Instantiate(prefab, new Vector3(0f, 0f), Quaternion.identity);
                        if (t != null) { log.LogMessage("Prefab Instantiated! Position: " + t.transform.position.ToString()); } else { log.LogMessage("Failed to Instantiated Prefab!"); }

                        #region[Test adding a custom component and make sure it works]
                        *//*
                        if (t != null)
                        {
                            log.LogMessage("Test adding a custom component and make sure it works");                        
                            var type = UnhollowerRuntimeLib.Il2CppType.Of<WindowDragHandler>();
                            comp = t.AddComponent(type).Cast<WindowDragHandler>();
                            log.LogMessage("Component testBool: " + comp.testBool.ToString());
                        }
                        *//*
                        #endregion
                    }
                    else { log.LogMessage("Failed to Load Asset!"); }
                    */
                    #endregion

                    log.LogMessage("Complete!");
                }
                else
                {
                    log.LogWarning("Skipping AssetBundle Loading - testassetBundle Doesn't Exist at: " + AppDomain.CurrentDomain.BaseDirectory + "\\AssetBundles\\testassetbundle");
                    log.LogWarning("Make sure the 'AssetBundles' folder from the Git Repo exists in the Game's root folder!");
                }
            }

            #endregion

            // Create a uGUI UI
            instance.CreateUI();

            #region[Display HotKeys]

            log.LogMessage(" ");
            log.LogMessage("HotKeys:");
            log.LogMessage("   Shift+Delete = Enable/Disable Option Toggle");
            log.LogMessage("   Keypress + optionToggle = Display Mouse/World/Viewport Positions for Debugging");
            log.LogMessage("   A = Dump All Scene's Objects (w/ optionToggle True prints components also)");
            log.LogMessage("   R = Dump Current Scene's Root Objects w/ Values (w/ optionToggle True prints components also)");
            log.LogMessage("   S = Display Scene Details");
            log.LogMessage("   U = Test Creating UI Elements");
            log.LogMessage("   X = Dump Objects to XML (w/ optionToggle uses FindObjectsOfType<>() (Finds more, but loss of Hierarchy)");
            log.LogMessage("   T = Toggle GUI GameObject Tooltip w/ optionToggle (Drag mouse around while holding down mouse button)");
            log.LogMessage("   Tab = Test Unity MonoBehavior Events");
            log.LogMessage(" ");

            #endregion

            initialized = true;
        }

        public void Awake()
        {
            log.LogMessage("TrainerComponent Awake() Fired!");
        }

        public void Start()
        {
            log.LogMessage("TrainerComponent Start() Fired!");
        }

        public void OnEnable()
        {
            log.LogMessage("TrainerComponent OnEnable() Fired!");
        }

        public void Update()
        {
            if (!updateFired) { BepInExLoader.log.LogMessage("TrainerComponent Update() Fired!"); updateFired = true; }

            if (!initialized) { Initialize(); }

            // Note: You can use the regular Input also for Key events, see the usings
            // Just an Option Toggle to give us more Key combinations
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.LeftShift) && Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.Delete) && Event.current.type == EventType.KeyDown)
            {
                optionToggle = !optionToggle;
                log.LogMessage("Option Toggle Enabled: " + optionToggle.ToString());
                Event.current.Use();
            }
            
            // Toggle TooltipGUI
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.T) && optionToggle && EventSystem.current != null && Event.current.type == EventType.KeyDown)
            {
                Event.current.Use();
                toolTipComp.enabled = !toolTipComp.enabled;
            }

            // Object Spy - Requires an EventSystem! (WIP, currently only detects UI elements)
            if (optionToggle && EventSystem.current != null && Event.current.type == EventType.MouseDrag)
            {
                //log.LogMessage("ObjectSpy Fired!");

                if (canvas != null)
                {
                    var saInput = EventSystem.current.GetComponent<StandaloneInputModule>();
                    var evData = saInput.GetMousePointerEventData();

                    string name = "", pname = "", tmpSpyText = "";

                    try
                    {
                        name = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerEnter.name.ToString();
                        pname = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerCurrentRaycast.module.name;

                        tmpSpyText = "GameObject: " + name + " - Parent: " + pname;

                        TooltipGUI.Tooltip = name;
                    }
                    catch { /* Not Implemented yet for World objects. TODO: Add Physics and Graphics Raycaster */ }

                    if (tmpSpyText != spyText)
                    {
                        log.LogMessage(""); // Just a Spacer
                        log.LogMessage("[GameObject]: " + name + " - Parent: " + pname);
                        spyText = tmpSpyText;

                        log.LogMessage("  [Parent Components]:");
                        var pcomps = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerCurrentRaycast.module.gameObject.GetGameObjectComponents();
                        foreach (var comp in pcomps)
                        {
                            log.LogMessage("    " + comp.Name);
                        }

                        log.LogMessage("  [GameObject Components]:");
                        var gcomps = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerEnter.GetGameObjectComponents();
                        foreach (var comp in gcomps)
                        {
                            log.LogMessage("    " + comp.Name);
                        }
                    }
                }

                Event.current.Use();
            }
            if (optionToggle && EventSystem.current != null && Event.current.type == EventType.MouseUp)
            {
                // Clears the tooltip on MouseUp
                TooltipGUI.Tooltip = "";
                Event.current.Use();
            }

            // Dump All Scenes GameObjects (w/ optionToggle True prints components also)
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) && Event.current.type == EventType.KeyDown)
            {
                DumpAll(GetAllScenesGameObjects());
                Event.current.Use();
            }

            // Dumping Root Scene Objects w/ Values (w/ optionToggle True prints components also)
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R) && Event.current.type == EventType.KeyDown)
            {
                log.LogMessage("Dumping Root Scene Objects w/ values...");
                Trainer.Tools.SceneDumper.DumpObjects(GetRootSceneGameObjects().ToArray());
                Event.current.Use();
            }

            // Create UI if needed otherwise Show/Hide
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.U) && Event.current.type == EventType.KeyDown)
            {
                if (canvas == null)
                {
                    instance.CreateUI();
                }
                else
                {
                    if (isVisible)
                    {
                        canvas.SetActive(false);
                        isVisible = false;
                    }
                    else
                    {
                        canvas.SetActive(true);
                        isVisible = true;
                    }
                }

                Event.current.Use();
            }
            
            // Display Scene Details
            if (UnityEngine.Input.GetKeyDown(KeyCode.S) && Event.current.type == EventType.KeyDown)
            {
                log.LogMessage(" ");
                log.LogMessage("Scene Details:");
                Scene activeScene = SceneManager.GetActiveScene();
                log.LogMessage("   Scene Name: " + activeScene.name);
                log.LogMessage("   Scene Build Index: " + activeScene.buildIndex.ToString());
                log.LogMessage("   Root Object Count: " + activeScene.rootCount.ToString());

                log.LogMessage(" ");
                log.LogMessage("   Camera's:");
                foreach (var cam in Camera.allCameras)
                {
                    log.LogMessage("      Cam: " + cam.name + " GameObject: " + cam.gameObject.name);
                }

                log.LogMessage(" ");
                log.LogMessage("   Predefined Layers... (-1 = Not Found)");
                string[] predefinedLayers = { "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Player", String.Empty };
                foreach (string layer in predefinedLayers)
                {
                    log.LogMessage("      Layer: [" + LayerMask.NameToLayer(layer) + "] " + layer);
                }

                log.LogMessage(" ");
                log.LogMessage("   Root Objects:");                
                foreach(var obj in GetRootSceneGameObjects())
                {
                    log.LogMessage("      Name: " + obj.name);
                    log.LogMessage("         Enabled: " + obj.activeSelf.ToString());
                    log.LogMessage("         Layer: " + obj.layer.ToString());
                    log.LogMessage("         Position: " + obj.transform.position.ToString());
                    log.LogMessage("         Local Position: " + obj.transform.localPosition.ToString());
                    log.LogMessage("         Components: ");
                    foreach(var comp in obj.GetComponents<Component>())
                    {
                        log.LogMessage("            " + comp.name + "(" + comp.GetIl2CppType().Name + ")");
                    }
                    log.LogMessage(" ");

                }
            }

            // Dump All Scene's Objects to XML (w/ optionToggle True uses FindObjectsOfType<GameOBject>() (Finds more, but at loss of Hierarchy))
            if (UnityEngine.Input.GetKeyDown(KeyCode.X) && Event.current.type == EventType.KeyDown)
            {
                log.LogMessage("");

                List<GameObjectDetails> objectTree = new List<GameObjectDetails>();

                // Doesn't show DontDestroyOnLoad objects. GameObject.FindObjectsOfType<GameObject>() does, but loses heirarchy
                if (!optionToggle)
                {
                    log.LogMessage("Dumping All Objects to XML using GetAllScenesGameObject()...");
                    foreach (var obj in GetAllScenesGameObjects()) { objectTree.Add(new GameObjectDetails(obj)); }
                }
                else
                {
                    log.LogMessage("Dumping All Objects to XML using GameObject.FindObjectsOfType<GameObject>()...");
                    foreach (var obj in GameObject.FindObjectsOfType<GameObject>()) { objectTree.Add(new GameObjectDetails(obj)); }
                }

                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\AllObjectsDump.xml", GameObjectDetails.XMLSerialize(objectTree));

                log.LogMessage("Complete!");
                log.LogMessage("XML written to " + (AppDomain.CurrentDomain.BaseDirectory + "\\AllObjectsDump.xml").Replace("\\\\", "\\"));

                string path = (AppDomain.CurrentDomain.BaseDirectory + "\\AllObjectsDump.xml").Replace("\\\\", "\\");                
                Application.OpenURL("file:///" + path.Replace("\\", "/")); // Opens in default associated app

                Event.current.Use();
            }

            // Display Mouse/World/View Positions for Debugging
            if (positionDebug == null) { positionDebug = GameObject.Find("TitleImage"); } // Just a refence object to compare values
            if (optionToggle && EventSystem.current != null && Event.current.type == EventType.KeyDown)
            {
                log.LogMessage(" ");
                log.LogMessage("Mouse Debug Info:");
                log.LogMessage("   TitleImage POS: " + positionDebug.transform.position.ToString());
                log.LogMessage("   Mouse POS:      " + UnityEngine.Input.mousePosition.ToString());
                log.LogMessage("   Mouse2World:    " + Camera.current.ScreenToWorldPoint(UnityEngine.Input.mousePosition).ToString());
                log.LogMessage("   Mouse2Viewport: " + Camera.current.ScreenToViewportPoint(UnityEngine.Input.mousePosition).ToString());
                log.LogMessage(" ");

                Event.current.Use();
            }

            // Unity MonoBehavior Event Testing
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab) && EventSystem.current != null && Event.current.type == EventType.KeyDown)
            {
                Event.current.Use();
                log.LogMessage("Testing Unity MonoBehavior Events...");
                eventsTester = Trainer.Tools.EventTestComponent.Create("EventTestComponentGO");
            }
            if (Trainer.Tools.EventTestComponent.eventsFired) { Destroy(eventsTester); }



        }

        public void OnGUI()
        {
            if (!onGuiFired) { BepInExLoader.log.LogMessage("TrainerComponent OnGUI() Fired!"); onGuiFired = true; }


        }

        #region[UI Helpers]

        private void CreateUI()
        {
            if (canvas == null)
            {
                log.LogMessage(" ");
                log.LogMessage("Creating UI Elements");

                // Create a GameObject with a Canvas
                canvas = instance.createUICanvas();
                Object.DontDestroyOnLoad(canvas);

                // Add a Panel to the Canvas. See createUIPanel for why we pass height/width as string
                uiPanel = instance.createUIPanel(canvas, "550", "200", null);

                // This is how we'll hook mouse Events for window dragging
                EventTrigger comp1 = new EventTrigger(uiPanel.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<EventTrigger>()).Pointer);
                WindowDragHandler comp2 = new WindowDragHandler(uiPanel.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<WindowDragHandler>()).Pointer);

                Image panelImage = uiPanel.GetComponent<Image>();
                panelImage.color = instance.HTMLString2Color("#2D2D30FF").Unbox<Color32>();

                #region[Panel Elements]

                // NOTE: Elements are spaced in increments/decrements of 35 in localPosition 

                #region[Add a Button]

                Sprite btnSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                GameObject uiButton = instance.createUIButton(uiPanel, btnSprite);
                uiButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 250, 0);

                #endregion

                #region[Add a Toggle]

                Sprite toggleBgSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#3E3E42FF"));
                Sprite toggleSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                GameObject uiToggle = instance.createUIToggle(uiPanel, toggleBgSprite, toggleSprite);
                uiToggle.GetComponentInChildren<Text>().color = Color.white;
                uiToggle.GetComponentInChildren<Toggle>().isOn = false;
                uiToggle.GetComponent<RectTransform>().localPosition = new Vector3(0, 215, 0);

                #endregion

                #region[Add a Slider]

                Sprite sliderBgSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#9E9E9EFF"));
                Sprite sliderFillSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                Sprite sliderKnobSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#191919FF"));
                GameObject uiSlider = instance.createUISlider(uiPanel, sliderBgSprite, sliderFillSprite, sliderKnobSprite);
                uiSlider.GetComponentInChildren<Slider>().value = 0.5f;
                uiSlider.GetComponent<RectTransform>().localPosition = new Vector3(0, 185, 0);

                #endregion

                #region[Add a Text (Label)]

                Sprite txtBgSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                GameObject uiText = instance.createUIText(uiPanel, txtBgSprite, "#FFFFFFFF");
                uiText.GetComponent<Text>().text = "This is a new Text Label";
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 150, 0);

                #endregion

                #region[Add a InputField]

                Sprite inputFieldSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                GameObject uiInputField = instance.createUIInputField(uiPanel, inputFieldSprite, "#000000FF");
                #region[Dev Note]
                // The following line is odd. It sets the text but doesn't display it, WTF? The default child object PlaceHolder 
                // has Text component but Unhollower thinks it's a UnityEngine.Graphic?
                //      uiInputField.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Search...";
                // Checked in Unity Editor too, it's definately Text, why does unhollower think it's Graphic?
                // 
                // We'll just use the InputField component directly. It works, but PlaceHolder is nice...
                #endregion
                uiInputField.GetComponent<InputField>().text = "Some Input Field...";
                uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(0, 115, 0);

                #endregion

                #region[Add a DropDown]

                // NOTE: This wierd, it does it's thing and work's but then the rest on the UI disappears... hmmm... :/

                Sprite dropdownBgSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                Sprite dropdownScrollbarSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#3E3E42FF"));
                Sprite dropdownDropDownSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#252526FF"));
                Sprite dropdownCheckmarkSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#7AB900FF"));
                Sprite dropdownMaskSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#1E1E1EFF"));
                GameObject uiDropDown = instance.createUIDropDown(uiPanel, dropdownBgSprite, dropdownScrollbarSprite, dropdownDropDownSprite, dropdownCheckmarkSprite, dropdownMaskSprite);
                Object.DontDestroyOnLoad(uiDropDown);
                uiDropDown.GetComponent<RectTransform>().localPosition = new Vector3(0, 75, 0);

                #endregion

                #region[Add a ScrollView]

                Sprite scrollviewBgSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#9E9E9EFF"));
                Sprite scrollviewScrollbarSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#3E3E42FF"));
                Sprite scrollviewMaskSprite = instance.createSpriteFrmTexture(instance.createDefaultTexture("#3E3E42FF"));
                GameObject uiScrollView = instance.createUIScrollView(uiPanel, scrollviewBgSprite, scrollviewMaskSprite, scrollviewScrollbarSprite);

                // Set some content
                GameObject content = uiScrollView.GetComponent<ScrollRect>().content.gameObject;
                GameObject contentTextObj = instance.createUIText(content, scrollviewBgSprite, "#FFFFFFFF");
                contentTextObj.GetComponent<Text>().text = "ScrollView Element";
                contentTextObj.GetComponent<RectTransform>().localPosition = new Vector3(120, -50, 0);

                uiScrollView.GetComponent<RectTransform>().localPosition = new Vector3(0, -75, 0);

                #endregion

                #region[Add a RawImage]

                // Our Test Sprite from testBundle for UI RawImage Element
                if (testAssetBundle != null && stompy != null)
                {
                    log.LogMessage("   Trying to Load Test Sprite...");
                    stompy = testAssetBundle.LoadAsset<Sprite>("assets/tools/customassets/test assets/externaltexture.png");
                    if (stompy != null) { log.LogMessage("      Sprite Loaded!"); } else { log.LogMessage("      Failed to Load Sprite!"); }

                    GameObject uiImage = instance.createUIRawImage(uiPanel, stompy);
                    uiImage.GetComponent<RectTransform>().localPosition = new Vector3(0, -220, 0);
                    uiImage.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f);
                }
                else { log.LogMessage("   Skipping - Test AssetBundle Not Loaded!"); }

                #endregion

                #endregion

                isVisible = true;

                log.LogMessage("Complete!");
            }
        }

        public Il2CppSystem.Object HTMLString2Color(Il2CppSystem.String htmlcolorstring)
        {
            Color32 color = new Color32();

            #region[DevNote]
            // Unity ref: https://docs.unity3d.com/ScriptReference/ColorUtility.TryParseHtmlString.html
            // Note: Color strings can also set alpha: "#7AB900" vs. w/alpha "#7AB90003" 
            //ColorUtility.TryParseHtmlString(htmlcolorstring, out color); // Unity's Method, This may have been stripped
            #endregion

            color = htmlcolorstring.HexToColor().Unbox<Color32>();
            //log.LogMessage("HexString: " + htmlcolorstring  + "RGBA(" + color.r.ToString() + "," + color.g.ToString() + "," + color.b.ToString() + "," + color.a.ToString() + ")");

            return color.BoxIl2CppObject();
        }

        public Texture2D createDefaultTexture(Il2CppSystem.String htmlcolorstring)
        {
            Color32 color = HTMLString2Color(htmlcolorstring).Unbox<Color32>();

            // Make a new sprite from a texture
            Texture2D SpriteTexture = new Texture2D(1, 1);
            SpriteTexture.SetPixel(0, 0, color);
            SpriteTexture.Apply();

            return SpriteTexture;
        }

        public Texture2D createTextureFromFile(Il2CppSystem.String FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            Texture2D Tex2D;
            Il2CppStructArray<byte> FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(265, 198);
                //Tex2D.LoadRawTextureData(FileData); // This is Broke. Unhollower/Texture2D doesn't like it...
                Tex2D.LoadImage(FileData, false);
                Tex2D.Apply();
                return Tex2D;
            }
            return null;
        }

        public Sprite createSpriteFrmTexture(Texture2D SpriteTexture)
        {
            // Create a new Sprite from Texture
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);

            return NewSprite;
        }

        public GameObject createUICanvas()
        {
            log.LogMessage("   Creating Canvas");

            // Create a new Canvas Object with required components
            GameObject CanvasGO = new GameObject("CanvasGO");
            Object.DontDestroyOnLoad(CanvasGO);

            Canvas canvas = new Canvas(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<Canvas>()).Pointer);

            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            UnityEngine.UI.CanvasScaler cs = new UnityEngine.UI.CanvasScaler(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.CanvasScaler>()).Pointer);

            cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            cs.referencePixelsPerUnit = 100f;
            cs.referenceResolution = new Vector2(1024f, 788f);

            UnityEngine.UI.GraphicRaycaster gr = new UnityEngine.UI.GraphicRaycaster(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.GraphicRaycaster>()).Pointer);

            return CanvasGO;
        }

        public GameObject createUIPanel(GameObject canvas, Il2CppSystem.String height, Il2CppSystem.String width, Sprite BgSprite = null)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            log.LogMessage("   Creating UI Panel");
            GameObject uiPanel = UIControls.CreatePanel(uiResources);
            uiPanel.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = uiPanel.GetComponent<RectTransform>();

            float size;
            size = Il2CppSystem.Single.Parse(height); // Their is no float support in Unhollower, this avoids errors
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            size = Il2CppSystem.Single.Parse(width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            // You can also use rectTransform.sizeDelta = new Vector2(width, height);

            return uiPanel;
        }

        public GameObject createUIButton(GameObject parent, Sprite NewSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.standard = NewSprite;

            log.LogMessage("   Creating UI Button");
            GameObject uiButton = UIControls.CreateButton(uiResources);
            uiButton.transform.SetParent(parent.transform, false);

            return uiButton;
        }

        public GameObject createUIToggle(GameObject parent, Sprite BgSprite, Sprite customCheckmarkSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.standard = BgSprite;
            uiResources.checkmark = customCheckmarkSprite;

            log.LogMessage("   Creating UI Toggle");
            GameObject uiToggle = UIControls.CreateToggle(uiResources);
            uiToggle.transform.SetParent(parent.transform, false);

            return uiToggle;
        }

        public GameObject createUISlider(GameObject parent, Sprite BgSprite, Sprite FillSprite, Sprite KnobSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;
            uiResources.standard = FillSprite;
            uiResources.knob = KnobSprite;

            log.LogMessage("   Creating UI Slider");
            GameObject uiSlider = UIControls.CreateSlider(uiResources);
            uiSlider.transform.SetParent(parent.transform, false);

            return uiSlider;
        }

        public GameObject createUIInputField(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.inputField = BgSprite;

            log.LogMessage("   Creating UI InputField");
            GameObject uiInputField = UIControls.CreateInputField(uiResources);
            uiInputField.transform.SetParent(parent.transform, false);

            var textComps = uiInputField.GetComponentsInChildren<Text>();
            foreach (var text in textComps)
            {
                text.color = HTMLString2Color(textColor).Unbox<Color32>();
            }

            return uiInputField;
        }

        public GameObject createUIDropDown(GameObject parent, Sprite BgSprite, Sprite ScrollbarSprite, Sprite DropDownSprite, Sprite CheckmarkSprite, Sprite customMaskSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            // Set the Background and Handle Image
            uiResources.standard = BgSprite;
            // Set the Scrollbar Background Image
            uiResources.background = ScrollbarSprite;
            // Set the Dropdown Handle Image
            uiResources.dropdown = DropDownSprite;
            // Set the Checkmark Image
            uiResources.checkmark = CheckmarkSprite;
            // Set the Viewport Mask
            uiResources.mask = customMaskSprite;

            log.LogMessage("   Creating UI DropDown");
            var uiDropdown = UIControls.CreateDropdown(uiResources);
            uiDropdown.transform.SetParent(parent.transform, false);

            return uiDropdown;
        }

        public GameObject createUIImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;
            log.LogMessage("   Creating UI Image");
            GameObject uiImage = UIControls.CreateImage(uiResources);
            uiImage.transform.SetParent(parent.transform, false);

            return uiImage;
        }

        public GameObject createUIRawImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            log.LogMessage("   Creating UI RawImage");
            GameObject uiRawImage = UIControls.CreateRawImage(uiResources);
            uiRawImage.transform.SetParent(parent.transform, false);

            return uiRawImage;
        }

        public GameObject createUIScrollbar(GameObject parent, Sprite ScrollbarSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = ScrollbarSprite;

            log.LogMessage("   Creating UI Scrollbar");
            GameObject uiScrollbar = UIControls.CreateScrollbar(uiResources);
            uiScrollbar.transform.SetParent(parent.transform, false);

            return uiScrollbar;
        }

        public GameObject createUIScrollView(GameObject parent, Sprite BgSprite, Sprite customMaskSprite, Sprite customScrollbarSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            // These 2 all need to be the same for some reason, I think due to scrollview automation.
            uiResources.background = BgSprite;
            uiResources.knob = BgSprite;

            uiResources.standard = customScrollbarSprite;
            uiResources.mask = customMaskSprite;

            log.LogMessage("   Creating UI ScrollView");
            GameObject uiScrollView = UIControls.CreateScrollView(uiResources);
            uiScrollView.transform.SetParent(parent.transform, false);

            return uiScrollView;
        }

        public GameObject createUIText(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            log.LogMessage("   Creating UI Text");
            GameObject uiText = UIControls.CreateText(uiResources);
            uiText.transform.SetParent(parent.transform, false);

            Text text = uiText.GetComponent<Text>();
            //uiText.transform.GetChild(0).GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(Font.Il2CppType, "Arial.ttf"); // Invalid Cast
            text.color = HTMLString2Color(textColor).Unbox<Color32>();

            return uiText;
        }

        #endregion

        #region[ICalls]

        #region[Get Objects]

        // Resolve the GetRootGameObjects ICall (internal Unity MethodImp functions)
        internal static getRootSceneObjects getRootSceneObjects_iCall = IL2CPP.ResolveICall<getRootSceneObjects>("UnityEngine.SceneManagement.Scene::GetRootGameObjectsInternal");
        private static void GetRootGameObjects_Internal(Scene scene, IntPtr list)
        {
            getRootSceneObjects_iCall(scene.handle, list);
        }

        private static Il2CppSystem.Collections.Generic.List<GameObject> GetRootSceneGameObjects()
        {
            var scene = SceneManager.GetActiveScene();
            var list = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);

            GetRootGameObjects_Internal(scene, list.Pointer);

            return list;
        }
        private static Il2CppSystem.Collections.Generic.List<GameObject> GetAllScenesGameObjects()
        {
            Scene[] array = new Scene[SceneManager.sceneCount];
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                array[i] = SceneManager.GetSceneAt(i);
            }

            var allObjectsList = new Il2CppSystem.Collections.Generic.List<GameObject>();
            foreach (var scene in array)
            {               
                var list = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);
                GetRootGameObjects_Internal(scene, list.Pointer);
                foreach (var obj in list) { allObjectsList.Add(obj); }
            }

            #region[DevNote]
            /*
            The reason these differ are that GetAllScenesObjects doen't get DontDestroyOnLoad objects and it maintains heirarchy so it looks like alot less
            For example: GetAllScenesObjects() doesn't find this Trainer and the games StageLoadManager object.
            */
            //log.LogMessage("AllScenesObject's Count: " + allObjectsList.Count.ToString());
            //log.LogMessage("FindAll<GameObject>() Count: " + GameObject.FindObjectsOfType<GameObject>().Count.ToString());
            #endregion

            return allObjectsList;
        }

        #endregion


        #endregion

        #region[Dump All Scenes GameObjects]

        private static string dumpLog = "";
        public static void DumpAll(Il2CppSystem.Collections.Generic.List<GameObject> rootObjects)
        {
            log.LogMessage("Dumping Objects...");

            foreach (GameObject obj in rootObjects)
            {
                dumpLog = "";
                level = 1;
                prevlevel = 0;

                // Dump this object
                log.LogMessage("[GameObject]: " + obj.name);
                dumpLog += "[GameObject]: " + obj.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    log.LogMessage("  [Components]:");
                    dumpLog += "  [Components]:\r\n";

                    var comps = obj.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        log.LogMessage("    " + comp.Name);
                        dumpLog += "    " + comp.Name + "\r\n";
                    }

                    dumpLog += "\r\n";
                }
                #endregion

                // Dump the children
                DisplayChildren(obj.transform);

                // Write the Dump File
                if (dumpLog != "")
                {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\")) { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\"); }
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\" + obj.name + "_DUMP.txt", dumpLog);
                }

                log.LogMessage("Dump Complete!");
            }
        }

        private static int level = 0;
        private static int prevlevel = 0;
        private static void DisplayChildren(Transform trans)
        {
            prevlevel = level;

            foreach (var child in trans)
            {
                var t = child.Cast<Transform>();

                // Adjust the indent
                string consoleprefix = "";
                for (int cnt = 0; cnt < level; cnt++) { consoleprefix += "  "; }

                // The Actual Logging
                log.LogMessage(consoleprefix + "[GameObject]: " + t.gameObject.name);
                dumpLog += consoleprefix + "[GameObject]: " + t.gameObject.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    log.LogMessage(consoleprefix + "  [Components]:");
                    dumpLog += consoleprefix + "  [Components]:\r\n";

                    var comps = t.gameObject.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        log.LogMessage(consoleprefix + "    " + comp.Name);
                        dumpLog += consoleprefix + "    " + comp.Name + "\r\n";
                    }

                    dumpLog += "\r\n";
                }
                #endregion

                // Out Inifinate Iterator
                if (t.childCount > 0)
                {
                    level += 1;
                    DisplayChildren(t);
                }
                else { level = prevlevel; }

            }
        }

        #endregion
    }

    #region[GameObjects Extensions]

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

        public static void DumpGameObject(this GameObject obj)
        {
            if (obj == null)
                return;

            Il2CppSystem.Collections.Generic.List<GameObject> tmpList = new Il2CppSystem.Collections.Generic.List<GameObject>();
            tmpList.Add(obj);
            TrainerComponent.DumpAll(tmpList);
        }

    }

    #endregion
}



