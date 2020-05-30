using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第三人称 摄像机控制.
/// </summary>
public class CameraController_Third : MonoBehaviour {

    private Transform m_Transform;
    private Transform playerPerson;     //朝向的人物(物体).        //最好朝向的是人物的头部.

    private float distance = 5;         //摄像机与人物的距离.
    private Vector3 dir = Vector3.zero; //摄像机指向人物的方向向量.

    private float maxDis = 8;           //摄像机与人物最大距离.
    private float minDis = 3;           //摄像机与人物最小距离.
    private float minAngleX = 10;       //X轴角度最小限制.(向下最低角度)
    private float maxAngleX = 60;       //X轴角度最大限制.(向上最高角度)
    private float scrollSpeed = 5;      //鼠标滚轮缩放速度.
    private float rotateSpeed = 4;      //视角拖动速度.

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        //playerPerson = GameObject.Find("Player/Person").GetComponent<Transform>();
        playerPerson = GameObject.Find("Player/Person/HeadPos").GetComponent<Transform>();  //最好朝向的是人物的头部.

        dir = playerPerson.position - m_Transform.localPosition;   //先获取摄像机指向人物的方向向量. //朝向头部时，不能使用playerPerson.localPosition
    }

    void LateUpdate()
    {
        CameraController_ThirdController();
    }

    /// <summary>
    /// 第三人称 摄像机旋转控制.
    /// </summary>
    private void CameraController_ThirdController()
    {
        float x = Input.GetAxis("Mouse X") * rotateSpeed;
        float y = Input.GetAxis("Mouse Y") * rotateSpeed;
        distance -= (Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);
        distance = Mathf.Clamp(distance, minDis, maxDis);

        //按住鼠标右键，旋转视角.
        if (Input.GetMouseButton(1))
        {
            Quaternion rotX = Quaternion.Euler(0, x, 0);
            //Quaternion rotY = Quaternion.Euler(-y, 0, 0);

            //摄像机世界坐标Z 比人物世界坐标Z 远(Z轴在人物前方)，上下滑动方向改变.
            if (m_Transform.position.z <= playerPerson.position.z)   //摄像机在人物后方.
            {
                #region 限制上下角度范围.(不同点：rotY的Euler是-y)
                Quaternion rotY = Quaternion.Euler(-y, 0, 0);
                if (m_Transform.localRotation.eulerAngles.x < minAngleX)       //当向下角度越界时.
                {
                    if (y < 0)  //达到最下，向上滑动.
                    {
                        dir = rotX * rotY * dir;
                    }
                    else        //达到最下，向下滑动. (不再向下滑)
                    {
                        dir = rotX * dir;
                    }
                }
                else if (m_Transform.localRotation.eulerAngles.x > maxAngleX)   //当向上角度越界时.
                {
                    if (y > 0)  //达到最上，向下滑动.
                    {
                        dir = rotX * rotY * dir;
                    }
                    else        //达到最上，向上滑动. (不再向上滑)
                    {
                        dir = rotX * dir;
                    }
                }
                else     //上下角度在范围内.
                {
                    dir = rotX * rotY * dir;
                }
                #endregion
            }
            else   //摄像机在人物前方.
            {
                #region 限制上下角度范围.(不同点：rotY的Euler是y)
                Quaternion rotY = Quaternion.Euler(y, 0, 0);
                if (m_Transform.localRotation.eulerAngles.x < minAngleX)       //当向下角度越界时.
                {
                    if (y < 0)  //达到最下，向上滑动.
                    {
                        dir = rotX * rotY * dir;
                    }
                    else        //达到最下，向下滑动. (不再向下滑)
                    {
                        dir = rotX * dir;
                    }
                }
                else if (m_Transform.localRotation.eulerAngles.x > maxAngleX)   //当向上角度越界时.
                {
                    if (y > 0)  //达到最上，向下滑动.
                    {
                        dir = rotX * rotY * dir;
                    }
                    else        //达到最上，向上滑动. (不再向上滑)
                    {
                        dir = rotX * dir;
                    }
                }
                else     //上下角度在范围内.
                {
                    dir = rotX * rotY * dir;
                }
                #endregion
            }
        }

        m_Transform.localPosition = playerPerson.position - dir.normalized * distance;
        
        m_Transform.LookAt(playerPerson);
    }
    

    /// <summary>
    /// 限制四元数的欧拉角X.(暂不使用)
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, 5, 50);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
