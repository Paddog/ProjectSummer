using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class DoorNetwork : NetworkBehaviour {

    [Command]
    public void CmdSendDoorState(bool state, NetworkInstanceId id) {
        GameObject door = NetworkServer.FindLocalObject(id);
        Door d = door.GetComponent<Door>();
        d.isOpen = state;
        d.animator.animator.SetBool("isOpen", d.isOpen);
        foreach(BoxCollider2D box in d.GetComponents<BoxCollider2D>()) {
            if(box.isTrigger == false) {
                box.enabled = !state;
            }
        }
    }

}
