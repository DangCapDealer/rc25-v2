using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BezierCurve
{
    private static float[] Factorial = new float[]
{
        1.0f,
        1.0f,
        2.0f,
        6.0f,
        24.0f,
        120.0f,
        720.0f,
        5040.0f,
        40320.0f,
        362880.0f,
        3628800.0f,
        39916800.0f,
        479001600.0f,
        6227020800.0f,
        87178291200.0f,
        1307674368000.0f,
        20922789888000.0f,
    };

    public static Vector3 Point3(float t, List<Vector3> controlPoints)
    {
        int N = controlPoints.Count - 1;
        if (N > 16)
        {
            Debug.Log("You have used more than 16 control points. The maximum control points allowed is 16.");
            controlPoints.RemoveRange(16, controlPoints.Count - 16);
        }
        if (t <= 0) return controlPoints[0];
        if (t >= 1) return controlPoints[controlPoints.Count - 1];

        Vector3 p = new Vector3();

        for (int i = 0; i < controlPoints.Count; ++i)
        {
            Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
            p += bn;
        }

        return p;
    }

    private static float Binomial(int n, int i)
    {
        float ni;
        float a1 = Factorial[n];
        float a2 = Factorial[i];
        float a3 = Factorial[n - i];
        ni = a1 / (a2 * a3);
        return ni;
    }

    private static float Bernstein(int n, int i, float t)
    {
        float t_i = Mathf.Pow(t, i);
        float t_n_minus_i = Mathf.Pow((1 - t), (n - i));

        float basis = Binomial(n, i) * t_i * t_n_minus_i;
        return basis;
    }

    public static List<Vector3> PointList3(List<Vector3> controlPoints, float interval = 0.01f)
    {
        int N = controlPoints.Count - 1;
        if (N > 16)
        {
            Debug.Log("You have used more than 16 control points. The maximum control points allowed is 16.");
            controlPoints.RemoveRange(16, controlPoints.Count - 16);
        }

        List<Vector3> reponse = new List<Vector3>();
        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector3 p = new Vector3();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            reponse.Add(p);
        }

        return reponse;
    }

    public static Vector3[] PointList3(float interval = 0.01f, params Vector3[] controlPoints)
    {
        int N = controlPoints.Length - 1;
        List<Vector3> reponse = new List<Vector3>();
        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector3 p = new Vector3();
            for (int i = 0; i < controlPoints.Length; ++i)
            {
                Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            reponse.Add(p);
        }

        return reponse.ToArray();
    }

    public static Vector2 Point2(float t, List<Vector2> controlPoints)
    {
        int N = controlPoints.Count - 1;
        if (N > 16)
        {
            Debug.Log("You have used more than 16 control points. The maximum control points allowed is 16.");
            controlPoints.RemoveRange(16, controlPoints.Count - 16);
        }

        if (t <= 0) return controlPoints[0];
        if (t >= 1) return controlPoints[controlPoints.Count - 1];

        Vector2 p = new Vector2();

        for (int i = 0; i < controlPoints.Count; ++i)
        {
            Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
            p += bn;
        }

        return p;
    }

    public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval = 0.01f)
    {
        int N = controlPoints.Count - 1;
        if (N > 16)
        {
            Debug.Log("You have used more than 16 control points. The maximum control points allowed is 16.");
            controlPoints.RemoveRange(16, controlPoints.Count - 16);
        }

        List<Vector2> points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            points.Add(p);
        }

        return points;
    }

    public static Vector2[] PointList2(float interval = 0.01f, params Vector2[] controlPoints)
    {
        int N = controlPoints.Length - 1;

        List<Vector2> points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Length; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            points.Add(p);
        }

        return points.ToArray();
    }

    private static Vector3 CalculateBezierPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5)
    {
        return Mathf.Pow(1 - t, 5) * p0 +
               5 * Mathf.Pow(1 - t, 4) * t * p1 +
               10 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 2) * p2 +
               10 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 3) * p3 +
               5 * (1 - t) * Mathf.Pow(t, 4) * p4 +
               Mathf.Pow(t, 5) * p5;
    }

    private static Vector3 CalculateBezierPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Công thức tính toán đường cong Bézier 2D
        return Mathf.Pow(1 - t, 3) * p0 +
               3 * Mathf.Pow(1 - t, 2) * t * p1 +
               3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
               Mathf.Pow(t, 3) * p3;
    }
}
