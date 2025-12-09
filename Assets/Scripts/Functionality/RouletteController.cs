using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class RouletteController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button spin_Button;
    [SerializeField] private Button Turbo_Button;

    [Header("Managers")]
    [SerializeField] private AudioController audioController;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private SpinManager spinManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BetManager betManager;
    [SerializeField] private BallScript BallManager;
    private List<BetPlacement> bet = new List<BetPlacement> { };
    internal float betCount = 0f;
    internal bool isSpinOn = false;

    // private List<BetPlacement> betPlacement = new List<BetPlacement>
    // {
    //     // new BetPlacement { type = "red", amount = 10 },
    //     // new BetPlacement { type = "column_1", amount = 5 },
    //     // new BetPlacement { type = "dozen_2", amount = 20 },
    //     // new BetPlacement { type = "high", amount = 15 },
    //     // new BetPlacement
    //     // {
    //     // type = "straight_up",
    //     // amount = 50,
    //     // numbers = new List<object> { 1 }
    //     // },
    //     // new BetPlacement
    //     // {
    //     // type = "straight_up",
    //     // amount = 50,
    //     // numbers = new List<object> { "00" }
    //     // }
    //     // {"type": "SPIN","payload": {"betPlacements": [{"type": "straight_up","numbers": [1,9],"amount": 1}],[{"type":"straight_up","numbers":[10],"amount":5}]}}
    //     new BetPlacement
    //     {
    //         type = "straight_up",
    //         amount = 1,
    //         numbers = new List<object> { 1 }
    //     },
    //     // new BetPlacement
    //     // {
    //     //     type = "straight_up",
    //     //     amount = 5,
    //     //     numbers = new List<object> { 10 }
    //     // },
    //     // new BetPlacement { type = "red", amount = 10 },
    //     // new BetPlacement { type = "split", amount = 5,numbers = new List<object> { 3,00 } },

    // };


    private void Start()
    {
        if (spin_Button) spin_Button.onClick.RemoveAllListeners();
        if (spin_Button) spin_Button.onClick.AddListener(delegate { StartSpinning(); });
        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if (Turbo_Button) Turbo_Button.onClick.AddListener(delegate { StartSpinning(); });
    }

    private void StartSpinning()
    {
        audioController.PlaySpinButton();
        uiManager.autoSpinCount = 1;
        StartCoroutine(TweenRoutine());
    }

    internal IEnumerator TweenRoutine()
    {
        if (betCount > socketManager.playerdata.balance)
        {
            uiManager.LowBalPopup();
            yield break;
        }
        if (betCount <= 0)
        {
            uiManager.EmptyBetPopup();
            yield break;
        }
        bet = betManager.betPlacement;
        uiManager.RaceTrackPopup_Object.SetActive(false);
        uiManager.StatisticsPopup_Object.SetActive(false);
        while (uiManager.autoSpinCount > 0)
        {
            isSpinOn = true;
            uiManager.winNumberAnimation = false;
            uiManager.ToggleButtons(false);
            uiManager.Closepopups();
            Debug.Log("Spinning Started");
            Debug.Log("Sending Data");
            socketManager.AccumulateResult(bet);
            Debug.Log(bet);
            yield return new WaitUntil(() => socketManager.isResultdone);
            UpdateBalance();
            uiManager.StartCoroutine(uiManager.UpdateResultUI());
            if (uiManager.turboSpin == false)
            {
                yield return new WaitForSeconds(0.5f);
                spinManager.StartSpinning(socketManager.resultData.payload.winningNumber);
                yield return new WaitUntil(() => BallManager.ballStopped);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
            // betManager.PlayWinningBetAnimation(socketManager.resultData.payload.winningBets);
            uiManager.ShowWinning(socketManager.resultData.payload.color, socketManager.resultData.payload.winningNumber, socketManager.resultData.payload.winAmount);
            yield return new WaitUntil(() => !uiManager.isWinPopupActive);
            Debug.Log("Animation Start");
            uiManager.StartWinNummberAnimation(socketManager.resultData.payload.winningNumber);
            uiManager.UpdateWinNumberHistoryUI(socketManager.resultData.payload.color, socketManager.resultData.payload.winningNumber);
            uiManager.UpdateHotColdNumbers(socketManager.resultData.payload.hot_numbers, socketManager.resultData.payload.cold_numbers);
            uiManager.ToggleButtons(true);
            socketManager.isResultdone = false;
            uiManager.autoSpinCount--;
            uiManager.AutoSpintCountText.text = uiManager.autoSpinCount.ToString();
            if (uiManager.autoSpinCount > 0)
            {
                betManager.AutoBetComplete();
            }
            else
            {
                betManager.OnRoundComplete();
                isSpinOn = false;
                if (uiManager.AutoSpinPopup2_Object.activeSelf)
                {
                    uiManager.AutoSpinPopup2_Object.SetActive(false);
                    uiManager.AutoSpinPopup_Object.SetActive(true);
                }
            }
        }
    }

    internal void UpdateBet()
    {
        uiManager.totalBetText.text = betCount.ToString();
    }

    internal void UpdateBalance()
    {
        double currBal = socketManager.playerdata.balance - betCount;
        uiManager.balanceText.text = currBal.ToString();
    }

}
