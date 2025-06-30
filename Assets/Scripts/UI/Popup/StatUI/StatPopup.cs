using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataModels;
using TMPro;

public class StatPopup : BasePopup
{
    [SerializeField] private Button closeBtn;

    public void Init()
    {
        closeBtn?.onClick.AddListener(OnClose);
    }
    private void OnClose()
    {
        if (ui_lock) return;
        ui_lock = true;
        Close();
    }
}
