using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryService
{
    private const int EMPTY = -1;

    private static bool _wired;
    private static ItemDatabase _db;

    public static void Initialize(ItemDatabase db)
    {
        _db = db;

        if (!_wired)
        {
            GameEvents.OnInventoryChanged += PersistSnapshot;
            _wired = true;
        }

        if (SaveService.Current != null && InvenManager.Instance != null)
        {
            EnsureArraySized(ref SaveService.Current.inventory, InvenManager.Instance.MaxInvenSlots, EMPTY);
            LoadIntoRuntime();
        }
    }

    public static void SyncFromSave()
    {
        if (SaveService.Current == null || InvenManager.Instance == null || _db == null)
        {
            Debug.LogWarning("[InventoryService] Sync From Save Skipped");
            return;
        }

        EnsureArraySized(ref SaveService.Current.inventory, InvenManager.Instance.MaxInvenSlots, EMPTY);
        LoadIntoRuntime();
    }

    private static void LoadIntoRuntime()
    {
        var ids = SaveService.Current.inventory;
        var list = new List<ItemData>(InvenManager.Instance.MaxInvenSlots);

        if (ids != null)
        {
            foreach (var id in ids)
            {
                if (id == EMPTY) continue;
                var entry = _db.GetItemById(id);
                var data = entry.data;
                if (data != null) list.Add(data);
            }
        }
        InvenManager.Instance.ReplaceAllFronmSave(list);

        GameEvents.RaiseInventoryChanged();
    }

    private static async void PersistSnapshot()
    {
        if (SaveService.Current == null || InvenManager.Instance == null) return;

        var mgr = InvenManager.Instance;
        var items = mgr.Items;
        int max = mgr.MaxInvenSlots;

        var snapshot = new int[max];
        for (int i = 0; i < max; i++) snapshot[i] = EMPTY;
        for (int i = 0; i < items.Count && i < max; i++)
            snapshot[i] = items[i]?.id ?? EMPTY;

        SaveService.Current.inventory = snapshot;

        try { await SaveService.SaveAllAsync(); }
        catch (System.Exception exp) { Debug.LogError($"[InventoryService] Save Failed : {exp}"); }
    }

    private static void EnsureArraySized(ref int[] arr, int size, int fill)
    {
        if (arr == null || arr.Length != size)
        {
            var newArr = new int[size];
            for (int i = 0; i < size; i++) newArr[i] = fill;

            if(arr != null)
            {
                int n = Mathf.Min(arr.Length, size);
                for (int i = 0; i < n; i++) newArr[i] = arr[i];
            }
            arr = newArr;
        }
    } 
}
