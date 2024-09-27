using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenUtility
{
    public static float OrthographicSize = 5.0f;
    const float defaultAspect = 2.0f;
    const float defaultOrthoSize = 5.0f;

    public static void CorrectOrthographicSize(Camera mainCamera)
    {
        float ratio = defaultAspect / GetAspectRatio();
        mainCamera.orthographicSize = defaultOrthoSize * ratio;
    }
    public static Rect GetScreenPixelRect(int PlayerID)
    {
        if(PlayerID < 0 || PlayerID > 3)
        {
            Debug.LogError("Attempting to access non-existant screen");
        }
        float w = Screen.width * 0.25f;
        return new Rect(w * PlayerID, 0, w, Screen.height);
    }
    public static Rect GetFullWorldRect()
    {
        float AR = GetAspectRatio();
        float w = OrthographicSize * AR;
        return new Rect(-w, -OrthographicSize, w, OrthographicSize);
    }
    public static Rect GetScreenWorldRect(int PlayerID)
    {
        if (PlayerID < 0 || PlayerID > 3)
        {
            Debug.LogError("Attempting to access non-existant screen");
        }
        float AR = GetAspectRatio();
        float w = OrthographicSize * AR;
        float screenWidth = w * 0.25f;
        return new Rect(-w + (screenWidth*PlayerID), -OrthographicSize, screenWidth, OrthographicSize);
    }
    public static Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        screenPosition.x /= Screen.width;
        screenPosition.y /= Screen.height;

        screenPosition -= Vector2.one * 0.5f;
        screenPosition *= OrthographicSize * 2.0f;
        float AR = GetAspectRatio();
        screenPosition.x *= GetAspectRatio();

        return screenPosition;
    }
    public static Vector2 ViewToWorld(Vector2 viewPosition)
    {
        viewPosition -= Vector2.one * 0.5f;
        viewPosition *= OrthographicSize * 2.0f;
        float AR = GetAspectRatio();
        viewPosition.x *= GetAspectRatio();

        return viewPosition;
    }

    public static float GetAspectRatio()
    {
        return (float)Screen.width / Screen.height;
    }
    /// <summary>
    /// Takes a horizontal position and converts it to fit on the current aspect ratio.
    /// Only applies to objects that were correctly positioned at 1344x672
    /// </summary>
    /// <param name="xVal"></param>
    /// <returns></returns>
    public static float ConvertToNonStandardAspect(float xVal)
    {
        //2 is the 1344x672 aspect ratio
        float ratio = 2 / GetAspectRatio();
        return xVal / ratio;
    }

    /// <summary>
    /// Is a particular point on screen?
    /// Useful for things that need to be disabled when they fly off screen!
    /// </summary>
    /// <param name="posToTest">The position we're testing.</param>
    /// <param name="screenID">Which player's screen do we want to test against? -1 means "only check the vertical axis".</param>
    /// <param name="buffer">A real world buffer to add to the check. Bigger values essential make the screen seem bigger.</param>
    /// <returns></returns>
    public static bool IsOnScreen(Vector2 posToTest, int screenID, float buffer = 0.0f)
    {
        float minY = -5.0f; float h = 10f;
        float minX = -10.0f; float w = 5.0f;

        bool onScreen = false;
        onScreen |= (posToTest.y + buffer) > minY && (posToTest.y - buffer) < (minY + h);
        if(screenID > -1)
            onScreen |= (posToTest.x + buffer) > (minX + (w*screenID)) && (posToTest.x - buffer) < (minX + (w * screenID) + w);
        return onScreen;
    }

    public static Vector2 ClampToScreen(Vector2 posToTest, int screenID, float buffer = 0.0f)
    {
        float minY = -5.0f; float h = 10f;
        float minX = -10.0f; float w = 5.0f;

        posToTest.x = Mathf.Clamp(posToTest.x, (minX + (w * screenID)) + buffer, minX + ((w * screenID) + w) - buffer);
        posToTest.y = Mathf.Clamp(posToTest.y, minY + buffer, (minY + h) - buffer);
        return posToTest;
    }
}
