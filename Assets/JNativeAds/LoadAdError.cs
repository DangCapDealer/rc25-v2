using UnityEngine;

namespace JKit.Monetize.Ads
{
    public class LoadAdError
    {
        private AndroidJavaObject error;

        public LoadAdError(AndroidJavaObject error)
        {
            this.error = error;
        }
    }
}