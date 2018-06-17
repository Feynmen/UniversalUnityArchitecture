using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public class SyncSolution : MonoBehaviour {

        [MenuItem("Project/Sync Solution #&s")]
        public static void Sync()
        {
            var editor = Type.GetType("UnityEditor.SyncVS, UnityEditor");
            var SyncSolution = editor.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            SyncSolution.Invoke(null, null);
            Debug.Log("Solution synced!");
        }
    }
}
