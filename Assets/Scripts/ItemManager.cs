using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public Dictionary<string, Item> itemDic = new Dictionary<string, Item>();
    public List<Item> itemList = new List<Item>();

	void Start () {
        itemDic.Add("KeyCard", new Item(0, "KeyCard", "KeyCard number: ", 1));
        itemList.Add(new Item(0, "KeyCard", "KeyCard number: 0", 1));
        itemDic.Add("Barricade", new Item(1, "Barricade", "Object to barricade corridors", 4));
        itemList.Add(new Item(1, "Barricade", "Object to barricade corridors", 4));
        itemDic.Add("SledgeHammer", new Item(2, "SledgeHammer", "Used to destroy walls", 1));
        itemList.Add(new Item(2, "SledgeHammer", "Used to destroy walls", 1));
        itemDic.Add("Tazer", new Item(3, "Tazer", "Used to stun other players", 1));
        itemList.Add(new Item(3, "Tazer", "Used to stun other players", 1));
    }
}
