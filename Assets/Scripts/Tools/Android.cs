using Tools.Patterns;
using UnityEngine;

namespace Tools
{
	public class Android : Singleton<Android> 
	{
		public void ShowToastMessage(string message)
		{
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			if (unityActivity != null)
			{
				var toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
						message, 0);
					toastObject.Call("show");
				}));
			}
		}
	}
}
