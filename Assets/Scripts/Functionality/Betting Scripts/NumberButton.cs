using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NumberButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BetDefinition betDef;
    [SerializeField] private Button btn;
    [SerializeField] private List<GameObject> highlightAreas;

    private BetManager betManager;
    private ChipSelector chipSelector;
    private RouletteController rouletteController;

    private void Awake()
    {
        betManager = FindObjectOfType<BetManager>();
        rouletteController = FindObjectOfType<RouletteController>();
        chipSelector = FindObjectOfType<ChipSelector>();
        betDef = GetComponent<BetDefinition>();

        if (btn != null)
            btn.onClick.AddListener(OnPressed);
    }

    private void OnPressed()
    {
        betManager.PlaceBet(betDef);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!rouletteController.isSpinOn)
        {
            // chipSelector.ShowCursorChip();
            foreach (var area in highlightAreas)
            {
                area.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!rouletteController.isSpinOn)
        {
            // chipSelector.HideCursorChip();
            foreach (var area in highlightAreas)
            {
                area.SetActive(false);
            }
        }
    }
}

