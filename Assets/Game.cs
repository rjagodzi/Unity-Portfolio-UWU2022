using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_UICombo;
    public TextMeshProUGUI UICombo => m_UICombo;
    [SerializeField] private TextMeshProUGUI m_UICash;
    public TextMeshProUGUI UICash => m_UICash;
    [SerializeField] private TextMeshProUGUI m_UITime;
    public TextMeshProUGUI UITime => m_UITime;

    [SerializeField] private TextMeshProUGUI m_UICalc;
    public TextMeshProUGUI UICalc => m_UICalc;


    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private TextMeshProUGUI m_Summary;

    public float timer = 90f;

    [SerializeField] private GameObject m_UIGoalPanel;
    public GameObject UIGoalPanel => m_UIGoalPanel;

    private Sequence m_GoalSequence;
    public bool IsGoal;

    public void Start()
    {
        m_GoalPosition = m_GoalPoint.position;
        Physics2D.IgnoreCollision(m_Player1.OwnCollider, m_Player2.OwnCollider, true);
        SwitchTurns();
        SetUICash(0);
        SetUICombo(0);
        SetUITimeLeft(90);
        SetUIGoalVisibility(false);
  
    }
    public int Cash;
    public void SetUICash(int cash)
    {
        m_UICash.SetText("Cash: " + cash + "€");
    }

    public void SetUICombo(int combo)
    {
        m_UICombo.SetText("Combo: " + combo);
    }

    public void SetUITimeLeft(int time)
    {
        m_UITime.SetText("Time Left: " + time);
    }

    public void SetCalc(int combo, float height)
    {
        m_UICalc.SetText(combo + " x " + height + "m = " + (combo * height) + "€");
    }

    public void SetUIGoalVisibility(bool val)
    {
        m_UIGoalPanel.SetActive(val);
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
        float ballHeight = Mathf.Round(m_Ball.transform.position.y);
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

            SetUIGoalVisibility(true);
            SetCalc((int)m_Ball.AppliedForce, Mathf.Max(0, ballHeight));
            Cash += (int)(m_Ball.AppliedForce * Mathf.Max(0, ballHeight));
            SetUICash(Cash);
            SetUICombo(0);
        });
        m_GoalSequence.AppendInterval(3f);
        m_GoalSequence.Append(m_Ball.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        m_GoalSequence.AppendCallback(() =>
        {
            SetUIGoalVisibility(false);
            m_Player1.Movement.canMove = true;
            m_Player2.Movement.canMove = true;
            m_Ball.Release();
            IsGoal = false;
            PlayersIgnoreCollision(m_Ball.Collider, false);
        });

        m_GoalSequence.Play();
    }
    public float yPos;
    public bool isGameOver;

    public void GameOver()
    {
        isGameOver = true;
        m_GameOverPanel.SetActive(true);
        m_Summary.SetText("You've earned " + Cash + "€\n Press R to try again!");
        m_Player1.Movement.canMove = false;
        m_Player2.Movement.canMove = false;
    }
    public void Update()
    {
        if (!isGameOver)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                GameOver();
            }
            SetUITimeLeft(Mathf.RoundToInt(timer));
            yPos = 1f;
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
            if (IsGoal)
            {
                yPos = Mathf.Max(m_Ball.transform.position.y, 1f);
            }
            if (yPos >= 1f)
            {
                m_MainCamera.transform.position = new Vector3(m_MainCamera.transform.position.x, yPos - 1f, m_MainCamera.transform.position.z);
            }
            m_Parallax.UpdateParallax(yPos);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

}
