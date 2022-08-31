using UnityEngine;

namespace Utilities
{
    public static class UIUtils
    {
        public static void PlaceUIElement(Camera camera, RectTransform canvasRectTransform,
            RectTransform elementRectTransform, Vector3 worldPosition)
        {
            Vector2 uiOffset = new Vector2(canvasRectTransform.sizeDelta.x / 2f, canvasRectTransform.sizeDelta.y / 2f);
            
            Vector2 viewportPosition = camera.WorldToViewportPoint(worldPosition);
            
            Vector2 proportionalPosition = new Vector2(viewportPosition.x * canvasRectTransform.sizeDelta.x, 
                viewportPosition.y * canvasRectTransform.sizeDelta.y);
            
            elementRectTransform.localPosition = proportionalPosition - uiOffset;
        }
    }
}