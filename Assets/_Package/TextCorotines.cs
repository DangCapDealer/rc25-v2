using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextCorotines : MonoBehaviour
{
    private TMP_Text txt;
    private float _timer;

    public string[] lstText;
    public int i;

    private void Start()
    {
        txt = GetComponent<TMP_Text>();
        i = 0;
        txt.text = lstText[i];
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 0.3f)
        {
            _timer = 0;
            i += 1;
            if(i >= lstText.Length)
            {
                i = 0;
            }
            txt.text = lstText[i];
        }    
    }
}
