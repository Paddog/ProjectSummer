using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Door : NetworkBehaviour {

    public NetworkAnimator animator;
    private BoxCollider2D box;

    [SyncVar(hook="ChangeState")]
    public bool isOpen = false;
    [SyncVar]
    public bool isLocked;

    public float cooldown = 0.5f;


    void Start()
    {
        animator = this.GetComponent<NetworkAnimator>();
        animator.animator.SetBool("isOpen", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isServer || isLocked)
            return;


        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerEnter!");
            isOpen = true;
            animator.animator.SetBool("isOpen", true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(!isServer || isLocked)
            return;

        if (collision.gameObject.tag == "Player")
        {
            Debug.LogError("TriggerExit!");
            isOpen = false;
            animator.animator.SetBool("isOpen", false);
        }
    }




    public void ChangeState(bool _isOpen) {
        animator.animator.SetBool("isOpen", _isOpen);
        /*
        BoxCollider2D[] boxColliders = this.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders) {
            if (boxCollider.isTrigger == false) {
                box = boxCollider;
            }
        }
        Debug.LogError("Animation abgespielt, Boxcollider deaktivieren");
        
        box.enabled = !_isOpen;
        */
    }

}
