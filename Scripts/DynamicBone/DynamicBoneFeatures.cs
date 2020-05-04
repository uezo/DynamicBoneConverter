using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;


namespace ChatdollKit
{
    // Methods
    public partial class DynamicBoneConverter : MonoBehaviour
    {
        private static string dynamicBoneQSaveFile = "Resources/DynamicBoneConverter/DynamicBoneQSaveData.asset";

        // DynamicBone
        public DynamicBoneConfiguration ExtractDynamicBones()
        {
            return new DynamicBoneConfiguration(gameObject);
        }

        public void ExportDynamicBonesToJson()
        {
            var fileName = $"{gameObject.name}_DB{DateTime.Now:yyyyMMddHHmmss}";
            var path = EditorUtility.SaveFilePanel("Export DynamicBones as JSON", string.Empty, fileName, "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Debug.Log($"Serializing all DynamicBones and DynamicBoneColliders");
                var configJson = JsonConvert.SerializeObject(ExtractDynamicBones());
                Debug.Log($"Writing JSON: {path}");
                File.WriteAllText(path, configJson);

                Debug.Log("Export DynamicBones and DynamicBoneColliders completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurs in exporting DynamicBones to JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void ExportDynamicBonesToAsset()
        {
            Debug.Log("Quick save DynamicBone data to asset");
            CreateDirectories(dynamicBoneQSaveFile, true);
            DynamicBoneQSaveData.QuickSave(ExtractDynamicBones(), Path.Combine(GetAssetDirectroyName(), dynamicBoneQSaveFile));
            Debug.Log("Quick save completed");
        }

        public void ImportDynamicBonesFromJson()
        {
            var path = EditorUtility.OpenFilePanel("Import DynamicBones from JSON", string.Empty, "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Debug.Log($"Reading JSON from: {path}");
                var configJson = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<DynamicBoneConfiguration>(configJson);

                // Add colliders before bones
                Debug.Log($"DynamicBoneColliders to import: {config.DynamicBoneColliders.Count}");
                foreach (var collider in config.DynamicBoneColliders)
                {
                    collider.ToDynamicBoneCollider(gameObject);
                }

                // Add bones
                Debug.Log($"DynamicBones to import: {config.DynamicBones.Count}");
                foreach (var bone in config.DynamicBones)
                {
                    bone.ToDynamicBone(gameObject);
                }

                Debug.Log("Import DynamicBones and DynamicBoneColliders completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurs in importing DynamicBones from JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void ImportDynamicBonesFromAsset()
        {
            Debug.Log("Quick load DynamicBone data from asset");
            DynamicBoneQSaveData.QuickLoad(gameObject, Path.Combine(GetAssetDirectroyName(), dynamicBoneQSaveFile));
            Debug.Log("Quick load completed");
        }

        public void RemoveDynamicBones()
        {
            var colliders = gameObject.GetComponentsInChildren<DynamicBoneCollider>();
            if (colliders != null)
            {
                Debug.Log($"DynamicBoneColliders to remove: {colliders.Length}");
                foreach (var collider in colliders)
                {
                    DestroyImmediate(collider);
                }
            }

            var bones = gameObject.GetComponentsInChildren<DynamicBone>();
            if (bones != null)
            {
                Debug.Log($"DynamicBones to remove: {bones.Length}");
                foreach (var bone in bones)
                {
                    DestroyImmediate(bone);
                }
            }

            Debug.Log("Remove all DynamicBones and DynamicBoneColliders completed");
        }
    }


    // Models
    public class DynamicBoneConfiguration
    {
        public List<SerializableDynamicBone> DynamicBones = new List<SerializableDynamicBone>();
        public List<SerializableDynamicBoneCollider> DynamicBoneColliders = new List<SerializableDynamicBoneCollider>();

        [JsonConstructor]
        public DynamicBoneConfiguration()
        {

        }

        public DynamicBoneConfiguration(GameObject gameObject)
        {
            foreach (var bone in gameObject.GetComponentsInChildren<DynamicBone>())
            {
                DynamicBones.Add(SerializableDynamicBone.FromDynamicBone(bone));
            }

            foreach (var collider in gameObject.GetComponentsInChildren<DynamicBoneCollider>())
            {
                DynamicBoneColliders.Add(SerializableDynamicBoneCollider.FromDynamicBoneCollider(collider));
            }
        }
    }

    [Serializable]
    public class SerializableDynamicBone
    {
        [SerializeField]
        public string AttachedTo;

        [SerializeField]
        public string m_Root = null;

        [SerializeField]
        public float m_UpdateRate = 60;

        [SerializeField]
        public DynamicBone.UpdateMode m_UpdateMode = DynamicBone.UpdateMode.Normal;

        [SerializeField, Range(0, 1)]
        public float m_Damping = 0.1f;

        [SerializeField]
        public AnimationCurve m_DampingDistrib = null;

        [SerializeField, Range(0, 1)]
        public float m_Elasticity = 0.1f;

        [SerializeField]
        public AnimationCurve m_ElasticityDistrib = null;

        [SerializeField, Range(0, 1)]
        public float m_Stiffness = 0.1f;

        [SerializeField]
        public AnimationCurve m_StiffnessDistrib = null;

        [SerializeField, Range(0, 1)]
        public float m_Inert = 0;

        [SerializeField]
        public AnimationCurve m_InertDistrib = null;

        [SerializeField]
        public float m_Radius = 0;

        [SerializeField]
        public AnimationCurve m_RadiusDistrib = null;

        [SerializeField]
        public float m_EndLength = 0;

        [SerializeField]
        public Vector3 m_EndOffset = Vector3.zero;

        [SerializeField]
        public Vector3 m_Gravity = Vector3.zero;

        [SerializeField]
        public Vector3 m_Force = Vector3.zero;

        [SerializeField]
        public List<string> m_Colliders = new List<string>();

        [SerializeField]
        public List<string> m_Exclusions = new List<string>();

        [SerializeField]
        public DynamicBone.FreezeAxis m_FreezeAxis = DynamicBone.FreezeAxis.None;

        [SerializeField]
        public bool m_DistantDisable = false;

        [SerializeField]
        public string m_ReferenceObject = null;

        [SerializeField]
        public float m_DistanceToObject = 20;

        [SerializeField]
        public Vector3 TransformScale = Vector3.zero;

        public SerializableDynamicBone()
        {

        }

        public static SerializableDynamicBone FromDynamicBone(DynamicBone bone)
        {
            Debug.Log($"Convert DynamicBone to serializable: Root={bone.m_Root?.name}");

            var serializableBone = new SerializableDynamicBone();
            serializableBone.AttachedTo = HierarchyPath.FromTransform(bone.gameObject.transform);
            serializableBone.TransformScale = bone.gameObject.transform.lossyScale;

            // Copy values
            serializableBone.m_Root = HierarchyPath.FromTransform(bone.m_Root);
            serializableBone.m_UpdateRate = bone.m_UpdateRate;
            serializableBone.m_UpdateMode = bone.m_UpdateMode;
            serializableBone.m_Damping = bone.m_Damping;
            serializableBone.m_DampingDistrib = bone.m_DampingDistrib;
            serializableBone.m_Elasticity = bone.m_Elasticity;
            serializableBone.m_ElasticityDistrib = bone.m_ElasticityDistrib;
            serializableBone.m_Stiffness = bone.m_Stiffness;
            serializableBone.m_StiffnessDistrib = bone.m_StiffnessDistrib;
            serializableBone.m_Inert = bone.m_Inert;
            serializableBone.m_InertDistrib = bone.m_InertDistrib;
            serializableBone.m_Radius = bone.m_Radius;
            serializableBone.m_RadiusDistrib = bone.m_RadiusDistrib;
            serializableBone.m_EndLength = bone.m_EndLength;
            serializableBone.m_EndOffset = bone.m_EndOffset;
            serializableBone.m_Gravity = bone.m_Gravity;
            serializableBone.m_Force = bone.m_Force;
            if (bone.m_Colliders != null)
            {
                serializableBone.m_Colliders = bone.m_Colliders.Select(c => HierarchyPath.FromTransform(c.gameObject.transform)).ToList();
            }
            if (bone.m_Exclusions != null)
            {
                serializableBone.m_Exclusions = bone.m_Exclusions.Select(e => HierarchyPath.FromTransform(e)).ToList();
            }
            serializableBone.m_FreezeAxis = bone.m_FreezeAxis;
            serializableBone.m_DistantDisable = bone.m_DistantDisable;
            serializableBone.m_ReferenceObject = HierarchyPath.FromTransform(bone.m_ReferenceObject);
            serializableBone.m_DistanceToObject = bone.m_DistanceToObject;

            return serializableBone;
        }

        public DynamicBone ToDynamicBone(GameObject gameObject)
        {
            var targetObject = HierarchyPath.ToGameObject(AttachedTo, gameObject);
            var attachedBone = targetObject.AddComponent<DynamicBone>();

            // Copy values
            attachedBone.m_Root = HierarchyPath.ToTransform(m_Root, gameObject);
            attachedBone.m_UpdateRate = m_UpdateRate;
            attachedBone.m_UpdateMode = m_UpdateMode;
            attachedBone.m_Damping = m_Damping;
            attachedBone.m_DampingDistrib = m_DampingDistrib;
            attachedBone.m_Elasticity = m_Elasticity;
            attachedBone.m_ElasticityDistrib = m_ElasticityDistrib;
            attachedBone.m_Stiffness = m_Stiffness;
            attachedBone.m_StiffnessDistrib = m_StiffnessDistrib;
            attachedBone.m_Inert = m_Inert;
            attachedBone.m_InertDistrib = m_InertDistrib;
            attachedBone.m_Radius = m_Radius;
            attachedBone.m_RadiusDistrib = m_RadiusDistrib;
            attachedBone.m_EndLength = m_EndLength;
            attachedBone.m_EndOffset = m_EndOffset;
            attachedBone.m_Gravity = m_Gravity;
            attachedBone.m_Force = m_Force;
            attachedBone.m_Colliders = new List<DynamicBoneColliderBase>();
            foreach (var colliderAttachedObject in HierarchyPath.ToGameObjects(m_Colliders, gameObject))
            {
                var colliders = colliderAttachedObject.GetComponents<DynamicBoneCollider>();
                if (colliders != null)
                {
                    foreach (var collider in colliders)
                    {
                        attachedBone.m_Colliders.Add(collider);
                    }
                }
            }
            attachedBone.m_Exclusions = HierarchyPath.ToTransforms(m_Exclusions, gameObject);
            attachedBone.m_FreezeAxis = m_FreezeAxis;
            attachedBone.m_DistantDisable = m_DistantDisable;
            attachedBone.m_ReferenceObject = HierarchyPath.ToTransform(m_ReferenceObject, gameObject);
            attachedBone.m_DistanceToObject = m_DistanceToObject;

            return attachedBone;
        }
    }

    [Serializable]
    public class SerializableDynamicBoneCollider
    {
        [SerializeField]
        public string AttachedTo;

        [SerializeField]
        public DynamicBoneColliderBase.Direction m_Direction = DynamicBoneColliderBase.Direction.Y;

        [SerializeField]
        public Vector3 m_Center = Vector3.zero;

        [SerializeField]
        public DynamicBoneColliderBase.Bound m_Bound = DynamicBoneColliderBase.Bound.Outside;

        [SerializeField]
        public float m_Radius = 0.5f;

        [SerializeField]
        public float m_Height = 0;

        [SerializeField]
        public Vector3 TransformScale = Vector3.zero;

        public SerializableDynamicBoneCollider()
        {

        }

        public static SerializableDynamicBoneCollider FromDynamicBoneCollider(DynamicBoneCollider collider)
        {
            Debug.Log($"Convert DynamicBoneCollider to serializable: {collider.name}");

            var serializableCollider = new SerializableDynamicBoneCollider();
            serializableCollider.TransformScale = collider.transform.lossyScale;
            serializableCollider.AttachedTo = HierarchyPath.FromTransform(collider.transform);

            // Copy values
            serializableCollider.m_Direction = collider.m_Direction;
            serializableCollider.m_Center = collider.m_Center;
            serializableCollider.m_Bound = collider.m_Bound;
            serializableCollider.m_Radius = collider.m_Radius;
            serializableCollider.m_Height = collider.m_Height;

            return serializableCollider;
        }

        public DynamicBoneCollider ToDynamicBoneCollider(GameObject gameObject)
        {
            var targetObject = HierarchyPath.ToGameObject(AttachedTo, gameObject);
            if (targetObject == null)
            {
                Debug.LogError($"GameObject to attatch collider is not found: {HierarchyPath.ReplaceRoot(AttachedTo, gameObject.name)}");
                return null;
            }
            var attachedCollider = targetObject.AddComponent<DynamicBoneCollider>();

            // Todo: apply transform scale ratio
            // Copy values
            attachedCollider.m_Center = m_Center;
            attachedCollider.m_Radius = m_Radius;
            attachedCollider.m_Direction = m_Direction;
            attachedCollider.m_Bound = m_Bound;
            attachedCollider.m_Height = m_Height;

            return attachedCollider;
        }
    }


    // Editor
    public partial class DynamicBoneConverterEditor : Editor
    {
        public void DynamicBoneGUI(DynamicBoneConverter executionScript)
        {
            // Import / Export
            GUILayout.Label("Data", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("JSON");
            if (GUILayout.Button("Export"))
            {
                executionScript.ExportDynamicBonesToJson();
            }
            else if (GUILayout.Button("Import"))
            {
                executionScript.ImportDynamicBonesFromJson();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Asset");
            if (GUILayout.Button("Q.Save"))
            {
                executionScript.ExportDynamicBonesToAsset();
            }
            else if (GUILayout.Button("Q.Load"))
            {
                executionScript.ImportDynamicBonesFromAsset();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Remove");
            if (GUILayout.Button("Remove all DynamicBones"))
            {
                if (EditorUtility.DisplayDialog("Remove DynamicBones", "Are you sure to remove all DynamicBones?", "OK", "Cancel"))
                {
                    executionScript.RemoveDynamicBones();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
