using System;
using UnityEngine;

namespace IdleActionFarm.Runtime.Wallet
{
    public delegate void OnWalletAdded(int difference, GameObject from = null);

    public class Wallet : MonoBehaviour
    {
        public event OnWalletAdded OnWalletChange;
        private uint _curMoney;
        private void Start() { OnWalletChange?.Invoke((int)_curMoney); }

        public void AddMoney(uint amount, GameObject from = null)
        {
            _curMoney += amount;
            OnWalletChange?.Invoke((int)amount, from);
        }

        // public bool GetMoney(uint amount)
        // {
        //     if (CurMoney < amount) return false;
        //     CurMoney -= amount;
        //     return true;
        // }
    }
}