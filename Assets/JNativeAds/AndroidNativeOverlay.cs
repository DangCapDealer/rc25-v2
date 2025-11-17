using System;
using UnityEngine;

namespace JKit.Monetize.Ads
{
    internal class AndroidNativeOverlay : AndroidJavaProxy, INativeOverlay
    {
        private AndroidJavaObject nativeOverlayAd;

        public AndroidNativeOverlay() : base("com.jackie.jnativeads.UnityNativeOverlayCallback")
        {
            nativeOverlayAd = new AndroidJavaObject("com.jackie.jnativeads.UnityNativeOverlay", this);
        }

        public event Action OnImpression;
        public event Action OnClicked;
        public event Action OnClosed;
        public event Action OnLoadSucceed;
        public event Action<LoadAdError> OnLoadFailed;

        public void Load(string adUnitId, long refresh) => this.nativeOverlayAd.Call("loadAd", adUnitId, Input.deviceOrientation.ToString(), refresh);

        public void Show() => this.nativeOverlayAd.Call("show");
        public void Destroy() => this.nativeOverlayAd.Call("destroy");

        private async void onLoadSucceed()
        {
            await Awaitable.MainThreadAsync();
            OnLoadSucceed?.Invoke();
        }

        private async void onLoadFailed(AndroidJavaObject error)
        {
            await Awaitable.MainThreadAsync();
            OnLoadFailed?.Invoke(new LoadAdError(error));
        }

        private async void onClosed()
        {
            await Awaitable.MainThreadAsync();
            OnClosed?.Invoke();
        }

        private async void onClicked()
        {
            await Awaitable.MainThreadAsync();
            OnClicked?.Invoke();
        }

        private async void onImpression()
        {
            await Awaitable.MainThreadAsync();
            OnImpression?.Invoke();
        }
    }
}