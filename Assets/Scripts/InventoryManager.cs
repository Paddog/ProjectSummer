using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public List<Item> items = new List<Item>();

    public void AddItem(Item item) {
        items.Add(item);
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
    }


    //TODO: Maybe we need this not sure
    public void UpdateGraphics() {

    }
}
