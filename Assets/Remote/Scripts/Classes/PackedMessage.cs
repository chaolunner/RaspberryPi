using System;

[Serializable]
public class PackedMessage
{
    public string message;
    public string type;
    public string sender;

    public PackedMessage(string message, string type, string sender = "Unity")
    {
        this.message = message;
        this.type = type;
        this.sender = sender;
    }

    public override bool Equals(object obj)
    {
        if (obj is Type)
        {
            return type == (obj as Type).Name;
        }
        if (obj is string)
        {
            return type == obj as string;
        }
        if (obj is PackedMessage)
        {
            var msg = obj as PackedMessage;
            return message == msg.message && type == msg.type && sender == msg.sender;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return type.GetHashCode() + message.GetHashCode() + sender.GetHashCode();
    }

    public static bool operator ==(PackedMessage msg, object obj)
    {
        return msg.Equals(obj);
    }

    public static bool operator !=(PackedMessage msg, object obj)
    {
        return !msg.Equals(obj);
    }
}
