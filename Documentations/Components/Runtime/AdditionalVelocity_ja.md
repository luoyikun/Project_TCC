﻿# AdditionalVelocity

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## 概要:
`AdditionalVelocity` は、キャラクターにカスタムの加速度を設定するコンポーネントです。このコンポーネントは、外部から設定された加速度を変更せずに、その加速度をキャラクターの「Brain」に反映させます。

## 機能と操作:
- **加速度の設定**: キャラクターに対する加速度を外部から設定可能です。
- **速度の取得**: 設定された加速度から速度を算出し、取得することができます。
- **加速度のリセット**: 加速度をリセットし、キャラクターの速度をゼロに戻すことが可能です。
- **ギズモの描画**: Unityエディタのギズモを用いて、設定された加速度を視覚的に確認できます。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `Velocity` | キャラクターに適用される加速度を表す `Vector3` 型のプロパティです。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public void`` ResetVelocity( )  | 加速度をリセットするメソッドです。 |
