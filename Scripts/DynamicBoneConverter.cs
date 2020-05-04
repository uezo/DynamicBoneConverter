using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace ChatdollKit
{
    public partial class DynamicBoneConverter : MonoBehaviour
    {
        // Check whether the directries exist and create if not exist
        public void CreateDirectories(string path, bool endsWithFilename = false)
        {
            var directries = path.Split('/');
            var currentPath = Application.dataPath;
            for (var i = 0; i < (endsWithFilename ? directries.Length - 1 : directries.Length); i++)
            {
                currentPath = Path.Combine(currentPath, directries[i]);
                Debug.Log(currentPath);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        // Get Asset directory name
        public string GetAssetDirectroyName()
        {
            var dirs = Application.dataPath.Split('/');
            return dirs[dirs.Length - 1];
        }
    }

    // Editor
    [CustomEditor(typeof(DynamicBoneConverter))]
    public partial class DynamicBoneConverterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var executionScript = target as DynamicBoneConverter;

            // Style
            var sectionHeaderStyle = new GUIStyle("ShurikenModuleTitle");
            sectionHeaderStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
            sectionHeaderStyle.border = new RectOffset(7, 7, 4, 4);
            sectionHeaderStyle.fixedHeight = 22;

            // GUI for each function
            var editorType = typeof(DynamicBoneConverterEditor);
            var dynamicBoneGUIMethod = editorType.GetMethod("DynamicBoneGUI", new Type[] { typeof(DynamicBoneConverter) });
            var springBoneGUIMethod = editorType.GetMethod("SpringBoneGUI", new Type[] { typeof(DynamicBoneConverter) });
            var converterGUIMethod = editorType.GetMethod("ConverterGUI", new Type[] { typeof(DynamicBoneConverter) });
            var guiArguments = new object[] { executionScript };

            // DynamicBone
            GUILayout.Label("DynamicBone", sectionHeaderStyle);
            if (dynamicBoneGUIMethod != null)
            {
                dynamicBoneGUIMethod.Invoke(this, guiArguments);
            }
            else
            {
                EditorGUILayout.HelpBox("DynamicBone doesn't exist. Import DynamicBone and DynamicBoneFeatures.cs to enable this feature.", MessageType.Info, true);
            }

            EditorGUILayout.Space();

            // VRM SpringBone
            GUILayout.Label("VRM SpringBone", sectionHeaderStyle);
            if (springBoneGUIMethod != null)
            {
                springBoneGUIMethod.Invoke(this, guiArguments);
            }
            else
            {
                EditorGUILayout.HelpBox("Import UniVRM and files in `VRMSpringBone` folder to enable this feature.", MessageType.Info, true);
            }

            EditorGUILayout.Space();

            // Converter
            GUILayout.Label("Converter", sectionHeaderStyle);
            if (converterGUIMethod != null && dynamicBoneGUIMethod != null && springBoneGUIMethod != null)
            {
                converterGUIMethod.Invoke(this, guiArguments);
            }
            else
            {
                EditorGUILayout.HelpBox("Import DynamicBone, UniVRM and all files in DynamicBoneConverter package to enable this feature.", MessageType.Info, true);
            }
        }
    }

    // Hierarchy path utility
    static class HierarchyPath
    {
        public static string FromTransform(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            var path = transform.gameObject.name;
            var parent = transform.parent;
            while (parent != null)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }

        public static string FromGameObject(GameObject gameObject)
        {
            return FromTransform(gameObject.transform);
        }

        public static GameObject ToGameObject(string path, GameObject rootObject = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (rootObject == null)
            {
                return GameObject.Find(path);
            }
            else
            {
                return GameObject.Find(ReplaceRoot(path, rootObject.name));
            }
        }

        public static List<GameObject> ToGameObjects(List<string> pathList, GameObject rootObject)
        {
            var gameObjects = new List<GameObject>();
            foreach (var path in pathList)
            {
                var gameObject = ToGameObject(path, rootObject);
                if (gameObject != null)
                {
                    gameObjects.Add(gameObject);
                }
            }
            return gameObjects;
        }

        public static Transform ToTransform(string path, GameObject rootObject = null)
        {
            var gameObject = ToGameObject(path, rootObject);
            if (gameObject != null)
            {
                return gameObject.transform;
            }
            return null;
        }

        public static List<Transform> ToTransforms(List<string> pathList, GameObject rootObject)
        {
            var transforms = new List<Transform>();
            foreach (var path in pathList)
            {
                var gameObject = ToGameObject(path, rootObject);
                if (gameObject != null)
                {
                    transforms.Add(gameObject.transform);
                }
            }
            return transforms;
        }

        public static string ReplaceRoot(string path, string rootName)
        {
            var splitPath = path.Split('/');
            splitPath[0] = rootName;
            return string.Join("/", splitPath);
        }
    }
}
