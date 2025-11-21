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

    public TextAsset firebaseConfigJson;
    public void LoadDefaultRemoteConfig()
    {
        if (firebaseConfigJson == null)
        {
            Debug.LogError("Firebase Config JSON file is not assigned!");
            return;
        }
        
        try
        {
            string jsonContent = firebaseConfigJson.text;
            FirebaseConfigData configData = JsonUtility.FromJson<FirebaseConfigData>(jsonContent);
            
            if (configData?.parameters == null)
            {
                Debug.LogError("Failed to parse Firebase config data!");
                return;
            }
            
            // Apply config to Manager
            if (Manager.Instance != null)
            {
                // Parse boolean values
                Manager.Instance.IsVIPMember = configData.parameters.inapp_membership?.defaultValue?.value == "true";
                
                // Parse timer values
                if (double.TryParse(configData.parameters.inter_home?.defaultValue?.value, out double interHome))
                    Manager.Instance.InterHomeReloadTimer = interHome;
                    
                if (double.TryParse(configData.parameters.inter_auto?.defaultValue?.value, out double interAuto))
                    Manager.Instance.InterAutoReloadTimer = interAuto;
                
                // Parse boolean flags (0 = false, 1 = true)
                Manager.Instance.IsPopupUnlock = configData.parameters.button_unlock_all_character?.defaultValue?.value == "1";
                Manager.Instance.IsBanner = configData.parameters.banner_normal_active?.defaultValue?.value == "1";
                Manager.Instance.IsMREC = configData.parameters.banner_mrec_active?.defaultValue?.value == "1";
                Manager.Instance.IsNativeBanner = configData.parameters.native_banner?.defaultValue?.value == "1";
                Manager.Instance.IsNativeMREC = configData.parameters.native_mrec?.defaultValue?.value == "1";
                Manager.Instance.IsNativeInter = configData.parameters.native_inter?.defaultValue?.value == "1";
                
                Debug.Log("✅ Firebase config loaded from JSON successfully!");
                LogConfigValues();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading Firebase config JSON: {e.Message}");
        }
    }

    private void LogConfigValues()
    {
        Debug.Log("🔧 Firebase Config Values:");
        Debug.Log($"- VIP Member: {Manager.Instance.IsVIPMember}");
        Debug.Log($"- Inter Home Timer: {Manager.Instance.InterHomeReloadTimer}s");
        Debug.Log($"- Inter Auto Timer: {Manager.Instance.InterAutoReloadTimer}s");
        Debug.Log($"- Popup Unlock: {Manager.Instance.IsPopupUnlock}");
        Debug.Log($"- Banner: {Manager.Instance.IsBanner}");
        Debug.Log($"- MREC: {Manager.Instance.IsMREC}");
        Debug.Log($"- Native Banner: {Manager.Instance.IsNativeBanner}");
        Debug.Log($"- Native MREC: {Manager.Instance.IsNativeMREC}");
        Debug.Log($"- Native Inter: {Manager.Instance.IsNativeInter}");
    }

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
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = Firebase.FirebaseApp.DefaultInstance;
                IsInitialized = true;
                RunAllMessage();
                // FetchDataAsync();
                LoadDefaultRemoteConfig();
                Debug.Log("✅ Firebase is ready to use.");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
        yield return WaitForSecondCache.WAIT_TIME_ONE;
        if (Manager.Instance != null)
            Manager.Instance.IsFirebase = true;
    }    

    public void LogUMP(string message)
    {
        if (IsInitialized == false) { AddMessage(() => LogMessage(message)); }   
        else { LogMessage(message); }    
    }

    public void LogEvent(string message)
    {
        if (IsInitialized == false) { AddMessage(() => LogMessage(message)); }
        else { LogMessage(message); }

        LogSystem.LogSuccess(message);
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
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            return;
        }

        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                Manager.Instance.IsPopupUnlock = FirebaseRemoteConfig.DefaultInstance.GetValue("button_unlock_all_character").StringValue == "0" ? false : true;
                Manager.Instance.InterHomeReloadTimer = FirebaseRemoteConfig.DefaultInstance.GetValue("inter_home").DoubleValue;
                Manager.Instance.InterAutoReloadTimer = FirebaseRemoteConfig.DefaultInstance.GetValue("inter_auto").DoubleValue;
                Manager.Instance.IsBanner = FirebaseRemoteConfig.DefaultInstance.GetValue("banner_normal_active").StringValue == "0" ? false : true;
                Manager.Instance.IsMREC = FirebaseRemoteConfig.DefaultInstance.GetValue("banner_mrec_active").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeBanner = FirebaseRemoteConfig.DefaultInstance.GetValue("native_banner").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeMREC = FirebaseRemoteConfig.DefaultInstance.GetValue("native_mrec").StringValue == "0" ? false : true;
                Manager.Instance.IsNativeInter = FirebaseRemoteConfig.DefaultInstance.GetValue("native_inter").StringValue == "0" ? false : true;
                Manager.Instance.IsVIPMember = FirebaseRemoteConfig.DefaultInstance.GetValue("inapp_membership").BooleanValue;
            });
    }
#else
    public bool IsInitialized = true;
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

    public void LogEvent(string message)
    {
        Debug.Log($"[{this.GetType().ToString()}] Log Event {message}.");
    }
#endif
}

[System.Serializable]
public class FirebaseConfigData
{
    public ParametersData parameters;
    public VersionData version;
}

[System.Serializable]
public class ParametersData
{
    public ParameterInfo inapp_membership;
    public ParameterInfo inter_home;
    public ParameterInfo native_banner;
    public ParameterInfo button_unlock_all_character;
    public ParameterInfo banner_mrec_active;
    public ParameterInfo native_inter;
    public ParameterInfo inter_auto;
    public ParameterInfo native_mrec;
    public ParameterInfo banner_normal_active;
}

[System.Serializable]
public class ParameterInfo
{
    public DefaultValue defaultValue;
    public string description;
    public string valueType;
}

[System.Serializable]
public class DefaultValue
{
    public string value;
}

[System.Serializable]
public class VersionData
{
    public string versionNumber;
    public string updateTime;
}
