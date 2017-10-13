using UnityEngine;

public static partial class Extensions
{
    public static Canvas GetCanvas(this RectTransform transform)
    {
        Transform parent = transform.parent;
        Canvas canvas = null;
        while (parent != null)
        {
            canvas = parent.GetComponent<Canvas>();
            if (canvas != null)
            { break; }
            parent = parent.transform.parent;
        }
        return canvas;
    }

    public static Vector3 GetLocalPositionFromRoot(this RectTransform transform)
    {
        Transform parent = transform.parent;
        Vector3 pos = transform.localPosition;
        while (parent != null)
        {
            // Topmost = Canvas -> Don't calculate its RectTransform
            if (parent.parent == null)
            { break; }

            pos += parent.transform.localPosition;
            parent = parent.transform.parent;
        }
        return pos;
    }

    public static RectOffset ToScreenSpace(this RectTransform transform)
    {
        Canvas canvas = transform.GetCanvas();

        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            case RenderMode.ScreenSpaceCamera:
                {
                    // Determine RectTransform dimensions taking into consideration canvas scaling.
                    float width = transform.rect.width * canvas.scaleFactor;
                    float height = transform.rect.height * canvas.scaleFactor;

                    // Get center point of RectTransform in Canvas Space.
                    Vector3 pos = transform.GetLocalPositionFromRoot();
                    float centerX = pos.x * canvas.scaleFactor;
                    float centerY = pos.y * canvas.scaleFactor;

                    // Adjust for anchor from center to lower left.
                    float left = centerX - (width * 0.5f);
                    float bottom = centerY - (height * 0.5f);

                    // Convert from Canvas Space to Screen Space.
                    left = left + (Screen.width * 0.5f);
                    bottom = bottom + (Screen.height * 0.5f);

#if UNITY_ANDROID || UNITY_IOS
                    // Adjust pixel by its tolerance.
                    const int TOLERANCE = 5;
                    bottom += TOLERANCE;
                    width -= TOLERANCE;
#endif

                    return new RectOffset((int)left, (int)(left + width), (int)(bottom + height), (int)bottom);
                }
            default:
                throw new System.Exception("Only apply for render mode [Screen Space - Overlay] and [Screen Space - Camera].");
        }
    }
}