using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if FIREBASE
using Firebase.Extensions;
using Firebase.Analytics;
using Firebase.RemoteConfig;
#endif
using System;
using System.Threading.Tasks;

[DefaultExecutionOrder(-9)]
public class FirebaseManager : MonoSingletonGlobal<FirebaseManager>
{
#if FIREBASE
    public bool IsInitialized = false;
    Firebase.FirebaseApp app;

    private void Start()
    {
        StartCoroutine(LoadFirebase());
    }

    private IEnumerator LoadFirebase()
    {
        Firebase.FirebaseApp.Create();
        yield return null;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
           // Debug.Log($"[{this.GetType().ToString()}] Firebase Set Analytics Collection Enabled");
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                IsInitialized = true;
               // Debug.Log($"[{this.GetType().ToString()}] Firebase Initialized");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                RunAllMessage();
                FetchDataAsync();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        yield return WaitForSecondCache.WAIT_TIME_ONE;
        if (Manager.Instance != null)
            Manager.Instance.IsFirebase = true;
    }    

    public void LogUMP(string message)
    {
        if (IsInitialized == false)
        {
            AddMessage(() => LogMessage(message));
        }   
        else
        {
            LogMessage(message);
        }    
    }

    public void LogEvent(string message)
    {
        if (IsInitialized == false)
        {
            AddMessage(() => LogMessage(message));
        }
        else
        {
            LogMessage(message);
        }
    }   

    private void LogMessage(string message)
    {
        FirebaseAnalytics.LogEvent(message);
    }

    private List<System.Action> Messages = new List<System.Action>();
    public void AddMessage(System.Action taskAction)
    {
        Messages.Add(taskAction);
    }

    private void RunAllMessage()
    {
        for (int i = 0; i < Messages.Count; i++)
        {
            Messages[i].Invoke();
        }

        ClearCompletedMessage();
    }
    private void ClearCompletedMessage()
    {
        Messages.RemoveAll(task => task == null);
    }

    public Task FetchDataAsync()
    {
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            // Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            // Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                //Manager.Instance.BannerReloadTimer = FirebaseRemoteConfig.DefaultInstance.GetValue("banner_reload_timer").DoubleValue;
                Manager.Instance.IsPopupUnlock = FirebaseRemoteConfig.DefaultInstance.GetValue("button_unlock_all_character").StringValue == "0" ? false : true;
                Manager.Instance.InterHomeReloadTimer = FirebaseRemoteConfig.DefaultInstance.GetValue("inter_home").DoubleValue;
                Manager.Instance.InterAutoReloadTimer = FirebaseRemoteConfig.DefaultInstance.GetValue("inter_auto").DoubleValue;
                Manager.Instance.IsBanner = FirebaseRemoteConfig.DefaultInstance.GetValue("banner_normal_active").StringValue == "0" ? false : true;
                Manager.Instance.IsMREC = FirebaseRemoteConfig.DefaultInstance.GetValue("banner_mrec_active").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeBanner = FirebaseRemoteConfig.DefaultInstance.GetValue("native_banner").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeMREC = FirebaseRemoteConfig.DefaultInstance.GetValue("native_mrec").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeInter = FirebaseRemoteConfig.DefaultInstance.GetValue("native_inter").StringValue == "0" ? false : true;
            });
    }
#else
    public bool IsNativeAd_Item = false;
    public void EventClickItem(string message, string number_click)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event Click Item.");
    }

    public void EventClickButton(string message, string number_click)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event Click Button.");
    }

    public void EventWinLevel(string number_win, string number_time)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event Win Level.");
    }

    public void EventLoseLevel(string number_lose, string number_time)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event Lose Level.");
    }

    public void EventFirstLoseLevel(string number_time)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event First Lose Level.");
    }

    public void EventLeaveLevel(string number_home, string number_time)
    {
        Debug.Log($"[{this.GetType().ToString()}] Event Leave Level.");
    }

    public void LogUMP(string message)
    {
        Debug.Log($"[{this.GetType().ToString()}] Log UMP.");
    }
#endif
}
