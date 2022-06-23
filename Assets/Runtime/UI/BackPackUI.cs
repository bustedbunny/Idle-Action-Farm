using System;
using TMPro;
using UnityEngine;

namespace IdleActionFarm.Runtime.UI
{
    public class BackPackUI : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        [SerializeField] private Backpack.Backpack backpack;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            backpack.OnBackPackChange += OnBackPackChange;
        }

        private void OnDestroy() { backpack.OnBackPackChange -= OnBackPackChange; }

        private void OnBackPackChange(int activeSlotInd) { _text.text = $"{activeSlotInd}/40"; }
    }
}