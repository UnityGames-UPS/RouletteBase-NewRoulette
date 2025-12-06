using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RaceTrackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button[] tableButtons;
    [SerializeField] private List<GameObject> highlightAreas;
    private Button btn;
    private BetDefinition betDef;
    private BetManager betManager;


    private void Awake()
    {
        betDef = GetComponent<BetDefinition>();
        btn = GetComponent<Button>();
    }
    private void Start()
    {
        betManager = FindObjectOfType<BetManager>();
        if (btn != null)
            btn.onClick.AddListener(OnPressed);
    }

    internal void OnPressed()
    {
        Debug.Log("Button Pressed");
        foreach (string num in betDef.numbers)
        {
            foreach (var btn in tableButtons)
            {
                BetDefinition tableDef = btn.GetComponent<BetDefinition>();

                if (tableDef.numbers.Contains(num))
                {
                    betManager.PlaceBet(tableDef);
                }
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var area in highlightAreas)
        {
            area.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var area in highlightAreas)
        {
            area.SetActive(false);
        }
    }
}