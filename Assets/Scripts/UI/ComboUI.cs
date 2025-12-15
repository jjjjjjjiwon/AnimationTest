// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class ComboUI : MonoBehaviour
// {
//     [Header("UI")]
//     public TextMeshProUGUI comboText;

//     [Header("Target")]
//     public PlayerCombat playerCombat;

//     void Start()
//     {
//         if(playerCombat == null)
//         {
//             PlayerEntity player = FindObjectOfType<PlayerEntity>();
//             if(player != null)
//             {
//                 playerCombat = player.Combat;
//             } 

//             if (comboText != null)
//             {
//                 comboText.text = "";
//             }
//         }
//     }

//     void Update()
//     {
//         if (comboText != null && playerCombat != null)
//         {
//             string comboDisplay = playerCombat.GetComboDisplay();
//             comboText.text = comboDisplay;
//         }
//     }
// }
