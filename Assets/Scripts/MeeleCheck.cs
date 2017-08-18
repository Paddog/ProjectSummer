using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCheck : MonoBehaviour {
    private CharController character;
    private BoxCollider2D meeleCollider;

    public bool isActive = false;
    public bool allowSwing = false;

    public GameObject curCollided;
    private void Start()
    {
        meeleCollider = this.GetComponent<BoxCollider2D>();
        character = this.transform.GetComponentInParent<CharController>();
        meeleCollider.enabled = false;
        if (meeleCollider == null) {
            return;
        }
    }

    private void Update()
    {
        if (meeleCollider == null || character == null || character.inventory == null)
        {
            return;
        }

        if (character.inventory.curSelectedItem != null)
        {
            if (character.inventory.curSelectedItem.name == "SledgeHammer" || character.inventory.curSelectedItem.name == "Tazer")
            {
                meeleCollider.enabled = true;
                isActive = true;
            }
            else
            {
                isActive = false;
                meeleCollider.enabled = false;
            }
        }
        else {
            isActive = false;
            meeleCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (meeleCollider == null || character == null)
        {
            return;
        }
        if (col.tag == "Barricade" || col.tag == "Player") {
            curCollided = col.gameObject;
            allowSwing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (meeleCollider == null || character == null)
        {
            return;
        }
        if (col.tag == "Barricade" || col.tag == "Player") {
            curCollided = null;
            allowSwing = false;
        }
    }
}
