using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 切换人物控制模式.
/// </summary>
public class SwitchPlayerController : MonoBehaviour {

    private Transform m_Transform;
    private Button first_BTN;
    private Button third_BTN;
    private Button god_BTN;

    private MyPlayerController playerController;

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        first_BTN = m_Transform.Find("First_BTN").GetComponent<Button>();
        third_BTN = m_Transform.Find("Third_BTN").GetComponent<Button>();
        god_BTN = m_Transform.Find("God_BTN").GetComponent<Button>();

        playerController = GameObject.Find("Player/Person").GetComponent<MyPlayerController>();

        first_BTN.onClick.AddListener(() => playerController.SetPlayerControllerMode(PlayerControllerMode.FIRST));
        third_BTN.onClick.AddListener(() => playerController.SetPlayerControllerMode(PlayerControllerMode.THIRD));
        god_BTN.onClick.AddListener(() => playerController.SetPlayerControllerMode(PlayerControllerMode.GOD));
	}
	
}
