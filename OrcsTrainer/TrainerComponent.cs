using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BepInEx;
using BepInEx.IL2CPP.UnityEngine; //For UnityEngine.Input
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace Trainer
{
    #region[Delegates]

#if DEBUG
    internal delegate void getRootSceneObjects(int handle, IntPtr list);
#endif

    #endregion

    public class TrainerComponent : MonoBehaviour
    {
        #region[Declarations]

        #region[Trainer]

        public static TrainerComponent instance;

#if DEBUG
        private static bool optionToggle = false;

        private static string spyText = "";

        private static GameObject canvas = null;
        private static bool isVisible = false;
        private static GameObject uiPanel = null;
#endif

        #endregion

        #region[For Orc's]

        private GameObject levelManagerGO;
        private mico.game.LevelManager levelManager = null;

#if DEBUG
        // For Orc's Testing
        //private List<GameObject> _unitButtons = new List<GameObject>();
        //private UnitButtonManager unitButtonManager = null;
#endif
        #endregion

        #endregion

        public TrainerComponent(IntPtr ptr) : base(ptr)
        {
#if DEBUG
            BepInExLoader.log.LogMessage("[Trainer] TestComponent - Entered Constructor");
#endif

            instance = this;
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

        [HarmonyPostfix] //Harmony requires static method
        public static void Update()
        {
#if DEBUG
            // Just an Option Toggle to give us more Key combinations
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.LeftShift) && Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.Delete) && Event.current.type == EventType.KeyDown)
            {
                optionToggle = !optionToggle;
                BepInExLoader.log.LogMessage("Option Toggle Enabled: " + optionToggle.ToString());
                Event.current.Use();
            }

            // Dump All Scenes GameObjects
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.PageDown) && Event.current.type == EventType.KeyDown)
            {
                DumpAll(GetRootSceneGameObjects());
                Event.current.Use();
            }

            // Test Creating UI Elements
            if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F5) && Event.current.type == EventType.KeyDown)
            {
                if (canvas == null)
                {
                    BepInExLoader.log.LogMessage("Test Creating UI Elements");

                    // Create a GameObject with a Canvas
                    canvas = instance.createUICanvas();
                    Object.DontDestroyOnLoad(canvas);

                    // Add a Panel to the Canvas. See createUIPanel for why we pass height/width as string
                    uiPanel = instance.createUIPanel(canvas, "550", "200", null);

                    // This is how we'll hook mouse Events for window dragging
                    uiPanel.AddComponent<EventTrigger>();
                    uiPanel.AddComponent<WindowDragHandler>();

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

                    #endregion

                    isVisible = true;

                    BepInExLoader.log.LogMessage("Complete!");
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

            // Object Spy - Requires an EventSystem! (Toggle F5 to test. WIP, currently only detects UI elements)
            if (optionToggle && EventSystem.current != null && Event.current.type == EventType.mouseDrag)
            {
                //BepInExLoader.log.LogMessage("ObjectSpy Fired!");

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
                    }
                    catch { /* Not Implemented yet for World objects. TODO: Add Physics and Graphics Raycaster */ }

                    if (tmpSpyText != spyText)
                    {
                        BepInExLoader.log.LogMessage(""); // Just a Spacer
                        BepInExLoader.log.LogMessage("[GameObject]: " + name + " - Parent: " + pname);
                        spyText = tmpSpyText;

                        BepInExLoader.log.LogMessage("  [Parent Components]:");
                        var pcomps = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerCurrentRaycast.module.gameObject.GetGameObjectComponents();
                        foreach (var comp in pcomps)
                        {
                            BepInExLoader.log.LogMessage("    " + comp.Name);
                        }

                        BepInExLoader.log.LogMessage("  [GameObject Components]:");
                        var gcomps = evData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData.pointerEnter.GetGameObjectComponents();
                        foreach (var comp in gcomps)
                        {
                            BepInExLoader.log.LogMessage("    " + comp.Name);
                        }
                    }
                }
            }
#endif

            #region[Orc's Cheats]

            if (instance.levelManager == null)
            {
                instance.levelManagerGO = GameObject.Find("LevelManager");
                if (instance.levelManagerGO != null)
                {
                    instance.levelManager = instance.levelManagerGO.GetComponent<mico.game.LevelManager>();
                }
            }
            else
            {
                // Give Resources
                if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F12) && Event.current.type == EventType.KeyDown)
                {
                    BepInExLoader.log.LogMessage("[Trainer]: Adding 5,000 Resources");
                    instance.levelManager.addResources(5000);
                    Event.current.Use();
                }

                // Kill All
                if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F11) && Event.current.type == EventType.KeyDown)
                {
                    BepInExLoader.log.LogMessage("[Trainer]: Killing ALL!");
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
                        instance.unitButtonManager = unitButtonsGO.GetComponent<UnitButtonManager>();

                        for (int idx = 0; idx < unitButtonsGO.transform.childCount; idx++)
                        {
                            var child = unitButtonsGO.transform.GetChild(idx);
                            instance._unitButtons.Add(child.gameObject);
                            BepInExLoader.log.LogMessage("[GameObject]: " + child.name + " IS_ACTIVE: " + child.gameObject.activeSelf.ToString());
                        }
                    }

                    Event.current.Use();
                }

                if (instance.unitButtonManager != null)
                {
                    foreach (var unitButton in instance._unitButtons)
                    {
                        instance.unitButtonManager.enableClick = true;

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
            }

            #endregion
        }


        #region[UI Helpers]
#if DEBUG
        public Il2CppSystem.Object HTMLString2Color(Il2CppSystem.String htmlcolorstring)
        {
            Color32 color = new Color32();

            #region[DevNote]
            // Unity ref: https://docs.unity3d.com/ScriptReference/ColorUtility.TryParseHtmlString.html
            // Note: Color strings can also set alpha: "#7AB900" vs. w/alpha "#7AB90003" 
            //ColorUtility.TryParseHtmlString(htmlcolorstring, out color); // Unity's Method, This may have been stripped
            #endregion

            color = htmlcolorstring.HexToColor().Unbox<Color32>();
            //BepInExLoader.log.LogMessage("HexString: " + htmlcolorstring  + "RGBA(" + color.r.ToString() + "," + color.g.ToString() + "," + color.b.ToString() + "," + color.a.ToString() + ")");

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
                Tex2D.Apply();
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
            BepInExLoader.log.LogMessage("Creating Canvas");

            // Create a new Canvas Object with required components
            GameObject CanvasGO = new GameObject("CanvasGO");
            Canvas canvas = CanvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            UnityEngine.UI.CanvasScaler cs = CanvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            cs.referencePixelsPerUnit = 100f;
            cs.referenceResolution = new Vector2(800f, 600f);

            UnityEngine.UI.GraphicRaycaster gr = CanvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            return CanvasGO;
        }

        public GameObject createUIPanel(GameObject canvas, Il2CppSystem.String height, Il2CppSystem.String width, Sprite BgSprite = null)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            BepInExLoader.log.LogMessage("Creating UI Panel");
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

            BepInExLoader.log.LogMessage("Creating UI Button");
            GameObject uiButton = UIControls.CreateButton(uiResources);
            uiButton.transform.SetParent(parent.transform, false);

            return uiButton;
        }

        public GameObject createUIToggle(GameObject parent, Sprite BgSprite, Sprite customCheckmarkSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.standard = BgSprite;
            uiResources.checkmark = customCheckmarkSprite;

            BepInExLoader.log.LogMessage("Creating UI Toggle");
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

            BepInExLoader.log.LogMessage("Creating UI Slider");
            GameObject uiSlider = UIControls.CreateSlider(uiResources);
            uiSlider.transform.SetParent(parent.transform, false);

            return uiSlider;
        }

        public GameObject createUIInputField(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.inputField = BgSprite;

            BepInExLoader.log.LogMessage("Creating UI InputField");
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

            BepInExLoader.log.LogMessage("Creating UI DropDown");
            var uiDropdown = UIControls.CreateDropdown(uiResources);
            uiDropdown.transform.SetParent(parent.transform, false);

            return uiDropdown;
        }

        public GameObject createUIImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            BepInExLoader.log.LogMessage("Creating UI Image");
            GameObject uiImage = UIControls.CreateImage(uiResources);
            uiImage.transform.SetParent(parent.transform, false);

            return uiImage;
        }

        public GameObject createUIRawImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            BepInExLoader.log.LogMessage("Creating UI RawImage");
            GameObject uiRawImage = UIControls.CreateRawImage(uiResources);
            uiRawImage.transform.SetParent(parent.transform, false);

            return uiRawImage;
        }

        public GameObject createUIScrollbar(GameObject parent, Sprite ScrollbarSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = ScrollbarSprite;

            BepInExLoader.log.LogMessage("Creating UI Scrollbar");
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

            BepInExLoader.log.LogMessage("Creating UI ScrollView");
            GameObject uiScrollView = UIControls.CreateScrollView(uiResources);
            uiScrollView.transform.SetParent(parent.transform, false);

            return uiScrollView;
        }

        public GameObject createUIText(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            BepInExLoader.log.LogMessage("Creating UI Text");
            GameObject uiText = UIControls.CreateText(uiResources);
            uiText.transform.SetParent(parent.transform, false);

            Text text = uiText.GetComponent<Text>();
            //uiText.transform.GetChild(0).GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(Font.Il2CppType, "Arial.ttf"); // Invalid Cast
            text.color = HTMLString2Color(textColor).Unbox<Color32>();

            return uiText;
        }
#endif
        #endregion

        #region[ICalls]
#if DEBUG

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

            return allObjectsList;
        }

#endif
        #endregion

        #region[Dump All Scenes GameObjects]

#if DEBUG
        private static string dumpLog = "";
        public static void DumpAll(Il2CppSystem.Collections.Generic.List<GameObject> rootObjects)
        {
            BepInExLoader.log.LogMessage("Dumping Objects...");

            foreach (GameObject obj in rootObjects)
            {
                dumpLog = "";
                level = 1;
                prevlevel = 0;

                // Dump this object
                BepInExLoader.log.LogMessage("[GameObject]: " + obj.name);
                dumpLog += "[GameObject]: " + obj.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    BepInExLoader.log.LogMessage("  [Components]:");
                    dumpLog += "  [Components]:\r\n";

                    var comps = obj.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        BepInExLoader.log.LogMessage("    " + comp.Name);
                        dumpLog += "    " + comp.Name + "\r\n";
                    }

                    dumpLog += "\r\n";
                }
                #endregion

                // Dump the children
                DisplayChildren(obj.transform);

                if (dumpLog != "")
                {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\")) { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\"); }
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\" + obj.name + "_DUMP.txt", dumpLog);
                }

                BepInExLoader.log.LogMessage("Dump Complete!");
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
                BepInExLoader.log.LogMessage(consoleprefix + "[GameObject]: " + t.gameObject.name);
                dumpLog += consoleprefix + "[GameObject]: " + t.gameObject.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    BepInExLoader.log.LogMessage(consoleprefix + "  [Components]:");
                    dumpLog += consoleprefix + "  [Components]:\r\n";

                    var comps = t.gameObject.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        BepInExLoader.log.LogMessage(consoleprefix + "    " + comp.Name);
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

        public static void DumpGameObject(this GameObject obj)
        {
            if (obj == null)
                return;

            Il2CppSystem.Collections.Generic.List<GameObject> tmpList = new Il2CppSystem.Collections.Generic.List<GameObject>();
            tmpList.Add(obj);
            TrainerComponent.DumpAll(tmpList);
        }

    }
#endif

    #endregion
}



