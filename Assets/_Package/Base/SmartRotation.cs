using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartRotation : MonoBehaviour
{
    public float speed = -45;
    public bool IsActive = false;
    public Vector3 directionRotation = new Vector3(0.0f, 0.0f, 2.0f);
    
    public void Init()
    {
        IsActive = true;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (!IsActive)
            return;
        transform.Rotate(directionRotation * speed * Time.deltaTime);
    }
}
