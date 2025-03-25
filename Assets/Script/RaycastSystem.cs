using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastSystem : MonoSingleton<RaycastSystem>
{
    public enum RaycastState
    {
        None,
        Running
    }

    public RaycastState state = RaycastState.None;
    public LayerMask IngoreLayer;
    public LayerMask OutlineLayer;
    public Camera RaycastCamera;

    void Update()
    {
        if (IsClickingUI() == true)
            return;

        if (RuntimeStorageData._StatusGame == RuntimeStorageData.StatusGame.Pause)
        {
            RaycastCheckingOnPauseState();
            return;
        }
        RaycastChecking();
    }

    private void RaycastCheckingOnPauseState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            state = RaycastState.Running;
            Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IngoreLayer))
            {
                GameEvent.OnTouchBeganPauseStateMethod(hit.collider.transform);
            }
        }

        if (Input.GetMouseButton(0) && state == RaycastState.Running)
        {
            GameEvent.OnTouchMovePauseStateMethod(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && state == RaycastState.Running)
        {
            state = RaycastState.None;
            GameEvent.OnTouchEndPauseStateMethod(Input.mousePosition);
        }
    }

    public bool CenterRaycast(out RaycastHit hitInfo, Vector2 touchPosition)
    {
        if (IsClickingUI() == true)
        {
            hitInfo = default;
            return false;
        }

        Ray ray = RaycastCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hitInfo, 1000f, ~IngoreLayer))
        {
            return true;
        }
        return false;
    }

    public bool PerformRaycast(out RaycastHit hitInfo, Vector2 touchPosition)
    {
        if (IsClickingUI() == true)
        {
            hitInfo = default;
            return false;
        }

        Ray ray = RaycastCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hitInfo, 1000f, ~OutlineLayer))
        {
            return true;
        }
        return false;
    }

    private void RaycastChecking()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            state = RaycastState.Running;
            GameEvent.OnTouchBeganMethod(Input.mousePosition);
            Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IngoreLayer))
            {
                GameEvent.OnTouchBeganMethod(hit);
            }
        }

        if (Input.GetMouseButton(0) && state == RaycastState.Running)
        {
            GameEvent.OnTouchDragMethod(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && state == RaycastState.Running)
        {
            state = RaycastState.None;
            GameEvent.OnTouchEndedMethod(Input.mousePosition);
        }
#endif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                state = RaycastState.Running;
                GameEvent.OnTouchBeganMethod(Input.mousePosition);
                Ray ray = RaycastCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IngoreLayer))
                {
                    GameEvent.OnTouchBeganMethod(hit);
                }
            }

            if (touch.phase == TouchPhase.Ended && state == RaycastState.Running)
            {
                state = RaycastState.None;
                GameEvent.OnTouchEndedMethod(touch.position);
            }

            if (touch.phase == TouchPhase.Moved && state == RaycastState.Running ||
                touch.phase == TouchPhase.Stationary && state == RaycastState.Running)
            {
                GameEvent.OnTouchDragMethod(touch.position);
            }
        }
        else if (Input.touchCount == 0 && state == RaycastState.Running)
        {
#if !UNITY_EDITOR
            state = RaycastState.None;
            GameEvent.OnTouchEndedMethod(Vector2.zero);
#endif
        }
    }

    public bool IsClickingUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var objSelected = EventSystem.current.currentSelectedGameObject;
            if (objSelected != null)
            {
                return true;
            }
        }
        return false;
    }
}
