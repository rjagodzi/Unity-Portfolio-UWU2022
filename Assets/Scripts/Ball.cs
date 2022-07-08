using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Transform m_Transform;
    [SerializeField] private Transform m_FollowCamera;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Collider2D m_Collider;
    [SerializeField] private bool m_IsHold;
    [SerializeField] private float m_BaseAngularVelocity;
    public bool IsHold => m_IsHold;

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Transform.position.y >= 1f)
        {
            m_FollowCamera.position = new Vector3(m_FollowCamera.position.x, m_Transform.position.y - 1f, m_FollowCamera.position.z); 
        }
    }

    public void Kick(Vector2 direction, float force)
    {
        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
        m_RigidBody.AddForce(direction * force);
        m_RigidBody.angularVelocity = m_BaseAngularVelocity * force;
    }

    public void Stop()
    {
        m_RigidBody.velocity = Vector2.zero;
        m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Release()
    {
        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
    }
}
