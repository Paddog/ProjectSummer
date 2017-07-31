using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class InteractController : NetworkBehaviour {

    public GameObject interactableGO;
    public DialogController dialog;

    private CharController character;


    void Start() {
        if(!isLocalPlayer)
            return;
        character = this.GetComponent<CharController>();
        dialog = GameObject.Find("Canvas").GetComponent<DialogController>();
    }


    void Update() {
        if(!isLocalPlayer)
            return;


        if(interactableGO != null && Input.GetKeyUp(KeyCode.E)) {
            dialog.ShowDialog(interactableGO.tag);
            character.cs = CharacterStates.interagieren;

            switch(interactableGO.tag) {
                case "KeyLock":
                    GameObject uiInteractGO = dialog.GetUiElement(interactableGO.tag);
                    GameObject buttonOK = uiInteractGO.transform.GetChild(0).gameObject;

                    buttonOK.GetComponent<Button>().onClick.AddListener(() => { InteractKeyLock(); });
                    break;
                case "PickUpItem":
                    InventoryManager im = this.GetComponent<InventoryManager>();
                    im.AddItem(interactableGO.GetComponent<PickUpItem>().item);
                    //TODO: I do think this needs to be synchronized with the server but im not exactly sure
                    CmdDestroyItemOnServer(interactableGO.GetComponent<NetworkIdentity>().netId);
                    break;
            }


        }
    }

    [Command]
    private void CmdDestroyItemOnServer(NetworkInstanceId id) {
        NetworkServer.Destroy(NetworkServer.FindLocalObject(id));
    }


    //CRITICAL: If you are host and client this dosnt work for some reason!
    private void InteractKeyLock() {
        if(!isLocalPlayer) {
            return;
        }

        InputField inputF = dialog.GetUiElement(interactableGO.tag).transform.GetChild(2).GetComponent<InputField>();
        
        if(interactableGO.GetComponent<KeyLock>().EnterCode(int.Parse(inputF.text))) {
            if(interactableGO.GetComponent<KeyLock>().isLocked == true) {
                CmdSendKeyLockState(false, interactableGO.GetComponent<NetworkIdentity>().netId);
                dialog.HideDialog(interactableGO.tag);
            } else {
                CmdSendKeyLockState(true, interactableGO.GetComponent<NetworkIdentity>().netId);
                dialog.HideDialog(interactableGO.tag);
            }
        }
        character.cs = CharacterStates.normal;
    }

    [Command]
    public void CmdSendKeyLockState(bool state, NetworkInstanceId id) {
        KeyLock curKeyLock = NetworkServer.FindLocalObject(id).GetComponent<KeyLock>();
        curKeyLock.isLocked = state;
        curKeyLock.conDoorScript.isLocked = state;
        curKeyLock.SyncKeyLockState();
    }


}
