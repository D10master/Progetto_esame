using UnityEngine;

public class Exit : MonoBehaviour
{
    private bool connected = false;

    public bool Connected
    {
        get
        {
            return connected;
        }

        set
        {
            connected = value;
        }
    }
}
