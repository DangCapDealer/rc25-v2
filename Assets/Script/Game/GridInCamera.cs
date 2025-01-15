using System;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class GridInCamera : MonoSingleton<GridInCamera>
{
    public Transform[] _objects;
    public float padding = 0.2f;
    public Camera camera;

    public void CreatePosition()
    {
        hidePosition();
        switch (GameManager.Instance.NumberOfCharacter)
        {
            case 8:
                createRowWithY(4, 0.5f);
                createRowWithY(4, 3.5f);
                break;
            case 9:
                createRowWithY(5, 0.5f);
                createRowWithY(4, 3.5f);
                break;
            case 10:
                createRowWithY(5, 0.5f);
                createRowWithY(5, 3.5f);
                break;
            default:
                createRowWithY(4, 0.5f);
                createRowWithY(3, 3.5f);
                break;
        }    
    }    

    public Vector3 GetPosition(int index)
    {
        return this.transform.GetChild(index).position;
    }    

    public Vector3 GetLastPosition()
    {
        int count = _objects.Length;
        for (int i = count - 1; i >= 0; i--)
        {
            var _child = _objects[i];
            if (_child.IsActive() == true)
            {
                return _child.position;
            }
        }
        return Vector3.zero; 
    }

    private void createRowWithY(int columns, float yPos)
    {
        int rows = 1;
        float camHeight = 2f * camera.orthographicSize;
        float camWidth = camHeight * camera.aspect;

        float gridWidth = camWidth - padding * (columns + 1);
        float gridHeight = camHeight - padding * (rows + 1);

        float cellWidth = gridWidth / columns;
        float cellHeight = gridHeight / rows;

        float startX = -camWidth / 2 + padding + cellWidth / 2;
        float startY = camHeight / 2 - padding - cellHeight / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float xPos = startX + j * (cellWidth + padding);
                var _object = getObject();
                _object.Show();
                _object.position = _object.position.WithXY(xPos, yPos);
            }
        }
    }   
    
    private void hidePosition()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            _objects[i].Hide();
        }
    }    

    private Transform getObject()
    {
        for(int i = 0; i < _objects.Length; i++)
        {
            if (_objects[i].IsActive() == false)
                return _objects[i];
        }
        return null; 
    }    
}
