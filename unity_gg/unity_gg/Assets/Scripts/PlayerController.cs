using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public string playerName = "Player";
    public int hp = 100;

    [Header("이동 및 점프")]
    public float moveSpeed = 5f;
    public float jumpPower = 8f;
    public Vector3 startPosition;
    public Transform visual;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private bool isGrounded = false;

    [Header("게임 설정")]
    public int lives = 3;
    private bool isGameOver = false;
    public float fallLimit = -10f;

    public GameObject gameOverPanel;
    public TMP_Text livesText;
    public string nextSceneName = "Stage2";

    [Header("캐릭터 방향 이미지")]
    public Sprite idleSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startPosition;

        if (livesText != null)
            livesText.text = "LIFE " + lives;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 캐릭터 이미지 가져오기
        if (visual != null)
        {
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
        }

        // 시작할 때 정지 이미지 표시
        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    public void OnMove(InputValue value)
    {
        if (isGameOver)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = value.Get<Vector2>();

        if (spriteRenderer == null)
            return;

        // 왼쪽 이동
        if (moveInput.x < -0.01f)
        {
            if (leftSprite != null)
                spriteRenderer.sprite = leftSprite;
        }
        // 오른쪽 이동
        else if (moveInput.x > 0.01f)
        {
            if (rightSprite != null)
                spriteRenderer.sprite = rightSprite;
        }
        // 정지
        else
        {
            if (idleSprite != null)
                spriteRenderer.sprite = idleSprite;
        }
    }

    public void OnJump(InputValue value)
    {
        if (isGameOver)
            return;

        if (value.isPressed && isGrounded)
        {
            Debug.Log("점프!");

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpPower
            );
        }
    }

    void Update()
    {
        // 좌우 이동
        if (!isGameOver)
        {
            transform.Translate(
                Vector3.right *
                moveInput.x *
                moveSpeed *
                Time.deltaTime
            );
        }

        // 맵 아래로 떨어지면 목숨 감소
        if (!isGameOver && transform.position.y < fallLimit)
        {
            LoseLife();
        }

        // 캐릭터 크기는 변경하지 않음
        // Inspector에서 설정한 Scale 그대로 유지
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver)
            return;

        // 다음 스테이지 이동
        if (collision.CompareTag("Goal"))
        {
            SceneManager.LoadScene(nextSceneName);
        }

        // 함정 충돌
        if (collision.CompareTag("Trap"))
        {
            LoseLife();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("착지");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("공중");
        }
    }

    void LoseLife()
    {
        if (isGameOver)
            return;

        lives -= 1;

        // 시작 위치로 돌아가기
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
        isGrounded = false;

        // 정지 이미지로 초기화
        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }

        if (livesText != null)
        {
            livesText.text = "LIFE " + lives;
        }

        // 목숨이 0이면 게임 오버
        if (lives <= 0)
        {
            isGameOver = true;
            Debug.Log("게임 오버");

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }
}