using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NativeSupport : MonoBehaviour
{
    public Text numberOfText;
    public Transform btnClose;
    private float timer = 0;

    private void OnEnable()
    {
        timer = 5;
        btnClose.SetActive(false);
        numberOfText.transform.SetActive(true);
        numberOfText.text = $"Skip ad in 3";
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            var timeInt = (int)timer;
            numberOfText.text = $"Skip ad in {timeInt.ToString()}";
            if (timer < 0)
            {
                btnClose.SetActive(true);
                numberOfText.transform.SetActive(false);
            }
        }
    }
}
