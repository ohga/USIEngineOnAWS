# USI エンジン on AWS
 [![Build Status](https://travis-ci.org/ohga/USIEngineOnAWS.svg?branch=master)](https://travis-ci.org/ohga/USIEngineOnAWS)
 [![Coverity Scan](https://scan.coverity.com/projects/11068/badge.svg)](https://scan.coverity.com/projects/ohga-usiengineonaws)
 [![Build status](https://ci.appveyor.com/api/projects/status/o7mel4jof0bj7f6k/branch/master?svg=true)](https://ci.appveyor.com/project/ohga/usiengineonaws/branch/master)
 [![License](https://img.shields.io/github/license/ohga/USIEngineOnAWS.svg?label=License&maxAge=86400)](https://github.com/ohga/USIEngineOnAWS/blob/master/LICENSE)
 [![Release](https://img.shields.io/github/release/ohga/USIEngineOnAWS.svg?label=Release&maxAge=60)](https://github.com/ohga/USIEngineOnAWS/releases)


これは [@HiraokaTakuya](https://twitter.com/HiraokaTakuya) 氏の[発言](https://twitter.com/HiraokaTakuya/status/792187555735216129)の実装の一つで AWS の[スポットインスタンス](http://docs.aws.amazon.com/ja_jp/AWSEC2/latest/UserGuide/using-spot-instances.html)上に将棋エンジンを作成し、将棋所や ShogiGUI 等のエンジンとして登録することができる実行ファイルを用意する Windows 用のツールです。 

AWS 上のマシンで将棋エンジンを動かすことで、ノート PC で 64 スレッド ponder ありの殴り合いを観戦するとか、複数エンジンで検討するとかに使えると思いますが、ネットワークレイテンシが無視できないような、一手 3 秒対局とかにはむいていません。平文で良いのなら遅延はだいぶマシになります。([Wiki](https://github.com/ohga/USIEngineOnAWS/wiki#%E8%AA%8D%E8%A8%BC%E3%81%AA%E3%81%97%E3%81%AE%E5%B9%B3%E6%96%87%E3%81%A7%E3%82%84%E3%82%8A%E5%8F%96%E3%82%8A%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95))

![alt tag](https://github.com/ohga/USIEngineOnAWS/wiki/images/screenshot-01.png)

## 動作環境

[.NET Framework 4.5](https://www.microsoft.com/ja-jp/download/details.aspx?id=30653) がインストールされている Windows であれば動くと思います。

また、[AWS を利用する為のアカウント](https://aws.amazon.com/jp/register-flow/)と `AmazonEC2FullAccess` の権限を持った [IAM ユーザのアクセスキー ID と シークレットアクセスキー](https://github.com/ohga/USIEngineOnAWS/wiki#iam-%E3%83%A6%E3%83%BC%E3%82%B6%E3%81%AE%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%82%AD%E3%83%BC-id-%E3%81%A8-%E3%82%B7%E3%83%BC%E3%82%AF%E3%83%AC%E3%83%83%E3%83%88%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%82%AD%E3%83%BC%E3%81%AE%E4%BD%9C%E6%88%90%E4%BE%8B)と、財布の中身が別途必要になります。

(私は Amazon の回し者ではないです。むしろ、この作り込みをした後に AWS の請求情報を見て、少量の憎しみをこめてこれを書いています。)

## 免責事項

コメントとか入れてないですが、ソースコード一式を同梱して配布しますので、必要に応じて修正してご活用ください。意図しない請求がきても、というか、いかなる損害について、当方は一切の責任を負えませんので、ご承知おきください。

## ダウンロード

https://github.com/ohga/USIEngineOnAWS/releases から入手できます。

## 比較的簡単な使い方

1. `USIEngineOnAWS.zip` を任意の場所に解凍します。
2. `USIEngineOnAWS.exe` を起動します。
3. `AmazonEC2FullAccess` の権限を持った IAM ユーザの認証情報を入力します。
4. メニューの「保存先の設定」で出力先フォルダを指定します。
5. インスタンスタイプを選択して、「現在の最安値を検索」ボタンを押下します。
5. 財布の中身と相談して、「スポットインスタンス価格(米ドル)」で入札価格を調整し「インスタンス削除の予約設定」を入力します。
6. 「インスタンス作成」ボタンを押下して、 5 〜 15 分程度待ちます。
7. 作成が完了するとフォルダが開かれるので、その中の `USIEngineViaSSH.exe` を実行し、 `usi` の入力への応答や `quit` の入力で終了するなど、正しく動作している事を確認し、将棋所や ShogiGUI 等のエンジンとして登録して使用します。
  ![alt tag](https://github.com/ohga/USIEngineOnAWS/wiki/images/screenshot-02.png)
8. 使い終ったら、インスタンスを停止して 7. で作成されたフォルダごと削除します。インスタンスが削除されると使用できなくなるので、 5. からやりなおしてください。

ツール画面の入力項目には、ツールチップでヘルプを書いているので、参考にしてください。

## 簡単じゃない使い方、もしくは注意事項をだらだらと

長くなったので [Wiki](https://github.com/ohga/USIEngineOnAWS/wiki) にしました。

## 謝辞

コンピュータ将棋に関わっている方々、過去に関わっていて、その研究をなんらかの形で残された方々、ソースコードを公開してくださっている方々、心からの御礼と深い感謝を申し上げます。

<!-- vim: se enc=utf8 ft=markdown -->
