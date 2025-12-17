using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChipSelector : MonoBehaviour
{
    [SerializeField] private List<ChipButton> chips = new List<ChipButton>();
    [SerializeField] private Image cursorChipImage;

    internal ChipButton selectedChip;

    private void Start()
    {
        foreach (var chip in chips)
        {
            chip.button.onClick.AddListener(() => OnChipSelected(chip));
        }

        // Default selected chip
        if (chips.Count > 0)
        {
            SelectChip(chips[2]);
        }

        Hide();
    }

    private void OnChipSelected(ChipButton chip)
    {
        SelectChip(chip);
    }

    private void SelectChip(ChipButton chip)
    {
        if (selectedChip != null)
            selectedChip.SetSelected(false);

        selectedChip = chip;
        selectedChip.SetSelected(true);

        cursorChipImage.sprite = chip.chipImage.sprite;
    }

    internal float GetSelectedChipValue()
    {
        return selectedChip != null ? selectedChip.value : 0f;
    }

    // 🔹 Called from NumberButton pointer events
    internal void ShowAt(Vector2 localPos)
    {
        cursorChipImage.gameObject.SetActive(true);
        cursorChipImage.rectTransform.anchoredPosition = localPos + Vector2.up * 10f;
    }

    internal void Hide()
    {
        cursorChipImage.gameObject.SetActive(false);
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
