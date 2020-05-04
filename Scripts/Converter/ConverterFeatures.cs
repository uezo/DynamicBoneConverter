using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;


namespace ChatdollKit
{
    public partial class DynamicBoneConverter : MonoBehaviour
    {
        public SerializableDynamicBoneCollider ConvertCollider(SerializableSpringBoneColliderGroup springBoneColliderGroup)
        {
            var dynamicBoneCollider = new SerializableDynamicBoneCollider();
            dynamicBoneCollider.AttachedTo = springBoneColliderGroup.AttachedTo;

            return dynamicBoneCollider;
        }

        public void ConvertToSpringBone(bool fillCollidersGap = true, bool attacheToSecondary = true)
        {
            var fileName = $"{gameObject.name}_DBtoSB{DateTime.Now:yyyyMMddHHmmss}";
            var path = EditorUtility.SaveFilePanel("Convert DynamicBones to SpringBones as JSON", string.Empty, fileName, "json");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Debug.Log("Converting DynamicBones to SpringBones");
                var dyConfig = ExtractDynamicBones();
                var spColliderGroups = ConvertColliders(dyConfig.DynamicBoneColliders, fillCollidersGap);
                var spBones = ConvertBones(dyConfig.DynamicBones, attacheToSecondary);

                var spConfig = new SpringBoneConfiguration();
                spConfig.SpringBones = spBones;
                spConfig.SpringBoneColliderGroups = spColliderGroups;
                var configJson = JsonConvert.SerializeObject(spConfig);

                Debug.Log("Writing SpringBones to JSON");
                File.WriteAllText(path, configJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurs in converting DynamicBones to SpringBones: {ex.Message}\n{ex.StackTrace}");
            }

            Debug.Log("Convert DynamicBones to SpringBones completed");
        }

        public List<SerializableSpringBone> ConvertBones(List<SerializableDynamicBone> dynamicBones, bool attacheToSecondary)
        {
            var springBones = new List<SerializableSpringBone>();

            foreach (var dyBone in dynamicBones)
            {
                var spBone = new SerializableSpringBone();

                if (attacheToSecondary)
                {
                    spBone.AttachedTo = $"{gameObject.name}/secondary";
                }
                else
                {
                    spBone.AttachedTo = dyBone.AttachedTo;
                }

                Debug.Log($"Object to attach: {spBone.AttachedTo}");

                spBone.m_stiffnessForce = dyBone.m_Stiffness * 4.0f;    // The max value of SpringBone is 4.0f

                spBone.m_gravityPower = dyBone.m_Gravity.magnitude > 2.0f ? 2.0f : dyBone.m_Gravity.magnitude;  // The max value is 2.0f

                spBone.m_gravityDir = spBone.m_gravityPower == 0 ? new Vector3(0, -1.0f, 0) : dyBone.m_Gravity / dyBone.m_Gravity.magnitude;

                spBone.RootBones = new List<string>() { dyBone.m_Root };

                // Radius of DynamicBone is relative to its parant. This value should be tuned manually because RadiusDistrib is not applied.
                spBone.m_hitRadius = dyBone.m_Radius * dyBone.TransformScale.x;

                spBone.ColliderGroups = new List<string>(dyBone.m_Colliders);

                springBones.Add(spBone);
            }

            return springBones;
        }

        public List<SerializableSpringBoneColliderGroup> ConvertColliders(List<SerializableDynamicBoneCollider> dynamicBoneColliders, bool fillCollidersGap)
        {
            var springBoneColliderGroups = new Dictionary<string, SerializableSpringBoneColliderGroup>();

            foreach (var dyCollider in dynamicBoneColliders)
            {
                Debug.Log($"Object to attach: {dyCollider.AttachedTo}");

                var spCollider = new SerializableSpringBoneCollider(dyCollider.m_Center, dyCollider.m_Radius * dyCollider.TransformScale.x);
                Debug.Log($"New collider: R={spCollider.Radius} / (X,Y,Z)=({spCollider.Offset.x},{spCollider.Offset.y},{spCollider.Offset.z})");

                if (springBoneColliderGroups.ContainsKey(dyCollider.AttachedTo))
                {
                    // Just add new collider to existing SpringBoneColliderGroup
                    springBoneColliderGroups[dyCollider.AttachedTo].Colliders.Add(spCollider);
                }
                else
                {
                    // Create new SpringBoneColliderGroup with new collider
                    var springBoneColliderGroup = new SerializableSpringBoneColliderGroup();
                    springBoneColliderGroup.AttachedTo = dyCollider.AttachedTo;
                    springBoneColliderGroup.Colliders.Add(spCollider);
                    springBoneColliderGroups.Add(dyCollider.AttachedTo, springBoneColliderGroup);
                }

                if (fillCollidersGap && dyCollider.m_Height > 0)
                {
                    // Culculate height
                    var spHeight = dyCollider.m_Height * dyCollider.TransformScale[(int)dyCollider.m_Direction];

                    // fillCount should be positive when spHeight / 2 > spCollider.Radius. The gap exists when the height > r * 2.
                    var distanceToEdge = spHeight / 2 - spCollider.Radius;
                    var fillCount = (int)Math.Ceiling(distanceToEdge / spCollider.Radius);

                    for (var i = 0; i < fillCount; i++)
                    {
                        var distance = distanceToEdge / fillCount * (i + 1);
                        var distanceVector = new Vector3(0.0f, 0.0f, 0.0f);
                        distanceVector[(int)dyCollider.m_Direction] += distance;

                        var gapCollider = JsonConvert.DeserializeObject<SerializableSpringBoneCollider>(JsonConvert.SerializeObject(spCollider));
                        gapCollider.Offset += distanceVector;
                        springBoneColliderGroups[dyCollider.AttachedTo].Colliders.Add(gapCollider);

                        var gapCollider2 = JsonConvert.DeserializeObject<SerializableSpringBoneCollider>(JsonConvert.SerializeObject(spCollider));
                        gapCollider2.Offset -= distanceVector;
                        springBoneColliderGroups[dyCollider.AttachedTo].Colliders.Add(gapCollider2);
                    }
                }
            }

            return springBoneColliderGroups.Values.ToList();
        }
    }

    // Editor
    public partial class DynamicBoneConverterEditor : Editor
    {
        private bool fillCollidersGap = true;
        private bool attachToSecondary = true;

        public void ConverterGUI(DynamicBoneConverter executionScript)
        {
            GUILayout.Label("Dynamic to VRM Spring", EditorStyles.boldLabel);
            fillCollidersGap = EditorGUILayout.ToggleLeft("Make height with multiple colliders", fillCollidersGap);
            attachToSecondary = EditorGUILayout.ToggleLeft("Attach SpringBones to secondary object", attachToSecondary);
            if (GUILayout.Button("Convert and export JSON"))
            {
                executionScript.ConvertToSpringBone(fillCollidersGap, attachToSecondary);
            }
        }
    }
}
