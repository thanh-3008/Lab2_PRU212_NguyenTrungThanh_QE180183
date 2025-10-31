using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo tất cả các Button Level vào đây theo thứ tự")]
    public Button[] levelButtons;

    [Tooltip("Kéo tất cả các Icon Khóa vào đây theo thứ tự")]
    public GameObject[] lockIcons;

    void Start()
    {
        UpdateLevelButtons();
    }

    void Update()
    {
        // Phím R để reset tiến trình (chỉ dùng khi test)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProgress();
        }
    }

    /// <summary>
    /// Cập nhật trạng thái tất cả các nút level
    /// </summary>
    public void UpdateLevelButtons()
    {
        // Lấy level cao nhất đã mở khóa
        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

        Debug.Log($"[LevelManagement] Highest Level Unlocked: {highestLevelUnlocked}");

        // Duyệt qua tất cả các nút
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Level của nút này (mảng bắt đầu từ 0, level bắt đầu từ 1)
            int buttonLevelIndex = i + 1;

            if (buttonLevelIndex > highestLevelUnlocked)
            {
                // ===== LEVEL BỊ KHÓA =====
                levelButtons[i].interactable = false;

                if (lockIcons[i] != null)
                {
                    lockIcons[i].SetActive(true);
                }

                Debug.Log($"  Level {buttonLevelIndex}: LOCKED");
            }
            else
            {
                // ===== LEVEL ĐÃ MỞ =====
                levelButtons[i].interactable = true;

                if (lockIcons[i] != null)
                {
                    lockIcons[i].SetActive(false);
                }

                // Xóa tất cả listener cũ để tránh trùng lặp
                levelButtons[i].onClick.RemoveAllListeners();

                // Thêm sự kiện OnClick
                int levelToLoad = buttonLevelIndex; // Biến tạm để tránh lỗi closure
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelToLoad));

                Debug.Log($"  Level {buttonLevelIndex}: UNLOCKED");
            }
        }
    }

    /// <summary>
    /// Tải scene level được chọn
    /// </summary>
    void LoadLevel(int level)
    {
        string sceneName = "Level" + level;
        Debug.Log($"Đang tải scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Reset toàn bộ tiến trình game (chỉ dùng khi test)
    /// </summary>
    public void ResetProgress()
    {
        Debug.Log("RESET PROGRESS - Xóa tất cả PlayerPrefs!");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Tải lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}