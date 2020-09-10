/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid {

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;

    Sprite[] sprites;
    int numberOfMines = 10;
    int remainingBombs;
    List<Block> blocks = new List<Block>();
    

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Sprite[] sprites) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.sprites = sprites;

        gridArray = new int[width, height];
        remainingBombs = numberOfMines;

        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    //debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);

                    //UtilsClass.CreateWorldSprite(gridArray[x, y].ToString(), sprite[UnityEngine.Random.Range(0,sprite.Length - 1)], GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, new Vector3(1, 1, 1), 0, Color.white);
                    Sprite sprite;
                    Block block;
                    if (isAddMine())
                    {
                        sprite = sprites[sprites.Length - 1];
                        //addBlockToGameScene(x, y, sprites.Length - 1);
                        addBlockToGameScene(x, y, 0);
                        block = new Block(x, y, true); ;
                    }
                    else
                    {
                        sprite = sprites[0];
                        addBlockToGameScene(x, y, 0);
                        block = new Block(x, y, false); ;
                    }
                    blocks.Add(block);
                    //addBlockToGameScene(x, y, sprites.Length - 1);
                    //UtilsClass.CreateWorldSprite(x + "," + y,sprite, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, new Vector3(1, 1, 1), 0, Color.white);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
            };
        }
    }

    private bool isAddMine()
    {
        if (numberOfMines > 0)
        {
            int random = UnityEngine.Random.Range(0,100);
            if (random < 20) {
                numberOfMines--;
                return true;
            } else
            return false ;
        }
        else
            return false;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        Debug.Log("" + x + ","+ y);
        //SetValue(x, y, value);
    }

    public void setFlag(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        Debug.Log("Clicked " + x + "," + y);
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            if (hasBomb(x, y)) {
                addBlockToGameScene(x, y, sprites.Length - 2);
                remainingBombs--;
            }

        }
    }
    public void SetSprite(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        Debug.Log("Clicked " + x + "," + y);
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            //printSrurroundingBlocks(x, y);
            if (hasBomb(x, y))
            {
                //addBlockToGameScene(x, y, sprites.Length - 1);
                // DEAD
                addBlockToGameScene(x, y, -2);
            }
            else
            {
                startGridSearch(x, y);
                /*int numberOfBombs = getNumberOfBombs(x, y);
                if (numberOfBombs == 0)
                    addBlockToGameScene(x, y, -1);
                else
                    addBlockToGameScene(x, y, numberOfBombs);*/
            }
        }
        else {
            Debug.Log("Wrong co-ordinates");
        }
    }

    private void startGridSearch(int x, int y) {

            for (int a = x - 1; a <= x + 1; a++)
            {
                for (int b = y - 1; b <= y + 1; b++)
                {

                    displayBombsAtThisPoint(a, b);
                }
            }
    }

    private void displayBombsAtThisPoint(int x, int y) {

        int bombs = getNumberOfBombs(x, y);
        if (bombs == 0)
            addBlockToGameScene(x, y, -1);
        else
        {
            if(!hasBomb(x,y))
                addBlockToGameScene(x, y, bombs);
        }
    }

    public void printSrurroundingBlocks(int x, int y) {
        for (int a = x - 1; a <= x + 1; a++) {
            for (int b = y - 1; b <= y + 1; b++) {
                /*if (a == x && b == y)
                    continue;*/
                Debug.Log(a + "," + b + " " + hasBomb(a, b));
                /*if (!hasBomb(a, b)) {
                    //printSrurroundingBlocks(a, b);
                */
                //Debug.Log("Has Bomb "+ hasBomb(a, b));
            }
        }
    }

    public int getNumberOfBombs(int x, int y)
    {
        int count = 0;
        for (int a = x - 1; a <= x + 1; a++)
        {
            for (int b = y - 1; b <= y + 1; b++)
            {
                if (a == x && b == y)
                    continue;
                if (hasBomb(a, b)) {
                    count++;
                }
            }
        }
        return count;
    }

    private bool hasBomb(int x, int y) {
        foreach (Block block in blocks) {
            if (block.getX() == x && block.getY() == y)
                return block.getIsBomb();
        }
        return false;
    }

    public int GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public int getRemainingBombs() {
        return remainingBombs;
    }
    private void addBlockToGameScene(int x, int y,int spriteIndex) {

        if (spriteIndex == -2) {
            // DEAD
            UnityEngine.Object.Destroy(GameObject.Find(x + "," + y));
            UtilsClass.CreateWorldSprite(x + "," + y, sprites[sprites.Length - 1], GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, new Vector3(1, 1, 1), 0, Color.white);
            return;
        }
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            if (spriteIndex >= 0)
            {
                UnityEngine.Object.Destroy(GameObject.Find(x + "," + y));
                UtilsClass.CreateWorldSprite(x + "," + y, sprites[spriteIndex], GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, new Vector3(1, 1, 1), 0, Color.white);
            }
            else
                UnityEngine.Object.Destroy(GameObject.Find(x + "," + y));
        }
        if (hasBomb(x,y))
        {
            UnityEngine.Object.Destroy(GameObject.Find(x + "," + y));
            UtilsClass.CreateWorldSprite(x + "," + y, sprites[0], GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, new Vector3(1, 1, 1), 0, Color.white);
        }
    }

    public int[] getGridCoOrdinates(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        int[] array = new int[2];
        array[0] = x;
        array[1] = y;
        return array;
    }
}
