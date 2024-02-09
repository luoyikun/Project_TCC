﻿# DialogueEntry

#### **Namespace**: Unity.ScenarioImporter
---

## 概要:
`DialogueEntry` は、ゲーム内の一つのダイアログエントリーを表すクラスです。このクラスは、ダイアログのテキスト、関連するイベント、およびそれが属するタグを含んでいます。シナリオやダイアログの流れを表すために使用されます。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `Text` | メッセージのテキスト内容です。 |
| `Events` | メッセージ内に格納されたイベントのリストです。 |
| `Tag` | メッセージが属するタグです。 |

---
## その他の注意事項
- `DialogueEntry` クラスは、シナリオやダイアログの管理において、テキスト、イベント、タグを組み合わせて使用することで、ゲーム内のストーリーテリングやイベントトリガーの管理を容易にします。
- このクラスはシリアライズ可能であり、Unityのインスペクター上で直接編集可能なフィールドを提供します。これにより、ゲームデザイナーやシナリオライターがゲーム内のダイアログを直感的に管理できるようになります。
- `Events` プロパティは、ダイアログが進行する中で発生する可能性のあるイベント（例えば、特定のキャラクターが登場する、アイテムを獲得するなど）をリストとして保持します。これにより、ダイアログのテキストだけでなく、ゲームプレイにおけるダイナミックな要素も表現できます。