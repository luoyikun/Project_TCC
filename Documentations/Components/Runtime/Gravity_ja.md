﻿# Gravity

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## 概要:
`Gravity` は、キャラクターに重力加速度を適用するコンポーネントです。設定された重力のスピードに応じて下向きの加速度が追加され、各キャラクターごとに加速度の乗数を変更することができます。地面に接触している間は加速度は適用されません。着地と離陸のタイミングでイベントが実行されます。ジャンプなどの上下に動くコンポーネントはこの値を操作することがあります。

## 機能と操作:
- **重力加速度の適用**: キャラクターに下向きの重力加速度を適用します。
- **重力乗数の設定**: 重力加速度の乗数を設定し、落下スピードを調整できます。
- **着地・離陸イベント**: 地面に着地したり、離れたりする際にイベントが発生します。
- **落下スピードの取得**: 負の値で落下中、正の値で上昇中を示します。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `OnLanding` | 地面に着地した際に発火するイベントです。 |
| `OnLeave` | キャラクターが地面から離れた際に発火するイベントです。 |
| `FallSpeed` | 現在の落下スピードを表します。 |
| `GravityScale` | 重力の乗数を表すプロパティです。 |
| `IsLeaved` | キャラクターが地面から離れたかどうかを示します。 |
| `IsLanded` | キャラクターが地面に着地したかどうかを示します。 |
| `CurrentState` | キャラクターの現在の状態（空中か地面上か）を表します。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
|  ``public void`` SetVelocity( ``Vector3 velocity`` )  | 落下速度を設定するメソッドです。 |

