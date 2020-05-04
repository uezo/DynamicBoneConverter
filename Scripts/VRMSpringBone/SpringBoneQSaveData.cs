using UnityEditor;
using UnityEngine;


namespace ChatdollKit
{
    public class SpringBoneQSaveData : ScriptableObject
    {
        public SerializableSpringBone[] Bones;
        public SerializableSpringBoneColliderGroup[] ColliderGroups;

        public void Parse(SpringBoneConfiguration configuration)
        {
            Bones = configuration.SpringBones.ToArray();
            ColliderGroups = configuration.SpringBoneColliderGroups.ToArray();
        }

        public static void QuickSave(SpringBoneConfiguration config, string path)
        {
            var assetData = CreateInstance<SpringBoneQSaveData>();
            assetData.Parse(config);

            AssetDatabase.CreateAsset(assetData, path);
        }

        public static void QuickLoad(GameObject gameObject, string path)
        {
            var assetData = AssetDatabase.LoadAssetAtPath<SpringBoneQSaveData>(path);

            // Add colliders before bones
            foreach (var colliderGroup in assetData.ColliderGroups)
            {
                colliderGroup.ToSpringBoneColliderGroup(gameObject);
            }

            // Add bones
            foreach (var bone in assetData.Bones)
            {
                bone.ToSpringBone(gameObject);
            }
        }
    }
}
