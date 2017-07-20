using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Door : NetworkBehaviour {

    private NetworkAnimator animator;
    private BoxCollider2D box;

    [SyncVar(hook="CS")]
    public bool isOpen = false;
    public float cooldown = 0.5f;


    void Start()
    {
        animator = this.GetComponent<NetworkAnimator>();
        animator.animator.SetBool("isOpen", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerEnter!");
            CmdSendDoorState(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerExit!");
            CmdSendDoorState(false);
        }
    }

    [Command]
    public void CmdSendDoorState(bool state) {
        isOpen = state;
    }


    private void CS(bool _isOpen)
    {
        StartCoroutine(ChangeState(_isOpen));
    }

    public IEnumerator ChangeState(bool _isOpen) {
        animator.animator.SetBool("isOpen", _isOpen);
        BoxCollider2D[] boxColliders = this.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders) {
            if (boxCollider.isTrigger == false) {
                box = boxCollider;
            }
        }

        Debug.LogError("Schwanz!");
        yield return new WaitForSeconds(1);
        box.enabled = !box.enabled;
    }

}
