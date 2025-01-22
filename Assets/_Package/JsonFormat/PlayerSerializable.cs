using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSerializable
{
    [System.Serializable]
    public class CharacterUnlockData
    {
        public string Id;
        public string Time;

        public CharacterUnlockData(string id, string time)
        {
            Id = id;
            Time = time;
        }
    }

    public string Id = "hihidochoo";
    public bool IsAds = false;
    public int Gold;
    public string Language;
    public List<string> Packages;
    public List<CharacterUnlockData> CharacterUnlocks;
    public int DayCheckIn = 0;
    public int NumberOfCheckIn = 0;

    public PlayerSerializable()
    {
        Id = SystemInfo.deviceUniqueIdentifier;
        IsAds = false;
        Gold = 0;
        Language = "English";
        Packages = new List<string>();
        CharacterUnlocks = new List<CharacterUnlockData>();
        NumberOfCheckIn = 0;
        DayCheckIn = 0;
    }

    public bool IsProductId(string productId)
    {
        if (Packages.Contains(productId)) return true;
        return false;
    }

    public void AddProductId(string productId)
    {
        if (Packages.Contains(productId) == false)
            Packages.Add(productId);
    }

    public CharacterUnlockData GetCharacterUnlockData(string productId)
    {
        for(int i = 0; i < CharacterUnlocks.Count; i++)
        {
            if (CharacterUnlocks[i].Id == productId)
                return CharacterUnlocks[i];
        }

        return null;
    }

    public void AddCharacterUnlockData(string productID)
    {
        var exists = CharacterUnlocks.Exists(x => x.Id == productID);
        if(exists == false)
        {
            CharacterUnlockData data = new CharacterUnlockData(productID, DateTime.Now.ToString());
            CharacterUnlocks.Add(data);
        }    
        else
        {
            var data = CharacterUnlocks.Find(x => x.Id == productID);
            data.Time = DateTime.Now.ToString();
        }    
    }
}

