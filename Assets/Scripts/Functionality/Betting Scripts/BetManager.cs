using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class BetManager : MonoBehaviour
{
    [SerializeField] private RectTransform chipRoot;
    [SerializeField] private ChipSelector chipSelector;
    [SerializeField] private AudioController audioController;
    [SerializeField] private RouletteController rouletteController;
    private Dictionary<string, List<GameObject>> placedChips = new Dictionary<string, List<GameObject>>();
    internal List<BetPlacement> betPlacement = new List<BetPlacement> { };

    [SerializeField]
    private float[] chipValues = { 0.10f, 0.50f, 1f, 2f, 5f, 10f, 25f, 50f, 100f, 500f, 1000f };

    private Dictionary<string, float> amountOnBet = new Dictionary<string, float>();

    private List<BetAction> betHistory = new List<BetAction>();
    private List<BetAction> lastRoundBets = new List<BetAction>();

    [System.Serializable]
    internal class BetAction
    {
        internal string betKey;
        internal float chipValue;
        internal RectTransform anchor;
    }

    #region Betting Logic

    internal void PlaceBet(BetDefinition betDef)
    {
        float chipValue = chipSelector.GetSelectedChipValue();
        if (chipValue <= 0)
            return;

        string key = BuildBetKey(betDef.betType, betDef.numbers);

        if (!amountOnBet.ContainsKey(key))
            amountOnBet[key] = 0f;

        amountOnBet[key] += chipValue;

        betHistory.Add(new BetAction
        {
            betKey = key,
            chipValue = chipValue,
            anchor = betDef.chipAnchor
        });

        audioController.PlayChip();
        rouletteController.betCount += chipValue;
        rouletteController.UpdateBet();
        RebuildChipStack(key, betDef.chipAnchor);
        UpdateBetPlacementForKey(key);
    }

    private void RebuildChipStack(string betKey, RectTransform anchor)
    {
        if (placedChips.ContainsKey(betKey))
        {
            foreach (var chip in placedChips[betKey])
                Destroy(chip);
            placedChips[betKey].Clear();
        }
        else
        {
            placedChips[betKey] = new List<GameObject>();
        }

        if (!amountOnBet.ContainsKey(betKey) || amountOnBet[betKey] <= 0f)
            return;

        float total = amountOnBet[betKey];
        var finalChips = ConvertToChipCombination(total);

        SpawnChipStack(betKey, anchor, finalChips);
    }

    private void SpawnChipStack(string betKey, RectTransform anchor, List<float> chips)
    {
        Vector2 basePos = chipRoot.InverseTransformPoint(anchor.transform.position);
        float stackOffset = 3f;

        for (int i = 0; i < chips.Count; i++)
        {
            float value = chips[i];

            ChipButton prefabChip = chipSelector.GetChipByValue(value);
            if (prefabChip == null || prefabChip.ChipPreab == null)
            {
                Debug.LogError("No prefab found for chip value: " + value);
                continue;
            }

            GameObject chip = Instantiate(prefabChip.ChipPreab, chipRoot);
            RectTransform chipRT = chip.GetComponent<RectTransform>();

            Vector2 finalPos = new Vector2(basePos.x, basePos.y + i * stackOffset);

            chipRT.anchoredPosition = finalPos + new Vector2(0, 40f);
            chipRT.localScale = Vector3.one * 0.3f;

            chipRT.DOAnchorPos(finalPos, 0.25f).SetEase(Ease.OutBack);
            chipRT.DOScale(0.7f, 0.2f);
            chipRT.DORotate(new Vector3(0, 0, Random.Range(-7f, 7f)), 0.3f);

            placedChips[betKey].Add(chip);
        }
    }

    #endregion

    #region Bet Functions

    internal void UndoLastBet()
    {
        audioController.PlayUIButton();

        if (betHistory.Count == 0)
            return;

        BetAction last = betHistory[betHistory.Count - 1];
        string key = last.betKey;

        if (!amountOnBet.ContainsKey(key))
            return;

        List<float> chipStack = GetCurrentChipStackValues(key);
        if (chipStack.Count == 0)
            return;

        float removedChipValue = chipStack[chipStack.Count - 1];

        amountOnBet[key] -= removedChipValue;
        rouletteController.betCount -= removedChipValue;
        rouletteController.UpdateBet();

        float toRemove = removedChipValue;
        for (int i = betHistory.Count - 1; i >= 0 && toRemove > 0f; i--)
        {
            if (betHistory[i].betKey != key)
                continue;

            toRemove -= betHistory[i].chipValue;
            betHistory.RemoveAt(i);
        }

        if (amountOnBet[key] <= 0f)
        {
            amountOnBet.Remove(key);

            if (placedChips.ContainsKey(key))
            {
                foreach (var c in placedChips[key])
                    Destroy(c);

                placedChips.Remove(key);
            }

            RemoveBetPlacementForKey(key);
            return;
        }

        RectTransform anchor = FindAnchorFromHistory(key);
        if (anchor != null)
        {
            RebuildChipStack(key, anchor);
            UpdateBetPlacementForKey(key);
        }
    }

    internal void DoubleBet()
    {
        audioController.PlayUIButton();
        if (betHistory.Count == 0)
            return;

        int originalCount = betHistory.Count;

        for (int i = 0; i < originalCount; i++)
        {
            BetAction a = betHistory[i];

            if (!amountOnBet.ContainsKey(a.betKey))
                amountOnBet[a.betKey] = 0f;

            amountOnBet[a.betKey] += a.chipValue;
            rouletteController.betCount += a.chipValue;
            rouletteController.UpdateBet();
            betHistory.Add(new BetAction
            {
                betKey = a.betKey,
                chipValue = a.chipValue,
                anchor = a.anchor
            });
        }

        var keys = amountOnBet.Keys.ToList();
        foreach (var key in keys)
        {
            RectTransform anchor = FindAnchorFromHistory(key);
            if (anchor != null)
            {
                RebuildChipStack(key, anchor);
                UpdateBetPlacementForKey(key);
            }
        }
    }

    internal void ClearAllBets()
    {
        audioController.PlayUIButton();
        ClearCurrentBetsVisualOnly();
        Debug.Log("All bets cleared.");
    }


    internal void Rebet()
    {
        audioController.PlayUIButton();
        if (lastRoundBets.Count == 0)
        {
            Debug.LogWarning("No previous round bets to rebet.");
            return;
        }

        ClearCurrentBetsVisualOnly();

        betHistory.Clear();

        foreach (var a in lastRoundBets)
        {
            betHistory.Add(new BetAction
            {
                betKey = a.betKey,
                chipValue = a.chipValue,
                anchor = a.anchor
            });

            if (!amountOnBet.ContainsKey(a.betKey))
                amountOnBet[a.betKey] = 0f;

            amountOnBet[a.betKey] += a.chipValue;
            rouletteController.betCount += a.chipValue;
        }

        HashSet<string> keys = new HashSet<string>(amountOnBet.Keys);
        foreach (string key in keys)
        {
            RectTransform anchor = FindAnchorFromHistory(key);
            if (anchor != null)
            {
                RebuildChipStack(key, anchor);
                UpdateBetPlacementForKey(key);
                rouletteController.UpdateBet();
            }
        }

        Debug.Log("REBETS restored successfully.");
    }

    #endregion

    #region Helper Functions

    private RectTransform FindAnchorFromHistory(string betKey)
    {
        for (int i = betHistory.Count - 1; i >= 0; i--)
        {
            if (betHistory[i].betKey == betKey)
                return betHistory[i].anchor;
        }
        return null;
    }

    private RectTransform FindAnchorFromHistoryForRebet(string betKey)
    {
        for (int i = lastRoundBets.Count - 1; i >= 0; i--)
        {
            if (lastRoundBets[i].betKey == betKey)
                return lastRoundBets[i].anchor;
        }
        return null;
    }

    private List<float> ConvertToChipCombination(float total)
    {
        List<float> result = new List<float>();

        for (int i = chipValues.Length - 1; i >= 0; i--)
        {
            float chip = chipValues[i];

            int count = Mathf.FloorToInt(total / chip);

            for (int c = 0; c < count; c++)
                result.Add(chip);

            total -= count * chip;

            if (total < 0.0001f)
                break;
        }

        return result;
    }

    private List<float> GetCurrentChipStackValues(string betKey)
    {
        if (!amountOnBet.ContainsKey(betKey))
            return new List<float>();

        float total = amountOnBet[betKey];
        return ConvertToChipCombination(total);
    }

    internal void OnRoundComplete()
    {
        rouletteController.betCount = 0;
        SaveRoundBets();
        ClearCurrentBetsVisualOnly();
    }

    internal void AutoBetComplete()
    {
        SaveRoundBets();
        foreach (var kvp in placedChips)
        {
            foreach (var chip in kvp.Value)
                Destroy(chip);
        }

        placedChips.Clear();
        amountOnBet.Clear();
        betHistory.Clear();
        betPlacement.Clear();
        Rebet();
    }

    private void SaveRoundBets()
    {
        lastRoundBets.Clear();

        foreach (var b in betHistory)
        {
            lastRoundBets.Add(new BetAction
            {
                betKey = b.betKey,
                chipValue = b.chipValue,
                anchor = b.anchor
            });
        }

        Debug.Log("Saved bets for REBET.");
    }

    private void ClearCurrentBetsVisualOnly()
    {
        foreach (var kvp in placedChips)
        {
            foreach (var chip in kvp.Value)
                Destroy(chip);
        }
        rouletteController.betCount = 0;
        rouletteController.UpdateBet();
        placedChips.Clear();
        amountOnBet.Clear();
        betHistory.Clear();
        betPlacement.Clear();
    }

    private string BuildBetKey(string type, List<string> numbers)
    {
        if (numbers == null || numbers.Count == 0)
            return type;
        return type + "_" + string.Join("_", numbers);
    }

    private void ParseBetKey(string key, out string type, out List<string> numbers)
    {
        numbers = new List<string>();

        if (string.IsNullOrEmpty(key))
        {
            type = "";
            return;
        }

        if (key.StartsWith("straight_up_"))
        {
            type = "straight_up";
            numbers.Add(key.Substring("straight_up_".Length));
            return;
        }

        string[] multiTypes = { "corner", "split", "street", "top_line", "six_line"};

        foreach (var multi in multiTypes)
        {
            if (key.StartsWith(multi + "_"))
            {
                type = multi;

                var parts = key.Substring((multi + "_").Length).Split('_');
                foreach (var p in parts)
                    numbers.Add(p);

                return;
            }
        }

        if (key.StartsWith("dozen_") || key.StartsWith("column_"))
        {
            type = key;
            return;
        }

        type = key;
    }

    private void UpdateBetPlacementForKey(string betKey)
    {
        ParseBetKey(betKey, out string type, out List<string> numberStrings);

        betPlacement.RemoveAll(bp => bp.type == type);

        float amt = amountOnBet.ContainsKey(betKey) ? amountOnBet[betKey] : 0f;
        if (amt <= 0f) return;

        List<object> finalNumbers = new List<object>();

        foreach (var n in numberStrings)
        {
            if (n == "00")
            {
                finalNumbers.Add("00");
            }
            else if (int.TryParse(n, out int numValue))
            {
                finalNumbers.Add(numValue);
            }
            else
            {
                Debug.LogWarning($"Invalid number format in betKey: {n}");
            }
        }

        betPlacement.Add(new BetPlacement
        {
            type = type,
            amount = amt,
            numbers = finalNumbers
        });
    }


    private void RemoveBetPlacementForKey(string betKey)
    {
        ParseBetKey(betKey, out string type, out List<string> numbers);

        betPlacement.RemoveAll(bp =>
        {
            if (bp == null) return false;
            if (bp.type != type) return false;
            if (bp.numbers == null) return numbers.Count == 0;
            var bpNums = bp.numbers.Select(o => o.ToString()).ToList();
            return bpNums.SequenceEqual(numbers);
        });
    }

    #endregion
    #region WiningAnimation

    internal void PlayWinningBetAnimation(List<WinningBet> winningBets)
    {
        foreach (var win in winningBets)
        {
            string betKey = BuildBetKey(win.type, null);

            RectTransform anchor = FindAnchorFromHistory(betKey);
            if (anchor == null)
                continue;

            if (placedChips.ContainsKey(betKey))
            {
                foreach (var c in placedChips[betKey])
                    Destroy(c);
                placedChips[betKey].Clear();
            }

            amountOnBet[betKey] = win.winAmount;
            RebuildChipStack(betKey, anchor);

            StartCoroutine(AnimateWinChipsToRoot(betKey));
        }
    }

    private IEnumerator AnimateWinChipsToRoot(string betKey)
    {
        if (!placedChips.ContainsKey(betKey))
            yield break;

        List<GameObject> chips = placedChips[betKey];

        Vector2 targetPos = Vector2.zero;
        float delay = 0f;

        foreach (var chip in chips)
        {
            RectTransform rt = chip.GetComponent<RectTransform>();

            rt.SetAsLastSibling();

            Sequence seq = DOTween.Sequence();
            seq.SetDelay(delay);

            seq.Join(rt.DOAnchorPos(targetPos, 0.45f).SetEase(Ease.InBack));

            seq.Join(rt.DOScale(0.15f, 0.45f));

            seq.OnComplete(() =>
            {
                Destroy(rt.gameObject);
            });

            delay += 0.05f;
        }

        yield return new WaitForSeconds(0.6f);

        placedChips.Remove(betKey);
        amountOnBet.Remove(betKey);
    }

    #endregion

    private void AddToBetList(string betKey, float amount)
    {
        ParseBetKey(betKey, out string type, out List<string> numbers);

        betPlacement.Add(new BetPlacement
        {
            type = type,
            amount = amount,
            numbers = new List<object>(numbers.Cast<object>())
        });
    }
}
