using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    [SerializeField] private Player m_CurrentTurn;

    [SerializeField] private Ball m_Ball;
    public static Ball Ball => Instance.m_Ball;

    [SerializeField] private Camera m_MainCamera;
    public static Camera MainCamera => Instance.m_MainCamera;

    [SerializeField] private Transform m_GoalPoint;
    private Vector3 m_GoalPosition;
    [SerializeField] private ParallaxController m_Parallax;

    private Sequence m_GoalSequence;
    public bool IsGoal;

    public void Start()
    {
        m_GoalPosition = m_GoalPoint.position;
        Physics2D.IgnoreCollision(m_Player1.OwnCollider, m_Player2.OwnCollider, true);
        SwitchTurns();
    }

    public void SwitchTurns()
    {
        Player nextPlayer;
        if(m_CurrentTurn == m_Player1)
        {
            nextPlayer = m_Player2;
        }
        else
        {
            nextPlayer = m_Player1;
        }
        if (m_CurrentTurn != null)
        {
            Physics2D.IgnoreCollision(m_CurrentTurn.OwnCollider, m_Ball.Collider, true);
        }
        Physics2D.IgnoreCollision(nextPlayer.OwnCollider, m_Ball.Collider, false);
        m_CurrentTurn = nextPlayer;
    }

    public void PlayersIgnoreCollision(Collider2D collider, bool ignore)
    {
        Physics2D.IgnoreCollision(m_Player1.OwnCollider, collider, ignore);
        Physics2D.IgnoreCollision(m_Player2.OwnCollider, collider, ignore);
    }

    public void Goal()
    {
        IsGoal = true;

        if(m_GoalSequence == null)
        {
            m_Ball.transform.localScale = Vector3.one;
            m_GoalSequence.Kill();
        }
        m_GoalSequence = DOTween.Sequence();
        PlayersIgnoreCollision(m_Ball.Collider, true);
        m_GoalSequence.AppendCallback(() =>
        {
            m_Player1.RigidBody.bodyType = RigidbodyType2D.Kinematic;
            m_Player2.RigidBody.bodyType = RigidbodyType2D.Kinematic;
            m_Player1.RigidBody.velocity = Vector3.zero;
            m_Player2.RigidBody.velocity = Vector3.zero;
            m_Player1.Movement.canMove = false;
            m_Player2.Movement.canMove = false;
            
            m_Player1.ReleaseBall(false);
            m_Player2.ReleaseBall(false);
            m_Ball.Stop();
        });
        m_GoalSequence.AppendInterval(1f);
        m_GoalSequence.AppendCallback(() =>
        {
            m_Player1.RigidBody.bodyType = RigidbodyType2D.Dynamic;
            m_Player2.RigidBody.bodyType = RigidbodyType2D.Dynamic;
        });
        m_GoalSequence.Join(m_Ball.transform.DOMove(new Vector3(0, -1f, 0f), 2f).SetEase(Ease.Linear));
        m_GoalSequence.Join(m_Ball.transform.DOScale(Vector3.one * 0.25f, 2f).SetEase(Ease.Linear));
        m_GoalSequence.AppendCallback(() =>
        {
            // ADD GOAL SOUND HERE
        });
        m_GoalSequence.AppendInterval(3f);
        m_GoalSequence.Append(m_Ball.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        m_GoalSequence.AppendCallback(() =>
        {
            m_Player1.Movement.canMove = true;
            m_Player2.Movement.canMove = true;
            m_Ball.Release();
            IsGoal = false;
            PlayersIgnoreCollision(m_Ball.Collider, false);
        });

        m_GoalSequence.Play();
    }


    public void Update()
    {
        float yPos = 1f;
        float p1Pos = m_Player1.transform.position.y;
        float p2Pos = m_Player2.transform.position.y;
        if (p1Pos < 1f && p2Pos < 1f)
        {
            yPos = 1f;
        }
        else if (p2Pos > p1Pos)
        {
            yPos = m_Player2.transform.position.y;
        }
        else if (p1Pos > p2Pos)
        {
            yPos = m_Player1.transform.position.y;
        }
        if(IsGoal)
        {
            yPos = Mathf.Max(m_Ball.transform.position.y, 1f);
        }
        if (yPos >= 1f)
        {
            m_MainCamera.transform.position = new Vector3(m_MainCamera.transform.position.x, yPos - 1f, m_MainCamera.transform.position.z);
        }
        m_Parallax.UpdateParallax(yPos);
    }

}
