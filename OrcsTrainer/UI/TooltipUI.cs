
// Currenty being implemented - WIP

/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Trainer.UI
{
    public class TooltipUI : MonoBehaviour
    {
        private static TooltipUI instance;

        //[SerializeField]
        private Camera uiCamera;
        //[SerializeField]
        private RectTransform canvasRectTransform;

        private Text tooltipText;
        private RectTransform backgroundRectTransform;
        private Func<string> getTooltipStringFunc; //ORIG

        public TooltipUI(IntPtr ptr) : base(ptr) { }

        private void Awake()
        {
            instance = this;

            // Make sure we have an active Camera
            uiCamera = Camera.main;
            if (uiCamera == null)
            {
                uiCamera = Camera.current;
                if (uiCamera == null) { Debug.LogError("TooltipUI - Main Camera not Found!"); }
            }

            // Find the canvas
            canvasRectTransform = GameObject.FindObjectOfType<Canvas>().gameObject.GetComponent<RectTransform>();
            //this.gameObject.transform.SetParent(canvasRectTransform, false); // Note: Set during LoadResources()
            transform.gameObject.layer = LayerMask.NameToLayer("UI");

            backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
            tooltipText = transform.gameObject.Find("text").GetComponent<Text>();

            //HideTooltip();
        }

        private void Update()
        {
            var check = PointerInteraction.instance.tmpHoverObject;
            if (check == null)
            {
                HideTooltip();
                return;
            }

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
            transform.localPosition = localPoint;

            SetText(PointerInteraction.instance.tmpHoverObject.name);
            //-SetText(getTooltipStringFunc());

            Vector2 anchoredPosition = transform.GetComponent<RectTransform>().anchoredPosition;
            if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
            {
                anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
            }
            if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
            {
                anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
            }

            // Adjustments
            anchoredPosition.x = anchoredPosition.x + 20f;
            anchoredPosition.y = anchoredPosition.y - 40f;

            transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        }

        private void ShowTooltip(string tooltipString)
        {
            ShowTooltip(() => tooltipString);
        }

        private void ShowTooltip(Func<string> getTooltipStringFunc)
        {
            gameObject.SetActive(true);
            transform.SetSiblingIndex(transform.childCount + 1);//.SetAsLastSibling();
            this.getTooltipStringFunc = getTooltipStringFunc;//
            Update();
        }

        private void SetText(string tooltipString)
        {
            tooltipText.text = tooltipString;
            float textPaddingSize = 4f;
            Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
            backgroundRectTransform.sizeDelta = backgroundSize;
        }

        private void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        public static void ShowTooltip_Static(string tooltipString)
        {
            instance.ShowTooltip(tooltipString);
        }

        public static void ShowTooltip_Static(Func<string> getTooltipStringFunc)
        {
            instance.ShowTooltip(getTooltipStringFunc);
        }

        public static void HideTooltip_Static()
        {
            instance.HideTooltip();
        }

        public static void AddTooltip(Transform transform, string tooltipString)
        {
            AddTooltip(transform, () => tooltipString);
        }

        public static void AddTooltip(Transform transform, Func<string> getTooltipStringFunc)
        {
            *//*
            if (transform.GetComponent<Button_UI>() != null)
            {
                transform.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () => TooltipUI.ShowTooltip_Static(getTooltipStringFunc);
                transform.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () => TooltipUI.HideTooltip_Static();
            }
            *//*
        }
    }

    public static class TooltipUI_Extensions
    {
        public static void AddTooltip(this Transform transform, string tooltipString)
        {
            transform.AddTooltip(() => tooltipString);
        }

        public static void AddTooltip(this Transform transform, Func<string> getTooltipStringFunc)
        {
            TooltipUI.AddTooltip(transform, getTooltipStringFunc);
        }
    }
}
*/