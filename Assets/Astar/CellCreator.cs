using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Color;

public class CellCreator : MonoBehaviour
{
    [SerializeField]
    private Cell org;
    
    public Cell[,] Cells = null;

    [SerializeField] private int x;
    [SerializeField] private int y;

    public static CellCreator Static
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CellCreator>();
            return _instance;
        }
    }
    private static CellCreator _instance = null;
    
    public static Cell GetCell(int idx)
    {
        return Static.transform.Find(idx.ToString()).GetComponent<Cell>();
    }

    public void Reset()
    {
        foreach (var cell in Cells)
        {
            Destroy(cell.gameObject);
        }
        Cells = null;

        org.gameObject.SetActive(true);
        CreateCells();
    }
    
    private void Awake()
    {
        CreateCells();
    }

    private void CreateCells()
    {
        Cells = new Cell[x, y];
        
        for (var i = 0; i < x; ++i)
        {
            for (var j = 0; j < y; ++j)
            {
                var cell = Instantiate(org, transform, false);
                cell.SetIdx((y * i) + j, x, y);
                cell.SetObstacle();
                cell.transform.localPosition = new Vector3(i, 0, j);
                cell.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                Cells[i, j] = cell;
            }
        }
        org.gameObject.SetActive(false);
        
        foreach (var ce in Cells)
        {
            Debug.Log($"Rounds. {ce.Idx}, Around : {ce.AroundsInfo()}");
        }
    }
}
