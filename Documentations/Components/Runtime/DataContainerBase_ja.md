﻿# DataContainerBase<T>

#### **Namespace**: Unity.SaveData.Core
---

## 概要:
`DataContainerBase<T>` は、任意のデータ型 `T` を保存するための基底コンポーネントです。この抽象クラスは、`SaveDataControl` によって管理されるデータの保存と識別を担います。

## 機能と操作:
- **データの識別**: データを一意に識別するための ID を保持します。
- **データの保存**: 特定のデータ型 `T` の値を保存し、アクセスを提供します。
- **自動登録**: コンポーネントが親に `SaveDataControl` を持たない場合、自動的に `SaveDataControl` を追加します。

## プロパティ
| Name | Description |
|------|-------------|
| `_id` | データを識別するための ID。 |
| `_value` | 保存されるデータの値。 |
| `Value` | `_value` の公開プロパティ。データの取得と設定が可能です。 |

## 使用例
- `DataContainerBase<T>` を継承するカスタムコンポーネントを作成し、具体的なデータ型 `T` を指定します。
- Unity エディタ上で、このコンポーネントをゲームオブジェクトに追加します。
- `_id` フィールドを設定してデータを一意に識別し、`_value` フィールドで保存するデータの値を指定します。
- スクリプト内で `Value` プロパティを使用して、保存されたデータにアクセスまたは変更します。

---
## その他の注意事項
- このコンポーネントは抽象クラスであり、直接インスタンス化することはできません。実際に使用するには、具体的なデータ型 `T` でサブクラスを作成する必要があります。
- `Reset` メソッドは、コンポーネントがゲームオブジェクトに追加された際に自動的に呼び出され、`SaveDataControl` の自動登録を試みます。これにより、データの保存とロードのプロセスが容易になります。

`DataContainerBase<T>` は、ゲーム開発においてデータを効率的に管理し、保存するための柔軟な基盤を提供します。カスタマイズ可能なデータ型と自動的な `SaveDataControl` 管理により、データの取り扱いが容易になります。
