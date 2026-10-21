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
    // [SerializeField] private int NeighborNumber;
    [SerializeField] private GameObject[] raceTrackBlurrImage;


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
        NeighborBlurrImage();
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

private void NeighborBlurrImage()
{
    for (int i = 0; i < betDef.numbers.Count; i++)
    {
        string num = betDef.numbers[i];
        int n;

        if (num == "00")
        {
            n = 37;
        }
        else if (!int.TryParse(num, out n))
        {
            continue;
        }
        if (n < 0 || n >= raceTrackBlurrImage.Length)
        {
            continue;
        }

        highlightAreas.Add(raceTrackBlurrImage[n]);
    }
}

}