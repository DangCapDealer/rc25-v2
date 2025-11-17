using System;
using UnityEngine;

namespace JKit.Monetize.Ads
{
    public class NativeOverlay
    {
        private INativeOverlay _client;

        public event Action OnImpression;
        public event Action OnClicked;
        public event Action OnClosed;

        private NativeOverlay(INativeOverlay client)
        {
            _client = client;

            _client.OnImpression += async () =>
            {
                await Awaitable.MainThreadAsync();
                OnImpression?.Invoke();
            };

            _client.OnClicked += async () =>
            {
                await Awaitable.MainThreadAsync();
                OnClicked?.Invoke();
            };

            _client.OnClosed += async () =>
            {
                await Awaitable.MainThreadAsync();
                OnClosed?.Invoke();
            };
        }

        public static void Load(string adUnitId, long refresh, Action<NativeOverlay, LoadAdError> callback)
        {
#if UNITY_ANDROID
            INativeOverlay client = new AndroidNativeOverlay();
#endif
            client.OnLoadSucceed += () => { callback(new NativeOverlay(client), null); };

            client.OnLoadFailed += error => { callback(null, error); };

            client.Load(adUnitId, refresh);
        }

        public void Show() => _client?.Show();
        public void Destroy() => _client?.Destroy();
    }


    public interface INativeOverlay
    {
        event Action OnImpression;
        event Action OnClicked;
        event Action OnClosed;
        event Action OnLoadSucceed;
        event Action<LoadAdError> OnLoadFailed;

        void Load(string adUnitId, long refresh);
        void Show();
        void Destroy();
    }
}