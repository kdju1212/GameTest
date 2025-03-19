using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public Transform exitPoint_A; // 문 앞쪽 출구
    public Transform exitPoint_B; // 문 뒤쪽 출구
    public Image progressBar;
    public TextMeshProUGUI interactText;

    private bool isHoldingE = false;
    private float holdTime = 1.5f;
    private float currentHoldTime = 0f;
    private Vector3 doorForward; // 문의 방향 저장

    void Start()
    {
        progressBar.fillAmount = 0f;
        progressBar.gameObject.SetActive(false);
        interactText.gameObject.SetActive(false);
        interactText.text = "Hold [E] for 1.5 seconds to open"; // 영어로 변경

        doorForward = transform.forward; // 문의 정면 방향 저장
    }

    void Update()
    {
        if (IsPlayerNearby() && IsLookingAtDoor()) // ✅ 플레이어가 문 근처에 있을 때만 실행
        {
            interactText.gameObject.SetActive(true); // UI 활성화

            if (Input.GetKey(KeyCode.E))
            {
                if (!isHoldingE)
                {
                    isHoldingE = true;
                    StartCoroutine(HoldToTeleport());
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                isHoldingE = false;
                currentHoldTime = 0f;
                progressBar.fillAmount = 0f;
                progressBar.gameObject.SetActive(false);
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }

    IEnumerator HoldToTeleport()
    {
        progressBar.gameObject.SetActive(true);

        while (isHoldingE && currentHoldTime < holdTime)
        {
            currentHoldTime += Time.deltaTime;
            progressBar.fillAmount = currentHoldTime / holdTime;
            yield return null;
        }

        if (currentHoldTime >= holdTime)
        {
            TeleportPlayer();
        }

        isHoldingE = false;
        currentHoldTime = 0f;
        progressBar.fillAmount = 0f;
        progressBar.gameObject.SetActive(false);
    }

    void TeleportPlayer()
    {
        Transform selectedExit = GetCorrectExit();

        if (selectedExit != null)
        {
            Quaternion originalRotation = player.rotation; // 🚀 플레이어의 원래 회전값 저장

            player.position = selectedExit.position; // 플레이어 위치 이동
            player.rotation = originalRotation; // 🚀 원래 방향 유지

            Debug.Log($"🚀 플레이어가 {selectedExit.name}에서 올바르게 텔레포트됨 (방향 유지)");

        }
        else
        {
            Debug.LogError("🚨 출구 위치가 설정되지 않음!");
        }

        // ✅ 문(Collider) 비활성화 후 일정 시간 후 다시 활성화
        Collider doorCollider = GetComponent<Collider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
            StartCoroutine(ReenableCollider(doorCollider));
        }
    }



    Transform GetCorrectExit()
    {
        // 플레이어가 문을 바라보는 방향 벡터 계산
        Vector3 playerToDoor = player.position - transform.position;
        float dot = Vector3.Dot(playerToDoor.normalized, doorForward);

        if (dot > 0) // 문 앞쪽에서 접근한 경우
        {
            return exitPoint_A;
        }
        else // 문 뒤쪽에서 접근한 경우
        {
            return exitPoint_B;
        }
    }

    IEnumerator ReenableCollider(Collider collider)
    {
        yield return new WaitForSeconds(1f); // 1초 후 다시 활성화
        collider.enabled = true;
    }

    bool IsLookingAtDoor()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // ✅ Ray를 시각적으로 확인
        Debug.DrawRay(ray.origin, ray.direction * 7f, Color.red, 0.1f);

        int doorLayerMask = LayerMask.GetMask("Door"); // ✅ "Door" 레이어만 감지하도록 설정

        if (Physics.Raycast(ray, out hit, 7f, doorLayerMask)) // ✅ 문 레이어만 감지
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    bool IsPlayerNearby()
    {
        return Vector3.Distance(player.position, transform.position) < 7f; // ✅ 문에서 7m 이내일 때만 true 반환
    }
}
