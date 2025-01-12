using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Text;
using UnityEngine.Events;
using System.Data;
using System.Xml.Linq;






#if UNITY_EDITOR
using System.Reflection;
#endif

public enum NetworkLoading
{
    Loading,
    Ready
}

public static class StaticVariable
{
    public static bool isLoaded = false;
    public static readonly string DATA_PLAYER = "player.data";
    public static readonly string DATA_SOUND = "sound.data";

    public static void PromiseFunction(UnityAction success, UnityAction error)
    {
        try
        {
            success?.Invoke();  
        }
        catch
        {
            error?.Invoke();
        }
    }

    public static Vector3 Round(this Vector3 originalVector, int digits)
    {
        originalVector = originalVector.WithX(Mathf.Round(originalVector.x * 100f) / 100f);
        originalVector = originalVector.WithY(Mathf.Round(originalVector.y * 100f) / 100f);
        originalVector = originalVector.WithZ(Mathf.Round(originalVector.z * 100f) / 100f);
        return originalVector;
    }

    public static Quaternion ToRotation(this Vector3 eulerAngles)
    {
        Quaternion rotation = Quaternion.Euler(eulerAngles);
        return rotation;
    }

    public static Vector3 ToEulerAngles(this Quaternion quaternion)
    {
        Vector3 eulerAngles = quaternion.eulerAngles;
        return eulerAngles;
    }

    public static Color toColor(this string hex)
    {
        //66331A
        if (!ColorUtility.TryParseHtmlString("#" + hex, out Color color))
        {
            Debug.LogError("Invalid hexadecimal color value: " + hex);
            color = Color.white; // Default color if conversion fails
        }

        return color;
    }

    public static void Show(this GameObject obj)
    {
        if(obj.activeInHierarchy == false)
            obj.SetActive(true);
    }

    public static void Hide(this GameObject obj)
    {
        if(obj.activeInHierarchy == true)
            obj.SetActive(false);
    }

    public static void Show(this Transform obj)
    {
        if (obj.gameObject.activeInHierarchy == false)
            obj.SetActive(true);
    }

    public static void Hide(this Transform obj)
    {
        if (obj.gameObject.activeInHierarchy == true)
            obj.SetActive(false);
    }


    public static void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }

    public static int Ramdom(params int[] arrays)
    {
        //Debug.Log("Array random");
        if (arrays == null || arrays.Length == 0)
            return 0;
        if (arrays.Length == 1)
            return arrays[0];
        return arrays[UnityEngine.Random.Range(0, Ramdom(arrays.Length))];
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T[] Add<T>(this T[] target, T item)
    {
        if (target == null)
        {
            return null;
        }

        T[] result = new T[target.Length + 1];
        target.CopyTo(result, 0);
        result[target.Length] = item;
        return result;
    }

    public static T[] OnlyAdd<T>(this T[] target, T item)
    {
        if (target == null)
        {
            return null;
        }

        for (int i = 0; i < target.Length; i++)
            if (Compare(target[i], item))
                return target;

        T[] result = new T[target.Length + 1];
        target.CopyTo(result, 0);
        result[target.Length] = item;
        return result;
    }

    public static bool Compare<T>(T x, T y)
    {
        return EqualityComparer<T>.Default.Equals(x, y);
    }

    public static bool IsFound<T>(this T[] target, T item)
    {
        for(int i = 0; i < target.Length; i++)
        {
            if (Compare(target[i], item) == true)
                return true;
        }
        return false;
    }

    public static bool IsActive(this Transform target)
    {
        if (target == null)
            return false;

        bool result = false;

        result = target.gameObject.activeInHierarchy;

        return result;
    }

    public static bool IsActive(this GameObject target)
    {
        if (target == null)
            return false;

        bool result = false;

        result = target.gameObject.activeInHierarchy;

        return result;
    }

    public static T GetOnce<T>(this T[] target)
    {
        var i = UnityEngine.Random.Range(0, target.Length);
        return target[i];
    }

    internal static Transform FindChildByRecursion(this Transform aParent, string aName)
    {
        if (aParent == null) return null;    
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildByRecursion(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    internal static T FindChildByRecursion<T>(this Transform aParent)
    {
        if (aParent == null) return default;
        var result = aParent.GetComponent<T>();
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildByRecursion<T>();
            if (result != null)
                return result;
        }
        return default;
    }

    internal static Transform FindChildByParent(this Transform aParent, string aName)
    {
        if (aParent == null) return null;
        for(int i = 0; i < aParent.childCount; i++)
        {
            var child = aParent.GetChild(i);
            if (child.name == aName)
                return child;
        }
        return null;
    }

    internal static Transform FindChildByParent(this GameObject aParent, string aName)
    {
        if (aParent == null) return null;
        for (int i = 0; i < aParent.transform.childCount; i++)
        {
            var child = aParent.transform.GetChild(i);
            if (child.name == aName)
                return child;
        }
        return null;
    }

    internal static Transform FindChild(this GameObject aParent, int index)
    {
        if (aParent == null) return null;
        if (index >= aParent.transform.childCount)
            return null;
        return aParent.transform.GetChild(index);
    }

    internal static T ToEnum<T>(this string value, T defaultValue) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        T result;
        var IsConvert = Enum.TryParse<T>(value, true, out result);
        return IsConvert ? result : defaultValue;
    }

    internal static T[] SetToLast<T>(this T[] arr)
    {
        if (arr.Length > 0)
        {
            T firstNumber = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                arr[i - 1] = arr[i];
            }
            arr[arr.Length - 1] = firstNumber;
            return arr;
        }
        else
        {
            return null;
        }
    }

    internal static T[] SetToFirst<T>(this T[] arr)
    {
        if (arr.Length > 0)
        {
            T lastNumber = arr[arr.Length - 1];
            for (int i = arr.Length - 1; i > 0; i--)
            {
                arr[i] = arr[i - 1];
            }
            arr[0] = lastNumber;
            return arr;
        }
        else
        {
            return null;
        }
    }

    internal static Transform Find(this Transform[] arr, string name)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            if (arr[i].name == name)
                return arr[i];
        }
        return null;
    }

    internal static float Round(this float original)
    {
        return Mathf.Round(original * 100f) / 100f;
    }

    internal static int ConvertAndRount(this float original)
    {
        if (original < 0)
            return -1;
        else if (original > 0)
            return 1;
        else
            return 0;
    }

    internal static void Log<T>(this T[] arr)
    {
        arr.SimpleForEach(a => Debug.Log(a));
    }

    internal static void Log<T>(this T _t)
    {
        Debug.Log(_t);
    }

    internal static bool IsChild(this Transform aParent, string aName)
    {
        if (aParent == null) return false;
        for (int i = 0; i < aParent.childCount; i++)
        {
            var child = aParent.GetChild(i);
            if (child.name == aName)
                return true;
        }
        return false;
    }

    //internal static int GetRandom(params int[] arrs)
    //{
    //    return arrs[UnityEngine.Random.Range(0, arrs.Length)];
    //}

    internal static void SetActive(this Transform _object, bool _active)
    {
        GameObject _rp = _object.gameObject;
        _rp.SetActive(_active);
    }

    internal static Transform GetChild(this GameObject _object, int index)
    {
        var _child = _object.transform.GetChild(index);
        return _child;
    }

    private static StringBuilder output = new StringBuilder();
    internal static string AddSpaceBeforeUppercase(this string input)
    {
        output.Clear();

        for (int i = 0; i < input.Length; i++)
        {
            if (i > 0 && char.IsUpper(input[i]))
            {
                output.Append(" ");
            }

            output.Append(input[i]);
        }

        return output.ToString();
    }

    internal static Vector2 position(this GameObject _object)
    {
        return _object.transform.position;
    }

    public static Vector2 travelAlongCircle(this Vector2 pos, Vector2 center, float distance)
    {
        Vector3 axis = Vector3.back;
        Vector2 dir = pos - center;
        float circumference = 2.0f * Mathf.PI * dir.magnitude;
        float angle = distance / circumference * 360.0f;
        dir = Quaternion.AngleAxis(angle, axis) * dir;
        return dir + center;
    }

    public static Vector3 PositionInCircumference(Vector3 center, float radius, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians);
        float z = Mathf.Sin(radians);
        Vector3 position = new Vector3(x, center.y, z);
        position *= radius;
        position += center;

        return position;
    }

    public static Vector3 GetCirclePosition(double radius, Vector3 center)
    {
        int point = UnityEngine.Random.RandomRange(5, 10);
        double slice = 2 * Math.PI / point;
        double angle = slice * UnityEngine.Random.RandomRange(0, point);
        int newX = (int)(center.x + radius * Math.Cos(angle));
        int newY = (int)(center.y + radius * Math.Sin(angle));

        return new Vector3(newX, newY);
    }

    public static Vector3 GetRandomDirection()
    {
        Vector3 rotatedVector = Quaternion.AngleAxis(UnityEngine.Random.RandomRange(0f, 360f), Vector3.up) * Vector3.forward;
        return rotatedVector;
    }

    private static string[] Months = new string[] { "Jan", "Feb", "Mar",
                                                  "Apr", "May", "Jun",
                                                  "Jul", "Aug", "Sep",
                                                  "Oct", "Nov", "Dec"};
    public static string ConvertMonthIntToString(int Month)
    {
        return Months[Month - 1];
    }

    public static DayOfWeek ConvertDateTimeToDayOfWeek(DateTime dateTime)
    {
        return dateTime.DayOfWeek;
    }

    public static DayOfWeek ConvertDateTimeToDayOfWeek(int year, int month, int day)
    {
        DateTime dt = new DateTime(year, month, day);
        return dt.DayOfWeek;
    }

    public static string ConvertMonthIntToString(string Month)
    {
        int _Month = int.Parse(Month);
        return ConvertMonthIntToString(_Month);
    }

    public static void SetPostions(this LineRenderer lineRenderer, params Vector3[] postions)
    {
        if (lineRenderer == null)
            return;
        if (lineRenderer.positionCount != postions.Length)
            lineRenderer.positionCount = postions.Length;
        lineRenderer.SetPositions(postions);
    }

    public static void OpenUrl(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
            string _script = string.Format("window.open('{0}', '_blank')", url);
            Application.ExternalEval(_script);
#else
            Application.OpenURL(url);
#endif
    }
}
