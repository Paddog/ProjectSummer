using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public enum CharacterStates {
    ducken,
    springen,
    interagieren,
    normal
}



[RequireComponent(typeof(Rigidbody2D))]
public class CharController : NetworkBehaviour {

    //Movement
    public float speed;
    public float jumpPower;
    private bool grounded = true;

    public Transform groundCheck;
    public float radius;
    public LayerMask groundLayer;

    private bool isDuck = false;

    [SerializeField]
    public DialogController DC;

    private Rigidbody2D rb;
    public CharacterStates cs;


    private float cooldown = 1;
    private float timestamp = 0;

    private CameraController mainCamera;

    private GameObject selector;
    public bool[] toolbarSelected = new bool[2];

    private InventoryManager inventory;
    void Start() {
        if(!isLocalPlayer)
            return;

        inventory = this.gameObject.GetComponent<InventoryManager>();
        selector = GameObject.FindGameObjectWithTag("Selector");
        rb = this.GetComponent<Rigidbody2D>();
        cs = CharacterStates.normal;
        mainCamera = GameObject.Find("SceneCamera").GetComponent<CameraController>();

        mainCamera.mainChar = this.gameObject;
	}

	void Update () {
        if (!isLocalPlayer) { return; }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (inventory.curSelectedItem != null)
            {
                CmdSpawnItemOnServer(inventory.curSelectedItem.name);
                inventory.RemoveItem(inventory.curSelectedItem);
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            selector.transform.localPosition = new Vector3(-110, 0, 0);
            toolbarSelected[0] = true;
            toolbarSelected[1] = false;
            toolbarSelected[2] = false;
            inventory.GetSelectedItem(toolbarSelected);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            selector.transform.localPosition = new Vector3(0, 0, 0);
            toolbarSelected[0] = false;
            toolbarSelected[1] = true;
            toolbarSelected[2] = false;
            inventory.GetSelectedItem(toolbarSelected);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3)) {
            selector.transform.localPosition = new Vector3(110, 0, 0);
            toolbarSelected[0] = false;
            toolbarSelected[1] = false;
            toolbarSelected[2] = true;
            inventory.GetSelectedItem(toolbarSelected);
        }


        //springen und ground check
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);

        if (grounded == false)
        {
            cs = CharacterStates.springen;
        }
        else if(grounded == true && isDuck == false) {
            cs = CharacterStates.normal;
        }

        if (Input.GetKey(KeyCode.Space) && cs == CharacterStates.normal){
            rb.AddForce(new Vector2(0, jumpPower));
            cs = CharacterStates.springen;
        }
        
        //Ducken
        if (Input.GetKeyDown(KeyCode.C) && timestamp <= Time.time)
        {
            timestamp = Time.time + cooldown;
            isDuck = !isDuck;
            if (isDuck)
            {
                speed = speed / 2;
            }
            else
            {
                speed = speed * 2;
            }
            CmdCrouchToggle(isDuck);
        }

    }

    [Command]
    private void CmdSpawnItemOnServer(string _name) {
        //TODO: Spawn Item at the correct position!
        GameObject loadedGO = Resources.Load(_name, typeof(GameObject)) as GameObject;
        GameObject instGO = (GameObject)Instantiate(loadedGO);
        instGO.transform.name = _name;
        NetworkServer.Spawn(instGO);
    }

    [Command]
    public void CmdCrouchToggle(bool id) {
        BoxCollider2D bc2d = this.GetComponent<BoxCollider2D>();
        if (id == true)
        {
            bc2d.size = new Vector2(bc2d.size.x, bc2d.size.y / 2);
            bc2d.offset = new Vector2(bc2d.offset.x, bc2d.size.y / -2);
            cs = CharacterStates.ducken;
        }
        else
        {
            bc2d.size = new Vector2(bc2d.size.x, bc2d.size.y * 2);
            bc2d.offset = new Vector2(0, 0);
        }
        RpcSendColliderInfo(bc2d.size.x, bc2d.size.y, bc2d.offset.x, bc2d.offset.y);
    }

    [ClientRpc]
    public void RpcSendColliderInfo(float x, float y, float offsetx, float offsety) {
        this.GetComponent<BoxCollider2D>().size = new Vector2(x, y);
        this.GetComponent<BoxCollider2D>().offset = new Vector2(offsetx, offsety);
    }

    void FixedUpdate(){
        if (!isLocalPlayer) { return; }
        //Bewegung Horizontal
        float x = 0;
        x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }



}
