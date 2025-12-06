using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ChipSelector : MonoBehaviour
{
    [SerializeField] private List<ChipButton> chips = new List<ChipButton>();
    internal ChipButton selectedChip;
    [SerializeField] private Image cursorChipImage;
    private bool chipSelected = false;


    private void Start()
    {
        foreach (var chip in chips)
        {
            chip.button.onClick.AddListener(() => OnChipSelected(chip));
        }
        selectedChip = chips[2];
        selectedChip.SetSelected(true);
        chipSelected = true;
        cursorChipImage.sprite = chips[2].chipImage.sprite;
    }

    private void OnChipSelected(ChipButton chip)
    {
        if (selectedChip != null)
            selectedChip.SetSelected(false);

        selectedChip = chip;
        selectedChip.SetSelected(true);

        chipSelected = true;

        cursorChipImage.sprite = chip.chipImage.sprite;

        Debug.Log("Selected Chip: " + GetSelectedChipValue());
    }


    internal float GetSelectedChipValue()
    {
        return selectedChip != null ? selectedChip.value : 0;
    }

    internal void ShowCursorChip()
    {
        if (chipSelected)
            cursorChipImage.gameObject.SetActive(true);
    }

    internal void HideCursorChip()
    {
        cursorChipImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cursorChipImage == null || !cursorChipImage.gameObject.activeSelf)
            return;

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorChipImage.canvas.transform as RectTransform,
            Input.mousePosition,
            cursorChipImage.canvas.worldCamera,
            out localPos
        );

        float offsetY = 10f; 
        cursorChipImage.rectTransform.anchoredPosition = localPos + new Vector2(0, offsetY);
    }

    internal ChipButton GetChipByValue(float value)
    {
        const float EPS = 0.0001f;

        foreach (var chip in chips)
        {
            if (Mathf.Abs(chip.value - value) <= EPS)
                return chip;
        }

        ChipButton best = null;
        float bestDiff = float.MaxValue;
        foreach (var chip in chips)
        {
            float diff = Mathf.Abs(chip.value - value);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = chip;
            }
        }

        if (best == null)
            Debug.LogError($"GetChipByValue: no chip buttons assigned in ChipSelector!");
        else
            Debug.LogWarning($"GetChipByValue: exact match not found for {value}, using closest {best.value}");

        return best;
    }

}
