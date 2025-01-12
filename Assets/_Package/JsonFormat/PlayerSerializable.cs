using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSerializable
{
    [System.Serializable]
    public class ItemLevel
    {
        public PoolName Id;
        public int Level;

        public ItemLevel(PoolName id, int level)
        {
            Id = id;
            Level = level;
        }
    }

    public string Id = "hihidochoo";
    public bool IsAds = false;
    public int Gold;
    public string Language;
    public List<string> Packages;
    public int LastDayLogin = 0;
    public int NumberOfDay = 0;

    public PlayerSerializable()
    {
        Id = SystemInfo.deviceUniqueIdentifier;
        IsAds = false;
        Gold = 0;
        Language = "English";
        Packages = new List<string>();
        NumberOfDay = 0;
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
}

