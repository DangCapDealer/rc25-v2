using System;
using UnityEngine;

public class StaticJsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static T getJson<T>(string json)
    {
        json = json.Replace("error", "");
        string newJson = "{ \"array\": " + json + "}";
        Wrapper2<T> _response = JsonUtility.FromJson<Wrapper2<T>>(newJson);
        return _response.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    [Serializable]
    private class Wrapper2<T>
    {
        public T array;
    }
}
