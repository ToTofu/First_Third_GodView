using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第三人称自由视角 摄像机控制器.
/// </summary>
public class ThirdCameraController : MonoBehaviour
{
    [Header("相机距离")]
    public float freeDistance = 2;
    [Header("相机最近距离")]
    public float minDistance = 0.5f;
    [Header("相机最远距离")]
    public float maxDistance = 20;
    [Header("是否可控制相机距离(鼠标中键)")]
    public bool canControlDistance = true;
    [Header("更改相机距离的速度")]
    public float distanceSpeed = 1;

    [Header("视角灵敏度")]
    public float rotateSpeed = 1;
    [Header("物体转向插值(灵敏度,取值为0到1)")]
    public float TargetBodyRotateLerp = 0.3f;
    [Header("需要转向的物体(摄像机物体)")]
    public GameObject TargetBody;           //此脚本能操作转向的物体(摄像机)
    [Header("相机焦点物体(最好是焦点物体的头部关节)")]
    public GameObject CameraPivot;          //相机焦点物体(人物)  
    [Header("===锁敌===,可不用填.")]
    public GameObject lockTarget = null;
    public float lockSlerp = 1;
    public GameObject lockMark;
    private bool marked;

    [Header("是否可控制物体转向")]
    public bool CanControlDirection = true;
    [Header("俯角(0-89)")]
    public float maxDepression = 80;
    [Header("仰角(0-89)")]
    public float maxEvelation = 80;


    private Vector3 PredictCameraPosition;
    private Vector3 offset;
    private Vector3 wallHit;
    private GameObject tmpMark;

    void Start()
    {
        offset = transform.position - CameraPivot.transform.position;
        if (TargetBody == null)
        {
            TargetBody = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("未绑定目标物体，默认替换为Player标签的物体");
        }
        if (!CameraPivot)
        {
            Debug.LogError("未绑定相机焦点物体");
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LockTarget();
        }
    }

    void FixedUpdate()
    {
        FreeCamera();

        if (lockTarget)
        {
            if (!marked)
            {
                tmpMark = Instantiate(lockMark, lockTarget.transform.position + new Vector3(0, 2.5f, 0), transform.rotation);
                tmpMark.transform.forward = -Vector3.up;
                marked = true;
            }
            else
            {
                tmpMark.transform.position = lockTarget.transform.position + new Vector3(0, 2.5f, 0);
                //tmpMark.transform.forward= -transform.up;
                tmpMark.transform.Rotate(Vector3.up * 30 * Time.fixedDeltaTime, Space.World);
            }
        }
    }

    /// <summary>
    /// 摄像机跟随.旋转.锁敌.
    /// </summary>
    private void FreeCamera()
    {
        offset = offset.normalized * freeDistance;
        transform.position = CameraPivot.transform.position + offset;//更新位置

        #region 控制角色方向开关.
        if (CanControlDirection)//控制角色方向开关
        {
            Quaternion TargetBodyCurrentRotation = TargetBody.transform.rotation;

            if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y - 45, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y - 135, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }


                else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y - 90, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y + 45, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y + 135, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }

                else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y + 90, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);
                }
            }
            else if (Input.GetKey(KeyCode.W))
            {
                TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);

            }
            else if (Input.GetKey(KeyCode.S))
            {
                TargetBody.transform.rotation = Quaternion.Lerp(TargetBodyCurrentRotation, Quaternion.Euler(new Vector3(TargetBody.transform.localEulerAngles.x, transform.localEulerAngles.y - 180, TargetBody.transform.localEulerAngles.z)), TargetBodyRotateLerp);

            }
        }
        #endregion

        #region 控制距离开关.
        if (canControlDistance)//控制距离开关
        {
            freeDistance -= Input.GetAxis("Mouse ScrollWheel") * distanceSpeed;
        }
        #endregion

        freeDistance = Mathf.Clamp(freeDistance, minDistance, maxDistance);

        //是否锁敌.
        if (!lockTarget)    //没有赋值.
        {
            transform.LookAt(lockTarget ? (lockTarget.transform.position) : CameraPivot.transform.position);
        }
        else
        {
            Quaternion tmp = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lockTarget.transform.position - transform.position), lockSlerp * Time.fixedDeltaTime);
            transform.rotation = tmp;
        }

        float eulerX = transform.localEulerAngles.x;//相机的x欧拉角,也就是垂直方向.
        float inputY = Input.GetAxis("Mouse Y");

        if (!lockTarget)
        {
            //水平视野.
            if (!lockTarget)
            {
                transform.RotateAround(CameraPivot.transform.position, Vector3.up, rotateSpeed * Input.GetAxis("Mouse X"));//x不用限制
            }

            //垂直视野限制.
            if (eulerX > maxDepression && eulerX < 90)//当向上角度越界时
            {
                if (inputY > 0)//如果鼠标是在向下滑动
                    transform.RotateAround(CameraPivot.transform.position, Vector3.right, -rotateSpeed * inputY);//允许滑动
            }
            else if (eulerX < 360 - maxEvelation && eulerX > 270)
            {
                if (inputY < 0)
                    transform.RotateAround(CameraPivot.transform.position, Vector3.right, -rotateSpeed * inputY);
            }
            else//角度正常时
            {
                transform.RotateAround(CameraPivot.transform.position, Vector3.right, -rotateSpeed * inputY);
            }
        }

        if (lockTarget)
        {
            offset = CameraPivot.transform.position - (lockTarget.transform.position);
        }
        else
        {
            offset = transform.position - CameraPivot.transform.position;//以上方向发生了变化,记录新的方向向量
        }

        offset = offset.normalized * freeDistance;

        #region 使用遮挡Inwall()方法.(当前不使用)
        /* 
        ///在一次FixedUpdate中,随时记录新的旋转后的位置,然后得到方向,然后判断是否即将被遮挡,如果要被遮挡,将相机移动到计算后的不会被遮挡的位置
        ///如果不会被遮挡,则更新位置为相机焦点位置+方向的单位向量*距离
        ///
        if (Inwall())//预测会被遮挡
        {
            print("Inwall");
            transform.position = CameraPivot.transform.position + (wallHit - CameraPivot.transform.position) * 0.8f;

            return;
        }
        else
        {
            transform.position = CameraPivot.transform.position + offset;
        }
        */
        #endregion
        transform.position = CameraPivot.transform.position + offset;
    }

    /// <summary>
    /// 摄像机与人物之间 是否被墙壁之类的遮挡.
    /// </summary>
    /// <returns></returns>
    private bool Inwall()
    {
        RaycastHit hit;
        //LayerMask mask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Mob")) | (1 << LayerMask.NameToLayer("Weapon")); //将物体的Layer设置为Ignore Raycast,Player和Mob来忽略相机的射线，不然相机将跳到某些物体前,比如怪物,玩家等,
        LayerMask mask = (1 << LayerMask.NameToLayer("Ignore Raycast")); //将物体的Layer设置为Ignore Raycast来忽略相机的射线，不然相机将跳到某些物体前,比如怪物,玩家等,
        mask = ~mask;//将以上的mask取反,表示射线将会忽略以上的层
        //Debug.DrawLine(CameraPivot.transform.position, transform.position - transform.forward, Color.red);

        PredictCameraPosition = CameraPivot.transform.position + offset.normalized * freeDistance;//预测的相机位置
        if (Physics.Linecast(CameraPivot.transform.position, PredictCameraPosition, out hit, mask))//碰撞到任意碰撞体,注意,因为相机没有碰撞器,所以是不会碰撞到相机的,也就是没有碰撞物时说明没有遮挡
        {//也就是说，这个if就是指被遮挡的情况
            wallHit = hit.point;//碰撞点位置
            //Debug.DrawLine(transform.position, wallHit, Color.green);
            return true;
        }
        else//没碰撞到，也就是说没有障碍物
        {
            return false;
        }
    }

    private void LockTarget()
    {
        if (lockTarget)
        {
            lockTarget = null;
            marked = false;
            Destroy(tmpMark);
            return;
        }

        Vector3 top = transform.position + new Vector3(0, 1, 0) + transform.forward * 5;
        LayerMask mask = (1 << LayerMask.NameToLayer("Mob")); //将物体的Layer设置为Ignore Raycast,Player和Mob来忽略相机的射线，不然相机将跳到某些物体前,比如怪物,玩家等,

        Collider[] cols = Physics.OverlapBox(top, new Vector3(0.5f, 0.5f, 5), transform.rotation, mask);
        foreach (var col in cols)
        {
            lockTarget = col.gameObject;
        }
    }
}
