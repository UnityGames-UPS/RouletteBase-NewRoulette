// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class OnCoinSelect : MonoBehaviour
// {
//     [SerializeField]
//     private Button This_Button;
//     [SerializeField]
//     private GameObject Related_Object;
//     [SerializeField]
//     private int CoinCounter;
//     [SerializeField]
//     private RouletteController _rouletteManager;

//     private void Start()
//     {
//         if (This_Button) This_Button.onClick.RemoveAllListeners();
//         if (This_Button) This_Button.onClick.AddListener(SelectCoin);
//     }

//     private void SelectCoin()
//     {
//         _rouletteManager.SelectCoin(Related_Object, CoinCounter);
//     }
// }
