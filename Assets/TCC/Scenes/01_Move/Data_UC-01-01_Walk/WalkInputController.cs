using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;

/// <summary>
/// 使用 Player Input 的 Move 动作驱动角色移动，并同步移动动画参数。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput), typeof(MoveControl), typeof(Animator))]
public sealed class WalkInputController : MonoBehaviour
{
    /// <summary>Player Input 中用于控制移动的动作名称。</summary>
    [SerializeField] private string _moveActionName = "Move";

    /// <summary>Animator 中接收当前移动速度的参数名称。</summary>
    [SerializeField] private string _animationSpeedParameter = "Speed";

    /// <summary>当前对象上的输入组件。</summary>
    private PlayerInput _playerInput;

    /// <summary>当前对象上的移动控制组件。</summary>
    private MoveControl _moveControl;

    /// <summary>当前对象上的动画组件。</summary>
    private Animator _animator;

    /// <summary>从 Player Input 当前动作映射中解析出的移动动作。</summary>
    private InputAction _moveAction;

    /// <summary>缓存组件引用，供输入回调和每帧动画同步使用。</summary>
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveControl = GetComponent<MoveControl>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>订阅 Player Input 当前动作映射中的移动动作。</summary>
    private void OnEnable()
    {
        InputActionMap actionMap = _playerInput.currentActionMap;
        _moveAction = actionMap != null
            ? actionMap.FindAction(_moveActionName)
            : _playerInput.actions.FindAction(_moveActionName);

        if (_moveAction == null)
        {
            Debug.LogError($"Player Input 中找不到移动动作 '{_moveActionName}'。", this);
            enabled = false;
            return;
        }

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
    }

    /// <summary>取消输入订阅并清除残留的移动输入。</summary>
    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
        }

        if (_moveControl != null)
        {
            _moveControl.Move(Vector2.zero);
        }
    }

    /// <summary>将 Move 动作当前的二维值传给角色移动组件。</summary>
    /// <param name="context">Input System 提供的输入动作回调上下文。</param>
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveControl.Move(context.ReadValue<Vector2>());
    }

    /// <summary>在 Move 动作取消时清空移动输入，使角色开始减速停止。</summary>
    /// <param name="context">Input System 提供的输入动作回调上下文。</param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveControl.Move(Vector2.zero);
    }

    /// <summary>接收动画片段中的命名事件，避免未处理的导入事件产生运行时警告。</summary>
    /// <param name="eventName">动画片段传入的事件名称；当前场景中的 Footsteps 事件不播放音效。</param>
    public void TriggerAnimationEvent(string eventName)
    {
        // Walk2 动画带有 Footsteps 事件；此场景未配置脚步音效，因此仅接收该事件。
    }

    /// <summary>每帧把 MoveControl 当前速度写入 Animator 的 Speed 参数。</summary>
    private void Update()
    {
        _animator.SetFloat(_animationSpeedParameter, _moveControl.CurrentSpeed);
    }
}
