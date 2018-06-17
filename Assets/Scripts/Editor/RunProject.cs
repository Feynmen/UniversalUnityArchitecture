using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectEditor
{
    [InitializeOnLoadAttribute]
    public class RunProject
    {
        [MenuItem("Project/Run %&r")]
        private static void RunAllProject()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            if (EditorBuildSettings.scenes[0] != null)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
                EditorApplication.isPlaying = true;
            }            
        }
    }    
}

