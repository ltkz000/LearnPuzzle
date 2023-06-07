using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeDataList;
    public List<Shape> shapeList;

    private void Start()
    {
        GenerateShape();
    }

    private void OnEnable()
    {
        GameEvents.RequestNewShape += GameEvents_RequestNewShape;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShape -= GameEvents_RequestNewShape;
    }

    private void GameEvents_RequestNewShape()
    {
        GenerateShape();
    }

    public void GenerateShape()
    {
        for (int i = 0; i < shapeList.Count; i++)
        {
            shapeList[i].RequestNewShape(shapeDataList[Random.Range(0, shapeDataList.Count)]);
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPostion() == false && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }

        Debug.LogError("No shape selected");
        return null;
    }
}
