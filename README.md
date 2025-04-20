# Info View Shader

キャラ情報表示みたいなUIを作れるシェーダー

![thumbnail](docs~/InfoViewShader.png)

## Install

### OpenUPM

See [OpenUPM page](https://openupm.com/packages/net.narazaka.unity.info-view-shader/)

### VCC用インストーラーunitypackageによる方法（おすすめ）

https://github.com/Narazaka/InfoViewShader/releases/latest から `net.narazaka.unity.info-view-shader-installer.zip` をダウンロードして解凍し、対象のプロジェクトにインポートする。

### VCCによる方法

1. https://vpm.narazaka.net/ から「Add to VCC」ボタンを押してリポジトリをVCCにインストールします。
2. VCCでSettings→Packages→Installed Repositoriesの一覧中で「Narazaka VPM Listing」にチェックが付いていることを確認します。
3. アバタープロジェクトの「Manage Project」から「Info View Shader」をインストールします。

## Usage

Exampleフォルダに例があります。

1. Cubeを描画される可能性のある領域を覆う大きさにして配置する。
2. InfoViewShader/BillboardWithOffsetで情報ウインドウを、InfoViewShader/BillboardConnectLineで指示線のマテリアルを作り、Cubeに適用する。
3. 常にワールド座標で回転が0になるようRotation Constraintのprefab hack等で調整する。

## License

[Zlib License](LICENSE.txt)
