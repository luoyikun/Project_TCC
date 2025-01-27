﻿# BoolContainer

#### **Namespace**: Unity.SaveData
---

## 概要:
`BoolContainer` は `DataContainerBase<bool>` の具体的な実装であり、単一のブール値を保存するためのデータコンテナです。このコンポーネントにより、ブール値をゲームオブジェクトに関連付けて管理し、必要に応じてその値を簡単に更新または参照できます。

## 主な特徴:
- **ブール値の保存**: 単一の `bool` 型の値を保存し、アクセスおよび更新を容易に行えます。
- **データ識別**: データコンテナを一意に識別するための ID (`PropertyName`) を提供します。

## 使用例:
- Unity エディタで `BoolContainer` コンポーネントをゲームオブジェクトに追加します。
- コンポーネントのプロパティを通じて、保存したいブール値を設定します。
- ゲームのロジック内で、このコンポーネントの `Value` プロパティを使用してブール値を取得または設定します。

## 追加情報:
- `BoolContainer` は、ゲームの状態管理や設定のオン/オフ切り替えなど、単一のブール値を基にした機能を実装する際に特に便利です。
- `DataContainerBase<bool>` から継承しているため、`SaveDataControl` システムと組み合わせて使用することで、データの保存と読み込みが容易になります。

`BoolContainer` は、ゲーム開発におけるブール値の効率的な管理と永続化を支援するシンプルながら強力なツールです。このコンポーネントを利用することで、ゲーム内でのブール値の使用がより柔軟かつ整理された形で行えるようになります。