using UnityEngine;

public static class ScreenBounds
{
    private static bool init = false;
    private static float lowerY;

    public static float LowerY
    {
        get
        {
            if (!init)
            {
                var cam = Camera.main;
                float zDist = Mathf.Abs(cam.transform.position.z);
                var bottom = cam.ViewportToWorldPoint(new Vector3(0, 0, zDist));
                lowerY = bottom.y;
                init = true;
            }
			return lowerY;
		}
    }
}
