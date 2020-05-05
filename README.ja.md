# DynamicBoneConverter

DynamicBoneからVRM SpringBoneへの変換をめっちゃ簡単にします。


# できること ✨

- DynamicBoneからVRM SpringBoneへの変換機能
- DynamicBone / VRM SpringBone設定のJSONファイルへの設定保存・読み込み機能
- 複数ボーンへのパラメータの一括設定機能

<img src="https://uezo.blob.core.windows.net/github/dynamicboneconverter/inspector.png" width="390">

# クイックスタート 🚀

- `DynamicBoneConverter.unitypackage`とその依存ライブラリをインポート
- 移植元DynamicBoneの設定されたモデルと、SpringBoneを設定したいモデルとの双方に`DynamicBoneConverter.cs`をアタッチ
- 移植元DynamicBoneのあるモデルのインスペクターにて、`Convert and export JSON`ボタンを押してJSONファイルを保存
- 移植先モデルのインスペクターで、`Import`ボタンを押して先ほど生成したJSONファイルを読み込み
- 生成されたSpringBoneのパラメーターを調整したらできあがり🍵


# 導入方法 🎁

`DynamicBoneConverter.unitypackage`をインポートして`DynamicBoneConverter.cs`をアタッチするだけでOK。

<img src="https://uezo.blob.core.windows.net/github/dynamicboneconverter/attach.png" width="500">


## 依存ライブラリ

- [DynamicBone](https://assetstore.unity.com/packages/tools/animation/dynamic-bone-16743) (tested on 1.2.1)
- [VRM SpringBone](https://github.com/vrm-c/UniVRM/releases) (tested on 0.55)
- [JSON.NET](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347?locale=ja-JP) (tested on 2.0.1)

DynamicBone設定やVRM SpringBone設定のバックアップツールとして使用する場合は、揺れものアセットは一方のみ入っていれば大丈夫です。変換する場合は両方必要。


# ボーン設定の書き出し 🚛

`Export`ボタンを押してJSONファイルの出力先を指定すればOK。

また、`Q.Save`ボタンを押せばアドベンチャーゲームでお馴染みのクイックセーブ（確認ダイアログなど無し）することもできます。1つのデータスロットを常に上書きするのでパラメーター設定時など一時的な設定退避用途にのみ利用ください。


# ボーン設定の読み込み 🚢

`Import`ボタンを押して読み込むJSONファイルを指定すればOK。

また、`Q.Load`ボタンを押してクイックセーブしたデータを直ちに読み込み・反映することもできます。


# Convert 🐟🍣

`Convert and export JSON`を押してJSONファイルの出力先を指定すれば、DynamicBoneとそのコライダーをSpringBoneのものに読み替えた後の内容でJSONファイルに保存されます。これを移植先モデルのインスペクターの`Import`で読み込むことでボーン設定を適用することができます。

## Options

- **Make height with multiple colliders**: 複数のコライダーを生成してカプセルコライダー様の形を作るかどうかの指定です。DynamicBoneにある`Height`プロパティの代替です。
- **Attach SpringBones to secondary object**: SpringBoneのアタッチ先を移植元と同じにするのではなくVRMモデルの`secondary`オブジェクトに集約します。


# パラメーター設定 🧂

Boneのパラメーターの一括設定支援GUIを提供します。DynamicBoneからVRM SpringBoneへの移植作業のために作ったので、現在はそのワークフローで必要になるVRM SpringBone用のGUIのみの提供となっています。

設定対象のボーンとパラメーターを選択してスライダーを動かすとリアルタイムで設定が反映されるので、効きを見ながら良さそうな値を見つけ出すことができると思います。


# コントリビューション大歓迎です

Issueの投稿やプルリクエストはどうぞお気軽に！
