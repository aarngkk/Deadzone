using UnityEngine;

public class DefaultCursorManager : MonoBehaviour
{
    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
