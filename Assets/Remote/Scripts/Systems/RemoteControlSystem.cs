using UniRx.Triggers;
using UniEasy.ECS;
using UnityEngine;
using UniRx;

public class RemoteControlSystem : SystemBehaviour
{
    private IGroup joysticks;
    private IGroup keycodes;

    public override void Initialize(IEventSystem eventSystem, IPoolManager poolManager, GroupFactory groupFactory, PrefabFactory prefabFactory)
    {
        base.Initialize(eventSystem, poolManager, groupFactory, prefabFactory);

        joysticks = this.Create(typeof(JoystickComponent));
        keycodes = this.Create(typeof(KeycodeComponent));
    }

    public override void OnEnable()
    {
        base.OnEnable();

        var direction = Vector2.zero;

        joysticks.OnAdd().Subscribe(entity =>
        {
            var joystick = entity.GetComponent<JoystickComponent>();

            joystick.Axis.DistinctUntilChanged().Subscribe(axis =>
            {
                direction = axis;
                EventSystem.Publish(new JoystickMessage(direction).ToEvent());
            }).AddTo(this.Disposer).AddTo(joystick.Disposer);
        }).AddTo(this.Disposer);

        keycodes.OnAdd().Subscribe(entity =>
        {
            var keycode = entity.GetComponent<KeycodeComponent>();

            keycode.OnPointerDownAsObservable().Subscribe(_ =>
            {
                if (keycode.KeyCode == KeyCode.UpArrow)
                {
                    direction += Vector2.up;
                }
                else if (keycode.KeyCode == KeyCode.DownArrow)
                {
                    direction += Vector2.down;
                }
                else if (keycode.KeyCode == KeyCode.LeftArrow)
                {
                    direction += 0.25f * Vector2.left;
                }
                else if (keycode.KeyCode == KeyCode.RightArrow)
                {
                    direction += 0.25f * Vector2.right;
                }
                EventSystem.Publish(new JoystickMessage(direction).ToEvent());
            }).AddTo(this.Disposer).AddTo(keycode.Disposer);

            keycode.OnPointerUpAsObservable().Subscribe(_ =>
            {
                if (keycode.KeyCode == KeyCode.UpArrow)
                {
                    direction -= Vector2.up;
                }
                else if (keycode.KeyCode == KeyCode.DownArrow)
                {
                    direction -= Vector2.down;
                }
                else if (keycode.KeyCode == KeyCode.LeftArrow)
                {
                    direction -= 0.25f * Vector2.left;
                }
                else if (keycode.KeyCode == KeyCode.RightArrow)
                {
                    direction -= 0.25f * Vector2.right;
                }
                EventSystem.Publish(new JoystickMessage(direction).ToEvent());
            }).AddTo(this.Disposer).AddTo(keycode.Disposer);
        }).AddTo(this.Disposer);
    }
}
