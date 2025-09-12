using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private bool centreHotspot;

    private Vector2 cursorHotspot;

    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Vector2 hotspot = centreHotspot ? cursorHotspot : Vector2.zero;
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}
