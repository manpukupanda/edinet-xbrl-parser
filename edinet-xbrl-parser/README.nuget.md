# EDINET XBRL Parser

## プロジェクト概要（Overview）

This library is a .NET class library designed to parse XBRL data disclosed via [EDINET](https://submit2.edinet-fsa.go.jp/), Japan’s electronic disclosure system operated by the Financial Services Agency.
Its purpose is to extract and transform XBRL files into programmatically accessible structures for further use in applications.

このライブラリは、[EDINET](https://submit2.edinet-fsa.go.jp/)にて開示される XBRL データをパースするための .NET クラスライブラリです。
金融庁が提供する電子開示システムから取得した XBRL ファイルを解析し、プログラム上で扱いやすい構造に変換することを目的としています。

---

## 主な機能（Features）

このライブラリは、EDINETにて開示される XBRL データを対象に、以下の機能を提供します：

- DTS(Discoverable Taxonomy Set)の解決  
- インスタンス（InlineXBRLにも対応）、スキーマ、リンクベースの解析  
- 拡張リンクロールの解決
- 不正な構造や欠損に対する明示的な例外処理  
- 外部依存を最小限に抑えた、責任範囲の明確な設計

---

## インストール方法（How to install）

このライブラリは、NuGetパッケージとして提供されています。プロジェクトに追加するには、以下のコマンドを使用してください：
```bash
dotnet add package Manpuku.Edinet.Xbrl
```

---

## 使用例（Usage）

このライブラリは、DI（依存性注入）を前提とした設計になっています。
IXbrlParser インターフェースを通じて、EDINETのXBRLデータを非同期に解析できます。
パーサは2種類あり、それぞれ用途と呼び出し方が異なります。

### 通常のXBRLパーサ
XBRLインスタンスファイル(.xbrl)またはスキーマファイル(.xsd)をエントリポイントとして解析します。

```csharp
services.AddSingleton<Manpuku.Edinet.Xbrl.IXbrlParser, Manpuku.Edinet.Xbrl.XbrlParser>();
```
```csharp
public interface IXbrlParser
{
    Task<XBRLDiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader);
}
```

使用例：
```csharp
var parser = serviceProvider.GetRequiredService<Manpuku.Edinet.Xbrl.IXbrlParser>();

// loader: URIからXDocumentを取得する非同期関数（呼び出し側で定義）
Func<Uri, Task<XDocument>> loader = async uri =>
{
    using var httpClient = new HttpClient();
    var stream = await httpClient.GetStreamAsync(uri);
    return XDocument.Load(stream);
};

var dts = await parser.ParseAsync(new Uri("https://example.com/entrypoint.xbrl"), loader);
```

### Inline XBRLパーサ
Inline XBRLドキュメント(_ixbrl.htm)をエントリポイントとして解析します。
```csharp
services.AddSingleton<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser, Manpuku.Edinet.Xbrl.InlineXBRL.XbrlParser>();
```
```csharp
public interface IXbrlParser
{
    Task<XBRLDiscoverableTaxonomySet> ParseInlineAsync(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader);
}
```

使用例：
```csharp
var inlineParser = serviceProvider.GetRequiredService<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser>();

Func<Uri, Task<XDocument>> loader = async uri =>
{
    using var httpClient = new HttpClient();
    var stream = await httpClient.GetStreamAsync(uri);
    return XDocument.Load(stream);
};

var dts = await inlineParser.ParseInlineAsync(new[] {
    new Uri("https://example.com/inline1_ixbrl.htm"),
    new Uri("https://example.com/inline2_ixbrl.htm")
}, loader);
```

### loader関数について
`loader` は呼び出し側が提供する責任を持ち、指定された `Uri` から `XDocument` を非同期で取得する関数です。
この設計により、HTTP取得・キャッシュ・ローカルファイル読み込みなど、環境に応じた柔軟な実装が可能です。

---
## ライセンス（License）
このプロジェクトはMITライセンスの下でライセンスされています。

---
## サポート（Support）
このプロジェクトに関する質問や問題は、GitHubの[Issues](https://github.com/manpukupanda/edinet-xbrl-parser/issues)セクションで受け付けています。
