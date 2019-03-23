using UnityEngine;

public static class PackedMessageExtensions
{
    public static string Pack(this RawMessage msg)
    {
        var message = JsonUtility.ToJson(msg);
        var type = msg.GetType();

        return JsonUtility.ToJson(new PackedMessage(message, type.Name));
    }

    public static PackedMessage ToPackedMessage(this string msg)
    {
        return JsonUtility.FromJson<PackedMessage>(msg);
    }

    public static T Unpack<T>(this PackedMessage msg, string sender = null)
    {
        if (!string.IsNullOrEmpty(sender) && msg.sender != sender)
        {
            return default(T);
        }
        if (msg == typeof(T))
        {
            return JsonUtility.FromJson<T>(msg.message);
        }
        return default(T);
    }

    public static SendMessageEvent ToEvent(this RawMessage msg)
    {
        return new SendMessageEvent(msg.Pack());
    }
}
