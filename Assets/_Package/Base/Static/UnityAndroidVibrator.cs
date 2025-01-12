using UnityEngine;
public class UnityAndroidVibrator : MonoSingletonGlobal<UnityAndroidVibrator> 
{
#if UNITY_ANDROID || UNITY_EDITOR 
    private static AndroidJavaObject plugin = null;
#endif

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
#if UNITY_ANDROID && !UNITY_EDITOR
        plugin = new AndroidJavaClass("com.oilwelldrilling.master.UnityAndroidVibrator").CallStatic<AndroidJavaObject>("instance");
#endif
    }


    /// <summary>
    /// <para>Vibrates For Given Amount Of Time.</para>
    /// <para>1 sec = 1000 Millseconds :)</para>
    /// </summary>
    /// <param name="DurationInMilliseconds">Duration in milliseconds.</param>
    public void VibrateForGivenDuration(int DurationInMilliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(RuntimeStorageData.Sound.isVibrate)
        {
            plugin.Call("VibrateForGivenDuration", DurationInMilliseconds);
        }
#endif
    }

    /// <summary>
    /// Stoping All Vibrate.
    /// </summary>
    public void StopVibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            plugin.Call("StopVibrate");
#endif
    }


    /// <summary>
    /// <para>Customs Vibrate or Vibration with Pattern.</para>
    /// <para>Start without a delay</para>
    /// <para>Each element then alternates between vibrate, sleep, vibrate, sleep...</para>
    /// <para>long[] Pattern = {0, 100, 100, 300};</para>
    /// </summary>
    /// <param name="Pattern">Pattern.</param>
    public void CustomVibrate(long[] Pattern)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (RuntimeStorageData.Sound.isVibrate)
        {
            plugin.Call("CustomVibrate", Pattern);
        }
#endif
    }
}