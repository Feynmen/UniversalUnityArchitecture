using UnityEngine;
using UnityEditor;

namespace Project.Editor
{
    public class DeleteAllPlayerprefs : MonoBehaviour
    {
        [MenuItem("Project/Clear All Data")]
        public static void ClearData()
        {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("All PlayerPrefs deleted");
        }
    }
}

