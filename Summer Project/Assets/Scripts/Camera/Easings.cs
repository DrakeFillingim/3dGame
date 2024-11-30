using UnityEngine;

public static class Easings
{
    public static float EaseInQuart(float t)
    {
        return Mathf.Pow(t, 4);
    }

    public static float EaseOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);
    }

    public static float EaseOutSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }
}
