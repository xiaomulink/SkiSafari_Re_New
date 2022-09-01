using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// 扇型攻击检测，并绘制检测区域
/// </summary>
public class Scope : MonoBehaviour
{

    public Transform attacked;  //受攻击着
    GameObject go;
    MeshFilter mf;
    MeshRenderer mr;
    Shader shader;

    void Start()
    {

    }

    void Update()
    {
        DrawTool.DrawSector(transform, transform.localPosition, 60, 3);
    }

   
 
}

