using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private CellCreator creator;
    [SerializeField] private Text guideText;

    private Camera mainCam = null;

    private Cell startCell = null;
    private Cell destinationCell = null;
    
    private List<Cell> openList = new List<Cell>();
    private List<Cell> closeList = new List<Cell>();
    
    private Queue<Cell> cmdStack = new Queue<Cell>();

    [SerializeField] private bool IsAllowDiagonalMoveThroughObstacle;
    
    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (cmdStack.Count >= 2)
                {
                    Reset();
                }

                Cell clickedCell = hit.collider.GetComponent<Cell>();
                clickedCell.SetPath();
                cmdStack.Enqueue(clickedCell);
            }
            else
            {
                Reset();
            }
            
            if (cmdStack.Count > 1)
            {
                OnFindPath();
                
                guideText.text = "길을 찾았습니다!";
            }
            else
            {
                SetGuilde();    
            }
            
        }
    }

    void Reset()
    {
        startCell = null;
        destinationCell = null;
        
        openList.Clear();
        closeList.Clear();
        
        cmdStack.Clear();
        
        CellCreator.Static.Reset();
    }
    
    void SetGuilde()
    {
        if (cmdStack.Count <= 0)
        {
            guideText.text = "시작지점을 선택하세요";
        }
        else if (cmdStack.Count == 1)
        {
            guideText.text = "도착지점을 선택하세요";
        }
    }
    
    void OnFindPath()
    {
        startCell = cmdStack.Dequeue();
        destinationCell = cmdStack.Dequeue();
        
        if (startCell && destinationCell)
        {
            //시작 지점을 open list에 추가
            openList.Add(startCell);

            FindPathRecursive(startCell);
        }
    }

    void FindPathRecursive(Cell cell)
    {
        //인접 셀 중 지나갈 수 있는 셀을 open list에 추가
        foreach (var around in cell.AroundDic)
        {
            Cell aroundCell = CellCreator.GetCell(around.Key);
            if (!aroundCell.IsObstacle && !closeList.Contains(aroundCell))
            {
                if (openList.Contains(aroundCell))
                {
                    if (aroundCell.G > cell.G + cell[around.Key])
                    {
                        aroundCell.Parent = cell;
                    }
                }
                else
                {
                    aroundCell.Parent = cell;
                    openList.Add(aroundCell);
                    
                    //만약 목적지가 열린목록에 추가된다면, 길 찾기를 완료한것으로 판단
                    if (aroundCell == destinationCell)
                    {
                        SuccessFindPath();
                        return;
                    }
                }
            }
        }

        openList.Remove(cell);
        closeList.Add(cell);

        Tuple<int, Cell> minF = new Tuple<int, Cell>(int.MaxValue, null);
        
        foreach (var checkCell in openList)
        {
            Cell parent = checkCell.Parent;
            
            //나의 G 비용은 부모의 G + 부모로부터 나한테 오는데 드는 비용 (대각선이면 14, 직각이면 10)
            int G = parent.G + parent[checkCell.Idx];
            
            checkCell.G = G;
            
            //H 비용
            int H = GetH(destinationCell, checkCell);

            int F = G + H;
            
            if (minF.Item1 > F)
            {
                minF = new Tuple<int, Cell>(F, checkCell);
            }
        }
        
        FindPathRecursive(minF.Item2);
    }

    int GetH(Cell start, Cell end)
    {
        return (Mathf.Abs(start.Row - end.Row) + Mathf.Abs(start.Column - end.Column)) * 10;
    }

    void SuccessFindPath()
    {
        Cell upStreamCell = destinationCell;
        
        while (upStreamCell.Parent != null)
        {
            upStreamCell.SetPath();
            upStreamCell = upStreamCell.Parent;
        }
        
        upStreamCell.SetPath();
    }
}
