using UnityEngine;

public static class AudioVolume
{
    public static float master = 1f;  // 0–1
    public static float music = 1f;   // 0–1
    public static float sfx = 1f;     // 0–1

    public static float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f)
            return -80f;
        return Mathf.Log10(linear) * 20f;
    }
}
