using UnityEngine;
using System.Collections.Generic;

public class BetDefinition : MonoBehaviour
{
    [SerializeField] internal string betType;
    [SerializeField] internal List<string> numbers = new();
    [SerializeField] private bool isRaceTrackbet = false;
    internal RectTransform chipAnchor;

    private void Start()
    {
        if (!isRaceTrackbet)
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

}
