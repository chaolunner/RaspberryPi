using UniEasy.ECS;
using UnityEngine;
using UniEasy;
using UniRx;

public class JoystickComponent : ComponentBehaviour
{
    public RectTransform Control;
    [Reorderable]
    public RectTransform[] Contents;
    public Vector2ReactiveProperty Axis;
}
