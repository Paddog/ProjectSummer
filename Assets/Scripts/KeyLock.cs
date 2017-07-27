using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



//CRITICAL: There will be 2 CodeLocks For 1 Door, The 2 coresponding CodeLocks have to be the same state and need to have the same code
public class KeyLock : NetworkBehaviour {

    public GameObject connectedDoor;
    public Door conDoorScript;
    public bool isCodeKeyLock;



    [SyncVar]
    public int code;
    [SyncVar(hook = "ChangeIsLockedState")]
    public bool isLocked;

    

	void Start () {
        conDoorScript = connectedDoor.GetComponent<Door>();
        if (isServer) {
            //TODO: Set Code(Predefine Puzzles)
            code = 1337;
            isLocked = true;
        }
	}
	

    private void ChangeIsLockedState(bool _isLocked) {
        isLocked = _isLocked;
        if (isLocked)
        {
            conDoorScript.isLocked = true;
        }
        else {
            conDoorScript.isLocked = false;
        }
             
    }

    public bool EnterCode(int _code) {
        if (_code == code)
        {
            return true;
        }
        else {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            collision.GetComponent<CharController>().interactableGO = this.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            collision.GetComponent<CharController>().interactableGO = null;
        }
    }


}
