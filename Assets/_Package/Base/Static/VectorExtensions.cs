
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 CreateX(float x = 0)
    {
        return Vector2.zero.WithX(0);
    }

    public static Vector2 CreateY(float y = 0)
    {
        return Vector2.zero.WithY(0);
    }

    public static Vector2 Create2D(float x = 0, float y = 0)
    {
        return Vector2.zero.WithX(x).WithY(y);
    }

    public static Vector3 Create3D(float x = 0, float y = 0, float z = 0)
    {
        return Vector3.zero.WithX(x).WithY(y).WithZ(z);
    }

    public static Vector3 RandomY(this Vector3 vec, float defaultRange, float range)
    {
        vec.y = defaultRange + Random.Range(-range, range);
        return vec;
    }

    public static Quaternion WithY(this Quaternion currentRotation, float y)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.y = y;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion WithZ(this Quaternion currentRotation, float z)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.z = z;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion WithX(this Quaternion currentRotation, float x)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.x = x;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion AddX(this Quaternion currentRotation, float x)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.x += x;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion Add(this Quaternion currentRotation, Vector3 _offset)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.x += _offset.x;
        eulerRotation.y += _offset.y;
        eulerRotation.z += _offset.z;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion AddY(this Quaternion currentRotation, float y)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.y += y;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion AddZ(this Quaternion currentRotation, float z)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        eulerRotation.z += z;
        return Quaternion.Euler(eulerRotation);
    }

    public static Quaternion ClampZ(this Quaternion currentRotation, Vector2 direction, float minz, float maxz)
    {
        Vector3 eulerRotation = currentRotation.eulerAngles;
        Debug.Log(direction);
        //eulerRotation.z = Mathf.Clamp(eulerRotation.z, minz, maxz);
        return Quaternion.Euler(eulerRotation);
    }

    public static Vector3 ScaleRate(this Vector3 vec, float scale)
    {
        vec *= scale;
        return vec;
    }

    public static Vector3 ScaleValue(this Vector3 vec, float scale)
    {
        vec = Vector3.one * scale;
        return vec;
    }

    public static Vector3 WithX(this Vector3 vec, float x)
    {
        vec.x = x;
        return vec;
    }
    
    public static Vector3 WithY(this Vector3 vec, float y)
    {
        vec.y = y;
        return vec;
    }

    public static Vector3 AddY(this Vector3 vec, float y)
    {
        vec.y += y;
        return vec;
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        vec.z = z;
        return vec;
    }

    public static Vector2 WithX(this Vector2 vec, float x)
    {
        vec.x = x;
        return vec;
    }

    public static Vector2 WithY(this Vector2 vec, float y)
    {
        vec.y = y;
        return vec;
    }

    public static Vector2 AddY(this Vector2 vec, float y)
    {
        vec.y += y;
        return vec;
    }

    public static Vector3 WithXY(this Vector3 vec, float x,float y)
    {
        vec.x = x;
        vec.y = y;
        return vec;
    }

    public static Vector3 WithXZ(this Vector3 vec, float x,float z)
    {
        vec.x = x;
        vec.z = z;
        return vec;
    }

    public static Vector3 WithYZ(this Vector3 vec, float y,float z)
    {
        vec.y = y;
        vec.z = z;
        return vec;
    }

    public static Vector3 AddX(this Vector3 vec, float x)
    {
        vec.x += x;
        return vec;
    }

    public static Vector3 AddZ(this Vector3 vec, float z)
    {
        vec.z += z;
        return vec;
    }

    public static Vector2 GetNearestPointToVector(this Vector2 vec,Vector2 point)
    {
        return vec.GetNearestPointToVector(point, out var distance, out var endNearest);
    }

    public static Vector2 GetNearestPointToVector(this Vector2 vec, Vector2 point, out float distance)
    {
       return vec.GetNearestPointToVector(point, out distance, out var endNearest);
    }

    public static Vector2 GetNearestPointToVector(this Vector2 vec,Vector2 point,out float distance,out bool endNearest)
    {
        if (Vector2.Angle(vec,point)>=90)
        {
            distance = point.magnitude;
            endNearest = true;
            return Vector2.zero;
        }

        if (Vector2.Angle((point - vec), -vec) >= 90)
        {
            distance = (point - vec).magnitude;
            endNearest = true;
            return vec;
        }

        endNearest = false;
        distance = Mathf.Sin(Vector2.Angle(vec, point) * Mathf.Deg2Rad)*point.magnitude;
        return point.magnitude * Mathf.Cos(Vector2.Angle(vec, point) * Mathf.Deg2Rad)  * vec.normalized;
    }

    public static Vector2 ToXZ(this Vector3 vec)
    {
        return new Vector2(vec.x,vec.z);
    }

    // ReSharper disable once InconsistentNaming
    public static Vector3 ToXZtoXYZ(this Vector2 vec,float? y=null)
    {
        return new Vector3(vec.x, y ?? 0, vec.y);
    }

    public static float RandomWithIn(this Vector2 vec)
    {
        return Random.Range(vec.x<vec.y? vec.x : vec.y, vec.x<vec.y? vec.y:vec.x);
    }

    public static int RandomWithIn(this Vector2Int vec)
    {
        return Random.Range(vec.x<vec.y? vec.x : vec.y, vec.x<vec.y? vec.y:vec.x);
    }

    public static float Lerb(this Vector2Int vec,float value)
    {
        return Mathf.Lerp(vec.x,vec.y,value);
    }

    public static float Clamp(this Vector2 vec, float val)
    {
        return Mathf.Clamp(val, vec.x, vec.y);
    }

    public static Vector2 Vector(float x, float y)
    {
        return Vector3.zero.WithXY(x, y);
    }

    public static float BoxDistance(Vector2 center, Vector2 target)
    {
        var distanceX = Mathf.Abs(center.x - target.x);
        var distanceY = Mathf.Abs(center.y - target.y);

        var maxValue = Mathf.Max(distanceX, distanceY);
        return maxValue;
    }

    public static float CricleDistance(Vector2 center, Vector2 target)
    {
        return Vector2.Distance(center, target);
    }
}

public static class PrimitiveExtensions
{
    public static int FloorTo(this int value, int digit)
    {
        var pow = (int)Mathf.Pow(10,digit);
        return (value / pow) * pow;
    }
}

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float a)
    {
        color.a = a;
        return color;
    }
}