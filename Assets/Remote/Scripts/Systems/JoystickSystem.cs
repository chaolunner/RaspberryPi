using UniRx.Triggers;
using UniEasy.ECS;
using UnityEngine;
using UniRx;

public class JoystickSystem : SystemBehaviour
{
    private IGroup joysticks;

    public override void Initialize(IEventSystem eventSystem, IPoolManager poolManager, GroupFactory groupFactory, PrefabFactory prefabFactory)
    {
        base.Initialize(eventSystem, poolManager, groupFactory, prefabFactory);

        joysticks = this.Create(typeof(JoystickComponent), typeof(ViewComponent));
    }

    public override void OnEnable()
    {
        base.OnEnable();

        joysticks.OnAdd().Subscribe(entity =>
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            var joystick = entity.GetComponent<JoystickComponent>();
            var rect = viewComponent.Transforms[0] as RectTransform;
            var radius = 0.5f * joystick.Control.sizeDelta.x;
            var localPoint = Vector2.zero;

            joystick.OnBeginDragAsObservable().Subscribe(evtData =>
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, evtData.position, null, out localPoint);

                joystick.Control.anchoredPosition = localPoint;
            }).AddTo(this.Disposer).AddTo(joystick.Disposer);

            joystick.OnDragAsObservable().Subscribe(evtData =>
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick.Control, evtData.position, null, out localPoint);

                if (localPoint.magnitude > radius)
                {
                    localPoint = radius * localPoint.normalized;
                }

                foreach (var content in joystick.Contents)
                {
                    content.anchoredPosition = localPoint;
                }

                joystick.Axis.Value = new Vector2(localPoint.x / radius, localPoint.y / radius);
            }).AddTo(this.Disposer).AddTo(joystick.Disposer);

            joystick.OnEndDragAsObservable().Subscribe(evtData =>
            {
                foreach (var content in joystick.Contents)
                {
                    content.anchoredPosition = Vector2.zero;
                }

                joystick.Axis.Value = Vector2.zero;
            }).AddTo(this.Disposer).AddTo(joystick.Disposer);
        }).AddTo(this.Disposer);
    }
}
