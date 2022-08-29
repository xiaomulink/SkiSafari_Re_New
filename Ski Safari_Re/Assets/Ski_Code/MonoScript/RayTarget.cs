using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayTarget : MonoBehaviour
{
    public static bool IsRay = true;

    public Camera m_Camera;
    public GameObject Console;
    private Ray ray;//从摄像机发出射线(根据鼠标在屏幕位置)

    private RaycastHit hitInfo;//获取射线信息

    public LayerMask mask;//层的mask

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Instantiate(Console);
        }
        catch
        {

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsRay)
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, mask))
            {
                Debug.DrawLine(ray.origin, hit.point);
                GameObject gameobj = hit.collider.gameObject;
                //注意要将对象的tag设置成collider才能检测到
                //if (gameobj.tag == "")
                {
                    if (gameobj.GetComponent<GUIButton>())
                    {
                        gameobj.GetComponent<GUIButton>().Click(Input.mousePosition);
                    }
                }
            }
        }
        /*|| (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && IsRay*/
        /* if (Input.GetMouseButtonDown(0) && IsRay)
         {
             Ray ray_ = m_Camera.ScreenPointToRay(Input.mousePosition);
             RaycastHit hit_;
             if (Physics.Raycast(ray_, out hit_, 1000, mask))
             {
                 Debug.DrawLine(ray_.origin, hit_.point);
                 GameObject gameobj = hit_.collider.gameObject;
                 //注意要将对象的tag设置成collider才能检测到
                 //if (gameobj.tag == "")
                 {
                     if (gameobj.GetComponent<GUIButton>())
                     {
                         gameobj.GetComponent<GUIButton>().Click(Input.mousePosition);
                     }
                 }
             }
         }
         if (Input.GetMouseButton(0) && IsRay)
         {
             Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
             RaycastHit hit;
             if (Physics.Raycast(ray, out hit, 1000, mask))
             {
                 Debug.DrawLine(ray.origin, hit.point);
                 GameObject gameobj = hit.collider.gameObject;
                 //注意要将对象的tag设置成collider才能检测到
                 //if (gameobj.tag == "")
                 {
                     if (gameobj.GetComponent<GUIButton>())
                     {
                         try
                         {
                             gameobj.GetComponent<GUIButton>().Drag(Input.mousePosition);
                         }
                         catch
                         {

                         }
                     }
                 }
             }




             if (Input.GetMouseButtonUp(0) && IsRay || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && IsRay)
             {
                 Ray ray_1 = m_Camera.ScreenPointToRay(Input.mousePosition);
                 RaycastHit hit_1;
                 if (Physics.Raycast(ray_1, out hit_1, 1000, mask))
                 {
                     Debug.DrawLine(ray_1.origin, hit_1.point);
                     GameObject gameobj = hit_1.collider.gameObject;
                     //注意要将对象的tag设置成collider才能检测到
                     //if (gameobj.tag == "")
                     {
                         if (gameobj.GetComponent<GUIButton>())
                         {
                             gameobj.GetComponent<GUIButton>().Click(Input.mousePosition);
                         }
                     }
                 }
             }
         }*/
    }

    private void FixedUpdate()
    {
    
    }
}

