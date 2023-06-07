using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    public int[,] line_Data = new int[9, 9]
    {
        {0, 1, 2, 3, 4, 5, 6, 7, 8},
        {9, 10, 11, 12, 13, 14, 15, 16, 17},
        {18, 19, 20, 21, 22, 23, 24, 25, 26},
        {27, 28, 29, 30, 31, 32, 33, 34, 35},
        {36, 37, 38, 39, 40, 41, 42, 43, 44},
        {45, 46, 47, 48, 49, 50, 51, 52, 53},
        {54, 55, 56, 57, 58, 59, 60, 61, 62},
        {63, 64, 65, 66, 67, 68, 69, 70, 71},
        {72, 73, 74, 75, 76, 77, 78, 79, 80},
    };
    public int[,] square_Data = new int[9, 9]
    {
        { 0,  1,  2,  9, 10, 11, 18, 19, 20},
        { 3,  4,  5, 12, 13, 14, 21, 22, 23},
        { 6,  7,  8, 15, 16, 17, 24, 25, 26},
        {27, 28, 29, 36, 37, 38, 45, 46, 47},
        {30, 31, 32, 39, 40, 41, 48, 49, 50},
        {33, 34, 35, 42, 43, 44, 51, 52, 53},
        {54, 55, 56, 63, 64, 65, 72, 73, 74},
        {57, 58, 59, 66, 67, 68, 75, 76, 77},
        {60, 61, 62, 69, 70, 71, 78, 79, 80},
    };
    public int[] columnIndexes = new int[9]
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8
    };

    public void GenerateArray(List<GridSquare> gridSquareList)
    {
        // GenereteLineArray(gridSquareList);
    }

    private void GenereteLineArray(List<GridSquare> gridSquareList)
    {
        for (int i = 0; i < gridSquareList.Count; i++)
        {
            line_Data[i / 9, i % 9] = gridSquareList[i].SquareIndex;
        }
    }

    private void GenerateSquareArray(List<GridSquare> gridSquareList)
    {
        square_Data = new int[9, 9]
        {
            { 0,  1,  2,  9, 10, 11, 18, 19, 20},
            { 3,  4,  5, 12, 13, 14, 21, 22, 23},
            { 6,  7,  8, 15, 16, 17, 24, 25, 26},
            {27, 28, 29, 36, 37, 38, 45, 46, 47},
            {30, 31, 32, 39, 40, 41, 48, 49, 50},
            {33, 34, 35, 42, 43, 44, 51, 52, 53},
            {54, 55, 56, 63, 64, 65, 72, 73, 74},
            {57, 58, 59, 66, 67, 68, 75, 76, 77},
            {60, 61, 62, 69, 70, 71, 78, 79, 80},
        };
    }

    public int[] GetVerticalLine(int squareIndex)
    {
        int[] line = new int[9];

        var squareColumn = squareIndex % 9;

        for (int i = 0; i < 9; i++)
        {
            line[i] = line_Data[i, squareColumn];
            // Debug.Log(line[i]);
        }

        return line;
    }

    public int GetGridSquareDataIndex(int squareIndex)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (square_Data[row, col] == squareIndex)
                {
                    return row;
                }
            }
        }

        return -1;
    }
}
