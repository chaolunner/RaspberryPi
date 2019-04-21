using UniEasy.ECS;
using UnityEngine;
using UniRx;

public class ClientSystem : SystemBehaviour
{
    [SerializeField] SocketClient client;

    public override void OnEnable()
    {
        base.OnEnable();

        client.OnReceived += OnReceived;

        EventSystem.Receive<SendMessageEvent>().Subscribe(evt =>
        {
            client.Send(evt.Message);
        }).AddTo(this.Disposer);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        client.OnReceived -= OnReceived;
    }

    private void OnReceived(string message)
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
