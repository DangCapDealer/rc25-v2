using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NativeAdPosition
{
    Banner,
    BannerCollapse,
    Interstitial
}

public class AdNativeManager : MonoSingleton<AdNativeManager>
{
    public RequestNativeAd[] requestNatives;

    public RequestNativeAd GetNativeAd(NativeAdPosition position)
    {
        for(int i = 0; i < requestNatives.Length; i++)
        {
            if (requestNatives[i].Position == position)
                return requestNatives[i];
        }    

        return null;
    }    
}
