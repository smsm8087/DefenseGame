﻿using System;
using FancyScrollView;
using System.Collections.Generic;
using CharacterSelect;
using UnityEngine;
using EasingCore;

public class CharacterScrollView : FancyScrollView<CharacterData, Context>
{
    [SerializeField] Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;

    Action<int> onSelectionChanged;

    protected override GameObject CellPrefab => cellPrefab;

    protected override void Initialize()
    {
        base.Initialize();

        Context.OnCellClicked = SelectCell;
        scroller.OnValueChanged(UpdatePosition);
        scroller.OnSelectionChanged(UpdateSelection);
    }

    void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();

        onSelectionChanged?.Invoke(index);
    }

    public void UpdateData(IList<CharacterData> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }

    public void OnSelectionChanged(Action<int> callback)
    {
        onSelectionChanged = callback;
    }

    public void SelectNextCell()
    {
        SelectCell(Context.SelectedIndex + 1);
    }

    public void SelectPrevCell()
    {
        SelectCell(Context.SelectedIndex - 1);
    }

    public void SelectCell(int index)
    {
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex)
        {
            return;
        }

        UpdateSelection(index);
        scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
    }
}


