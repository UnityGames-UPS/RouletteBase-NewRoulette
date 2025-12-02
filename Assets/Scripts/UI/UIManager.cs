using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Transforms")]
    // [SerializeField]
    // private Transform Timer_Transform;
    [SerializeField] private Transform Roulette_NumberCircle;
    // [SerializeField]
    // private Transform Roulette_InnerCircle;
    [SerializeField] private Transform Roulette_BallContainer;
    [SerializeField] private Transform Ball_Transform;

    [Header("TMP_Texts")]
    // [SerializeField]
    // private TMP_Text Timer_Text;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text totalBetText;
    [SerializeField] private TMP_Text totalWiningText;
    [SerializeField] private TMP_Text RedWinNumberHistoryText;
    [SerializeField] private TMP_Text GreenWinNumberHistoryText;
    [SerializeField] private TMP_Text BlackWinNumberHistoryText;

    [Header("Buttons")]
    // [SerializeField]
    // private Button GameExit_Button;
    [SerializeField] private Button DoubleBetButton;
    [SerializeField] private Button UndoBetButton;
    [SerializeField] private Button ClearBetsButton;
    [SerializeField] private Button RebetFromLastBetButton;
    [SerializeField] private Button PaytableButton;
    [SerializeField] private Button PaytableInsideButton;
    [SerializeField] private Button PaytableCloseButton;
    [SerializeField] private Button RaceTrackButton;
    [SerializeField] private Button StatisticsButton;
    [SerializeField] private Button FavouriteBetButton;
    [SerializeField] private List<Button> DisableButtons_DuringSpin;

    [Header("GameObjects")]
    // [SerializeField]
    // private GameObject StartBettingPopup;
    // [SerializeField]
    // private GameObject StopBettingPopup;
    // [SerializeField]
    // private GameObject SpinPanel_Object;
    [SerializeField] private GameObject MainPopup_Object;
    [SerializeField] private GameObject PaytablePopup_Object;
    [SerializeField] private GameObject RaceTrackPopup_Object;
    [SerializeField] private GameObject StatisticsPopup_Object;
    [SerializeField] private GameObject FavouriteBetPopup_Object;
    // [SerializeField]
    // private GameObject winNumber_Object;

    [Header("Win Popup")]
    [SerializeField] private GameObject WinNumberPopup_Object;
    [SerializeField] private GameObject RedWinNumberPopup_Object;
    [SerializeField] private GameObject BlackWinNumberPopup_Object;
    [SerializeField] private GameObject GreenWinNumberPopup_Object;
    [SerializeField] private TMP_Text WinNumberPopup_Text;
    internal bool isWinPopupActive = false;

    [Header("Lists & Arrays")]
    [SerializeField] private Transform[] BallStopPoint;
    [SerializeField] private List<string> NumberCode;
    [SerializeField] private List<int> PreviousNumbers;
    [SerializeField] private List<TMP_Text> Previous_Text;
    [SerializeField] private List<Image> Previous_Image;

    [Header("Colors")]
    [SerializeField] private Color blackColor;
    [SerializeField] private Color redColor;
    [SerializeField] private Color greenColor;

    [Header("Managers")]
    [SerializeField] private RouletteController rouletteManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioController;
    [SerializeField] private BallScript BallManager;
    [SerializeField] private BetManager betManager;

    [Header("Other Stuff")]
    [SerializeField] private int Timer = 30;
    [SerializeField] private int numberAnnounced = 0;
    [SerializeField] private Vector3 initialBallPosition;
    [SerializeField] private Image winNumber_Image;

    private Tweener ballMovement = null;
    private Tweener OuterRouletteMovement = null;
    private Tweener InnerRouletteMovement = null;

    [SerializeField] private GameObject Stopper_pref;
    private GameObject Stopper;

    private void Awake()
    {
        initialBallPosition = Ball_Transform.localPosition;
    }

    private void Start()
    {
        // if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        // if (GameExit_Button) GameExit_Button.onClick.AddListener(CallOnExitFunction);
        // Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
        if (DoubleBetButton) DoubleBetButton.onClick.RemoveAllListeners();
        if (DoubleBetButton) DoubleBetButton.onClick.AddListener(() => betManager.DoubleBet());
        if (UndoBetButton) UndoBetButton.onClick.RemoveAllListeners();
        if (UndoBetButton) UndoBetButton.onClick.AddListener(() => betManager.UndoLastBet());
        if (ClearBetsButton) ClearBetsButton.onClick.RemoveAllListeners();
        if (ClearBetsButton) ClearBetsButton.onClick.AddListener(() => betManager.ClearAllBets());
        if (RebetFromLastBetButton) RebetFromLastBetButton.onClick.RemoveAllListeners();
        if (RebetFromLastBetButton) RebetFromLastBetButton.onClick.AddListener(() => betManager.Rebet());
        if (PaytableButton) PaytableButton.onClick.RemoveAllListeners();
        if (PaytableButton) PaytableButton.onClick.AddListener(() => TogglePopup(PaytablePopup_Object));
        if (PaytableInsideButton) PaytableInsideButton.onClick.RemoveAllListeners();
        if (PaytableInsideButton) PaytableInsideButton.onClick.AddListener(() => TogglePopup(PaytablePopup_Object));
        if (PaytableCloseButton) PaytableCloseButton.onClick.RemoveAllListeners();
        if (PaytableCloseButton) PaytableCloseButton.onClick.AddListener(() => ClosePopup(PaytablePopup_Object));
        if (RaceTrackButton) RaceTrackButton.onClick.RemoveAllListeners();
        if (RaceTrackButton) RaceTrackButton.onClick.AddListener(() => TogglePopup(RaceTrackPopup_Object));
        if (StatisticsButton) StatisticsButton.onClick.RemoveAllListeners();
        if (StatisticsButton) StatisticsButton.onClick.AddListener(() => TogglePopup(StatisticsPopup_Object));
        if (FavouriteBetButton) FavouriteBetButton.onClick.RemoveAllListeners();
        if (FavouriteBetButton) FavouriteBetButton.onClick.AddListener(() => TogglePopup(FavouriteBetPopup_Object));
    }


    // private void CallOnExitFunction()
    // {
    //     Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    // }



    private void TogglePopup(GameObject popup)
    {
        audioController.PlayButtonAudio();
        if (popup.activeSelf)
        {
            popup.SetActive(false);
            // MainPopup_Object.SetActive(false);
            return;
        }
        if (!popup.activeSelf)
        {
            popup.SetActive(true);
            // MainPopup_Object.SetActive(true);
            return;
        }
    }

    private void ClosePopup(GameObject popup)
    {
        audioController.PlayButtonAudio();
        popup.SetActive(false);
    }
    internal string GetHoverText(string key)
    {
        switch (key)
        {
            case "paytable":
                if (PaytablePopup_Object.activeSelf)
                {
                    return "Close paytable";
                }
                return "Open paytable";
            case "racetrack":
                if (RaceTrackPopup_Object.activeSelf)
                {
                    return "Close racetrack";
                }
                return "Open racetrack";
            case "statistics":
                if (StatisticsPopup_Object.activeSelf)
                {
                    return "Close statistics";
                }
                return "Open statistics";
            case "favouriteBet":
                if (FavouriteBetPopup_Object.activeSelf)
                {
                    return "Close favourite bets";
                }
                return "Open favourite bets";
            case "doublebet":
                return "Double all bets";
            case "undobet":
                return "Undo last bet";
            case "clearbets":
                return "Clear all bets";
            case "rebetfromlastbet":
                return "Rebet from previous round";
            default:
                return "";
        }
    }
    internal void InitializeUIData()
    {
        balanceText.text = socketManager.playerdata.balance.ToString("F2");
    }
    internal IEnumerator UpdateResultUI()
    {
        totalBetText.text = socketManager.resultData.payload.totalBetAmount.ToString("F2");
        yield return new WaitForSeconds(1f);
        UpdateUI();
        Debug.Log("UI Updated");
    }
    private void UpdateBetUI(int totalBet)
    {
        totalBetText.text = totalBet.ToString("F2");
    }
    private void UpdateUI()
    {
        balanceText.text = socketManager.resultData.player.balance.ToString("F2");
        totalWiningText.text = socketManager.resultData.payload.winAmount.ToString("F2");
    }
    internal void UpdateWinNumberHistoryUI(string color, int number)
    {
        string num = number.ToString();

        string blank = "";
        string redLine = blank;
        string greenLine = blank;
        string blackLine = blank;

        switch (color.ToLower())
        {
            case "red":
                redLine = num;
                break;
            case "green":
                greenLine = num;
                break;
            case "black":
                blackLine = num;
                break;
        }

        RedWinNumberHistoryText.text = redLine + "\n" + RedWinNumberHistoryText.text;
        GreenWinNumberHistoryText.text = greenLine + "\n" + GreenWinNumberHistoryText.text;
        BlackWinNumberHistoryText.text = blackLine + "\n" + BlackWinNumberHistoryText.text;
    }

    internal void ShowWinning(string color, int number)
    {
        isWinPopupActive = true;
        RedWinNumberPopup_Object.SetActive(false);
        BlackWinNumberPopup_Object.SetActive(false);
        GreenWinNumberPopup_Object.SetActive(false);

        WinNumberPopup_Text.text = number.ToString();

        if (color.ToLower() == "red")
        {
            RedWinNumberPopup_Object.SetActive(true);
        }
        else if (color.ToLower() == "black")
        {
            BlackWinNumberPopup_Object.SetActive(true);
        }
        else if (color.ToLower() == "green")
        {
            GreenWinNumberPopup_Object.SetActive(true);
        }

        WinNumberPopup_Object.SetActive(true);

        RectTransform popup = WinNumberPopup_Object.GetComponent<RectTransform>();

        popup.DOKill();
        popup.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.Append(popup.DOScale(1f, 0.35f).SetEase(Ease.OutBack));

        seq.AppendInterval(3f);

        seq.Append(popup.DOScale(0f, 0.3f).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            WinNumberPopup_Object.SetActive(false);
            isWinPopupActive = false;
        });
        
    }

    internal void ToggleButtons(bool status)
    {
        foreach (Button btn in DisableButtons_DuringSpin)
        {
            btn.interactable = status;
        }
    }

}
