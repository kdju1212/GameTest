using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;   // 이동 속도
    public float jumpForce = 4f;   // 점프 힘
    private Rigidbody rb;
    private bool isGrounded;       // 바닥에 있는지 확인

    public Transform cameraTransform;  // 🎥 카메라 Transform 추가! (플레이어의 카메라는 카메라만 움직여야 하기 때문에 별도로 지정)

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody 가져오기
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");  // A, D 키
        float moveZ = Input.GetAxis("Vertical");    // W, S 키

        // 🎯 카메라가 보는 방향 기준으로 이동!
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0; // 위/아래 기울어짐 방지
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveZ + right * moveX;
        moveDirection.Normalize();

        // 🎮 이동 적용 (이동만 하고 회전은 하지 않음)
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // ⬆ 점프 기능
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  // 점프하면 공중 상태
        }
    }

    // 🛠 바닥에 닿았는지 체크 (바닥 감지)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // 바닥에 닿으면 다시 점프 가능
        }
    }
}
