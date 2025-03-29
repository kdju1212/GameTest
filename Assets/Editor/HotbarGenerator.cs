#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HotbarGenerator : MonoBehaviour
{
    [MenuItem("GameObject/UI/Hotbar Generator", false, 10)]
    static void GenerateHotbar()
    {
        // 🔷 Canvas 만들기
        GameObject canvasGO = new GameObject("HotbarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 🔶 Panel 만들기
        GameObject panelGO = new GameObject("HotbarPanel", typeof(RectTransform), typeof(Image));
        panelGO.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0); // 하단 중앙
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.pivot = new Vector2(0.5f, 0);
        panelRect.anchoredPosition = new Vector2(0, 50);
        panelRect.sizeDelta = new Vector2(600, 80);

        // 🔳 슬롯 8개 생성
        for (int i = 0; i < 8; i++)
        {
            GameObject slotGO = new GameObject($"Slot_{i + 1}", typeof(RectTransform), typeof(Image));
            slotGO.transform.SetParent(panelGO.transform, false);

            RectTransform slotRect = slotGO.GetComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(60, 60);
            slotRect.anchoredPosition = new Vector2(i * 65 - 227.5f, 0); // 8칸에 맞춰 조정

            Image img = slotGO.GetComponent<Image>();
            img.color = new Color(0.8f, 0.8f, 0.8f, 0.6f); // 회색 반투명
        }

        // 🔶 선택 테두리 추가
        GameObject selector = new GameObject("SlotSelector", typeof(RectTransform), typeof(Image));
        selector.transform.SetParent(panelGO.transform, false);
        RectTransform selRect = selector.GetComponent<RectTransform>();
        selRect.sizeDelta = new Vector2(70, 70);
        selRect.anchoredPosition = new Vector2(-227.5f, 0); // 첫 번째 슬롯 위치로 조정

        Image selImage = selector.GetComponent<Image>();
        selImage.color = Color.yellow;
        selImage.raycastTarget = false;

        Debug.Log("✅ Hotbar UI 생성 완료 (8개 슬롯)!");
        Selection.activeGameObject = canvasGO;
    }
}
#endif
