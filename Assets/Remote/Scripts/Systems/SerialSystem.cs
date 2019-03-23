using UniEasy.ECS;
using UnityEngine;
using UniRx;

public class SerialSystem : SystemBehaviour
{
    [SerializeField] SerialHandler serialHandler;

    public override void Initialize(IEventSystem eventSystem, IPoolManager poolManager, GroupFactory groupFactory, PrefabFactory prefabFactory)
    {
        base.Initialize(eventSystem, poolManager, groupFactory, prefabFactory);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        serialHandler.OnDataReceived += OnDataReceived;

        EventSystem.Receive<SendMessageEvent>().Subscribe(evt =>
        {
            serialHandler.Write(evt.Message);
        }).AddTo(this.Disposer);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        serialHandler.OnDataReceived -= OnDataReceived;
    }

    private void OnDataReceived(string message)
    {
        var packedMessage = message.ToPackedMessage();

        if (packedMessage == typeof(JoystickMessage))
        {
            EventSystem.Publish(packedMessage.Unpack<JoystickMessage>());
        }

#if UNITY_EDITOR
        Debug.Log(string.Format("Get Message: {1}, From: {0}", packedMessage.sender, packedMessage.message));
#endif
    }
}
