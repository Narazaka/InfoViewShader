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

### NDMF(Modular Avatar)の入ったVRChat アバタープロジェクトの場合

事前にNDMFまたはModular Avatarをインストールして下さい。

1. アバターかアバター内のオブジェクトを右クリックして `InfoViewShader→Create InfoView`で1つの情報表示が作れます。
     - Add Componentから`InfoView`コンポーネントを追加する方法も可能です。
2. InfoViewコンポーネントを設定して下さい。

### 一般的な場合

Exampleフォルダに例があります。

1. Cubeを描画される可能性のある領域を覆う大きさにして配置する。
2. InfoViewShader/BillboardWithOffsetで情報ウインドウを、InfoViewShader/BillboardConnectLineで指示線のマテリアルを作り、Cubeに適用する。

## License

[Zlib License](LICENSE.txt)
