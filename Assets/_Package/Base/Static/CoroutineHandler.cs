using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;

/// <summary>
/// This class allows us to start Coroutines from non-Monobehaviour scripts
/// Create a GameObject it will use to launch the coroutine on
/// </summary>
public class CoroutineHandler : MonoBehaviour
{
    static protected CoroutineHandler m_Instance;
    static public CoroutineHandler instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject o = new GameObject("CoroutineHandler");
                DontDestroyOnLoad(o);
                m_Instance = o.AddComponent<CoroutineHandler>();
            }

            return m_Instance;
        }
    }

    public void OnDisable()
    {
        if (m_Instance)
        {
            StopAllCoroutines();
            Destroy(m_Instance.gameObject);
        }
    }

    public static Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    public static void StopStaticCoroutine(Coroutine coroutine)
    {
        instance.StopCoroutine(coroutine);
    }
}



//////////////////////////////////////////////////////
public static class CoroutineUtils {
    //public static IEnumerator Chain(params IEnumerator[] actions)
    //{
    //    foreach (IEnumerator action in actions)
    //    {
    //        yield return SomeSingletonGO.instance.StartCoroutine(action);
    //    }
    //}

    public static IEnumerator LerpNormalizedEnumerator(Action<float> onCallOnFrame, Action onFinished = null,
                                  float lerpSpeed = 1f, float startNormalized = 0f, float targetNormalized = 1.1f)
    {
        var currentNormalized = startNormalized;
        while (true)
        {
            currentNormalized = Mathf.Lerp(currentNormalized, targetNormalized, lerpSpeed * Time.deltaTime);

            if (currentNormalized >= 1)
            {
                currentNormalized = 1f;
                onCallOnFrame?.Invoke(currentNormalized);
                break;
            }
            
            
            onCallOnFrame?.Invoke(currentNormalized);
            yield return null;
        }

        onFinished?.Invoke();

    }

    public static Coroutine LerpNormalized(Action<float> onCallOnFrame, Action onFinished = null, float lerpSpeed = 1f, float startNormalized = 0f, float targetNormalized = 1.1f)
    {
        return CoroutineHandler.StartStaticCoroutine(LerpNormalizedEnumerator(onCallOnFrame, onFinished, lerpSpeed, startNormalized, targetNormalized));
    }

    public static Coroutine PlayManyCoroutine(float timeDelay, float timeBetween, params Action[] actions)
    {
        return CoroutineHandler.StartStaticCoroutine(DelayManySeconds(timeDelay, timeBetween, actions));
    }

    private static IEnumerator DelayManySeconds(float timeDelay, float timeBetween, params Action[] actions)
    {
        yield return WaitForSecondCache.GetWFSCache(timeDelay);
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i]();

            yield return WaitForSecondCache.GetWFSCache(timeBetween);
        }
    }

    public static Coroutine PlayCoroutine(Action action, float delay)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, delay));
    }

    public static Coroutine PlayCoroutineHaftSecond(Action action)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, 0.5f));
    }

    public static Coroutine PlayCoroutineOneSecond(Action action)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, 1));
    }

    public static Coroutine PlayCoroutineOneHaftSecond(Action action)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, 1.5f));
    }

    public static Coroutine PlayCoroutineThreeSecond(Action action)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, 3));
    }

    public static Coroutine PlayCoroutineFiveSecond(Action action)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySeconds(action, 5));
    }

    public static Coroutine PlayCoroutineRequire(Action action, float delay, Func<bool> req)
    {
        return CoroutineHandler.StartStaticCoroutine(DelaySecondsAndRequire(action, delay, req));
    }

    private static IEnumerator DelaySecondsAndRequire(Action action, float delay, Func<bool> req)
    {
        yield return WaitForSecondCache.GetWFSCache(delay);
        yield return new WaitUntil(req);
        action();
    }

    private static IEnumerator DelaySeconds(Action action, float delay)
    {
        yield return WaitForSecondCache.GetWFSCache(delay);
        action();
    }

    public static IEnumerator Do(Action action)
    {
        action();
        yield return null;
    }

    public static Coroutine MoveTowards(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, Func<float, float> speed = null)
    {
        return CoroutineHandler.StartStaticCoroutine(MoveTowardsEnumerator(start, end, onCallOnFrame, onFinished, speed));
    }

    public static Coroutine MoveTowards(Quaternion start, Quaternion end, Action<Quaternion> onFrame, Action onFinished = null, float normalizedSpeed = 1)
    {
        return MoveTowards(0, 1, (n) =>
        {
            onFrame?.Invoke(Quaternion.Lerp(start, end, n));
        }, onFinished, normalizedSpeed);
    }

    public static Coroutine MoveTowards(Vector3 start, Vector3 end, Action<Vector3> onFrame, Action onFinished = null, float speed = 1)
    {
        return MoveTowards(0, 1, (n) =>
        {
            onFrame?.Invoke(Vector3.Lerp(start, end, n));
        }, onFinished, speed / (end - start).magnitude);
    }

    public static Coroutine MoveTowards(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, float speed = 1f)
    {
        return MoveTowards(start, end, onCallOnFrame, onFinished, (n) => speed);
    }

    public static Coroutine MoveTowardsUnscaleTime(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, float speed = 1f)
    {
        return CoroutineHandler.StartStaticCoroutine(MoveTowardsUnscaleTimeEnumerator(start, end, onCallOnFrame, onFinished, (n) => speed));
    }

    public static Coroutine MoveTowardsUnscaleTime(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, Func<float, float> speed = null)
    {
        return CoroutineHandler.StartStaticCoroutine(MoveTowardsUnscaleTimeEnumerator(start, end, onCallOnFrame, onFinished, speed));
    }

    public static Coroutine MoveTowardsUnscaleTime(Vector3 start, Vector3 end, Action<Vector3> onFrame, Action onFinished = null, float speed = 1)
    {
        return MoveTowardsUnscaleTime(0, 1, (n) =>
        {
            onFrame?.Invoke(Vector3.Lerp(start, end, n));
        }, onFinished, speed / (end - start).magnitude);    
    }

    public static IEnumerator MoveTowardsEnumerator(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, Func<float, float> speed = null)
    {
        speed = speed ?? (f => 1f);
        if (Math.Abs(start - end) < float.Epsilon)
        {
            onFinished?.Invoke();
            yield break;
        }

        var currentNormalized = start;
        while (true)
        {
            currentNormalized = Mathf.MoveTowards(currentNormalized, end, speed(currentNormalized) * Time.deltaTime);

            if (start < end && currentNormalized >= end || start > end && currentNormalized <= end)
            {

                currentNormalized = end;
                onCallOnFrame?.Invoke(currentNormalized);
                break;
            }

            onCallOnFrame?.Invoke(currentNormalized);
            yield return null;
        }

        onFinished?.Invoke();
    }

    public static IEnumerator MoveTowardsUnscaleTimeEnumerator(float start = 0f, float end = 1f, Action<float> onCallOnFrame = null, Action onFinished = null, Func<float, float> speed = null)
    {
        speed = speed ?? (f => 1f);
        if (Math.Abs(start - end) < float.Epsilon)
        {
            onFinished?.Invoke();
            yield break;
        }

        var currentNormalized = start;
        while (true)
        {
            currentNormalized = Mathf.MoveTowards(currentNormalized, end, speed(currentNormalized) * Time.unscaledDeltaTime);

            if (start < end && currentNormalized >= end || start > end && currentNormalized <= end)
            {

                currentNormalized = end;
                onCallOnFrame?.Invoke(currentNormalized);
                break;
            }

            onCallOnFrame?.Invoke(currentNormalized);
            yield return null;
        }

        onFinished?.Invoke();
    }
}




//Threadings
public static class TaskUtils
{
    public static async void PlayTask(Task ActionTask)
    {
        await ActionTask;
    }
}

//TaskUtils.PlayTask(Task.Run(async () =>
//{
//    await Task.Delay(10000);
//    LogSystem.LogWarning("100");

//    await Task.Delay(2000);
//    LogSystem.LogWarning("200");

//    await Task.Delay(3000);
//    LogSystem.LogWarning("300");
//}));