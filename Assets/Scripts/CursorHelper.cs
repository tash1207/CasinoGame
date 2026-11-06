using UnityEngine;

public static class CursorHelper
{
    static Vector2 defaultCursorHotspot = new Vector2(14, 11);

    public static void ResetCursor()
    {
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }
}
