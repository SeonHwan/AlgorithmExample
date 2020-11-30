using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour
{
    [SerializeField] private TextMesh idx;

    public Dictionary<int, SpriteAlignment> AroundDic = new Dictionary<int, SpriteAlignment>(8);
    
    public bool IsObstacle { get; private set; }

    public int Idx { get; private set; }

    public Cell Parent { get; set; }

    public int Row { get; private set; }
    public int Column { get; private set; }
    
    //장애물이 될 확률을 조정하기 위해 불연속 랜덤값 추출을 위한 분포도
    private float[] mObstacleProbe = new float[] {0.6f, 0.4f};

    public int G = 0;
    private static readonly int MtrlColor = Shader.PropertyToID("_Color");

    public int this[int aroundId]
    {
        get
        {
            SpriteAlignment align = AroundDic[aroundId];
            
            switch (align)
            {
                case SpriteAlignment.BottomLeft:
                case SpriteAlignment.BottomRight:
                case SpriteAlignment.TopLeft:
                case SpriteAlignment.TopRight:
                    return 14;
                default:
                    return 10;
            }
        }
    }
    public void SetIdx(int primary, int w, int h)
    {
        Idx = primary;
        idx.text = primary.ToString();

        transform.name = primary.ToString();

        Row = primary / w;
        Column = primary % w;
        
        if (Idx % h != 0)
        {
            AroundDic.Add(Idx - 1, SpriteAlignment.LeftCenter);
            //left
            // Arounds.Add(Idx - 1, 10);    
        }

        if (Idx >= h)
        {
            AroundDic.Add(Idx - h, SpriteAlignment.TopCenter);
            //top
            // Arounds.Add(Idx - h, 10);
        }

        if (Idx % h != (h - 1))
        {
            AroundDic.Add(Idx + 1, SpriteAlignment.RightCenter);
            //right
            // Arounds.Add(Idx + 1, 10);    
        }

        if (Idx < (w * h) - h)
        {
            AroundDic.Add(Idx + h, SpriteAlignment.BottomCenter);
            //bottom
            // Arounds.Add(Idx + h, 10);    
        }

        if (Idx % h != 0 && Idx >= h)
        {
            AroundDic.Add(Idx - (h + 1), SpriteAlignment.TopLeft);
            //leftTop
            // Arounds.Add(Idx - (h+1), 14);    
        }

        if (Idx >= h && Idx % h != (h - 1))
        {
            AroundDic.Add(Idx - (h - 1), SpriteAlignment.TopRight);
            //rightTop
            // Arounds.Add(Idx - (h-1), 14);    
        }

        if (Idx % h != (h - 1) && Idx < (w * h) - h)
        {
            AroundDic.Add(Idx + (h + 1), SpriteAlignment.BottomRight);
            //right bottom
            // Arounds.Add(Idx + (h+1), 14);    
        }

        if (Idx % h != 0 && Idx < (w * h) - h)
        {
            AroundDic.Add(Idx + (h - 1), SpriteAlignment.BottomLeft);
            //left bottom
            // Arounds.Add(Idx + (h-1), 14);
        }

        foreach (var around in AroundDic.Where(around => around.Key < 0 || around.Key >= w * h))
        {
            AroundDic.Remove(around.Key);
        }
        // Arounds.RemoveAll(_ => _ < 0 || _ >= w * h);
    }

    public void SetObstacle()
    {
        float total = 0;
        foreach (var elem in mObstacleProbe)
        {
            total += elem;
        }
        
        float returnVal = mObstacleProbe.Length - 1;
        float randomPoint = Random.value * total;
        for (var i = 0; i < mObstacleProbe.Length; ++i)
        {
            if (randomPoint < mObstacleProbe[i])
            {
                returnVal = i;
                break;
            }
            
            randomPoint -= mObstacleProbe[i];
        }
        
        IsObstacle = returnVal > 0.7f;
        
        GetComponent<Renderer>().material.SetColor(MtrlColor, IsObstacle ? Color.black : Color.white);
    }
    
    public string AroundsInfo()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var around in AroundDic)
        {
            sb.Append(around);
            sb.Append(", ");
        }

        return sb.ToString();
    }

    public void SetPath()
    {
        GetComponent<Renderer>().material.SetColor(MtrlColor, Color.red);
    }
}
