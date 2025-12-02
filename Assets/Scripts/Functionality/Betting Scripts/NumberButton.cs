using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NumberButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // [SerializeField] private int numberValue;
    // [SerializeField] private RectTransform chipAnchor;
    private BetDefinition betDef; // ← assign in inspector
    [SerializeField] private Button btn;
    [SerializeField] private List<GameObject> highlightAreas;

    private BetManager betManager;
    private ChipSelector chipSelector;

    private void Awake()
    {
        betManager = FindObjectOfType<BetManager>();
        chipSelector = FindObjectOfType<ChipSelector>();
        betDef = GetComponent<BetDefinition>();

        if (btn != null)
            btn.onClick.AddListener(OnPressed);
    }

    // private void OnPressed()
    // {
    //     betManager.PlaceStraightUpBet(numberValue, chipAnchor);
    // }

    public void OnPressed()
    {
        betManager.PlaceBet(betDef);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        chipSelector.ShowCursorChip();
        foreach (var area in highlightAreas)
        {
            area.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chipSelector.HideCursorChip();
        foreach (var area in highlightAreas)
        {
            area.SetActive(false);
        }
    }
}

