using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_BallHoldingPoint;
    [SerializeField] private Ball m_Ball;
    [SerializeField] private Collider2D m_BallCollider;

    [SerializeField] private RectTransform m_DownMarker;
    [SerializeField] private RectTransform m_UpMarker;
    [SerializeField] private Camera m_MainCamera;

    [SerializeField] private string ActionButton;

    public bool HasBall
    {
        get { return m_Ball != null; }
    }

    [Header("Components")]
    [SerializeField] private Collider2D m_OwnCollider;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Movement m_Movement;

    public void Start()
    {
        m_OwnCollider = GetComponent<Collider2D>();
        m_Movement = GetComponent<Movement>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_MainCamera = Camera.main;
        SetMarkerVisibility(true, false);
        SetMarkerVisibility(false, false);
    }

    public void SetMarkerVisibility(bool up, bool value)
    {
        if (up)
        {
            m_UpMarker.gameObject.SetActive(value);
        }
        else
        {
            m_DownMarker.gameObject.SetActive(value);
        }
    }

    public void OnBecameVisible()
    {
        SetMarkerVisibility(true, false);
        SetMarkerVisibility(false, false);
    }

    public void OnBecameInvisible()
    {
        if(m_Ball.transform.position.y < transform.position.y)
        {
            SetMarkerVisibility(true, true);
        }
        else
        {
            SetMarkerVisibility(false, true);
        }
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
            StopCoroutine(DelayedGravity(0));
            StartCoroutine(DelayedGravity(0.5f));
            m_RigidBody.velocity = Vector3.zero;
            Ball ball = collision.collider.GetComponent<Ball>();
            m_Movement.canMove = false;
            m_BallCollider = collision.collider;
            m_Ball = ball;
            m_Ball.RigidBody.angularVelocity = 0;
            m_Ball.Stop();
            m_Ball.SetArrowVisibility(true);
            Physics2D.IgnoreCollision(m_OwnCollider, m_BallCollider, true);
        }
    }

    IEnumerator DelayedGravity(float time)
    {
        yield return new WaitForSeconds(time);
        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
    }


    public void Update()
    {
        if (Input.GetButtonDown(ActionButton))
        {
            if(!HasBall)
            {
                m_Movement.DoJump();
            }
            else
            {
                m_Ball.Kick();
                StopCoroutine(DelayedGravity(0));
                ReleaseBall();
            }
        }
        if (HasBall)
        {
            m_Ball.Angle = Mathf.Atan2(Input.GetAxis(m_Movement.VerticalAxis), Input.GetAxis(m_Movement.HorizontalAxis)) * Mathf.Rad2Deg;
            m_Ball.transform.position = m_BallHoldingPoint.position;
            if (m_Movement.Collision.onGround)
            {
                m_Ball.SetAppliedForce(0);
            }
        }

        UpdateMarker();
    }

    public void UpdateMarker()
    {
        Vector3 targPos = transform.position;
        Vector3 camForward = m_MainCamera.transform.forward;
        Vector3 camPos = m_MainCamera.transform.position + camForward;
        float distInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);
        if (distInFrontOfCamera < 0f)
        {
            targPos -= camForward * distInFrontOfCamera;
        }
        Vector2 pos = RectTransformUtility.WorldToScreenPoint(m_MainCamera, targPos);
        m_DownMarker.anchoredPosition = new Vector2(pos.x - Screen.width/2, 0);
        m_UpMarker.anchoredPosition = new Vector2(pos.x - Screen.width / 2, -25f);
    }

    public void ReleaseBall()
    {
        if (HasBall)
        {
            m_Ball.Release();
            m_Ball.SetArrowVisibility(false);
            StopCoroutine(RestorePhysics(0));
            StartCoroutine(RestorePhysics(1f));
            m_Ball = null;
        }
    }

    IEnumerator RestorePhysics(float time)
    {
        yield return new WaitForSeconds(time);
        m_Movement.canMove = true;
        Physics2D.IgnoreCollision(m_OwnCollider, m_BallCollider, false);
        m_BallCollider = null;
    }

}
