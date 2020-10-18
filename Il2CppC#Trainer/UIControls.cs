
using System;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Trainer
{
    #region[Delegates]

    internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

    internal delegate bool parseHTMLString(IntPtr HTMLString, IntPtr result);

    #endregion

    public class UIControls : MonoBehaviour
    {
        public UIControls(IntPtr ptr) : base(ptr) { }

        #region[Elements]

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject gameObject = new GameObject(name);
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return gameObject;
        }

        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.AddComponent<RectTransform>();
            UIControls.SetParentAndAlign(gameObject, parent);
            return gameObject;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            lbl.color = UIControls.s_TextColor;
            lbl.AssignDefaultFont();
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (!(parent == null))
            {
                child.transform.SetParent(parent.transform, false);
                UIControls.SetLayerRecursively(child, parent.layer);
            }
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                UIControls.SetLayerRecursively(transform.GetChild(i).gameObject, layer);
            }
        }

        public static GameObject CreatePanel(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Panel", UIControls.s_ThickElementSize);
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.anchoredPosition = Vector2.zero;
            component.sizeDelta = Vector2.zero;
            Image image = gameObject.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_PanelColor;
            return gameObject;
        }

        public static GameObject CreateButton(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Button", UIControls.s_ThickElementSize);
            GameObject gameObject2 = new GameObject("Text");
            gameObject2.AddComponent<RectTransform>();
            UIControls.SetParentAndAlign(gameObject2, gameObject);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_DefaultSelectableColor;
            Button defaultColorTransitionValues = gameObject.AddComponent<Button>();
            UIControls.SetDefaultColorTransitionValues(defaultColorTransitionValues);
            Text text = gameObject2.AddComponent<Text>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            UIControls.SetDefaultTextValues(text);
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.sizeDelta = Vector2.zero;
            return gameObject;
        }

        public static GameObject CreateText(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Text", UIControls.s_ThickElementSize);
            Text text = gameObject.AddComponent<Text>();
            text.text = "New Text";
            UIControls.SetDefaultTextValues(text);
            return gameObject;
        }

        public static GameObject CreateImage(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Image", UIControls.s_ImageElementSize);
            gameObject.AddComponent<Image>();
            return gameObject;
        }

        public static GameObject CreateRawImage(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("RawImage", UIControls.s_ImageElementSize);
            gameObject.AddComponent<RawImage>();
            return gameObject;
        }

        public static GameObject CreateSlider(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Slider", UIControls.s_ThinElementSize);
            GameObject gameObject2 = UIControls.CreateUIObject("Background", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Fill Area", gameObject);
            GameObject gameObject4 = UIControls.CreateUIObject("Fill", gameObject3);
            GameObject gameObject5 = UIControls.CreateUIObject("Handle Slide Area", gameObject);
            GameObject gameObject6 = UIControls.CreateUIObject("Handle", gameObject5);
            Image image = gameObject2.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_DefaultSelectableColor;
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(0f, 0.25f);
            component.anchorMax = new Vector2(1f, 0.75f);
            component.sizeDelta = new Vector2(0f, 0f);
            RectTransform component2 = gameObject3.GetComponent<RectTransform>();
            component2.anchorMin = new Vector2(0f, 0.25f);
            component2.anchorMax = new Vector2(1f, 0.75f);
            component2.anchoredPosition = new Vector2(-5f, 0f);
            component2.sizeDelta = new Vector2(-20f, 0f);
            Image image2 = gameObject4.AddComponent<Image>();
            image2.sprite = resources.standard;
            image2.type = Image.Type.Sliced;
            image2.color = UIControls.s_DefaultSelectableColor;
            RectTransform component3 = gameObject4.GetComponent<RectTransform>();
            component3.sizeDelta = new Vector2(10f, 0f);
            RectTransform component4 = gameObject5.GetComponent<RectTransform>();
            component4.sizeDelta = new Vector2(-20f, 0f);
            component4.anchorMin = new Vector2(0f, 0f);
            component4.anchorMax = new Vector2(1f, 1f);
            Image image3 = gameObject6.AddComponent<Image>();
            image3.sprite = resources.knob;
            image3.color = UIControls.s_DefaultSelectableColor;
            RectTransform component5 = gameObject6.GetComponent<RectTransform>();
            component5.sizeDelta = new Vector2(20f, 0f);
            Slider slider = gameObject.AddComponent<Slider>();
            slider.fillRect = gameObject4.GetComponent<RectTransform>();
            slider.handleRect = gameObject6.GetComponent<RectTransform>();
            slider.targetGraphic = image3;
            slider.direction = Slider.Direction.LeftToRight;
            UIControls.SetDefaultColorTransitionValues(slider);
            return gameObject;
        }

        public static GameObject CreateScrollbar(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Scrollbar", UIControls.s_ThinElementSize);
            GameObject gameObject2 = UIControls.CreateUIObject("Sliding Area", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Handle", gameObject2);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_DefaultSelectableColor;
            Image image2 = gameObject3.AddComponent<Image>();
            image2.sprite = resources.standard;
            image2.type = Image.Type.Sliced;
            image2.color = UIControls.s_DefaultSelectableColor;
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(-20f, -20f);
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            RectTransform component2 = gameObject3.GetComponent<RectTransform>();
            component2.sizeDelta = new Vector2(20f, 20f);
            Scrollbar scrollbar = gameObject.AddComponent<Scrollbar>();
            scrollbar.handleRect = component2;
            scrollbar.targetGraphic = image2;
            UIControls.SetDefaultColorTransitionValues(scrollbar);
            return gameObject;
        }

        public static GameObject CreateToggle(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Toggle", UIControls.s_ThinElementSize);
            GameObject gameObject2 = UIControls.CreateUIObject("Background", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Checkmark", gameObject2);
            GameObject gameObject4 = UIControls.CreateUIObject("Label", gameObject);
            Toggle toggle = gameObject.AddComponent<Toggle>();
            toggle.isOn = true;
            Image image = gameObject2.AddComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_DefaultSelectableColor;
            Image image2 = gameObject3.AddComponent<Image>();
            image2.sprite = resources.checkmark;
            Text text = gameObject4.AddComponent<Text>();
            text.text = "Toggle";
            UIControls.SetDefaultTextValues(text);
            toggle.graphic = image2;
            toggle.targetGraphic = image;
            UIControls.SetDefaultColorTransitionValues(toggle);
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(0f, 1f);
            component.anchorMax = new Vector2(0f, 1f);
            component.anchoredPosition = new Vector2(10f, -10f);
            component.sizeDelta = new Vector2(20f, 20f);
            RectTransform component2 = gameObject3.GetComponent<RectTransform>();
            component2.anchorMin = new Vector2(0.5f, 0.5f);
            component2.anchorMax = new Vector2(0.5f, 0.5f);
            component2.anchoredPosition = Vector2.zero;
            component2.sizeDelta = new Vector2(20f, 20f);
            RectTransform component3 = gameObject4.GetComponent<RectTransform>();
            component3.anchorMin = new Vector2(0f, 0f);
            component3.anchorMax = new Vector2(1f, 1f);
            component3.offsetMin = new Vector2(23f, 1f);
            component3.offsetMax = new Vector2(-5f, -2f);
            return gameObject;
        }

        public static GameObject CreateInputField(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("InputField", UIControls.s_ThickElementSize);
            GameObject gameObject2 = UIControls.CreateUIObject("Placeholder", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Text", gameObject);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = resources.inputField;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_DefaultSelectableColor;
            InputField inputField = gameObject.AddComponent<InputField>();
            UIControls.SetDefaultColorTransitionValues(inputField);
            Text text = gameObject3.AddComponent<Text>();
            text.text = "";
            text.supportRichText = false;
            UIControls.SetDefaultTextValues(text);
            Text text2 = gameObject2.AddComponent<Text>();
            text2.text = "Enter text...";
            text2.fontStyle = FontStyle.Italic;
            Color color = text.color;
            color.a *= 0.5f;
            text2.color = color;
            RectTransform component = gameObject3.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.sizeDelta = Vector2.zero;
            component.offsetMin = new Vector2(10f, 6f);
            component.offsetMax = new Vector2(-10f, -7f);
            RectTransform component2 = gameObject2.GetComponent<RectTransform>();
            component2.anchorMin = Vector2.zero;
            component2.anchorMax = Vector2.one;
            component2.sizeDelta = Vector2.zero;
            component2.offsetMin = new Vector2(10f, 6f);
            component2.offsetMax = new Vector2(-10f, -7f);
            inputField.textComponent = text;
            inputField.placeholder = text2;
            return gameObject;
        }

        public static GameObject CreateDropdown(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Dropdown", UIControls.s_ThickElementSize);
            GameObject gameObject2 = UIControls.CreateUIObject("Label", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Arrow", gameObject);
            GameObject gameObject4 = UIControls.CreateUIObject("Template", gameObject);
            GameObject gameObject5 = UIControls.CreateUIObject("Viewport", gameObject4);
            GameObject gameObject6 = UIControls.CreateUIObject("Content", gameObject5);
            GameObject gameObject7 = UIControls.CreateUIObject("Item", gameObject6);
            GameObject gameObject8 = UIControls.CreateUIObject("Item Background", gameObject7);
            GameObject gameObject9 = UIControls.CreateUIObject("Item Checkmark", gameObject7);
            GameObject gameObject10 = UIControls.CreateUIObject("Item Label", gameObject7);
            GameObject gameObject11 = UIControls.CreateScrollbar(resources);
            gameObject11.name = "Scrollbar";
            UIControls.SetParentAndAlign(gameObject11, gameObject4);
            Scrollbar component = gameObject11.GetComponent<Scrollbar>();
            component.SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform component2 = gameObject11.GetComponent<RectTransform>();
            component2.anchorMin = Vector2.right;
            component2.anchorMax = Vector2.one;
            component2.pivot = Vector2.one;
            component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
            Text text = gameObject10.AddComponent<Text>();
            UIControls.SetDefaultTextValues(text);
            text.alignment = TextAnchor.MiddleLeft;
            Image image = gameObject8.AddComponent<Image>();
            image.color = new Color32(245, 245, 245, byte.MaxValue);
            Image image2 = gameObject9.AddComponent<Image>();
            image2.sprite = resources.checkmark;
            Toggle toggle = gameObject7.AddComponent<Toggle>();
            toggle.targetGraphic = image;
            toggle.graphic = image2;
            toggle.isOn = true;
            Image image3 = gameObject4.AddComponent<Image>();
            image3.sprite = resources.standard;
            image3.type = Image.Type.Sliced;
            ScrollRect scrollRect = gameObject4.AddComponent<ScrollRect>();
            scrollRect.content = (RectTransform)gameObject6.transform;
            scrollRect.viewport = (RectTransform)gameObject5.transform;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.verticalScrollbar = component;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing = -3f;
            Mask mask = gameObject5.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image image4 = gameObject5.AddComponent<Image>();
            image4.sprite = resources.mask;
            image4.type = Image.Type.Sliced;
            Text text2 = gameObject2.AddComponent<Text>();
            UIControls.SetDefaultTextValues(text2);
            text2.alignment = TextAnchor.MiddleLeft;
            Image image5 = gameObject3.AddComponent<Image>();
            image5.sprite = resources.dropdown;
            Image image6 = gameObject.AddComponent<Image>();
            image6.sprite = resources.standard;
            image6.color = UIControls.s_DefaultSelectableColor;
            image6.type = Image.Type.Sliced;
            Dropdown dropdown = gameObject.AddComponent<Dropdown>();
            dropdown.targetGraphic = image6;
            UIControls.SetDefaultColorTransitionValues(dropdown);
            dropdown.template = gameObject4.GetComponent<RectTransform>();
            dropdown.captionText = text2;
            dropdown.itemText = text;
            text.text = "Option A";
            dropdown.options.Add(new Dropdown.OptionData
            {
                text = "Option A"
            });
            dropdown.options.Add(new Dropdown.OptionData
            {
                text = "Option B"
            });
            dropdown.options.Add(new Dropdown.OptionData
            {
                text = "Option C"
            });
            dropdown.RefreshShownValue();
            RectTransform component3 = gameObject2.GetComponent<RectTransform>();
            component3.anchorMin = Vector2.zero;
            component3.anchorMax = Vector2.one;
            component3.offsetMin = new Vector2(10f, 6f);
            component3.offsetMax = new Vector2(-25f, -7f);
            RectTransform component4 = gameObject3.GetComponent<RectTransform>();
            component4.anchorMin = new Vector2(1f, 0.5f);
            component4.anchorMax = new Vector2(1f, 0.5f);
            component4.sizeDelta = new Vector2(20f, 20f);
            component4.anchoredPosition = new Vector2(-15f, 0f);
            RectTransform component5 = gameObject4.GetComponent<RectTransform>();
            component5.anchorMin = new Vector2(0f, 0f);
            component5.anchorMax = new Vector2(1f, 0f);
            component5.pivot = new Vector2(0.5f, 1f);
            component5.anchoredPosition = new Vector2(0f, 2f);
            component5.sizeDelta = new Vector2(0f, 150f);
            RectTransform component6 = gameObject5.GetComponent<RectTransform>();
            component6.anchorMin = new Vector2(0f, 0f);
            component6.anchorMax = new Vector2(1f, 1f);
            component6.sizeDelta = new Vector2(-18f, 0f);
            component6.pivot = new Vector2(0f, 1f);
            RectTransform component7 = gameObject6.GetComponent<RectTransform>();
            component7.anchorMin = new Vector2(0f, 1f);
            component7.anchorMax = new Vector2(1f, 1f);
            component7.pivot = new Vector2(0.5f, 1f);
            component7.anchoredPosition = new Vector2(0f, 0f);
            component7.sizeDelta = new Vector2(0f, 28f);
            RectTransform component8 = gameObject7.GetComponent<RectTransform>();
            component8.anchorMin = new Vector2(0f, 0.5f);
            component8.anchorMax = new Vector2(1f, 0.5f);
            component8.sizeDelta = new Vector2(0f, 20f);
            RectTransform component9 = gameObject8.GetComponent<RectTransform>();
            component9.anchorMin = Vector2.zero;
            component9.anchorMax = Vector2.one;
            component9.sizeDelta = Vector2.zero;
            RectTransform component10 = gameObject9.GetComponent<RectTransform>();
            component10.anchorMin = new Vector2(0f, 0.5f);
            component10.anchorMax = new Vector2(0f, 0.5f);
            component10.sizeDelta = new Vector2(20f, 20f);
            component10.anchoredPosition = new Vector2(10f, 0f);
            RectTransform component11 = gameObject10.GetComponent<RectTransform>();
            component11.anchorMin = Vector2.zero;
            component11.anchorMax = Vector2.one;
            component11.offsetMin = new Vector2(20f, 1f);
            component11.offsetMax = new Vector2(-10f, -2f);
            gameObject4.SetActive(false);
            return gameObject;
        }

        public static GameObject CreateScrollView(UIControls.Resources resources)
        {
            GameObject gameObject = UIControls.CreateUIElementRoot("Scroll View", new Vector2(200f, 200f));
            GameObject gameObject2 = UIControls.CreateUIObject("Viewport", gameObject);
            GameObject gameObject3 = UIControls.CreateUIObject("Content", gameObject2);
            GameObject gameObject4 = UIControls.CreateScrollbar(resources);
            gameObject4.name = "Scrollbar Horizontal";
            UIControls.SetParentAndAlign(gameObject4, gameObject);
            RectTransform component = gameObject4.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.right;
            component.pivot = Vector2.zero;
            component.sizeDelta = new Vector2(0f, component.sizeDelta.y);
            GameObject gameObject5 = UIControls.CreateScrollbar(resources);
            gameObject5.name = "Scrollbar Vertical";
            UIControls.SetParentAndAlign(gameObject5, gameObject);
            gameObject5.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform component2 = gameObject5.GetComponent<RectTransform>();
            component2.anchorMin = Vector2.right;
            component2.anchorMax = Vector2.one;
            component2.pivot = Vector2.one;
            component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
            RectTransform component3 = gameObject2.GetComponent<RectTransform>();
            component3.anchorMin = Vector2.zero;
            component3.anchorMax = Vector2.one;
            component3.sizeDelta = Vector2.zero;
            component3.pivot = Vector2.up;
            RectTransform component4 = gameObject3.GetComponent<RectTransform>();
            component4.anchorMin = Vector2.up;
            component4.anchorMax = Vector2.one;
            component4.sizeDelta = new Vector2(0f, 300f);
            component4.pivot = Vector2.up;
            ScrollRect scrollRect = gameObject.AddComponent<ScrollRect>();
            scrollRect.content = component4;
            scrollRect.viewport = component3;
            scrollRect.horizontalScrollbar = gameObject4.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = gameObject5.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3f;
            scrollRect.verticalScrollbarSpacing = -3f;
            Image image = gameObject.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = UIControls.s_PanelColor;
            Mask mask = gameObject2.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image image2 = gameObject2.AddComponent<Image>();
            image2.sprite = resources.mask;
            image2.type = Image.Type.Sliced;
            return gameObject;
        }

        private const float kWidth = 160f;

        private const float kThickHeight = 30f;

        private const float kThinHeight = 20f;

        private static Vector2 s_ThickElementSize = new Vector2(160f, 30f);

        private static Vector2 s_ThinElementSize = new Vector2(160f, 20f);

        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);

        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);

        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);

        private static Color s_TextColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

        public struct Resources
        {
            public Sprite standard;

            public Sprite background;

            public Sprite inputField;

            public Sprite knob;

            public Sprite checkmark;

            public Sprite dropdown;

            public Sprite mask;
        }

        #endregion

        #region[ICall Tests]

        // TryParseHTMLString ICall - Isn't working right yet. Color is always RGBA(0,0,0,0)
        /* 
        internal static parseHTMLString ParseHTMLString_iCall = IL2CPP.ResolveICall<parseHTMLString>("UnityEngine.ColorUtility::DoTryParseHtmlColor");
        public static bool TryParseHtmlString(Il2CppSystem.String htmlString)
        {
            Color32 tmp = new Color32();

            bool result = ParseHTMLString_iCall(htmlString.Pointer, StructToPtr(tmp));

            BepInExLoader.log.LogMessage("[Trainer] TryParseHTMLString: Result: " + result.ToString()  + " HTML: " + htmlString + " Color = R: " + tmp.r.ToString() + " G: " + tmp.g.ToString() + " B: " + tmp.b.ToString() + " A: " + tmp.a.ToString());

            return result;
        }

        public static IntPtr StructToPtr(object obj)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }
        */
        #endregion
    }

    public static class Extensions
    {
        // Load Image ICall
        internal static d_LoadImage LoadImage_iCall = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
        public static bool LoadImage(this Texture2D tex, byte[] data, bool markNonReadable)
        {
            var il2cppArray = new Il2CppStructArray<byte>(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                il2cppArray[i] = data[i];
            }

            return LoadImage_iCall(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        // Convert Hex string to Color32
        public static Il2CppSystem.Object HexToColor(this string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r = 0, g = 0, b = 0, a = 0;

            #region[Another way but Il2Cpp doesn't like it]
            /*
            r = int.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            g = int.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            b = int.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            if (hexString.Length == 8)
                a = int.Parse(hexString.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            */
            #endregion
            r = Convert.ToByte(hexString.Substring(0, 2), 16);
            g = Convert.ToByte(hexString.Substring(2, 2), 16);
            b = Convert.ToByte(hexString.Substring(4, 2), 16);
            if (hexString.Length == 8)
                a = Convert.ToByte(hexString.Substring(6, 2), 16);

            //BepInExLoader.log.LogMessage("[Trainer] HexString: " + hexString + " RGBA(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + " ," + a.ToString() + ")");

            return (new Color32(r, g, b, a)).BoxIl2CppObject();
        }

    }
}
