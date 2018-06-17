using UnityEngine;

namespace Core.Components
{
	public class SetDefaultAnchore : MonoBehaviour
	{
		private void Awake()
		{
			var rect = GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
		}
	}
}
