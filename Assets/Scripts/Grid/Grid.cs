using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LineIndicator lineIndicator;
    public ShapeStorage shapeStorage;
    public int columns;
    public int rows;
    public float squareGap;
    public GridSquare gridSquare;
    public RectTransform startPosRect;
    public Vector2 startPos;
    public float squareScale;
    public float everySquareOffset;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    [SerializeField] private List<GridSquare> _gridSquares = new List<GridSquare>();

    private void Awake()
    {
        startPos = new Vector2(startPosRect.anchoredPosition.x, startPosRect.anchoredPosition.y);
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquarePos();
        InitLineIndicator();
    }

    private void InitLineIndicator()
    {
        lineIndicator.GenerateArray(_gridSquares);
    }

    private void SpawnGridSquares()
    {
        int squareIndex = 0;
        Vector3 _squareScale = new Vector3(squareScale, squareScale, squareScale);
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                _gridSquares.Add(Instantiate(gridSquare, transform));
                _gridSquares[_gridSquares.Count - 1].SquareIndex = squareIndex;
                _gridSquares[_gridSquares.Count - 1].transform.localScale = _squareScale;
                _gridSquares[_gridSquares.Count - 1].SetImage(lineIndicator.GetGridSquareDataIndex(squareIndex) % 2 == 0);
                squareIndex++;
            }
        }
    }

    private void SetGridSquarePos()
    {
        int columnNumber = 0;
        int rowNumber = 0;
        Vector2 squareGapNumber = Vector2.zero;
        bool rowMoved = false;

        RectTransform squareRect = _gridSquares[0].squareRect;

        _offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        _offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOffset;

        for (int i = 0; i < _gridSquares.Count; i++)
        {
            if (columnNumber + 1 > columns)
            {
                squareGapNumber.x = 0;
                //Next column
                columnNumber = 0;
                rowNumber++;
                rowMoved = false;
            }

            float posXOffset = _offset.x * columnNumber + (squareGapNumber.x * squareGap);
            float posYOffset = _offset.y * rowNumber + (squareGapNumber.y * squareGap);

            if (columnNumber > 0 && columnNumber % 3 == 0)
            {
                squareGapNumber.x++;
                posXOffset += squareGap;
            }

            if (rowNumber > 0 && rowNumber % 3 == 0 && rowMoved == false)
            {
                rowMoved = true;
                squareGapNumber.y++;
                posYOffset += squareGap;
            }

            _gridSquares[i].squareRect.anchoredPosition = new Vector2(startPos.x + posXOffset, startPos.y - posYOffset);
            _gridSquares[i].squareRect.localPosition = new Vector3(startPos.x + posXOffset, startPos.y - posYOffset, 0.0f);

            columnNumber++;
        }
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += GameEvents_CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= GameEvents_CheckIfShapeCanBePlaced;
    }

    private void GameEvents_CheckIfShapeCanBePlaced()
    {
        List<int> squareIndexList = new List<int>();

        foreach (GridSquare gridSquare in _gridSquares)
        {
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexList.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        Shape currentShapeSelected = shapeStorage.GetCurrentSelectedShape();
        if (currentShapeSelected == null) return;

        if (currentShapeSelected.totalSquareNumber == squareIndexList.Count)
        {
            foreach (var squareIndex in squareIndexList)
            {
                _gridSquares[squareIndex].PlaceShapeOnBoard();
            }

            int shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPostion() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShape();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPoint();
        }
    }

    private void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //Add Columns
        foreach (var column in lineIndicator.columnIndexes)
        {
            lines.Add(lineIndicator.GetVerticalLine(column));
        }

        //Add Rows
        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (int i = 0; i < 9; i++)
            {
                data.Add(lineIndicator.line_Data[row, i]);
            }

            lines.Add(data.ToArray());
        }

        // Add Squares
        for (int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (int i = 0; i < 9; i++)
            {
                data.Add(lineIndicator.square_Data[square, i]);
                // Debug.Log(lineIndicator.square_Data[square, i]);
            }
            // Debug.Log("--------------");

            lines.Add(data.ToArray());
        }

        int completedLines = CheckIfSquareAreCompleted(lines);

        if (completedLines > 2)
        {
            //Play animation bonus
        }

        int totalScores = 10 * completedLines;
        GameEvents.AddScores(totalScores);
        CheckIfPlayerLost();
    }

    private int CheckIfSquareAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        int linesCompleted = 0;

        foreach (var line in data)
        {
            bool lineCompleted = true;
            foreach (var squareIndex in line)
            {
                GridSquare comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            bool completed = false;

            foreach (int squareIndex in line)
            {
                GridSquare comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                comp.ClearOccupied();
                completed = true;
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        int validShape = 0;

        for (int i = 0; i < shapeStorage.shapeList.Count; i++)
        {
            bool isShapeActive = shapeStorage.shapeList[i].IsAnyOfShapeSquareActive();
            if (isShapeActive && CheckIfShapeCanBePlaceOnGrid(shapeStorage.shapeList[i]))
            {
                shapeStorage.shapeList[i].ActiveShape();
                validShape++;
            }
        }

        if (validShape == 0)
        {
            //Game Over
            GameEvents.GameOver(false);
            Debug.Log("Lose");
        }
    }

    private bool CheckIfShapeCanBePlaceOnGrid(Shape shape)
    {
        ShapeData shapeData = shape.currentShapeData;
        int shapeColumns = shapeData.columns;
        int shapeRows = shapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        int squareIndex = 0;

        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (shapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }

                squareIndex++;
            }
        }

        if (shape.totalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Out of moves");
        }

        List<int[]> squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach (var square in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                GridSquare tempGridSquare = _gridSquares[square[squareIndexToCheck]].GetComponent<GridSquare>();
                if (tempGridSquare.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        List<int[]> squareList = new List<int[]>();
        int lastColumnIndex = 0;
        int lastRowIndex = 0;
        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 9)
        {
            List<int> rowData = new List<int>();

            for (int row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (int column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    // Debug.Log("Row: " + row + " - " + "Column: " + column);
                    rowData.Add(lineIndicator.line_Data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }
        }

        return squareList;
    }
}


