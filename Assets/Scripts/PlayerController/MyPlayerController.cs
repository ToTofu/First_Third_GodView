using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人物控制模式.
/// </summary>
public enum PlayerControllerMode : byte
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
    /// 上帝模式.
    /// </summary>
    GOD,
    /// <summary>
    /// 
    /// </summary>
    DEFAULT
}

/// <summary>
/// 角色控制器.
/// </summary>
public class MyPlayerController : MonoBehaviour {

    private Transform m_Transform;                              //Transform.
    private Animator m_Animator;                                //动画组件.
    private CharacterController m_CC;                           //人物控制器组件.
    private Rigidbody m_Rigidbody;                              //Rigidbody组件.
    private CapsuleCollider m_CapsuleCollider;                  //胶囊碰撞体. 不使用人物控制器时，使能胶囊碰撞体.

    private Camera playerCamera;                                //人物摄像机.
    private CameraController_First playerCameraScript_First;    //人物摄像机第一人称脚本.
    private CameraController_Third playerCameraScript_Third;    //人物摄像机第三人称脚本.

    private PlayerControllerMode playerControllerMode;          //当前人物控制模式.

    private float h = 0;                    //A D键输入.
    private float v = 0;                    //W S键输入.

    private bool isRun = false;             //是否奔跑.
    private float walkSpeed_Third = 0.01f;  //第三人称 行走速度.
    //private float runSpeed_Third = 0.1f;  //第三人称 奔跑速度.  //当前动画默认是会移动的，所以速度已经提升.
    private float walkSpeed_First = 0.03f;  //第一人称 行走速度.
    private float runSpeed_First = 0.1f;    //第一人称 奔跑速度.  //当前动画默认是移动的.但没有向后走的动画.所以第一人称不使用动画.
    private float walkSpeed_God = 0.03f;    //漫游模式 行走速度.
    private float runSpeed_God = 0.1f;      //漫游模式 奔跑速度.

	void Start () {
        FindInit();

        SetPlayerControllerMode(PlayerControllerMode.GOD);
	}

	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //根据控制模式，使用不同的控制方法.
        switch (playerControllerMode)
        {
            case PlayerControllerMode.FIRST:
                MoveUpdate_FirstController();   
                break;
            case PlayerControllerMode.THIRD:
                MoveUpdate_ThirdController();
                break;
            case PlayerControllerMode.GOD:
                MoveUpdate_GodController();
                break;
            case PlayerControllerMode.DEFAULT:
                break;
        }
	}

    /// <summary>
    /// 查找初始化.
    /// </summary>
    private void FindInit()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Animator = gameObject.GetComponent<Animator>();
        m_CC = gameObject.GetComponent<CharacterController>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        playerCamera = Camera.main;
        playerCameraScript_First = playerCamera.GetComponent<CameraController_First>();
        playerCameraScript_Third = playerCamera.GetComponent<CameraController_Third>();
    }

    /// <summary>
    /// 设置人物控制模式.
    /// </summary>
    public void SetPlayerControllerMode(PlayerControllerMode mode)
    {
        playerControllerMode = mode;
        switch (playerControllerMode)
        {
            case PlayerControllerMode.FIRST:
                if (!m_CC.enabled) m_CC.enabled = true;
                if (!m_Rigidbody.useGravity) m_Rigidbody.useGravity = true;
                if (m_CapsuleCollider.enabled) m_CapsuleCollider.enabled = false;

                playerCameraScript_First.enabled = true;
                playerCameraScript_Third.enabled = false;

                //playerCamera.transform.parent = m_Transform;
                playerCamera.transform.SetParent(m_Transform, false);
                playerCamera.transform.localPosition = new Vector3(0, 1.4f, 0);   //摄像机跳到人物头部前方.
                playerCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                
                break;
            case PlayerControllerMode.THIRD:
                if (!m_CC.enabled) m_CC.enabled = true;
                if (!m_Rigidbody.useGravity) m_Rigidbody.useGravity = true;
                if (m_CapsuleCollider.enabled) m_CapsuleCollider.enabled = false;

                playerCameraScript_First.enabled = false;
                playerCameraScript_Third.enabled = true;

                //playerCamera.transform.parent = m_Transform.parent;
                playerCamera.transform.SetParent(m_Transform.parent, false);
                playerCamera.transform.localPosition = new Vector3(0, m_Transform.localPosition.y + 2.2f, m_Transform.localPosition.z - 2.2f); //摄像机跳到人物后上方. 低于人物，视角会判断异常.
                playerCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                
                break;
            case PlayerControllerMode.GOD:
                //todo: BUG，其他模式转换为上帝模式时，在变换人物碰撞体有时候会出现，人物不受控往一个方向移动.

                //todo:是否锁定Rigidbody的Freeze Position?
                if (m_CC.enabled) m_CC.enabled = false;
                if (m_Rigidbody.useGravity) m_Rigidbody.useGravity = false;
                if (!m_CapsuleCollider.enabled) m_CapsuleCollider.enabled = true;

                playerCameraScript_First.enabled = true;
                playerCameraScript_Third.enabled = false;

                //playerCamera.transform.parent = m_Transform;
                playerCamera.transform.SetParent(m_Transform, false);
                playerCamera.transform.localPosition = new Vector3(0, 1.4f, 0);   //摄像机跳到人物头部前方.
                playerCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);

                break;
            case PlayerControllerMode.DEFAULT:
                break;
        }
    }

    /// <summary>
    /// 第三人称.人物移动操作.
    /// </summary>
    private void MoveUpdate_ThirdController()
    {
        //按住左SHIFT 加速.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }

        //按下了 W A S D.
        if (v != 0 || h != 0)
        {
            if (isRun)  //执行奔跑动画.
            {
                m_Animator.SetBool("Run", true);
            }
            else        //执行行走动画.
            {
                m_Animator.SetBool("Run", false);
                m_Animator.SetBool("Walk", true);
            }

            float rotY = playerCamera.transform.rotation.eulerAngles.y;     //摄像机世界坐标前方.
            Vector3 dir = new Vector3(h, 0, v);                             //移动方向.
            dir = Quaternion.Euler(0, rotY, 0) * dir;                       //移动方向的前方为摄像机前方.（按下W，向摄像机正前方行走）（前进方向必须是摄像机的前方）

            //m_CC.SimpleMove(dir);
            m_CC.Move(dir * walkSpeed_Third);
            m_Transform.localRotation = Quaternion.LookRotation(dir);
        }
        else
        {
            m_Animator.SetBool("Walk", false);
        }
    }

    /// <summary>
    /// 第一人称.人物移动操作.
    /// </summary>
    private void MoveUpdate_FirstController()
    {
        //按住左SHIFT 加速.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;   //加速.
        }
        else
        {
            isRun = false;  //正常速度.
        }

        //按下了 W A S D.
        if (v != 0 || h != 0)
        {
            //todo:----->>当前动画默认是移动的.而且没有向后走的动画.所以第一人称不使用动画.
            float rotY = playerCamera.transform.rotation.eulerAngles.y;     //摄像机世界坐标前方.
            Vector3 dir = new Vector3(h, 0, v);                             //移动方向.
            dir = Quaternion.Euler(0, rotY, 0) * dir;                       //移动方向的前方为摄像机前方.（按下W，向摄像机正前方行走）（前进方向必须是摄像机的前方）

            if (isRun)  //加速.
            {
                //m_CC.SimpleMove(dir * runSpeed_First);
                m_CC.Move(dir * runSpeed_First);
            }
            else        //正常速度.
            {
                //m_CC.SimpleMove(dir * walkSpeed_First);
                m_CC.Move(dir * walkSpeed_First);
            }
        }
    }

    /// <summary>
    /// 漫游模式.人物移动操作.（不受重力影响，飞行.）
    /// </summary>
    private void MoveUpdate_GodController()
    {
        //按住左SHIFT 加速.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;   //加速.
        }
        else
        {
            isRun = false;  //正常速度.
        }

        //按下了 W A S D.
        if (v != 0 || h != 0)
        {
            float rotY = playerCamera.transform.localRotation.eulerAngles.y;    //摄像机自身坐标前方.左右.
            float rotX = playerCamera.transform.localRotation.eulerAngles.x;    //摄像机自身坐标前方.上下.
            Vector3 dir = new Vector3(h, 0, v);                                 //移动方向.
            dir = Quaternion.Euler(rotX, rotY, 0) * dir;                        //移动方向的前方为摄像机前方.（按下W，向摄像机正前方行走）（前进方向必须是摄像机的前方）

            if (isRun)  //加速.
            {
                //m_CC.SimpleMove(dir * runSpeed_First);
                //m_CC.Move(dir * runSpeed_First);

                m_Transform.Translate(dir * runSpeed_God);
            }
            else        //正常速度.
            {
                //m_CC.SimpleMove(dir * walkSpeed_First);
                //m_CC.Move(dir * walkSpeed_First);

                m_Transform.Translate(dir * walkSpeed_God);
            }
        }
    }
}
