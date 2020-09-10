using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    private int x;
    private int y;
    private bool isBomb;

    public Block(int x, int y, bool isBomb) {
        this.x = x;
        this.y = y;
        this.isBomb = isBomb;
    }

    public int getX() {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public bool getIsBomb() {
        return isBomb;
    }
}
