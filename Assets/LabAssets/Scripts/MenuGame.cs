using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGame : MonoBehaviour
{
    [Header("Level Settings (Chỉ dùng cho Pause Menu)")]
    public int currentLevelIndex;
    public int totalNumberOfLevels;

    [Header("UI References")]
    public GameObject nextLevelButton;
    public GameObject panelPause;
    public GameObject panelFinish;

    private bool isPause;

    /// <summary>
    /// OnEnable chạy khi panel được bật.
    /// Gọi hàm kiểm tra nút với delay nhỏ.
    /// </summary>
    void OnEnable()
    {
        // Invoke sẽ gọi hàm sau 0.1 giây
        Invoke(nameof(UpdateNextLevelButton), 0.1f);
    }

    void OnDisable()
    {
        // Hủy Invoke nếu panel bị tắt trước khi hàm được gọi
        CancelInvoke(nameof(UpdateNextLevelButton));
    }

    /// <summary>
    /// Kiểm tra và cập nhật trạng thái nút Next Level
    /// </summary>
    public void UpdateNextLevelButton()
    {
        if (nextLevelButton == null)
        {
            Debug.LogWarning("nextLevelButton chưa được gán trong Inspector!");
            return;
        }

        // Lấy level cao nhất đã mở khóa
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        // Debug để kiểm tra
        Debug.Log($"[MenuGame] Kiểm tra nút Next Level:");
        Debug.Log($"  - Current Level: {currentLevelIndex}");
        Debug.Log($"  - Highest Unlocked: {highestLevelUnlocked}");
        Debug.Log($"  - Total Levels: {totalNumberOfLevels}");

        // Điều kiện hiển thị nút:
        // 1. Level tiếp theo đã được mở khóa (highestLevelUnlocked > currentLevelIndex)
        // 2. Chưa phải level cuối cùng (currentLevelIndex < totalNumberOfLevels)
        if (highestLevelUnlocked > currentLevelIndex && currentLevelIndex < totalNumberOfLevels)
        {
            nextLevelButton.SetActive(true);
            Debug.Log("  ✓ Nút Next Level: HIỂN THỊ");
        }
        else
        {
            nextLevelButton.SetActive(false);
            Debug.Log("  ✗ Nút Next Level: ẨN");
        }
    }

    void Update()
    {
        // Xử lý phím ESC cho Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // ==================== CÁC HÀM MENU ====================

    public void PlayGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HowToPlay()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1.0f;
        string nextSceneName = "Level" + (currentLevelIndex + 1);
        Debug.Log("Đang tải scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    public void Resume()
    {
        if (panelPause != null)
        {
            panelPause.SetActive(false);
        }
        Time.timeScale = 1.0f;
        isPause = false;
    }

    public void Pause()
    {
        if (panelPause != null)
        {
            panelPause.SetActive(true);
        }
        Time.timeScale = 0f;
        isPause = true;
    }
}