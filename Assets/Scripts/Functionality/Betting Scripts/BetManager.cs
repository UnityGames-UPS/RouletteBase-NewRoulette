using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class BetManager : MonoBehaviour
{
    [SerializeField] private RectTransform chipRoot;
    [SerializeField] private ChipSelector chipSelector;

    // keyed by betKey (eg "straight_up_7", "split_2_5", "red")
    private Dictionary<string, List<GameObject>> placedChips = new Dictionary<string, List<GameObject>>();
    private List<BetPlacement> betPlacement = new List<BetPlacement> { };

    [SerializeField]
    private float[] chipValues = { 0.10f, 0.50f, 1f, 2f, 5f, 10f, 25f, 50f, 100f, 500f, 1000f };

    // total amount per betKey
    private Dictionary<string, float> amountOnBet = new Dictionary<string, float>();

    private List<BetAction> betHistory = new List<BetAction>();
    private List<BetAction> lastRoundBets = new List<BetAction>();

    [System.Serializable]
    internal class BetAction
    {
        internal string betKey;         // unique ID like "split_2_5" or "even"
        internal float chipValue;
        internal RectTransform anchor;
    }

    #region Betting Logic

    internal void PlaceBet(BetDefinition betDef)
    {
        float chipValue = chipSelector.GetSelectedChipValue();
        if (chipValue <= 0)
            return;

        // build key (type + numbers). If numbers is empty -> just use type
        string key = BuildBetKey(betDef.betType, betDef.numbers);

        if (!amountOnBet.ContainsKey(key))
            amountOnBet[key] = 0f;

        amountOnBet[key] += chipValue;

        // Store history
        betHistory.Add(new BetAction
        {
            betKey = key,
            chipValue = chipValue,
            anchor = betDef.chipAnchor
        });

        // Rebuild visuals and backend entry
        RebuildChipStack(key, betDef.chipAnchor);
        UpdateBetPlacementForKey(key);
    }

    // Rebuild stack for a betKey. Will parse type & numbers from the key
    private void RebuildChipStack(string betKey, RectTransform anchor)
    {
        // ensure list exists
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

            // optionally set visual label if prefab has text, etc.
            placedChips[betKey].Add(chip);
        }
    }

    #endregion

    #region Bet Functions

    internal void UndoLastBet()
    {
        if (betHistory.Count == 0)
            return;

        BetAction last = betHistory[betHistory.Count - 1];
        betHistory.RemoveAt(betHistory.Count - 1);

        string key = last.betKey;
        float value = last.chipValue;

        if (!amountOnBet.ContainsKey(key))
            return;

        amountOnBet[key] -= value;

        if (amountOnBet[key] <= 0f)
        {
            amountOnBet.Remove(key);

            if (placedChips.ContainsKey(key))
            {
                foreach (var c in placedChips[key])
                    Destroy(c);
                placedChips.Remove(key);
            }

            // remove backend entries for this key
            RemoveBetPlacementForKey(key);
            return;
        }

        // rebuild visuals & backend for this key
        RebuildChipStack(key, last.anchor);
        UpdateBetPlacementForKey(key);
    }

    internal void DoubleBet()
    {
        if (betHistory.Count == 0)
            return;

        int originalCount = betHistory.Count;

        for (int i = 0; i < originalCount; i++)
        {
            BetAction a = betHistory[i];

            if (!amountOnBet.ContainsKey(a.betKey))
                amountOnBet[a.betKey] = 0f;

            amountOnBet[a.betKey] += a.chipValue;

            betHistory.Add(new BetAction
            {
                betKey = a.betKey,
                chipValue = a.chipValue,
                anchor = a.anchor
            });
        }

        // Rebuild all stacks
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
        foreach (var kvp in placedChips)
        {
            foreach (var chip in kvp.Value)
                Destroy(chip);
        }

        placedChips.Clear();
        amountOnBet.Clear();
        betHistory.Clear();
        betPlacement.Clear();

        Debug.Log("All bets cleared.");
    }

    internal void Rebet()
    {
        if (lastRoundBets.Count == 0) return;

        ClearCurrentBetsVisualOnly();

        foreach (var a in lastRoundBets)
        {
            if (!amountOnBet.ContainsKey(a.betKey))
                amountOnBet[a.betKey] = 0f;

            amountOnBet[a.betKey] += a.chipValue;
        }

        HashSet<string> keys = new HashSet<string>(amountOnBet.Keys);

        foreach (string key in keys)
        {
            RectTransform anchor = FindAnchorFromHistoryForRebet(key);
            if (anchor != null)
            {
                RebuildChipStack(key, anchor);
                UpdateBetPlacementForKey(key);
            }
        }

        foreach (var a in lastRoundBets)
        {
            betHistory.Add(new BetAction
            {
                betKey = a.betKey,
                chipValue = a.chipValue,
                anchor = a.anchor
            });
        }
    }

    #endregion

    #region Helper Functions

    // find anchor from history (most recent)
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

    internal void OnRoundComplete()
    {
        SaveRoundBets();
        ClearCurrentBetsVisualOnly();
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

        placedChips.Clear();
        amountOnBet.Clear();
        betHistory.Clear();
        betPlacement.Clear();
    }

    // build betKey from type and number list
    private string BuildBetKey(string type, List<string> numbers)
    {
        if (numbers == null || numbers.Count == 0)
            return type;
        return type + "_" + string.Join("_", numbers);
    }

    // parse betKey into type + number list
    private void ParseBetKey(string key, out string type, out List<string> numbers)
    {
        numbers = new List<string>();
        if (string.IsNullOrEmpty(key))
        {
            type = "";
            return;
        }

        var parts = key.Split('_');
        type = parts[0];
        if (parts.Length > 1)
        {
            numbers = parts.Skip(1).ToList();
        }
    }

    // update the betPlacement list for backend: remove old entry for key then add new one
    private void UpdateBetPlacementForKey(string betKey)
    {
        // remove any existing entries with same key (based on type+numbers)
        ParseBetKey(betKey, out string type, out List<string> numbers);

        // remove existing matching entries
        betPlacement.RemoveAll(bp =>
        {
            if (bp == null) return false;
            if (bp.type != type) return false;
            // compare numbers as strings
            if (bp.numbers == null) return numbers.Count == 0;
            var bpNums = bp.numbers.Select(o => o.ToString()).ToList();
            return bpNums.SequenceEqual(numbers);
        });

        // add new
        float amt = amountOnBet.ContainsKey(betKey) ? amountOnBet[betKey] : 0f;
        if (amt <= 0f) return;

        betPlacement.Add(new BetPlacement
        {
            type = type,
            amount = amt,
            numbers = new List<object>(numbers.Cast<object>())
        });
    }

    // remove any backend betPlacement entries that match this key
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

    // helper kept from old code but updated signature
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
