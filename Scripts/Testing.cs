/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;

public class Testing : MonoBehaviour {

    private Grid grid;
    private float mouseMoveTimer;
    private float mouseMoveTimerMax = .01f;

    int[] gridIndex = new int[2];

    [SerializeField] Sprite[] sprite;

    private void Start() {
        grid = new Grid(8, 8, 1.5f, new Vector3(0, 0), sprite);
    }

    private void Update()
    {
        HandleClickToModifyGrid();
        addFlag();
        
    }

    private void addFlag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            grid.setFlag(UtilsClass.GetMouseWorldPosition());
        }
        checkBombs();
    }

    private void checkBombs()
    {
        if (grid.getRemainingBombs() <= 0) {
            // VICTORY
            Debug.Log("VICTORY");
        }
    }

    private void HandleClickToModifyGrid() {
        if (Input.GetMouseButtonDown(0)) {
            //grid.SetValue(UtilsClass.GetMouseWorldPosition(), 1);
            grid.SetSprite(UtilsClass.GetMouseWorldPosition());
        }
    }

    private void HandleHeatMapMouseMove() {
        mouseMoveTimer -= Time.deltaTime;
        if (mouseMoveTimer < 0f) {
            mouseMoveTimer += mouseMoveTimerMax;
            int gridValue = grid.GetValue(UtilsClass.GetMouseWorldPosition());
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), gridValue + 1);
        }
    }
}

