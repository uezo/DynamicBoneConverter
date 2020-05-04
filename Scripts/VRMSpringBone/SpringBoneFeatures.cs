using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRM;
using Newtonsoft.Json;


namespace ChatdollKit
{
    public partial class DynamicBoneConverter  : MonoBehaviour
    {
        private static string springBoneQSaveFile = "Resources/DynamicBoneConverter/SpringBoneQSaveData.asset";

        // Spring Bone
        public SpringBoneConfiguration ExtractSpringBones()
        {
            return new SpringBoneConfiguration(gameObject);
        }

        public void ExportSpringBonesToJson()
        {
            var fileName = $"{gameObject.name}_SB{DateTime.Now:yyyyMMddHHmmss}";
            var path = EditorUtility.SaveFilePanel("Export SpringBones as JSON", string.Empty, fileName, "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Debug.Log($"Serializing all SpringBones and SpringBoneColliderGroups");
                var configJson = JsonConvert.SerializeObject(ExtractSpringBones());
                Debug.Log($"Writing JSON: {path}");
                File.WriteAllText(path, configJson);

                Debug.Log("Export SpringBones and SpringBoneColliderGroups completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurs in exporting SpringBones to JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void ExportSpringBonesToAsset()
        {
            Debug.Log("Quick save SpringBone data to asset");
            CreateDirectories(springBoneQSaveFile, true);
            SpringBoneQSaveData.QuickSave(ExtractSpringBones(), Path.Combine(GetAssetDirectroyName(), springBoneQSaveFile));
            Debug.Log("Quick save completed");
        }

        public void ImportSpringBonesFromJson()
        {
            var path = EditorUtility.OpenFilePanel("Import SpringBones from JSON", string.Empty, "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Debug.Log($"Reading JSON from: {path}");
                var configJson = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<SpringBoneConfiguration>(configJson);

                // Add colliders before bones
                Debug.Log($"SpringBoneColliderGroups to import: {config.SpringBoneColliderGroups.Count}");
                foreach (var collider in config.SpringBoneColliderGroups)
                {
                    collider.ToSpringBoneColliderGroup(gameObject);
                }

                // Add bones
                Debug.Log($"SpringBones to import: {config.SpringBones.Count}");
                foreach (var bone in config.SpringBones)
                {
                    bone.ToSpringBone(gameObject);
                }

                Debug.Log("Import SpringBones and SpringBoneColliderGroups completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurs in importing SpringBones from JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void ImportSpringBonesFromAsset()
        {
            Debug.Log("Quick load SpringBone data from asset");
            SpringBoneQSaveData.QuickLoad(gameObject, Path.Combine(GetAssetDirectroyName(), springBoneQSaveFile));
            Debug.Log("Quick load completed");
        }

        public void RemoveSpringBones()
        {
            var colliderGroups = gameObject.GetComponentsInChildren<VRMSpringBoneColliderGroup>();
            if (colliderGroups != null)
            {
                Debug.Log($"SpringBoneColliderGroups to remove: {colliderGroups.Length}");
                foreach (var colliderGroup in colliderGroups)
                {
                    DestroyImmediate(colliderGroup);
                }
            }

            var bones = gameObject.GetComponentsInChildren<VRMSpringBone>();
            if (bones != null)
            {
                Debug.Log($"SpringBones to remove: {bones.Length}");
                foreach (var bone in bones)
                {
                    DestroyImmediate(bone);
                }
            }

            Debug.Log("Remove all SpringBones and SpringBoneColliderGroups completed");
        }
    }


    // Spring Bone
    public class SpringBoneConfiguration
    {
        public List<SerializableSpringBone> SpringBones = new List<SerializableSpringBone>();
        public List<SerializableSpringBoneColliderGroup> SpringBoneColliderGroups = new List<SerializableSpringBoneColliderGroup>();

        [JsonConstructor]
        public SpringBoneConfiguration()
        {

        }

        public SpringBoneConfiguration(GameObject gameObject)
        {
            foreach (var bone in gameObject.GetComponentsInChildren<VRMSpringBone>())
            {
                SpringBones.Add(SerializableSpringBone.FromSpringBone(bone));
            }

            foreach (var colliderGroup in gameObject.GetComponentsInChildren<VRMSpringBoneColliderGroup>())
            {
                SpringBoneColliderGroups.Add(SerializableSpringBoneColliderGroup.FromSpringBoneColliderGroup(colliderGroup));
            }
        }
    }

    [Serializable]
    public class SerializableSpringBone
    {
        [SerializeField]
        public string AttachedTo;

        [SerializeField]
        public string m_comment;

        [Header("Settings")]
        [SerializeField, Range(0, 4)]
        public float m_stiffnessForce = 1.0f;

        [SerializeField, Range(0, 2)]
        public float m_gravityPower = 0;

        [SerializeField]
        public Vector3 m_gravityDir = new Vector3(0, -1.0f, 0);

        [SerializeField, Range(0, 1)]
        public float m_dragForce = 0.4f;

        [SerializeField]
        public string m_center;

        [SerializeField]
        public List<string> RootBones = new List<string>();

        [Header("Collider")]
        [SerializeField, Range(0, 0.5f)]
        public float m_hitRadius = 0.02f;

        [SerializeField]
        public List<string> ColliderGroups = new List<string>();

        public SerializableSpringBone()
        {

        }

        public static SerializableSpringBone FromSpringBone(VRMSpringBone bone)
        {
            Debug.Log($"Convert SpringBone to serializable: Root={bone.RootBones[0].name}");

            var serializableBone = new SerializableSpringBone();
            serializableBone.AttachedTo = HierarchyPath.FromTransform(bone.gameObject.transform);

            // Copy values
            serializableBone.m_comment = bone.m_comment;
            serializableBone.m_stiffnessForce = bone.m_stiffnessForce;
            serializableBone.m_gravityPower = bone.m_gravityPower;
            serializableBone.m_gravityDir = bone.m_gravityDir;
            serializableBone.m_dragForce = bone.m_dragForce;
            if (bone.m_center != null)
            {
                serializableBone.m_center = HierarchyPath.FromTransform(bone.m_center);
            }
            if (bone.RootBones != null)
            {
                foreach (var rootBone in bone.RootBones)
                {
                    serializableBone.RootBones.Add(HierarchyPath.FromTransform(rootBone));
                }
            }
            serializableBone.m_hitRadius = bone.m_hitRadius;
            if (bone.ColliderGroups != null)
            {
                foreach (var colliderGroup in bone.ColliderGroups)
                {
                    serializableBone.ColliderGroups.Add(HierarchyPath.FromTransform(colliderGroup.transform));
                }
            }

            return serializableBone;
        }

        public VRMSpringBone ToSpringBone(GameObject gameObject)
        {
            var targetObject = HierarchyPath.ToGameObject(AttachedTo, gameObject);
            var attachedBone = targetObject.AddComponent<VRMSpringBone>();

            // Copy values
            attachedBone.m_comment = m_comment;
            attachedBone.m_stiffnessForce = m_stiffnessForce;
            attachedBone.m_gravityPower = m_gravityPower;
            attachedBone.m_gravityDir = m_gravityDir;
            attachedBone.m_dragForce = m_dragForce;
            attachedBone.m_center = HierarchyPath.ToTransform(m_center);
            attachedBone.RootBones = HierarchyPath.ToTransforms(RootBones, gameObject);
            attachedBone.m_hitRadius = m_hitRadius;
            attachedBone.ColliderGroups = new VRMSpringBoneColliderGroup[ColliderGroups.Count];
            for (var i = 0; i < ColliderGroups.Count; i++)
            {
                var cgAttachedObject = HierarchyPath.ToGameObject(ColliderGroups[i], gameObject);
                if (cgAttachedObject != null)
                {
                    attachedBone.ColliderGroups[i] = cgAttachedObject.GetComponent<VRMSpringBoneColliderGroup>();
                }
            }
            return attachedBone;
        }
    }

    [Serializable]
    public class SerializableSpringBoneColliderGroup
    {
        [SerializeField]
        public string AttachedTo;

        [SerializeField]
        public List<SerializableSpringBoneCollider> Colliders = new List<SerializableSpringBoneCollider>();

        public SerializableSpringBoneColliderGroup()
        {

        }

        // Convert from VRMSpringBoneCollider
        public static SerializableSpringBoneColliderGroup FromSpringBoneColliderGroup(VRMSpringBoneColliderGroup colliderGroup)
        {
            Debug.Log($"Convert SpringBoneColliderGroup to serializable: Root={colliderGroup.transform.name}");

            var serializableCollider = new SerializableSpringBoneColliderGroup();
            serializableCollider.AttachedTo = HierarchyPath.FromTransform(colliderGroup.transform);

            // Copy values
            if (colliderGroup.Colliders != null)
            {
                foreach (var collider in colliderGroup.Colliders)
                {
                    serializableCollider.Colliders.Add(new SerializableSpringBoneCollider(collider.Offset, collider.Radius));
                }
            }

            return serializableCollider;
        }

        // Convert to VRMSpringBoneCollider
        public VRMSpringBoneColliderGroup ToSpringBoneColliderGroup(GameObject gameObject)
        {
            var targetObject = HierarchyPath.ToGameObject(AttachedTo, gameObject);
            var attachedColliderGroup = targetObject.AddComponent<VRMSpringBoneColliderGroup>();

            // Copy values
            attachedColliderGroup.Colliders = new VRMSpringBoneColliderGroup.SphereCollider[Colliders.Count];
            for (var i = 0; i < Colliders.Count; i++)
            {
                attachedColliderGroup.Colliders[i] = new VRMSpringBoneColliderGroup.SphereCollider()
                {
                    Offset = Colliders[i].Offset,
                    Radius = Colliders[i].Radius
                };
            }

            return attachedColliderGroup;
        }
    }

    [Serializable]
    public class SerializableSpringBoneCollider
    {
        [SerializeField]
        public Vector3 Offset = Vector3.zero;

        [SerializeField, Range(0, 1.0f)]
        public float Radius = 0;

        public SerializableSpringBoneCollider()
        {

        }

        public SerializableSpringBoneCollider(Vector3 offset, float radius)
        {
            Offset = offset;
            Radius = radius;
        }
    }


    // Editor
    public partial class DynamicBoneConverterEditor : Editor
    {
        // SpringBone
        private bool isOpenSpringBonesToConfigureSection = true;
        private Dictionary<VRMSpringBone, bool> springBonesToConfigure = new Dictionary<VRMSpringBone, bool>();
        private float sbGravityPower;
        private bool sbApplyGravity;
        private Vector3 sbGravityDirection;
        private bool sbApplyGravityDirection;
        private float sbStiffness;
        private bool sbApplyStiffness;
        private float sbDragForce;
        private bool sbApplyDragForce;
        private float sbRadius;
        private bool sbApplyRadius;

        public void SpringBoneGUI(DynamicBoneConverter executionScript)
        {
            var toggleLeftLayout = new GUILayoutOption[] { GUILayout.Width(120) };

            // Import / Export
            GUILayout.Label("Data", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("JSON");
            if (GUILayout.Button("Export"))
            {
                executionScript.ExportSpringBonesToJson();
            }
            else if (GUILayout.Button("Import"))
            {
                executionScript.ImportSpringBonesFromJson();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Asset");
            if (GUILayout.Button("Q.Save"))
            {
                executionScript.ExportSpringBonesToAsset();
            }
            else if (GUILayout.Button("Q.Load"))
            {
                executionScript.ImportSpringBonesFromAsset();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Remove");
            if (GUILayout.Button("Remove all SpringBones"))
            {
                if (EditorUtility.DisplayDialog("Remove SpringBones", "Are you sure to remove all SpringBones?", "OK", "Cancel"))
                {
                    executionScript.RemoveSpringBones();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Bulk application
            GUILayout.Label("Bulk Application", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            isOpenSpringBonesToConfigureSection = EditorGUILayout.Foldout(isOpenSpringBonesToConfigureSection, "SpringBones");
            if (isOpenSpringBonesToConfigureSection)
            {
                var springBones = executionScript.gameObject.GetComponentsInChildren<VRMSpringBone>();
                if (springBones != null)
                {
                    foreach (var bone in springBones)
                    {
                        springBonesToConfigure[bone] = EditorGUILayout.ToggleLeft(bone.name + ": " + string.Join(", ", bone.RootBones.Select(rb => rb.name)), springBonesToConfigure.ContainsKey(bone) && springBonesToConfigure[bone]);
                    }
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            sbApplyStiffness = EditorGUILayout.ToggleLeft("Stiffness Force", sbApplyStiffness, toggleLeftLayout);
            sbStiffness = EditorGUILayout.Slider(sbStiffness, 0.0f, 4.0f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            sbApplyGravity = EditorGUILayout.ToggleLeft("Gravity Power", sbApplyGravity, toggleLeftLayout);
            sbGravityPower = EditorGUILayout.Slider(sbGravityPower, 0.0f, 2.0f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            sbApplyGravityDirection = EditorGUILayout.ToggleLeft("Gravity Direction", sbApplyGravityDirection, toggleLeftLayout);
            sbGravityDirection = EditorGUILayout.Vector3Field(string.Empty, sbGravityDirection);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            sbApplyDragForce = EditorGUILayout.ToggleLeft("Drag Force", sbApplyDragForce, toggleLeftLayout);
            sbDragForce = EditorGUILayout.Slider(sbDragForce, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            sbApplyRadius = EditorGUILayout.ToggleLeft("Hit Radius", sbApplyRadius, toggleLeftLayout);
            sbRadius = EditorGUILayout.Slider(sbRadius, 0.0f, 0.5f);
            EditorGUILayout.EndHorizontal();

            // Apply
            foreach (var bone in springBonesToConfigure.Where(kv => kv.Value == true).Select(kv => kv.Key))
            {
                if (sbApplyStiffness) bone.m_stiffnessForce = sbStiffness;
                if (sbApplyGravity) bone.m_gravityPower = sbGravityPower;
                if (sbApplyGravityDirection) bone.m_gravityDir = sbGravityDirection;
                if (sbApplyDragForce) bone.m_dragForce = sbDragForce;
                if (sbApplyRadius) bone.m_hitRadius = sbRadius;
            }
        }
    }
}
