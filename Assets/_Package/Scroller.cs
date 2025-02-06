using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Scroller : MonoBehaviour {
    private RawImage _img;
    public Vector2 _direction;
    private Rect _uvRect;

    private void Start()
    {
        _img = this.GetComponent<RawImage>();
    }
 
    private void Update()
    {
        _uvRect.position = _img.uvRect.position + _direction * Time.deltaTime;
        _uvRect.size = _img.uvRect.size;
        _img.uvRect = _uvRect;
    }
}