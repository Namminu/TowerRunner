using UnityEngine;

public static class ScreenBounds
{
    private static bool init = false;
    private static float lowerY;
    private static float leftX, rightX;

    private static void Init()
    {
        if (init) return;
        
        var cam = Camera.main;
        float z = Mathf.Abs(cam.transform.position.z);
        var boundLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, z));
        var boundRight = cam.ViewportToWorldPoint(new Vector3(1f, 0f, z));
        leftX = boundLeft.x;
        rightX = boundRight.x;
        lowerY = boundLeft.y;
		init = true;
    }

    public static float LowerY
    {
        get { Init(); return lowerY; }
    }

    public static float LeftX
    {
		get { Init(); return leftX; }
	}

    public static float RightX
    {
		get { Init(); return rightX; }
	}
}
