using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChipScrollController : MonoBehaviour
{
    private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float scrollDuration = 0.2f;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private float chipWidth;
    private float spacing;
    private int currentIndex = 0;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        scrollRect.horizontal = false;
        scrollRect.vertical = false;

        HorizontalLayoutGroup layout = content.GetComponent<HorizontalLayoutGroup>();
        spacing = layout.spacing;

        chipWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width + spacing;

        leftButton.onClick.AddListener(ScrollLeft);
        rightButton.onClick.AddListener(ScrollRight);
    }

    private void ScrollLeft()
    {
        if (currentIndex <= 0) return;

        currentIndex--;
        SnapToChip();
    }

    private void ScrollRight()
    {
        int maxIndex = content.childCount - Mathf.FloorToInt(viewport.rect.width / chipWidth);
        if (currentIndex >= maxIndex) return;

        currentIndex++;
        SnapToChip();
    }

    private void SnapToChip()
    {
        float targetX = currentIndex * chipWidth;
        float maxScroll = content.rect.width - viewport.rect.width;

        float normalizedPos = Mathf.Clamp01(targetX / maxScroll);

        scrollRect.DONormalizedPos(new Vector2(normalizedPos, 0), scrollDuration).SetEase(Ease.OutCubic);

        Debug.Log("Snapped to chip index: " + currentIndex);
    }
}
