using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    [SerializeField] private Transform m_BallTransform;
    private Transform m_Transform;

    public void Start()
    {
        m_Transform = GetComponent<Transform>();
    }

    public void Update()
    {
        if(m_BallTransform.position.y >= 1f)
        {
            m_Transform.position = new Vector3(m_Transform.position.x, m_BallTransform.position.y, m_Transform.position.y);
        }
        else
        {
            m_Transform.position = Vector3.zero;
        }
    }
}
