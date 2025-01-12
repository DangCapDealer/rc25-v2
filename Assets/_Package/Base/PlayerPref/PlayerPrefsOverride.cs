using UnityEngine;

public class PlayerPrefsOverride : PlayerPrefs
{
    public static void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return GetInt(key, defaultValue ? 1 : 0) != 0;
    }
}