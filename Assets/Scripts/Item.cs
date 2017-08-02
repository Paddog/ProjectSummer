using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item  {
    public int id;
    public string name;
    public string description;
    public int space;

    public Item (int _id, string _name, string _description, int _space) {
        id = _id;
        name = _name;
        description = _description;
        space = _space;
    }

}
