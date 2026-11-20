# UC-01-01_Walk 场景运行说明

场景文件：`Assets/TCC/Scenes/01_Move/UC-01-01_Walk.unity`

## 移动控制流程

`WASD`、方向键或手柄左摇杆输入 → Input System 的 `Player/Move` 动作（`Vector2`）→ Player 上的 `WalkInputController` → `MoveControl.Move()` → Brain 应用移动速度和方向。

- 输入动作及绑定定义在 `Assets/TCC/Scenes/01_Move/Data_UC-01-01_Walk/UC-01-01_Move.inputactions`。键盘使用 WASD 和方向键，手柄使用左摇杆；动作名为 `Move`，类型为 `Value/Vector2`。
- Player 上的 `WalkInputController` 通过 `PlayerInput` 取得 `Move` 动作；输入动作触发时将 `Vector2` 传给 `MoveControl.Move()`，输入取消时传入零向量以便角色减速停止。
- `MoveControl` 保存输入方向，并按相机的水平朝向将它转换为世界方向；角色移动时会朝该方向转身。角色处于可移动坡面时，速度方向会沿地面投影。
- 场景中的 `MoveControl` 参数为：移动速度 `1.2`、转向速度 `10`、加速度 `6`、刹车力度 `12`、移动优先级 `1`、转向优先级 `1`、停止阈值 `0.2`、可沿坡面移动的角度 `45°`。
- `MoveControl` 的移动优先级仅在速度高于停止阈值时生效。Brain 的 `MoveManager` 选择当前优先级最高的移动控制器，再由 `BrainBase` 应用最终速度和位置。

## 动画与状态显示

- `WalkInputController` 每帧读取 `MoveControl.CurrentSpeed` 并写入 Animator 的 `Speed` 参数，用于驱动行走动画。Player 上原有的 Visual Scripting Script Machine 已由该脚本替代。
- 行走动画片段包含 `TriggerAnimationEvent("Footsteps")` 事件；`WalkInputController` 提供接收方法。本场景没有配置脚步音效播放。
- 另一个状态显示子图以 `0.2` 为阈值，在 TextMeshPro 状态文本中切换 `State:Walking` 和 `State:Stop`。
