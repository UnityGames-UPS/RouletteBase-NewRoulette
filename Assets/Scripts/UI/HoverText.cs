using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] internal string hoverKey;
    // private string hoverMessage;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TextMeshProUGUI hoverTextDisplay;
    private Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverTextDisplay != null)
        {
            hoverTextDisplay.text = uiManager.GetHoverText(hoverKey);
            hoverTextDisplay.gameObject.SetActive(true);
        }
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if (hoverTextDisplay != null)
    //     {
    //         hoverTextDisplay.gameObject.SetActive(false);
    //         hoverTextDisplay.text = uiManager.GetHoverText(hoverKey);
    //         hoverTextDisplay.gameObject.SetActive(true);
    //     }
    // }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverTextDisplay != null)
        {
            hoverTextDisplay.text = "";
            hoverTextDisplay.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
            //    Invoke("RefreshHoverText", 1f);
            RefreshHoverText();
            });
        }
    }

    private void RefreshHoverText()
    {
        Debug.Log("Refreshing Hover Text");
        if (hoverTextDisplay != null)
        {
            hoverTextDisplay.gameObject.SetActive(false);
            hoverTextDisplay.text = uiManager.GetHoverText(hoverKey);
            hoverTextDisplay.gameObject.SetActive(true);
        }
    }
}