using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
// using System.Numerics;
// using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{

    [Header("TMP_Texts")]
    [SerializeField] internal TMP_Text balanceText;
    [SerializeField] internal TMP_Text totalBetText;
    [SerializeField] private TMP_Text totalWiningText;
    [SerializeField] private TMP_Text RedWinNumberHistoryText;
    [SerializeField] private TMP_Text GreenWinNumberHistoryText;
    [SerializeField] private TMP_Text BlackWinNumberHistoryText;
    [SerializeField] internal TMP_Text AutoSpintCountText;
    [SerializeField] private TMP_Text min_Text;
    [SerializeField] private TMP_Text max_Text;

    [Header("Hot & Cold Number")]
    [SerializeField] private TMP_Text[] hotNumberTexts;
    [SerializeField] private TMP_Text[] hotCountTexts;

    [SerializeField] private TMP_Text[] coldNumberTexts;
    [SerializeField] private TMP_Text[] coldCountTexts;


    [Header("Buttons")]
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
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button AutoSpinButton;
    [SerializeField] private Button AutoSpinStopButton;
    [SerializeField] private Button InfoButton;
    [SerializeField] private Button HomeButton;

    [Header("GameObjects")]
    [SerializeField] private GameObject PaytablePopup_Object;
    [SerializeField] internal GameObject RaceTrackPopup_Object;
    [SerializeField] internal GameObject StatisticsPopup_Object;
    [SerializeField] private GameObject FavouriteBetPopup_Object;
    [SerializeField] private GameObject SettingsPopup_Object;
    [SerializeField] internal GameObject AutoSpinPopup_Object;
    [SerializeField] internal GameObject AutoStopPopup_Object;
    [SerializeField] private GameObject InfoPopup_Object;
    [SerializeField] private GameObject WiningImage;
    [SerializeField] private GameObject SpinButton;
    [SerializeField] private GameObject TurboButton;
    [SerializeField] private GameObject Ball;

    [Header("RaceTrack Panel")]
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private GameObject ZeroPanel;
    [SerializeField] private GameObject OnePanel;
    [SerializeField] private GameObject TwoPanel;
    [SerializeField] private GameObject ThreePanel;
    // [SerializeField] private GameObject FourPanel;
    // [SerializeField] private GameObject FivePanel;
    // [SerializeField] private GameObject SixPanel;
    // [SerializeField] private GameObject SevenPanel;
    // [SerializeField] private GameObject EightPanel;
    [SerializeField] private TMP_Text raceTrackNumber_Text;

    [Header("Sound Panel")]
    [SerializeField] private Button VolumeButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject VolumePopup_Object;

    [Header("Popup Objects")]
    [SerializeField] private GameObject MainPopup_Object;
    [SerializeField] private GameObject LowBalancePopup_Object;
    [SerializeField] private GameObject BetLimitPopup_Object;
    [SerializeField] private GameObject EmptyBetPopup_Object;
    [SerializeField] private GameObject DisconnectPopup_Object;
    [SerializeField] private GameObject ReconnectPopup_Object;
    [SerializeField] private GameObject QuitPopup_Object;
    [SerializeField] private Button LowBalanceButton;
    [SerializeField] private Button BetLimitButton;
    [SerializeField] private Button EmptyBetButton;
    [SerializeField] private Button QuitYesButton;
    [SerializeField] private Button QuitNoButton;

    [Header("Win Popup")]
    [SerializeField] private GameObject WinNumberPopup_Object;
    [SerializeField] private GameObject RedWinNumberPopup_Object;
    [SerializeField] private GameObject BlackWinNumberPopup_Object;
    [SerializeField] private GameObject GreenWinNumberPopup_Object;
    [SerializeField] private TMP_Text WinNumberPopup_Text;
    [SerializeField] private TMP_Text WinAmount_Text;
    [SerializeField] private GameObject WinAmount_Object;

    [Header("Settings Popup")]
    [SerializeField] private Toggle BgMusicToggle;
    [SerializeField] private Slider BgMusicSlider;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Toggle quickSpinToggle;

    [Header("Lists")]
    [SerializeField] private List<Button> DisableButtons_DuringSpin;
    [SerializeField] private List<GameObject> DisableGameObject_DuringSpin;
    [SerializeField] private List<Button> CloseButtons;
    [SerializeField] private List<Button> AutoSpinOptionButtons;
    [SerializeField] private RectTransform[] WiningImagePositions;

    [Header("Managers")]
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioController;
    [SerializeField] private BallScript BallManager;
    [SerializeField] private BetManager betManager;
    [SerializeField] private RouletteController rouletteController;

    [Header("Other Stuff")]
    internal bool isWinPopupActive = false;
    internal bool turboSpin = false;
    internal bool AutoSpin = false;
    private bool isExit = false;
    internal bool winNumberAnimation = false;
    internal int autoSpinCount = 0;
    private float lastMusicVolume = 1f;
    private float lastSoundVolume = 1f;
    private int raceTrackNumber = 0;

    private void Start()
    {
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
        if (PaytableCloseButton) PaytableCloseButton.onClick.AddListener(() => ClosePanel(PaytablePopup_Object));
        if (RaceTrackButton) RaceTrackButton.onClick.RemoveAllListeners();
        if (RaceTrackButton) RaceTrackButton.onClick.AddListener(() => TogglePopup(RaceTrackPopup_Object));
        if (StatisticsButton) StatisticsButton.onClick.RemoveAllListeners();
        if (StatisticsButton) StatisticsButton.onClick.AddListener(() => TogglePopup(StatisticsPopup_Object));
        if (FavouriteBetButton) FavouriteBetButton.onClick.RemoveAllListeners();
        if (FavouriteBetButton) FavouriteBetButton.onClick.AddListener(() => TogglePopup(FavouriteBetPopup_Object));
        if (SettingsButton) SettingsButton.onClick.RemoveAllListeners();
        if (SettingsButton) SettingsButton.onClick.AddListener(() => TogglePopup(SettingsPopup_Object));
        if (VolumeButton) VolumeButton.onClick.RemoveAllListeners();
        if (VolumeButton) VolumeButton.onClick.AddListener(() => TogglePopup(VolumePopup_Object));
        if (volumeSlider) volumeSlider.onValueChanged.RemoveAllListeners();
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        if (AutoSpinButton) AutoSpinButton.onClick.RemoveAllListeners();
        if (AutoSpinButton) AutoSpinButton.onClick.AddListener(() => TogglePopup(AutoSpinPopup_Object));
        if (InfoButton) InfoButton.onClick.RemoveAllListeners();
        if (InfoButton) InfoButton.onClick.AddListener(() => TogglePopup(InfoPopup_Object));
        if (AutoSpinStopButton) AutoSpinStopButton.onClick.RemoveAllListeners();
        if (AutoSpinStopButton) AutoSpinStopButton.onClick.AddListener(() => StopAutoSpin());
        if (HomeButton) HomeButton.onClick.RemoveAllListeners();
        if (HomeButton) HomeButton.onClick.AddListener(() => OpenPopup(QuitPopup_Object));
        if (LowBalanceButton) LowBalanceButton.onClick.RemoveAllListeners();
        if (LowBalanceButton) LowBalanceButton.onClick.AddListener(() => ClosePopup(LowBalancePopup_Object));
        if (BetLimitButton) BetLimitButton.onClick.RemoveAllListeners();
        if (BetLimitButton) BetLimitButton.onClick.AddListener(() => ClosePopup(LowBalancePopup_Object));
        if (EmptyBetButton) EmptyBetButton.onClick.RemoveAllListeners();
        if (EmptyBetButton) EmptyBetButton.onClick.AddListener(() => ClosePopup(EmptyBetPopup_Object));
        if (QuitYesButton) QuitYesButton.onClick.RemoveAllListeners();
        if (QuitYesButton) QuitYesButton.onClick.AddListener(delegate
        {
            CallOnExitFunction();
        });
        if (QuitNoButton) QuitNoButton.onClick.RemoveAllListeners();
        if (QuitNoButton) QuitNoButton.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
            }
        });
        if (plusButton) plusButton.onClick.RemoveAllListeners();
        if (plusButton) plusButton.onClick.AddListener(() => RaceTrackBets(true));
        if (minusButton) minusButton.onClick.RemoveAllListeners();
        if (minusButton) minusButton.onClick.AddListener(() => RaceTrackBets(false));

        ClosepopupButtons();
        AutoSpinButtons();
        SettingsPopupButton();
    }

    private void TogglePopup(GameObject popup)
    {
        audioController.PlayUIButton();
        if (popup == SettingsPopup_Object && (AutoSpinPopup_Object.activeSelf || VolumePopup_Object.activeSelf || InfoPopup_Object.activeSelf))
        {
            AutoSpinPopup_Object.SetActive(false);
            VolumePopup_Object.SetActive(false);
            InfoPopup_Object.SetActive(false);
            AutoStopPopup_Object.SetActive(false);
        }
        if (popup == AutoSpinPopup_Object && (SettingsPopup_Object.activeSelf || VolumePopup_Object.activeSelf || InfoPopup_Object.activeSelf))
        {
            SettingsPopup_Object.SetActive(false);
            VolumePopup_Object.SetActive(false);
            InfoPopup_Object.SetActive(false);
        }
        if (popup == InfoPopup_Object && (SettingsPopup_Object.activeSelf || VolumePopup_Object.activeSelf || AutoSpinPopup_Object.activeSelf))
        {
            SettingsPopup_Object.SetActive(false);
            AutoSpinPopup_Object.SetActive(false);
            VolumePopup_Object.SetActive(false);
            AutoStopPopup_Object.SetActive(false);
        }
        if (popup.activeSelf)
        {
            popup.SetActive(false);
            return;
        }
        if (!popup.activeSelf)
        {
            popup.SetActive(true);
            return;
        }
    }

    private void ClosePanel(GameObject popup)
    {
        audioController.PlayUIButton();
        audioController.PlayUIButton();
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
            // case "favouriteBet":
            //     if (FavouriteBetPopup_Object.activeSelf)
            //     {
            //         return "Close favourite bets";
            //     }
            //     return "Open favourite bets";
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
        min_Text.text = socketManager.initialData.bets.limits.min.ToString();
        max_Text.text = socketManager.initialData.bets.limits.max.ToString();
    }
    internal IEnumerator UpdateResultUI()
    {
        // totalBetText.text = socketManager.resultData.payload.totalBetAmount.ToString("F2");
        yield return new WaitUntil(() => BallManager.ballStopped);
        UpdateUI();
        Debug.Log("UI Updated");
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

    internal void ShowWinning(string color, int number, float winAmount)
    {
        isWinPopupActive = true;
        RedWinNumberPopup_Object.SetActive(false);
        BlackWinNumberPopup_Object.SetActive(false);
        GreenWinNumberPopup_Object.SetActive(false);
        WinAmount_Object.SetActive(false);

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

        if (winAmount > 0)
        {
            WinAmount_Object.SetActive(true);
            WinAmount_Text.text = winAmount.ToString();
        }
        audioController.PlayWinPopup();
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
        foreach (GameObject GO in DisableGameObject_DuringSpin)
        {
            GO.SetActive(status);
        }
    }

    private void ClosepopupButtons()
    {
        foreach (Button btn in CloseButtons)
        {
            btn.onClick.AddListener(() =>
            {
                SettingsPopup_Object.SetActive(false);
                AutoSpinPopup_Object.SetActive(false);
                AutoStopPopup_Object.SetActive(false);
                InfoPopup_Object.SetActive(false);
            });
        }
    }

    internal void Closepopups()
    {
        SettingsPopup_Object.SetActive(false);
        AutoSpinPopup_Object.SetActive(false);
        // AutoSpinPopup2_Object.SetActive(false);
        InfoPopup_Object.SetActive(false);
    }

    private void AutoSpinButtons()
    {
        foreach (Button btn in AutoSpinOptionButtons)
        {
            btn.onClick.AddListener(() =>
            {
                audioController.PlayUIButton();
                string btnName = btn.gameObject.name;
                switch (btnName)
                {
                    case "10":
                        autoSpinCount = 10;
                        break;
                    case "25":
                        autoSpinCount = 25;
                        break;
                    case "50":
                        autoSpinCount = 50;
                        break;
                    case "100":
                        autoSpinCount = 100;
                        break;
                    case "250":
                        autoSpinCount = 250;
                        break;
                    case "500":
                        autoSpinCount = 500;
                        break;
                    case "750":
                        autoSpinCount = 750;
                        break;
                    case "1000":
                        autoSpinCount = 1000;
                        break;
                }
                AutoSpinPopup_Object.SetActive(false);
                AutoSpintCountText.text = (autoSpinCount - 1).ToString();
                if (rouletteController.betCount > 0)
                {
                    AutoStopPopup_Object.SetActive(true);
                }
                rouletteController.StartCoroutine(rouletteController.TweenRoutine());
            });
        }
    }
    private void StopAutoSpin()
    {
        audioController.PlayUIButton();
        autoSpinCount = 0;
        AutoStopPopup_Object.SetActive(false);
        AutoSpinPopup_Object.SetActive(false);
    }

    internal void StartWinNummberAnimation(int winNum)
    {
        // int winNum = socketManager.resultData.payload.winningNumber;

        if (winNum.ToString() == "00")
        {
            StartCoroutine(WiningNumberAnimation(WiningImagePositions[37].anchoredPosition));
        }
        else
        {
            StartCoroutine(WiningNumberAnimation(WiningImagePositions[winNum].anchoredPosition));
        }
    }


    private IEnumerator WiningNumberAnimation(Vector2 position)
    {
        winNumberAnimation = true;

        RectTransform winRect = WiningImage.GetComponent<RectTransform>();
        Vector2 originalAnchoredPos = winRect.anchoredPosition;
        winRect.anchoredPosition = position;

        Image innerImage = WiningImage.transform.GetChild(0).GetComponent<Image>();
        Image outerImage = WiningImage.transform.GetChild(1).GetComponent<Image>();

        innerImage.gameObject.SetActive(true);
        outerImage.gameObject.SetActive(true);

        innerImage.color = new Color(1, 1, 1, 0);
        outerImage.color = new Color(1, 1, 1, 0);

        Sequence seq = DOTween.Sequence();
        seq.SetAutoKill(false);
        seq.SetLoops(-1);

        seq.Append(innerImage.DOFade(1f, 0.2f));
        seq.AppendInterval(0.2f);

        seq.Append(outerImage.DOFade(1f, 0.2f));
        seq.AppendInterval(0.2f);

        seq.Append(innerImage.DOFade(0f, 0.2f));
        seq.AppendInterval(0.2f);

        seq.Append(outerImage.DOFade(0f, 0.2f));
        seq.AppendInterval(0.2f);

        seq.Play();
        yield return new WaitUntil(() => !winNumberAnimation);

        seq.Kill();
        winRect.anchoredPosition = originalAnchoredPos;
        innerImage.gameObject.SetActive(false);
        outerImage.gameObject.SetActive(false);
    }


    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayUIButton();
        StartCoroutine(socketManager.CloseSocket());
    }

    internal void LowBalPopup()
    {
        OpenPopup(LowBalancePopup_Object);
    }
    internal void BetLimitPopup()
    {
        OpenPopup(BetLimitPopup_Object);
    }

    internal void EmptyBetPopup()
    {
        OpenPopup(EmptyBetPopup_Object);
    }

    internal void QuitPopup()
    {
        OpenPopup(QuitPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            isExit = true;
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayUIButton();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayUIButton();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }

    private void SettingsPopupButton()
    {
        BgMusicSlider.onValueChanged.RemoveAllListeners();
        BgMusicToggle.onValueChanged.RemoveAllListeners();

        BgMusicSlider.value = 0.5f;
        BgMusicToggle.isOn = true;

        BgMusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        BgMusicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

        soundSlider.onValueChanged.RemoveAllListeners();
        soundToggle.onValueChanged.RemoveAllListeners();

        soundSlider.value = 0.5f;
        soundToggle.isOn = true;

        soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

        quickSpinToggle.onValueChanged.RemoveAllListeners();
        quickSpinToggle.isOn = turboSpin;
        quickSpinToggle.onValueChanged.AddListener(OnQuickSpinToggle);

    }

    private void OnMusicSliderChanged(float value)
    {
        audioController.SetBGVolume(value);

        if (value <= 0f)
            BgMusicToggle.isOn = false;
        else
        {
            lastMusicVolume = value;
            BgMusicToggle.isOn = true;
        }
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        if (!isOn)
        {
            lastMusicVolume = BgMusicSlider.value;
            BgMusicSlider.value = 0f;
        }
        else
        {
            BgMusicSlider.value = Mathf.Max(lastMusicVolume, 0.1f);
        }
    }

    private void OnVolumeSliderChanged(float value)
    {
        audioController.SetBGVolume(value);
        audioController.SetSoundVolume(value);

        if (value <= 0f)
            VolumeButton.interactable = false;
        else
        {
            lastMusicVolume = value;
            lastSoundVolume = value;
            VolumeButton.interactable = true;
        }
    }

    private void OnSoundSliderChanged(float value)
    {
        audioController.SetSoundVolume(value);

        if (value <= 0f)
            soundToggle.isOn = false;
        else
        {
            lastSoundVolume = value;
            soundToggle.isOn = true;
        }
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        if (!isOn)
        {
            lastSoundVolume = soundSlider.value;
            soundSlider.value = 0f;
        }
        else
        {
            soundSlider.value = Mathf.Max(lastSoundVolume, 0.1f);
        }
    }

    private void OnQuickSpinToggle(bool isOn)
    {
        turboSpin = isOn;
        if (!isOn)
        {
            TurboButton.SetActive(false);
            SpinButton.SetActive(true);
            Ball.SetActive(true);
        }
        else
        {
            SpinButton.SetActive(false);
            TurboButton.SetActive(true);
            Ball.SetActive(false);
        }
    }

    internal void UpdateHotColdNumbers(List<HotNumber> hotNumbers, List<ColdNumber> coldNumbers)
    {
        for (int i = 0; i < hotNumberTexts.Length; i++)
        {
            if (i < hotNumbers.Count)
            {
                hotNumberTexts[i].gameObject.SetActive(true);
                hotCountTexts[i].gameObject.SetActive(true);

                hotNumberTexts[i].text = hotNumbers[i].number.ToString();
                hotCountTexts[i].text = hotNumbers[i].count.ToString();
            }
            else
            {
                hotNumberTexts[i].gameObject.SetActive(false);
                hotCountTexts[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < coldNumberTexts.Length; i++)
        {
            if (i < coldNumbers.Count)
            {
                coldNumberTexts[i].gameObject.SetActive(true);
                coldCountTexts[i].gameObject.SetActive(true);

                coldNumberTexts[i].text = coldNumbers[i].number.ToString();
                coldCountTexts[i].text = coldNumbers[i].count.ToString();
            }
            else
            {
                coldNumberTexts[i].gameObject.SetActive(false);
                coldCountTexts[i].gameObject.SetActive(false);
            }
        }
    }

    private void RaceTrackBets(bool plus)
    {

        if (plus && raceTrackNumber < 3)
        {
            raceTrackNumber++;
        }
        else if (!plus && raceTrackNumber > 0)
        {
            raceTrackNumber--;
        }

        raceTrackNumber_Text.text = raceTrackNumber.ToString();
        switch (raceTrackNumber)
        {
            case 0:
                OnePanel.SetActive(false);
                ZeroPanel.SetActive(true);
                break;
            case 1:
                ZeroPanel.SetActive(false);
                TwoPanel.SetActive(false);
                OnePanel.SetActive(true);
                break;
            case 2:
                OnePanel.SetActive(false);
                ThreePanel.SetActive(false);
                TwoPanel.SetActive(true);
                break;
            case 3:
                TwoPanel.SetActive(false);
                // FourPanel.SetActive(false);
                ThreePanel.SetActive(true);
                break;
                // case 4:
                //     ThreePanel.SetActive(false);
                //     FivePanel.SetActive(false);
                //     FourPanel.SetActive(true);
                //     break;
                // case 5:
                //     FourPanel.SetActive(false);
                //     SixPanel.SetActive(false);
                //     FivePanel.SetActive(true);
                //     break;
                // case 6:
                //     FivePanel.SetActive(false);
                //     SevenPanel.SetActive(false);
                //     SixPanel.SetActive(true);
                //     break;
                // case 7:
                //     SixPanel.SetActive(false);
                //     EightPanel.SetActive(false);
                //     SevenPanel.SetActive(true);
                //     break;
                // case 8:
                //     SevenPanel.SetActive(false);
                //     EightPanel.SetActive(true);
                //     break;
        }
    }

}
