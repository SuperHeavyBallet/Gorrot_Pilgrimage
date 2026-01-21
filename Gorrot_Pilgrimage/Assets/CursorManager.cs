using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    // Hotspot is the “click point” within the image.
    // For a classic arrow, hotspot is usually (0, cursorTexture.height) or (0, 0)
    // depending on how you drew it.
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        if (cursorTexture == null)
        {
            Debug.LogWarning("Cursor texture not assigned.");
            return;
        }

        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
}
