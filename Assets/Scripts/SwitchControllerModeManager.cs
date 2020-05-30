using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 人物控制模式Enum.
/// </summary>
public enum PlayerControllerType : byte
{
    /// <summary>
    /// 第一人称.
    /// </summary>
    FIRST,
    /// <summary>
    /// 第三人称.
    /// </summary>
    THIRD,
    /// <summary>
    /// 漫游模式.
    /// </summary>
    FREE,
    /// <summary>
    /// 默认.
    /// </summary>
    DEFAULT
}

/// <summary>
/// 切换人物控制模式 管理器.
/// </summary>
public class SwitchControllerModeManager : MonoBehaviour {
    
    private GameObject firstObj;    //第一人称角色.
    private GameObject thirdObj;    //第三人称角色.

    private Button first_BTN;       //切换到第一人称视角.
    private Button third_BTN;       //切换到第三人称视角.
    private Camera mainCamera;      //主摄像机.


    //记录刚进入第一人称视角时候的欧拉角和离开第一视角时候的欧拉角(Y方向)
    //float pre1fsAngle = 0;
    //float cur1fsAngle = 0;

	void Start () {
        firstObj = GameObject.Find("Player/FirstPersonController");
        thirdObj = GameObject.Find("Player/ThirdPersonController");
        mainCamera = Camera.main;

        first_BTN = transform.Find("Button").GetComponent<Button>();
        third_BTN = transform.Find("Button (1)").GetComponent<Button>();

        first_BTN.onClick.AddListener(ToFirstController);
        third_BTN.onClick.AddListener(ToThirdController);

        firstObj.SetActive(false);
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToFirstController();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToThirdController();
        }
	}

    /// <summary>
    /// 切换到第一人称视角.
    /// </summary>
    private void ToFirstController()
    {
        //记录一开始
        //pre1fsAngle = cam_1fs.transform.eulerAngles.y;
        //pre1fsAngle = cam_3rd.transform.eulerAngles.y;  //记录的第一人称(这里取的是第三人称,其实是一样的)一开始的y方向欧拉角，这里没用上面注释掉的写法是防止重复按f1键切换然后覆盖初始值导致旋转角度差值缩小
        
        //pre1fsAngle = Camera.main.transform.localEulerAngles.y;
       
        if (!firstObj.activeSelf)   //防止重复切换.
        {
            firstObj.SetActive(true);

            mainCamera.GetComponent<ThirdCameraController>().enabled = false;
            mainCamera.transform.parent = firstObj.transform;
            mainCamera.transform.localPosition = new Vector3(0, 0.6f, 0);
            mainCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);

            firstObj.transform.localPosition = new Vector3(thirdObj.transform.localPosition.x, 0.8f, thirdObj.transform.localPosition.z);
            //firstObj.transform.localRotation = thirdObj.transform.localRotation;
            firstObj.transform.localRotation = Quaternion.Euler(new Vector3(0, thirdObj.transform.localEulerAngles.y, 0));

            thirdObj.SetActive(false);

        }
    }

    /// <summary>
    /// 切换到第三人称视角.
    /// </summary>
    private void ToThirdController()
    {
        //cur1fsAngle = mainCamera.transform.eulerAngles.y;  //记录

        if (!thirdObj.activeSelf)   //防止重复切换.
        {
            thirdObj.SetActive(true);

            //thirdObj.transform.localPosition = firstObj.transform.localPosition;
            //注意这里Mathf里面的方法是幅度，我这里就进行了一个角度转幅度的计算:幅度=角度*pi/180
            //float angle = (cur1fsAngle - pre1fsAngle) * Mathf.PI / 180;
            //thirdObj.GetComponent<ThirdPersonController>().v = Mathf.Cos(angle);
            //thirdObj.GetComponent<ThirdPersonController>().h = Mathf.Sin(angle);
            //print("旋转角度:" + (cur1fsAngle - pre1fsAngle));
            //thirdObj.GetComponent<ThirdPersonController>().flag = true;  //这个flag标志是让ThirdPersonController的update方法执行改变上面的v,h一次，然后第二帧的时候就执行v=Input.GetAxisRaw("Vertical")和h=Input.GetAxisRaw("Horizontal")

            thirdObj.transform.localPosition = firstObj.transform.localPosition;
            thirdObj.transform.localRotation = firstObj.transform.localRotation;

            firstObj.SetActive(false);

            mainCamera.GetComponent<ThirdCameraController>().enabled = true;
            mainCamera.transform.parent = GameObject.Find("Player").transform;
            mainCamera.transform.localPosition = new Vector3(0, 10, 0);
            mainCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
