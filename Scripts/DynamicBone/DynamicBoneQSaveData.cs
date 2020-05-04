using UnityEditor;
using UnityEngine;


namespace ChatdollKit
{
    public class DynamicBoneQSaveData : ScriptableObject
    {
        public SerializableDynamicBone[] Bones;
        public SerializableDynamicBoneCollider[] Colliders;

        public void Parse(DynamicBoneConfiguration configuration)
        {
            Bones = configuration.DynamicBones.ToArray();
            Colliders = configuration.DynamicBoneColliders.ToArray();
        }

        public static void QuickSave(DynamicBoneConfiguration config, string path)
        {
            var assetData = CreateInstance<DynamicBoneQSaveData>();
            assetData.Parse(config);

            AssetDatabase.CreateAsset(assetData, path);
        }

        public static void QuickLoad(GameObject gameObject, string path)
        {
            var assetData = AssetDatabase.LoadAssetAtPath<DynamicBoneQSaveData>(path);

            // Add colliders before bones
            foreach (var collider in assetData.Colliders)
            {
                collider.ToDynamicBoneCollider(gameObject);
            }

            // Add bones
            foreach (var bone in assetData.Bones)
            {
                bone.ToDynamicBone(gameObject);
            }
        }
    }
}
