using UnityEngine;

namespace Helpers
{
    public static class HelperCanvasGroup  
    {
        public static bool IsEnabled(this CanvasGroup canvasGroup)
        {
            return canvasGroup.alpha == 1;
        }
        
        public static void SetActive(this CanvasGroup canvasGroup, bool activeState)
        {
            canvasGroup.alpha = activeState ? 1 : 0;
            canvasGroup.blocksRaycasts = activeState;
            canvasGroup.interactable = activeState;
        }
        
        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.SetActive(true);
        }
        
        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.SetActive(false);
        }
    }
}
