using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第三人称 摄像机旋转.
/// </summary>
public class PlayerCameraController : MonoBehaviour {

    private GameObject player_Third;

	void Start () {
        player_Third = GameObject.Find("Player/ThirdPersonController");
	}

    void Update()
    {
        transform.LookAt(player_Third.transform);

        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = -Input.GetAxis("Mouse Y");

            //Quaternion rot = Quaternion.AngleAxis(x, Vector3.up);
            //Quaternion rotY = Quaternion.AngleAxis(y, Vector3.right);

            //Vector3 dir = transform.localPosition - player_Third.transform.localPosition;
            //transform.localPosition = player_Third.transform.localPosition + rotY*rot * dir;

            //Quaternion myRot = transform.localRotation;
            //transform.localRotation = rotY*rot * myRot;
            //transform.localRotation *= rot;

            transform.RotateAround(player_Third.transform.localPosition, Vector3.up, x);
            transform.RotateAround(player_Third.transform.localPosition, Vector3.right, y);

            Vector3 dir = transform.localPosition - player_Third.transform.localPosition;
            Quaternion rot2 = Quaternion.Euler(new Vector3(y, x, 0));
            transform.localPosition = player_Third.transform.localPosition + rot2 * dir;
        }
    }
}
