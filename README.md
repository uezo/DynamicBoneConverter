# DynamicBoneConverter

Convert DynamicBone to VRM SpringBone extremely easily.


# Features ✨

- Convert DynamicBone to VRM SpringBone
- Export / Import Db and Sb settings to / from JSON file
- Provide helper GUI for bulk parameter application

<img src="https://uezo.blob.core.windows.net/github/dynamicboneconverter/inspector.png" width="390">

# Quick start 🚀

- Import `DynamicBoneConverter.unitypackage` and its dependencies
- Attach `DynamicBoneConverter.cs` to both the model with DynamicBone to convert and the model to be attached converted SpringBones
- On the inspector of the model with DynamicBone, press `Convert and export JSON`
- On the inspector of the other side, press `Import` and select the JSON file exported just before
- Tune some parameters to optimize to SpringBone


# Install 🎁

Simply import `DynamicBoneConverter.unitypackage` and attach `DynamicBoneConverter.cs` to your model you want to manage bones.

<img src="https://uezo.blob.core.windows.net/github/dynamicboneconverter/attach.png" width="500">


## Dependencies

- [DynamicBone](https://assetstore.unity.com/packages/tools/animation/dynamic-bone-16743) (tested on 1.2.1)
- [VRM SpringBone](https://github.com/vrm-c/UniVRM/releases) (tested on 0.55)
- [Json.Net](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347?locale=ja-JP) (tested on 2.0.1)

You can use this extension just as an backup tool for DynamicBone / VRM SpringBone without importing VRM SpringBone / DynamicBone.


# Export 🚛

Press `Export` button and select the location to save JSON file.

Another way is Quick save. Press `Q.Save` button then the bones will be saved immediately without any confirmation. This is for temporary use. The data will be overwritten everytime.


# Import 🚢

Press `Import` button and select JSON file to import.

Another way is Quick load. Press `Q.Load` button then the newest Q.Save data will be applied immediately without any confirmation.


# Convert 🐟🍣

Press `Convert and export JSON` button. You can apply the converted bone settings to your model by importing JSON file.


## Options

- **Make height with multiple colliders**: Create a capsule-collider like shape by multiple colliders instead of height property of DynamicBone.
- **Attach SpringBones to secondary object**: Add SpringBones not to the objects DynamicBones are originally appended but to `secondary` object of VRM model.


# Parameter tuning 🧂

Helper GUI for tuning parameters. This feature is provided for SpringBone only for now to support the workflow of DynamicBone to VRM SpringBone conversion. Select bones and parameters and configure them. The value changed is applied realtime.


# Contributions are welcome!

Feel free to post issues or pull requests.


