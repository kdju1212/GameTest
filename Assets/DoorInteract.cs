using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public Transform exitPoint_A; // 문 앞쪽 출구
    public Transform exitPoint_B; // 문 뒤쪽 출구

    public static Image progressBarFill;
    public static GameObject progressBarBackground;
    public static TextMeshProUGUI interactText;
    public static Transform canvasTransform;

    private static DoorInteract currentDoor = null;

    private bool isHoldingE = false;
    private float holdTime = 1.5f;
    private float currentHoldTime = 0f;
    private Vector3 doorForward;

    void Start()
    {
        doorForward = transform.forward;

        GameObject canvasObj = GameObject.Find("DoorCanvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;

            interactText = canvasTransform.Find("InteractText")?.GetComponent<TextMeshProUGUI>();

            Transform background = canvasTransform.Find("ProgressBarBackground");
            if (background != null)
            {
                progressBarBackground = background.gameObject;
                progressBarFill = background.Find("ProgressBarFill")?.GetComponent<Image>();
            }
        }
        else
        {
            Debug.LogError("🚨 DoorCanvas를 찾을 수 없습니다!");
        }

        // UI 초기화
        ResetAllUI();
    }

    void Update()
    {
        if (progressBarFill == null || interactText == null || canvasTransform == null || progressBarBackground == null)
            return;

        bool isNearby = IsPlayerNearby();
        bool isLooking = IsLookingAtDoor();

        if (isNearby && isLooking)
        {
            if (currentDoor != this)
            {
                currentDoor = this;
                MoveCanvasToOppositeExit();
                interactText.gameObject.SetActive(true);
                progressBarBackground.SetActive(false);
                progressBarFill.gameObject.SetActive(false);
            }

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
                ResetProgressBar(); // 🔄 게이지만 꺼짐 (Text는 유지)
            }
        }
        else if (currentDoor == this && (!isNearby || !isLooking))
        {
            ResetAllUI();           // 🧹 전체 UI 꺼짐
            currentDoor = null;
        }
    }

    IEnumerator HoldToTeleport()
    {
        progressBarBackground.SetActive(true);
        progressBarFill.gameObject.SetActive(true);

        while (isHoldingE && currentHoldTime < holdTime)
        {
            currentHoldTime += Time.deltaTime;
            progressBarFill.fillAmount = currentHoldTime / holdTime;
            yield return null;
        }

        if (currentHoldTime >= holdTime)
        {
            TeleportPlayer();
        }

        ResetProgressBar(); // ✅ 게이지만 꺼짐
    }

    void TeleportPlayer()
    {
        Transform selectedExit = GetCorrectExit();
        if (selectedExit != null)
        {
            player.position = selectedExit.position;
            Debug.Log($"🚪 {gameObject.name} 통해 {selectedExit.name}로 텔레포트됨!");
        }
        else
        {
            Debug.LogError("🚨 출구 위치가 설정되지 않음!");
        }
    }

    void MoveCanvasToOppositeExit()
    {
        Transform selectedExit = GetCorrectExit();
        Transform oppositeExit = (selectedExit == exitPoint_A) ? exitPoint_B : exitPoint_A;

        if (oppositeExit != null)
        {
            canvasTransform.position = oppositeExit.position + new Vector3(0, 1.5f, 0);
        }

        Vector3 lookDir = player.position - canvasTransform.position;
        lookDir.y = 0;
        canvasTransform.rotation = Quaternion.LookRotation(-lookDir);
    }

    Transform GetCorrectExit()
    {
        Vector3 playerToDoor = player.position - transform.position;
        float dot = Vector3.Dot(playerToDoor.normalized, doorForward);
        return dot > 0 ? exitPoint_A : exitPoint_B;
    }

    bool IsLookingAtDoor()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Door");

        if (Physics.Raycast(ray, out hit, 7f, layerMask))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    bool IsPlayerNearby()
    {
        return Vector3.Distance(player.position, transform.position) < 7f;
    }

    // ✅ 게이지만 초기화
    void ResetProgressBar()
    {
        isHoldingE = false;
        currentHoldTime = 0f;
        progressBarFill.fillAmount = 0f;
        progressBarFill.gameObject.SetActive(false);
        progressBarBackground.SetActive(false);
    }

    // ✅ 전체 UI 초기화
    void ResetAllUI()
    {
        ResetProgressBar();
        if (interactText != null) interactText.gameObject.SetActive(false);
    }
}
// 03-24 git 등록 테스트