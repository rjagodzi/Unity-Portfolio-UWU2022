using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    private Transform m_Transform;
    [SerializeField] private Transform m_FollowCamera;
    [SerializeField] private Rigidbody2D m_RigidBody;
    public Rigidbody2D RigidBody => m_RigidBody;
    [SerializeField] private Collider2D m_Collider;
    public Collider2D Collider => m_Collider;
    [SerializeField] private bool m_IsHold;
    [SerializeField] private float m_BaseAngularVelocity;
    [SerializeField] private Transform m_AimingDirection;
     public float Angle;
    [SerializeField] private float m_InitialForceMultiplier = 1;
    [SerializeField] private float m_AppliedForce = 0;
    [SerializeField] private Transform m_SpriteTransform;

    [Header("Kick Effect")]
    [SerializeField] private Ease m_EaseCurve = Ease.InCubic;
    [SerializeField] private float m_BaseDelay = 0.15f;
    [SerializeField] private float m_DelayLimit = 1f;
    [SerializeField] private float m_SquashStep = 0.1f;
    [SerializeField] private float m_SquashLimit = 0.25f;

    public float DebugForce;
    public Vector2 DebugDirection;
    public bool IsHold => m_IsHold;
    public bool IsKicking = false;

    private Sequence m_KickingSequence;


    public void SetArrowVisibility(bool value)
    {
        m_AimingDirection.gameObject.SetActive(value);
    }

    public void SetAppliedForce(float val)
    {
        m_AppliedForce = val;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = GetComponent<Transform>();
        SetArrowVisibility(false);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(Angle - 90, Vector3.forward);
        m_AimingDirection.rotation = rotation;
        DebugDirection = new Vector2(Mathf.Sin(Mathf.Deg2Rad * -(Angle - 90)), Mathf.Cos(Mathf.Deg2Rad * -(Angle - 90)));
      //  if(Input.GetButtonDown("Fire1") && !IsKicking)
       // {
       //     Kick(DebugDirection, 1, m_Angle);
     //   }
    }

    public void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_AppliedForce = 0;
            if (m_RigidBody.angularVelocity > 0)
            {
                m_RigidBody.angularVelocity = Mathf.Min(m_BaseAngularVelocity * m_AppliedForce * ((m_RigidBody.angularVelocity > 0) ? 1 : -1), m_RigidBody.angularVelocity);
            }
            else
            {
                m_RigidBody.angularVelocity = Mathf.Min(m_BaseAngularVelocity * m_AppliedForce * ((m_RigidBody.angularVelocity > 0) ? 1 : -1), m_RigidBody.angularVelocity);
            }
        }
    }

    public void Kick()
    {
        Kick(DebugDirection, 1, Angle);
    }

    public void Kick(Vector2 direction, float force, float angle)
    {
        IsKicking = true;
        if(m_KickingSequence != null)
        {
            m_KickingSequence = DOTween.Sequence();
        }

        m_RigidBody.angularVelocity = 0;
        m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
        m_RigidBody.velocity = Vector2.zero;
        m_AppliedForce += force;

        m_SpriteTransform.DOScaleY(Mathf.Max(m_SquashLimit, 1f-(m_SquashStep * m_AppliedForce)), Mathf.Min(m_BaseDelay * m_AppliedForce, m_DelayLimit))
                         .SetEase(m_EaseCurve)
                         .OnComplete(() => { m_SpriteTransform.localScale = Vector3.one; });

        DOTween.To(() => m_RigidBody.angularVelocity,
                   x => m_RigidBody.angularVelocity = x,
                   m_BaseAngularVelocity * m_AppliedForce * ((direction.x > 0) ? -1 : 1),
                   Mathf.Min(m_BaseDelay * m_AppliedForce, m_DelayLimit))
               .OnComplete(() =>
               {
                   m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
                   m_RigidBody.AddForce(direction * ( 1 + (m_AppliedForce * 0.1f) ) * m_InitialForceMultiplier);
                   IsKicking = false;
               })
               .SetEase(m_EaseCurve);
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
