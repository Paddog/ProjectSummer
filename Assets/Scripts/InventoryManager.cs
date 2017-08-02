using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public Dictionary<int,Item> items = new Dictionary<int, Item>();
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    public Item curSelectedItem;

    private CharController character;
    private GameObject toolBar;
    void Start() {
        character = this.gameObject.GetComponent<CharController>();
        toolBar = GameObject.Find("ToolBar");
        for(int i = 0; i < toolBar.transform.childCount; i++) {
            inventorySlots.Add(toolBar.transform.GetChild(i).GetComponent<InventorySlot>());
        }
    }

    void Update()
    {
        if (curSelectedItem != null)
        {
            Debug.LogError(curSelectedItem.name);
        }
    }

    public void AddItem(Item item) {
        if(items.Count < 3) {
            for (int i = 0; i < 3; i++)
            {
                if (!items.ContainsKey(i)) {
                    items.Add(i, item);
                    break;
                }
            }
            SyncItemsWithInventory();
        } else {
            Debug.LogError("Inventory full!");
        }
    }

    public void RemoveItem(Item item) {
        for (int i = 0; i < 3; i++)
        {
            if (items.ContainsKey(i)) {
                if (items[i] == item)
                {
                    items.Remove(i);
                }
            }
        }
        SyncItemsWithInventory();
    }

    private void SyncItemsWithInventory() {
        for(int i = 0; i < 3; i++) {
            if (items.ContainsKey(i)) {
                inventorySlots[i].curItem = items[i];
            }
        }
        GetSelectedItem(character.toolbarSelected);
    }
   
    public void GetSelectedItem(bool[] toolbarPos) {
        for (int i = 0; i < toolbarPos.Length; i++)
        {
            if (toolbarPos[i] == true) {
                if (items.Count == i + 1)
                {
                    curSelectedItem = items[i];
                }
                else {
                    curSelectedItem = null;
                }
            }
        }
    }

}
