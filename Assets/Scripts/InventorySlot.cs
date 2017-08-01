using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour {

    private Item _curItem;
    public Item curItem {
        get { return _curItem; }
        set { _curItem = value;
            OnCurrentItemChange();
        }
    }


    private Image curImage;

    void Start() {
        GameObject child = this.transform.GetChild(0).gameObject;
        curImage = child.GetComponent<Image>();
    }
   

    public void OnCurrentItemChange() {
        
        if(curItem != null) {
            Sprite loadedImage = Resources.Load(curItem.name, typeof(Sprite)) as Sprite;
            if(loadedImage != null) {
                curImage.sprite = loadedImage;
            } else {
                Debug.LogError("Sprite for this item is missing: " + curItem.name);
            }

        } else {
            Sprite loadedImage = Resources.Load("Default", typeof(Sprite)) as Sprite;
            if(loadedImage != null) {
                curImage.sprite = loadedImage;
            } else {
                Debug.LogError("Sprite for this item is missing: " + curItem.name);
            }
        }
    }

}
