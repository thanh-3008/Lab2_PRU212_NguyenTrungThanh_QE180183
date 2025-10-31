using UnityEngine;

/// <summary>
/// Script này điều khiển camera để nó luôn đi theo một đối tượng mục tiêu (target)
/// một cách mượt mà.
/// </summary>
public class CameraController : MonoBehaviour
{
    // Tham chiếu đến Transform của đối tượng mục tiêu (thường là người chơi)
    // Bạn sẽ gán đối tượng này vào trong Unity Editor.
    public Transform target;

    // Tốc độ làm mượt chuyển động của camera.
    // Giá trị càng nhỏ, camera di chuyển càng mượt và có độ trễ.
    // Giá trị càng lớn, camera bám theo càng sát.
    public float smoothSpeed = 0.125f;

    // Khoảng cách và góc lệch ban đầu giữa camera và mục tiêu.
    // Script sẽ tự động tính toán giá trị này.
    private Vector3 offset;

    // Hàm Start được gọi một lần khi script được kích hoạt.
    void Start()
    {
        // Kiểm tra xem đã gán target chưa
        if (target == null)
        {
            Debug.LogError("Chưa gán đối tượng Target cho CameraController!");
            return;
        }

        // Tính toán khoảng cách ban đầu từ camera đến mục tiêu.
        // Đây chính là khoảng cách mà camera sẽ luôn cố gắng duy trì.
        offset = transform.position - target.position;
    }

    // Hàm LateUpdate được gọi mỗi frame, sau khi tất cả các hàm Update đã được thực thi.
    // Dùng LateUpdate cho camera giúp tránh hiện tượng rung/giật, vì chúng ta cập nhật
    // vị trí camera sau khi mục tiêu đã hoàn thành di chuyển trong frame đó.
    void LateUpdate()
    {
        // Nếu không có target thì không làm gì cả
        if (target == null) return;

        // Tính toán vị trí mong muốn của camera trong frame này.
        // Vị trí này bằng vị trí của mục tiêu cộng với khoảng cách lệch ban đầu.
        Vector3 desiredPosition = target.position + offset;

        // Sử dụng hàm Lerp (Linear Interpolation) để di chuyển camera từ vị trí hiện tại
        // tới vị trí mong muốn một cách mượt mà.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Cập nhật vị trí mới cho camera.
        transform.position = smoothedPosition;
    }
}