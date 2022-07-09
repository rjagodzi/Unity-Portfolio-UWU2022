using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public void Awake()
    {
        Instance = this;
    }


    [SerializeField] private Player m_Player1;
    public static Player Player1 => Instance.m_Player1;
    [SerializeField] private Player m_Player2;
    public static Player Player2 => Instance.m_Player2;

    [SerializeField] private Ball m_Ball;
    public static Ball Ball => Instance.m_Ball;

    [SerializeField] private Camera m_MainCamera;
    public static Camera MainCamera => Instance.m_MainCamera;


    public void Update()
    {
        if (m_Ball.transform.position.y >= 1f)
        {
            Vector3 midPoint = (m_Player1.transform.position + m_Ball.transform.position) / 2;
            m_MainCamera.transform.position = new Vector3(m_MainCamera.transform.position.x, m_Ball.transform.position.y - 1f, m_MainCamera.transform.position.z);
        }
    }

}
