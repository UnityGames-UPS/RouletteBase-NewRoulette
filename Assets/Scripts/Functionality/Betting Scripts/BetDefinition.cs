using UnityEngine;
using System.Collections.Generic;

public class BetDefinition : MonoBehaviour
{
    [SerializeField] internal string betType;                      // "straight_up", "red", "even", "split", "corner", etc.
    [SerializeField] internal List<string> numbers = new();        // Use string to support "00"
    internal RectTransform chipAnchor;            // Where the chip stacks should appear

    private void Start()
    {
        if (betType == "straight_up")
        {
            chipAnchor = GetComponentsInChildren<RectTransform>()[2];
        }
        else
        {
            chipAnchor = GetComponentsInChildren<RectTransform>()[1];
            Debug.Log(chipAnchor.name);
        }
    }
}
