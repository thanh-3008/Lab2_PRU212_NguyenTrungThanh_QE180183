using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource backGroundMusic;
    public AudioSource sfxMusic;

    // Các AudioClip này có thể được gán sẵn trong Inspector
    // và gọi từ các script khác mà không cần truyền trực tiếp file âm thanh.
    public AudioClip die;
    public AudioClip coin;
    public AudioClip diemLuu;
    public AudioClip music;
    public AudioClip boot;

    // --- PHIÊN BẢN CẢI TIẾN CỦA HÀM PlayMusic ---
    /// <summary>
    /// Phát một đoạn nhạc nền. Sẽ thay thế nhạc nền hiện tại.
    /// </summary>
    /// <param name="musicToPlay">AudioClip để phát.</param>

    public void Start()
    {
        PlayMusic(music);        
    }
    public void PlayMusic(AudioClip musicToPlay)
    {
        // 1. Kiểm tra xem có truyền vào clip hợp lệ không
        if (musicToPlay == null)
        {
            Debug.LogWarning("Không thể PlayMusic vì AudioClip là null.");
            return;
        }

        // 2. Gán clip mới cho AudioSource
        backGroundMusic.clip = musicToPlay;

        // 3. (Quan trọng) Bật vòng lặp cho nhạc nền
        backGroundMusic.loop = true;

        // 4. Phát nhạc
        backGroundMusic.Play();
    }

    /// <summary>
    /// Phát một hiệu ứng âm thanh (SFX) một lần.
    /// </summary>
    /// <param name="sfxToPlay">AudioClip của hiệu ứng.</param>
    public void PlaySfx(AudioClip sfxToPlay)
    {
        if (sfxToPlay == null)
        {
            Debug.LogWarning("Không thể PlaySfx vì AudioClip là null.");
            return;
        }
        // PlayOneShot là lựa chọn tốt nhất cho SFX vì nó không ngắt các âm thanh khác đang phát trên cùng AudioSource.
        sfxMusic.PlayOneShot(sfxToPlay);
    }

    // Ví dụ về cách gọi các hàm này từ một script khác:
    // AudioController audioController;
    // audioController.PlaySfx(audioController.coin);
}
