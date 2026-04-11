using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어의 물리적 이동, 점프, 회전을 담당하는 컨트롤러 스크립트
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;        // 기본 이동 속도
    [SerializeField] private float dashSpeed = 20f;   // 대쉬 이동 속도
    [SerializeField] private float dashDuration = 0.2f; // 대쉬 지속 시간

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform; // 위아래 회전을 제어할 카메라 (보통 Main Camera)
    [SerializeField] private float mouseSensitivity = 2f; // 마우스 회전 감도

    private Rigidbody rb;
    private bool isGround = true;  // 바닥인지 아닌지 확인 여부
    private bool isDashing = false; // 현재 대쉬 중인지 여부
    private float xRotation = 0f;  // 카메라 상하 회전 값 누적
    private Vector3 inputDir;      // 입력 받은 방향 벡터

    // 외부(SkillInput 등)에서 플레이어가 움직이고 있는지 확인하기 위한 프로퍼티
    public bool IsMoving => inputDir != Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 1. 키보드 입력 처리 (WASD / 방향키)
        float vx = Input.GetAxisRaw("Horizontal");
        float vz = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(vx, 0, vz).normalized;

        // 2. 점프 처리 (바닥에 닿아있을 때만 가능)
        if (isGround)
        {
            Jump();
        }

        // 3. 상하좌우 마우스 회전 처리
        Rotate();
    }

    private void FixedUpdate()
    {
        // 대쉬 중이 아닐 때만 일반 이동 물리 로직 실행
        if (!isDashing)
        {
            Move();
        }
    }

    /// <summary>
    /// 입력 방향(inputDir)을 플레이어 정면 기준으로 변환하여 이동 처리(로컬)
    /// </summary>
    public void Move()
    {
        // 로컬 좌표 기준의 입력 방향을 월드 좌표 방향으로 변환
        Vector3 moveDir = transform.TransformDirection(inputDir);
        Vector3 movement = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        rb.linearVelocity = movement;
    }

    /// <summary>
    /// 플레이어의 좌우 회전(몸체) 및 상하 회전(카메라) 처리
    /// </summary>
    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 몸체 좌우 회전 (Y축 기준 회전)
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 상하 회전 (X축 기준 회전, -45 ~ 45도로 각도 제한)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    /// <summary>
    /// 스페이스바 입력 시 위쪽 방향으로 힘을 가해 점프
    /// </summary>
    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 대쉬 기능을 실행 (SkillInput에서 호출됨)
    /// </summary>
    public void Dash()
    {
        if (!isDashing && IsMoving)
        {
            StartCoroutine(DashRoutine());
        }
    }

    /// <summary>
    /// 일정 시간 동안 대쉬하는 코루틴
    /// </summary>
    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector3 dashDir = transform.TransformDirection(inputDir);
        rb.linearVelocity = dashDir * dashSpeed + new Vector3(0, rb.linearVelocity.y, 0);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Floor 태그가 붙은 물체와 충돌 시 점프 가능 상태로 변경
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGround = true;
        }
    }
}
