using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class PickUpItem : MonoBehaviour {
    public int itemID;
    public string itemName;

    public Item item;


    void Start() {
        ItemManager im = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        if(itemName == null || itemName == "") {
            foreach(Item _item in im.itemList) {
                if(_item.id == itemID) {
                    item = _item;
                }
            }
        } else {
            item = im.itemDic[itemName];
        }

        if(item.name == "KeyCard") {
            item.description += Random.Range(1, 15);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<InteractController>().interactableGO = this.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<InteractController>().interactableGO = null;
        }
    }


}
