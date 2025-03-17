using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2( 0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;
    public SquareTextureData squareTextureData;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    // Start is called before the first frame update

    private LineIndicator _lineIndicator;
    private Config.SquareColor currentActiveSquareColor_ = Config.SquareColor.NotSet;
    private List<Config.SquareColor> colorsIntheGrid_ = new List<Config.SquareColor>();

    private void OnEnable()
    {
        GameEvent.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvent.UpdateSquareColor += OnUpdateSquareColor;
    }
    private void OnDisable()
    {
        GameEvent.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvent.UpdateSquareColor -= OnUpdateSquareColor;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreatGrid();
        currentActiveSquareColor_ = squareTextureData.activeSquareTextures[0].squareColor;
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        currentActiveSquareColor_ = color;
    }

    private List<Config.SquareColor> GetAllSquareColorsInTheGrid()
    {
        var colors = new List<Config.SquareColor>();
        foreach (var square in _gridSquares)
        {
            var gridSqaure = square.GetComponent<GridSquare>();
            if(gridSqaure.SquareOccupied)
            {
                var color = gridSqaure.GetCurrentColor();
                if(colors.Contains(color) == false)
                {
                    colors.Add(color);
                }
            }
        }
        return colors;
    }
    // Update is called once per frame
    private void CreatGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();

    }    
    private void SpawnGridSquares()
    {
        // 0, 1, 2, 3, 4,
        // 5, 6, 7, 8, 9

        int square_index = 0;
        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                square_index++;
            }
        }
    }
    private void SetGridSquaresPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;
        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach(GameObject square in _gridSquares)
        {
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                column_number = 0;
                row_number++;
                row_moved = false;
            }
            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);
            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }
            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0.0f);
            column_number++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexs = new List<int>();

        foreach (var square in _gridSquares )
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if(gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexs.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
                //gridSquare.ActivateSquare();
            }
        }
        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if(currentSelectedShape.TotalSquareNumber == squareIndexs.Count)
        {
            foreach(var squareIndex in squareIndexs)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(currentActiveSquareColor_);
            }
            var shapeLeft = 0;

            foreach(var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapequareActive())
                {
                    shapeLeft++;
                }
            }
            
            if(shapeLeft == 0)
            {
                GameEvent.RequestNewShapes();
            }
            else
            {
                GameEvent.SetShapeInactive();
            }
            CheckIfAnyLineIsCompleted();
        }
        
        else
        {
            GameEvent.MoveShapeToStartPosition();
        }
    }
    
    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        foreach( var column in _lineIndicator.columnIndexs)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }
      
        for(var row =0; row < 9; row ++)
        {
            List<int> data = new List<int>(9);
            for (var index =0; index < 9; index ++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }
            lines.Add(data.ToArray());

        }
        //square
        for (var sqaure = 0; sqaure <9; sqaure ++)
        {
            List<int> data = new List<int>(9);
            for (var index =0; index <9; index ++)
            {
                data.Add(_lineIndicator.square_data[sqaure, index]);
            }
            lines.Add(data.ToArray());
        }

        colorsIntheGrid_ = GetAllSquareColorsInTheGrid();

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if (completedLines >= 2)
        {
            GameEvent.ShowWriting();
        }
        var totalScores = 10 * completedLines;
        var bonusScores = ShouldPlayColorBonusAnimation();
        GameEvent.AddScores(totalScores + bonusScores);
        CheckIfPlayerLost();
    }

    private int ShouldPlayColorBonusAnimation()
    {
        var colorsInTheGridAfterLineRemoved = GetAllSquareColorsInTheGrid();
        Config.SquareColor colorToPlayBonusFor = Config.SquareColor.NotSet;
        foreach (var squareColor in colorsIntheGrid_)
        {
            if (colorsInTheGridAfterLineRemoved.Contains(squareColor) == false)
            {
                colorToPlayBonusFor = squareColor;
            }
        }
        if(colorToPlayBonusFor == Config.SquareColor.NotSet)
        {
            Debug.Log("Cannot find color");
            return 0;
        }
        if(colorToPlayBonusFor == currentActiveSquareColor_)
        {
            return 0;
        }

        GameEvent.ShowBonusScreen(colorToPlayBonusFor);

        return 50;
    }
    private int CheckIfSquaresAreCompleted (List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;
        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }
            if(lineCompleted)
            {
                completedLines.Add(line);
            }
        }
        foreach (var line in completedLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if(completed)
            {
                linesCompleted++;
            }

            
        }
        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for(var index = 0; index < shapeStorage.shapeList.Count; index ++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapequareActive();
            if(CheckIfShapeCanBePlaceOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }
        }
        if(validShapes == 0)
        {
            GameEvent.GameOver(false);
            //Debug.Log("Gameover");
        }
    }

    private bool CheckIfShapeCanBePlaceOnGrid(Shape currentShape)

    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        List<int> originalShapeFillUpSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex = 0; rowIndex< shapeRows; rowIndex ++)
        {
            for (var columnsIndex =0; columnsIndex< shapeColumns; columnsIndex ++)
            {
                if (currentShapeData.board[rowIndex].column[columnsIndex])
                {
                    originalShapeFillUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }
        if (currentShape.TotalSquareNumber != originalShapeFillUpSquares.Count)
            Debug.LogError("Number Invalid");

        var sqaureList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach(var number in sqaureList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in originalShapeFillUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }
            if(shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var sqaureList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;
        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 9)
        {
            var rowData = new List<int>();

            for(var row = lastRowIndex; row < lastRowIndex + rows; row ++)
            {
                for(var column = lastColumnIndex; column< lastColumnIndex + columns; column ++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }


            sqaureList.Add(rowData.ToArray());

            lastColumnIndex++;
            if(lastColumnIndex + (columns - 1)>=9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if(safeIndex > 100)
            {
                break;
            }
        }
        return sqaureList;
    }
}
