using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class RouletteController : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    private Transform CoinContainer_Transform;

    [Header("Lists and Arrays")]
    [SerializeField]
    private GameObject[] Coins_Prefab;
    [SerializeField]
    private GameObject[] activeCoins_Object;
    [SerializeField]
    private List<GameObject> instantiated_Coins;
    [SerializeField] private Button spin_Button;

    [Header("Integers")]
    [SerializeField]
    private int CoinCounter = 0;
    [SerializeField]
    private int[] amount_array;
    [SerializeField]
    private int totalBet = 0;

    [SerializeField]
    private TMP_Text Totalbet_Text;
    [SerializeField]
    private Button cancelBet_Button;

    [Header("Managers")]
    [SerializeField] private AudioController audioController;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private RouletteSpinController rouletteSpinController;
    [SerializeField] private UIManager uiManager;
    private List<BetPlacement> betPlacement = new List<BetPlacement>
    {
        // new BetPlacement { type = "red", amount = 10 },
        // new BetPlacement { type = "column_1", amount = 5 },
        // new BetPlacement { type = "dozen_2", amount = 20 },
        // new BetPlacement { type = "high", amount = 15 },
        // new BetPlacement
        // {
        // type = "straight_up",
        // amount = 50,
        // numbers = new List<object> { 1 }
        // },
        // new BetPlacement
        // {
        // type = "straight_up",
        // amount = 50,
        // numbers = new List<object> { "00" }
        // }
        // {"type": "SPIN","payload": {"betPlacements": [{"type": "straight_up","numbers": [1,9],"amount": 1}],[{"type":"straight_up","numbers":[10],"amount":5}]}}
        new BetPlacement
        {
            type = "straight_up",
            amount = 1,
            numbers = new List<object> { 1 }
        },
        // new BetPlacement
        // {
        //     type = "straight_up",
        //     amount = 5,
        //     numbers = new List<object> { 10 }
        // },
        // new BetPlacement { type = "red", amount = 10 },
        // new BetPlacement { type = "split", amount = 5,numbers = new List<object> { 3,00 } },

    };


    private void Start()
    {
        if (activeCoins_Object[CoinCounter]) activeCoins_Object[CoinCounter].SetActive(true);
        if (cancelBet_Button) cancelBet_Button.onClick.RemoveAllListeners();
        if (cancelBet_Button) cancelBet_Button.onClick.AddListener(CancelBet);
        if (spin_Button) spin_Button.onClick.RemoveAllListeners();
        if (spin_Button) spin_Button.onClick.AddListener(delegate { StartSpinning(); });
    }

    internal void CancelBet()
    {
        foreach (GameObject coin in instantiated_Coins)
        {
            Destroy(coin);
        }
        instantiated_Coins.Clear();
        instantiated_Coins.TrimExcess();
        if (Totalbet_Text) Totalbet_Text.text = "your bet amount: $00";
        Canvas.ForceUpdateCanvases();
        totalBet = 0;
    }

    // internal void SelectCoin(GameObject activeObject, int counter)
    // {
    //     audioController.PlayButtonAudio();
    //     foreach (GameObject objs in activeCoins_Object)
    //     {
    //         objs.SetActive(false);
    //     }
    //     activeObject.SetActive(true);
    //     CoinCounter = counter;
    // }

    internal void BetOnButton(Transform parent, string code = null)
    {
        audioController.PlayWLAudio("chip", false);
        GameObject coin = Instantiate(Coins_Prefab[CoinCounter], CoinContainer_Transform);
        coin.transform.SetParent(parent);
        coin.transform.DOLocalMove(new Vector2(0, 0), 0.5f);
        instantiated_Coins.Add(coin);
        totalBet += amount_array[CoinCounter];
        if (Totalbet_Text) Totalbet_Text.text = "your bet amount: $" + totalBet;
        Canvas.ForceUpdateCanvases();
    }

    private void StartSpinning()
    {
        audioController.PlayWLAudio("spin");
        StartCoroutine(TweenRoutine());
    }

    private IEnumerator TweenRoutine()
    {
        uiManager.ToggleButtons(false);
        Debug.Log("Spinning Started");
        Debug.Log("Sending Data");
        socketManager.AccumulateResult(betPlacement);
        yield return new WaitUntil(() => socketManager.isResultdone);
        uiManager.StartCoroutine(uiManager.UpdateResultUI());
        yield return new WaitForSeconds(2f);
        rouletteSpinController.Spin(socketManager.resultData.payload.winningNumber.ToString());
        yield return new WaitForSeconds(8f);
        uiManager.ShowWinning(socketManager.resultData.payload.color, socketManager.resultData.payload.winningNumber);
        yield return new WaitUntil(() => !uiManager.isWinPopupActive);
        uiManager.UpdateWinNumberHistoryUI(socketManager.resultData.payload.color, socketManager.resultData.payload.winningNumber);
        uiManager.ToggleButtons(true);
    }

}
