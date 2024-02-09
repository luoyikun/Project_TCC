﻿# IndicatorRegister

#### **Namespace**: Unity.TinyCharacterController.Utility
---

## 概要:
`IndicatorRegister` は、キャラクターにインジケーターを簡単に追加するためのコンポーネントです。このコンポーネントがアクティブになると、`_ui` オブジェクトを自動的に生成し、登録します。オブジェクトは `GameObjectPool` の機能を使用して作成されるため、生成されるUIは `PooledGameObject` を持ち、`GameObjectPool` に登録されている必要があります。

## 機能と操作:
- **UIの生成と登録**: 指定されたUIを自動的に生成し、キャラクターに追加します。
- **インジケーターの自動追跡**: 生成されたUIに `Indicator` コンポーネントがある場合、対象キャラクターを自動的に追跡します。
- **UI Pinの設定**: 生成されたUIに `UiPin` コンポーネントがある場合、世界座標を設定します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_ui` | 生成するUI。 |
| `Instance` | 生成されたUIのインスタンス。 |
| `HasInstance` | UIのインスタンスが生成されているかどうか。 |

## メソッド
- 公開されているメソッドはありません。


---
## その他の注意事項
- このコンポーネントは、`Indicator` または `UiPin` コンポーネントを持つUIをキャラクターに追加する際に役立ちます。自動的に対象キャラクターを追跡するインジケーターや、特定の位置に固定されるUIピンなど、様々なUI要素を簡単に追加できます。
- `GameObjectPool` を使用してUIを管理することで、パフォーマンスの最適化が図れます。