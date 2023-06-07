using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0.0f, 700.0f);

    public ShapeData currentShapeData;
    public int totalSquareNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _rectTransform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;


    private void Awake()
    {
        _shapeStartScale = GetComponent<RectTransform>().localScale;
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = _rectTransform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPoint += GameEvents_MoveShapeToStartPoint;
        GameEvents.SetShapeInactive += GameEvents_SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPoint -= GameEvents_MoveShapeToStartPoint;
        GameEvents.SetShapeInactive -= GameEvents_SetShapeInactive;
    }

    private void GameEvents_MoveShapeToStartPoint()
    {
        _rectTransform.localPosition = _startPosition;
    }

    public bool IsOnStartPostion()
    {
        return _rectTransform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void DeactiveShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactiveShapeSquare();
            }
        }

        _shapeActive = false;
    }

    private void GameEvents_SetShapeInactive()
    {
        if (!IsOnStartPostion() && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActiveShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActiveShapeSquare();
            }
        }

        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        _rectTransform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        currentShapeData = shapeData;
        totalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShape.Count < totalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
                                        squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                // Debug.Log("Row: " + row + " - " + "Column: " + column);
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                                        GetYPositionForShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;
                }
            }
        }
    }

    public float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        // Debug.Log("Row: " + row);
        float shiftOnY = 0.0f;

        if (shapeData.rows > 1) // Vertical pos calculation
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleSquareIndex) //move it on the negative
                {
                    shiftOnY = moveDistance.y * 1;
                }
                else if (row > middleSquareIndex) //move it on plus
                {
                    shiftOnY = moveDistance.y * -1;
                }

                shiftOnY *= multiplier;
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 2);
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIndex2)
                {
                    shiftOnY = (moveDistance.y / 2) * -1;
                }
                if (row == middleSquareIndex1)
                {
                    shiftOnY = moveDistance.y / 2;
                }

                if (row < middleSquareIndex1 && row < middleSquareIndex2) //move it on the negative
                {
                    shiftOnY = moveDistance.y * 1;
                }
                else if (row > middleSquareIndex1 && row > middleSquareIndex2) //move it on the plus
                {
                    shiftOnY = moveDistance.y * -1;
                }

                shiftOnY *= multiplier;
            }
        }

        return shiftOnY;
    }

    public float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        // Debug.Log("Column: " + column);
        float shiftOnX = 0.0f;

        if (shapeData.columns > 1) // Vertical pos calculation
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;

                if (column < middleSquareIndex) //move it on the negative
                {
                    shiftOnX = moveDistance.x * -1;
                }
                else if (column > middleSquareIndex) //move it on plus
                {
                    shiftOnX = moveDistance.x * 1;
                }

                shiftOnX *= multiplier;
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 2);
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x / 2;
                }
                if (column == middleSquareIndex1)
                {
                    shiftOnX = (moveDistance.x / 2) * -1;
                }

                if (column < middleSquareIndex1 && column < middleSquareIndex2) //move it on the negative
                {
                    shiftOnX = moveDistance.x * -1;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2) //move it on the plus
                {
                    shiftOnX = moveDistance.x * 1;
                }

                shiftOnX *= multiplier;
            }
        }

        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.zero;
        _rectTransform.pivot = Vector2.zero;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _rectTransform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rectTransform.localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
