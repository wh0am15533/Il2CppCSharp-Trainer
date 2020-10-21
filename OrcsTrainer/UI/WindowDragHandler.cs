
using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;

namespace Trainer
{
	public class WindowDragHandler : MonoBehaviour
	{
		private const int NON_EXISTING_TOUCH = -98456;

		private static RectTransform rectTransform;

		private static int pointerId = NON_EXISTING_TOUCH;
		private static Vector2 initialTouchPos;

        public WindowDragHandler(IntPtr ptr) : base(ptr) { }

        public void Awake()
		{
			rectTransform = gameObject.GetComponent<RectTransform>();
		}

        [HarmonyPostfix]
        public static void OnBeginDrag(PointerEventData eventData)
		{
            if (pointerId != NON_EXISTING_TOUCH)
			{
				eventData.pointerDrag = null;
				return;
			}
            
            pointerId = eventData.pointerId;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, Camera.current, out initialTouchPos);
        }

        [HarmonyPostfix]
        public static void OnDrag(PointerEventData eventData)
		{
            // This is odd... On first drag, it doesn't appear to move. The more you drag the panel, the better the dragging works. ???

            if (eventData.pointerId != pointerId)
				return;

            Vector2 touchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, Camera.current, out touchPos);

            //rectTransform.anchoredPosition += touchPos - initialTouchPos; // This is VERY slow, barely moves!

            // This is better, but mouse needs clamping
            var tmp = touchPos - initialTouchPos;
            rectTransform.gameObject.transform.position += new Vector3(tmp.x, tmp.y, Camera.current.nearClipPlane);
        }

        [HarmonyPostfix]
        public static void OnEndDrag(PointerEventData eventData)
		{
			if(eventData.pointerId != pointerId)
				return;

			pointerId = NON_EXISTING_TOUCH;
		}

        #region[Dev Notes]

        // This works also and is quick, but the panel doesn't move until you release mouse button.
        //
        // Note: PointerEventData can be used as an object spy as it identifies objects under mouse.

        /* Either Hook EventTrigger or use this for Test Usage:
        if (EventSystem.current != null && Event.current.type == EventType.mouseDrag)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current.Pointer);
            if (canvas != null) { uiPanel.GetComponent<WindowDragHandler>().OnDrag(pointerEventData); }
        }
        */

        /*
        [HarmonyPostfix]
        public static void OnPointerDown(PointerEventData eventData)
        {
            //BepInExLoader.log.LogMessage("[Trainer] Down PointerEventData:\r\n" + eventData.ToString());

            ...set initial position 
        }

        [HarmonyPostfix]
        public static void OnPointerUp(PointerEventData eventData)
        {
            //BepInExLoader.log.LogMessage("[Trainer] Up PointerEventData:\r\n" + eventData.ToString());

            uiPanel.transform.position = new Vector3(eventData.position.x, eventData.position.y, Camera.current.nearClipPlane);
        }
        */

        #endregion
    }
}