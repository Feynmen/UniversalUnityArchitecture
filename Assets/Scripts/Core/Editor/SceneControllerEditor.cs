using Core.Controllers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneControllerBase),true)] 
[CanEditMultipleObjects]
public class SceneControllerEditor : Editor
{
    private SerializedProperty _sceneNameProperty;
    private void OnEnable()
    {
        _sceneNameProperty = serializedObject.FindProperty("_sceneName");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying)
        {
            var sceneName = (target as SceneControllerBase).gameObject.scene.name;
            if (!_sceneNameProperty.stringValue.Equals(sceneName))
            {
                _sceneNameProperty.stringValue = sceneName;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}