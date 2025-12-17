using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NumberButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private Button btn;
    [SerializeField] private List<GameObject> highlightAreas;

    private BetDefinition betDef;
    private BetManager betManager;
    private ChipSelector chipSelector;
    private RouletteController rouletteController;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;

        betDef = GetComponent<BetDefinition>();
        betManager = FindObjectOfType<BetManager>();
        chipSelector = FindObjectOfType<ChipSelector>();
        rouletteController = FindObjectOfType<RouletteController>();

        if (btn != null)
            btn.onClick.AddListener(() => { });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rouletteController.isSpinOn) return;

        ShowHighlight();
        UpdateCursorPosition(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (rouletteController.isSpinOn) return;

        UpdateCursorPosition(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (rouletteController.isSpinOn) return;

        betManager.PlaceBet(betDef);

        // 🔥 IMPORTANT: hide cursor chip on tap end (fixes mobile stuck issue)
        chipSelector.Hide();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rouletteController.isSpinOn) return;

        HideHighlight();
        chipSelector.Hide();
    }

    private void UpdateCursorPosition(PointerEventData eventData)
    {
        RectTransform parentRect = rectTransform.parent as RectTransform;

        if (parentRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos
        );

        chipSelector.ShowAt(localPos);
    }

    private void ShowHighlight()
    {
        foreach (var area in highlightAreas)
            area.SetActive(true);
    }

    private void HideHighlight()
    {
        foreach (var area in highlightAreas)
            area.SetActive(false);
    }
}
