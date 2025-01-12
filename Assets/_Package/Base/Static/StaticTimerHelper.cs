using System;
using System.Collections;
using UnityEngine;

public static class StaticTimerHelper
{
    public static int CurrentTimeInSecond()
    {
        return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static int CurrentNextTimeInSecond(int minute)
    {
        DateTime today = DateTime.Now;
        return (int)(today.AddMinutes(minute) - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static int CurrentTimeDayInSecond()
    {
        return (int)(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static int CurrenTimeInDay()
    {
        return (int) (DateTime.Now - new DateTime(1970, 1, 1)).TotalDays;
    }

    public static int CurrenTimeAddDay(int day)
    {
        DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
        return (int)(today.AddDays(day) - new DateTime(1970, 1, 1)).TotalDays;
    }

    public static int CurrentSecondInDay()
    {
        return (int) (DateTime.Now.Hour* 3600 + DateTime.Now.Minute* 60 + DateTime.Now.Second);
    }

    public static int GetCurrentSecondToTargetDay(DateTime dataTime)
    {
        return (int)(dataTime - DateTime.Now).TotalSeconds;
    }

    public static int GetCurrentSecondToTargetDayUtc(DateTime dateTime)
    {
        return (int)(dateTime - DateTime.UtcNow).TotalSeconds;
    }

    public static DateTime ConvertSecondToDateTime(int seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return DateTime.Today.Add(time);
    }

    public static TimeSpan ConverSecondToTimeSpan(int seconds)
    {
        return TimeSpan.FromSeconds(seconds);
    }

    public static string ConvertSeconds(int totalSeconds) 
    { 
        int minutes = totalSeconds / 60; 
        int remainingSeconds = totalSeconds % 60; 
        return $"{minutes}m {remainingSeconds}s";
    }

    public static string FormatTime(int totalSeconds) 
    { 
        int minutes = totalSeconds / 60; 
        int seconds = totalSeconds % 60; 
        return string.Format("{0:00}:{1:00}", minutes, seconds); 
    }
}
