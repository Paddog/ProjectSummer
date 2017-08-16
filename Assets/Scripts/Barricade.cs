using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Barricade : NetworkBehaviour {
    [SyncVar]
    public int health = 100;
}
