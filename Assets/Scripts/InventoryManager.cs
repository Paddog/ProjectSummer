using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public List<Item> items = new List<Item>();
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();


    private GameObject toolBar;
    void Start() {
        toolBar = GameObject.Find("ToolBar");
        for(int i = 0; i < toolBar.transform.childCount; i++) {
            inventorySlots.Add(toolBar.transform.GetChild(i).GetComponent<InventorySlot>());
        }
    }

    public void AddItem(Item item) {
        if(items.Count < 3) {
            items.Add(item);
            SyncItemsWithInventory();
        } else {
            Debug.LogError("Inventory full!");
        }
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
        SyncItemsWithInventory();
    }

    private void SyncItemsWithInventory() {
        int count = 0;
        foreach(Item item in items) {
            inventorySlots[count].curItem = item;
            count++;
        }
    }

}
