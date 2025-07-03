using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSupport : MonoBehaviour
{
    void Update()
    {
        // mode battle có thêm tính toán về điểm số
        if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            mode3_CaculateCharacter();
        }    
    }

    public void OnCreateGame()
    {
        mode3_NumberOfCharacter = 0;
    }    

    private int mode3_NumberOfCharacter = 0;
    private int mode3_ScoreLeft = 1;
    private int mode3_ScoreRight = 1;

    public float Mode3GetSpeed()
    {
        var speed = (float)mode3_ScoreLeft / (float)mode3_ScoreRight;
        speed -= 1;
        speed = Mathf.Clamp(speed, -0.5f, 0.5f);
        return speed;
    }    

    private void mode3_CaculateCharacter()
    {
        var character = GameSpawn.Instance.CreateObjects;
        if(character.Count != mode3_NumberOfCharacter)
        {
            mode3_ScoreLeft = 1;
            mode3_ScoreRight = 1;
            mode3_NumberOfCharacter = character.Count;
            for (int i = 0; i < character.Count; i++)
            {
                if(character[i] != null)
                {
                    if (character[i].position().x > 0)
                        mode3_ScoreRight += 1;
                    else if (character[i].position().x < 0)
                        mode3_ScoreLeft += 1;
                }    
            }
        }    
    }    
}
