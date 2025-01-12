using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameEvent
{
    public delegate void ChangeLanguage();
    public static event ChangeLanguage OnChangeLanguage;
    public static void OnChangeLanguageMethod()
    {
        if (OnChangeLanguage != null)
            OnChangeLanguage();
    }

    public delegate void TouchBegan(RaycastHit hit);
    public static event TouchBegan OnTouchBegan;
    public static void OnTouchBeganMethod(RaycastHit hit)
    {
        if (OnTouchBegan != null)
            OnTouchBegan(hit);
    }

    public delegate void TouchPBegan(Vector2 touchPosition);
    public static event TouchPBegan OnTouchPBegan;
    public static void OnTouchBeganMethod(Vector2 touchPosition)
    {
        if (OnTouchPBegan != null)
            OnTouchPBegan(touchPosition);
    }

    public delegate void TouchDrag(Vector2 touchPosition);
    public static event TouchDrag OnTouchDrag;
    public static void OnTouchDragMethod(Vector2 touchPosition)
    {
        if (OnTouchDrag != null)
            OnTouchDrag(touchPosition);
    }

    public delegate void TouchEnded(Vector2 touchPosition);
    public static event TouchEnded OnTouchEnded;
    public static void OnTouchEndedMethod(Vector2 touchPosition)
    {
        if (OnTouchEnded != null)
            OnTouchEnded(touchPosition);
    }

    public delegate void CreateObject(GameObject go);
    public static event CreateObject OnCreateObject;
    public static void OnCreateObjectMethod(GameObject go)
    {
        if (OnCreateObject != null)
            OnCreateObject(go);
    }

    public delegate void RemoveObject(GameObject obj);
    public static event RemoveObject OnRemoveObject;
    public static void RemoveObjectMethod(GameObject obj)
    {
        if (OnRemoveObject != null)
            OnRemoveObject(obj);
    }

    public delegate void TouchBeganPauseState(Transform transform);
    public static event TouchBeganPauseState OnTouchBeganPauseState;
    public static void OnTouchBeganPauseStateMethod(Transform transform)
    {
        if (OnTouchBeganPauseState != null)
            OnTouchBeganPauseState(transform);
    }

    public delegate void TouchMovePauseState(Vector2 vector);
    public static event TouchMovePauseState OnTouchMovePauseState;
    public static void OnTouchMovePauseStateMethod(Vector2 vector)
    {
        if (OnTouchMovePauseState != null)
            OnTouchMovePauseState(vector);
    }

    public delegate void TouchEndPauseState(Vector2 vector);
    public static event TouchEndPauseState OnTouchEndPauseState;
    public static void OnTouchEndPauseStateMethod(Vector2 vector)
    {
        if (OnTouchEndPauseState != null)
            OnTouchEndPauseState(vector);
    }

    public delegate void IAPurchase(string productId);
    public static IAPurchase OnIAPurchase;
    public static void OnIAPurchaseMethod(string productId)
    {
        if (OnIAPurchase != null)
            OnIAPurchase?.Invoke(productId);
    }


    public delegate void ReciveMessage(string mesage, params string[] args);
    public static ReciveMessage OnReciveMessage;
    public static void OnSendMessageMethod(string mesage, params string[] args)
    {
        if (OnReciveMessage != null)
            OnReciveMessage?.Invoke(mesage, args);
    }

    public delegate void UIDragDown(string msg);
    public static event UIDragDown OnUIDragDown;
    public static void OnUIDragDownMethod(string msg)
    {
        if (OnUIDragDown != null)
            OnUIDragDown?.Invoke(msg);
    }

    public delegate void UIDragUp(string msg);
    public static event UIDragUp OnUIDragUp;
    public static void OnUIDragUpMethod(string msg)
    {
        if (OnUIDragUp != null)
            OnUIDragUp?.Invoke(msg);
    }

    public delegate void UIDrag(string msg);
    public static event UIDrag OnUIDrag;
    public static void OnUIDragMethod(string msg)
    {
        if (OnUIDrag != null)
            OnUIDrag?.Invoke(msg);
    }

    public delegate void ThemeStype(string msg);
    public static event ThemeStype OnThemeStype;
    public static void OnThemeStypeMethod(string msg)
    {
        if (OnThemeStype != null)
            OnThemeStype?.Invoke(msg);
    }

}
