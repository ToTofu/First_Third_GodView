using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第一人称 摄像机控制.
/// </summary>
public class CameraController_First : MonoBehaviour {

    private Transform m_Transform;
    private Transform playerPerson;     //摄像机所在的人物(物体).

    private float minRotX = -40;        //视角上下方向的限制 最高为-40.
    private float maxRotX = 30;         //视角上下方向的限制 最低为30.

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        playerPerson = GameObject.Find("Player/Person").GetComponent<Transform>();
	}
	
	void LateUpdate () {
        float x = Input.GetAxis("Mouse X");
        float y = -Input.GetAxis("Mouse Y");

        playerPerson.Rotate(Vector3.up, x, Space.Self);
        m_Transform.Rotate(Vector3.right, y, Space.Self);

        m_Transform.localRotation = ClampRotationAroundXAxis(m_Transform.localRotation);
	}

    /// <summary>
    /// 限制四元数的欧拉角X.
    /// </summary>
    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minRotX, maxRotX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
