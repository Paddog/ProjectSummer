using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Door : NetworkBehaviour {

    public NetworkAnimator animator;
    private BoxCollider2D box;

    [SyncVar(hook="ChangeState")]
    public bool isOpen = false;
    public float cooldown = 0.5f;


    void Start()
    {
        animator = this.GetComponent<NetworkAnimator>();
        animator.animator.SetBool("isOpen", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isServer)
            return;


        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerEnter!");
            //collision.GetComponent<DoorNetwork>().CmdSendDoorState(true, this.GetComponent<NetworkIdentity>().netId);
            isOpen = true;
            ChangeState(isOpen);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(!isServer)
            return;

        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerExit!");
            isOpen = false;
            //collision.GetComponent<DoorNetwork>().CmdSendDoorState(false ,this.GetComponent<NetworkIdentity>().netId);
            ChangeState(isOpen);
        }
    }


    public void ChangeState(bool _isOpen) {
        animator.animator.SetBool("isOpen", _isOpen);
        BoxCollider2D[] boxColliders = this.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders) {
            if (boxCollider.isTrigger == false) {
                box = boxCollider;
            }
        }
        box.enabled = !_isOpen;
    }

}
