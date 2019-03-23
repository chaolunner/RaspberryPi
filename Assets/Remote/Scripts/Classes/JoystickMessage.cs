using UnityEngine;

public class JoystickMessage : RawMessage
{
    public int identifier;
    public Vector2 axis;

    public JoystickMessage(Vector2 axis, int identifier = 0)
    {
        this.identifier = identifier;
        this.axis = axis;
    }
}
