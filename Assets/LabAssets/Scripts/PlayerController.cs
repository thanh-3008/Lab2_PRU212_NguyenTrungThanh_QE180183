using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float torqueAmount = 20f;
    public float jumpForce = 500f;

    [Header("Trick System")]
    [Tooltip("Số điểm nhận được cho mỗi vòng xoay 360 độ trên không.")]
    public int pointsPerSpin = 5;

    [Header("Collision Settings")]
    public Collider2D headCollider;

    [Header("Level Settings")]
    [Tooltip("Level hiện tại là bao nhiêu? (e.g., 1, 2, 3...)")]
    public int currentLevelIndex = 1;
    [Tooltip("Tên của Scene Chọn Level")]
    public string levelSelectSceneName = "LevelSelect";

    [Header("UI Settings")]
    [Tooltip("Kéo Panel Hoàn thành/Thua cuộc vào đây")]
    public GameObject finishPanel;

    [Tooltip("Text hiển thị tổng điểm hiện tại")]
    public TextMeshProUGUI coinText;

    [Tooltip("Text hiển thị thời gian đang chạy")]
    public TextMeshProUGUI timerText;

    [Tooltip("Text hiển thị kết quả (Game Over / Level Up)")]
    public TextMeshProUGUI resultText;

    [Tooltip("Text hiển thị tổng điểm đạt được khi kết thúc")]
    public TextMeshProUGUI finalScoreText;

    [Tooltip("Text hiển thị thời gian hoàn thành")]
    public TextMeshProUGUI finalTimeText;

    [Header("Component References")]
    [Tooltip("Sprite của Level (chứa SurfaceEffector2D)")]
    public GameObject levelSprite;
    public AudioController audioController;
    public ParticleSystem boot;
    public ParticleSystem die;

    // Private Variables
    private Rigidbody2D rb2d;
    private float horizontalInput;
    private int groundContactCount = 0;
    private bool isGrounded => groundContactCount > 0;
    private float accumulatedRotation = 0f;
    private bool wasGrounded = true;
    private SurfaceEffector2D surfaceEffector;
    private Vector2 newCenterOfMass;
    private bool isTimerRunning = false;
    private float elapsedTime = 0f;
    private bool isGameEnded = false;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (headCollider != null)
        {
            Vector2 headOffset = headCollider.offset;
            newCenterOfMass = new Vector2(headOffset.x - 1f, 0.5f);
            rb2d.centerOfMass = newCenterOfMass;
        }
        else
        {
            Debug.LogWarning("Chưa gán Head Collider! Chức năng GameOver và Trọng tâm sẽ không chính xác.");
        }
    }

    void Start()
    {
        boot.Stop();
        surfaceEffector = levelSprite.GetComponent<SurfaceEffector2D>();

        if (finishPanel != null)
            finishPanel.SetActive(false);

        Time.timeScale = 1f;
        elapsedTime = 0f;
        isTimerRunning = true;
        isGameEnded = false;

        // Ẩn các text kết thúc
        if (resultText != null) resultText.text = "";
        if (finalScoreText != null) finalScoreText.text = "";
        if (finalTimeText != null) finalTimeText.text = "";
    }

    void Update()
    {
        if (!enabled) return;

        // Cập nhật đồng hồ
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay(elapsedTime);
        }

        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.F))
        {
            audioController.PlaySfx(audioController.boot);
            StartTangToc();
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliseconds = (timeToDisplay % 1) * 100;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    void FixedUpdate()
    {
        if (!enabled) return;

        if (horizontalInput != 0)
            rb2d.AddTorque(-horizontalInput * torqueAmount);

        HandleSpinTricks();
    }

    private void HandleSpinTricks()
    {
        if (!isGrounded)
        {
            accumulatedRotation += rb2d.angularVelocity * Time.fixedDeltaTime;

            if (Mathf.Abs(accumulatedRotation) >= 260f)
            {
                AddScore(pointsPerSpin);
                Debug.Log("Trick Completed! +" + pointsPerSpin + " points!");
                accumulatedRotation -= 260f * Mathf.Sign(accumulatedRotation);
            }
        }

        if (isGrounded && !wasGrounded)
            accumulatedRotation = 0f;

        wasGrounded = isGrounded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContactCount++;
            if (collision.otherCollider == headCollider && !isGameEnded)
            {
                audioController.PlaySfx(audioController.die);
                die.Play();
                GameOver();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            groundContactCount--;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            audioController.PlaySfx(audioController.coin);
            AddScore(1);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("FinishLine") && !isGameEnded)
        {
            audioController.PlaySfx(audioController.diemLuu);
            UnlockNextLevel();
            LevelComplete();
        }
    }

    private void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce);
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER! Đầu đã chạm đất!");
        isGameEnded = true;
        DisablePlayerMovement();

        if (resultText != null)
            resultText.text = "GAME OVER";

        StartCoroutine(ShowFinishPanelAfterSave());
    }

    private void LevelComplete()
    {
        Debug.Log("LEVEL UP!");
        isGameEnded = true;
        DisablePlayerMovement();

        if (resultText != null)
            resultText.text = "LEVEL UP!";

        StartCoroutine(ShowFinishPanelAfterSave());
    }

    private void DisablePlayerMovement()
    {
        isTimerRunning = false;

        this.enabled = false;
        rb2d.linearVelocity = Vector2.zero;
        rb2d.angularVelocity = 0f;
        rb2d.isKinematic = true;
        boot.Stop();
    }

    private void UnlockNextLevel()
    {
        int nextLevelToUnlock = currentLevelIndex + 1;
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        if (nextLevelToUnlock > highestLevelUnlocked)
        {
            PlayerPrefs.SetInt("HighestLevelUnlocked", nextLevelToUnlock);
            PlayerPrefs.Save();
            Debug.Log("Đã mở khóa Level: " + nextLevelToUnlock);
        }
    }

    private void AddScore(int amount)
    {
        int currentScore = int.Parse(coinText.text);
        currentScore += amount;
        coinText.text = currentScore.ToString();
    }

    public void StartTangToc()
    {
        StopCoroutine(TangToc());
        StartCoroutine(TangToc());
    }

    private IEnumerator ShowFinishPanelAfterSave()
    {
        yield return new WaitForSeconds(0.6f);

        if (finishPanel != null)
            finishPanel.SetActive(true);

        // --- Cập nhật kết quả ---
        if (finalScoreText != null)
        {
            int score = int.Parse(coinText.text);
            finalScoreText.text = "Score: " + score.ToString();
        }

        if (finalTimeText != null)
        {
            float minutes = Mathf.FloorToInt(elapsedTime / 60);
            float seconds = Mathf.FloorToInt(elapsedTime % 60);
            float milliseconds = (elapsedTime % 1) * 100;
            finalTimeText.text = string.Format("Time: {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    public IEnumerator TangToc()
    {
        boot.Play();
        surfaceEffector.speed = 40;
        yield return new WaitForSeconds(5f);
        boot.Stop();
        surfaceEffector.speed = 20;
    }
}
