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
    [SyncVar]
    public bool isStunned = false;
    
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
    public bool[] toolbarSelected;

    public GameObject buildCheckGO;

    private bool isGhostObjectSpawned = false;
    private GameObject barricadeGhost;


    public InventoryManager inventory;
    private Item lastItem;


    private MeeleCheck MC;


    private float timer;
    void Start() {
        if(!isLocalPlayer)
            return;

        inventory = this.gameObject.GetComponent<InventoryManager>();
        selector = GameObject.FindGameObjectWithTag("Selector");
        rb = this.GetComponent<Rigidbody2D>();
        cs = CharacterStates.normal;
        mainCamera = GameObject.Find("SceneCamera").GetComponent<CameraController>();

        mainCamera.mainChar = this.gameObject;

        toolbarSelected = new bool[3];

        toolbarSelected[0] = false;
        toolbarSelected[1] = true;
        toolbarSelected[2] = false;

        buildCheckGO = this.transform.GetChild(1).gameObject;
        MC = this.GetComponentInChildren<MeeleCheck>();
    }

	void Update () {
        if (isServer) {
            if (isStunned)
            {
                if (timer < Time.time) {
                    isStunned = false;
                    RpcUnstunPlayer();
                }
            }
        }

        if (!isLocalPlayer) {
            return;
        }
        if (isStunned) {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (inventory.curSelectedItem != null)
            {
                //TODO: Find the right position at the feet of the character, maybe use the groundcheck!
                CmdSpawnItemOnServer(inventory.curSelectedItem.name, this.transform.position.x, this.transform.position.y);
                inventory.RemoveItem(inventory.curSelectedItem);
                if (isGhostObjectSpawned)
                {
                    Destroy(barricadeGhost);
                    isGhostObjectSpawned = false;
                }
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

        if (inventory.curSelectedItem != null) {
            if (inventory.curSelectedItem.name == "Barricade" && isGhostObjectSpawned == false)
            {
                GameObject loadedGO = Resources.Load("BarricadeGhost", typeof(GameObject)) as GameObject;
                barricadeGhost = Instantiate(loadedGO) as GameObject;
                barricadeGhost.transform.position = buildCheckGO.transform.position;
                isGhostObjectSpawned = true;
            }

        }

        if (inventory.curSelectedItem.name != "Barricade" || inventory.curSelectedItem.id == -1)
        {
            Destroy(barricadeGhost);
            isGhostObjectSpawned = false;
        }


        if (isGhostObjectSpawned)
        {
            barricadeGhost.transform.position = buildCheckGO.transform.position;
            if (Input.GetKeyUp(KeyCode.F))
            {
                CmdSpawnBarricadeOnServer(buildCheckGO.transform.position.x, buildCheckGO.transform.position.y);
                Destroy(barricadeGhost);
                isGhostObjectSpawned = false;
                inventory.RemoveItem(inventory.curSelectedItem);
            }
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


        if (MC.allowSwing)
        {
            if (Input.GetKeyUp(KeyCode.F)) {
                if (MC.curCollided.tag == "Barricade")
                {
                    CmdDamageBarricade(25, MC.curCollided.GetComponent<NetworkIdentity>().netId);
                }
                else if (MC.curCollided.tag == "Player") {
                    CmdStunPlayer(5, MC.curCollided.GetComponent<NetworkIdentity>().netId);
                }
            }
        }

        lastItem = inventory.curSelectedItem;
    }

    [Command]
    private void CmdStunPlayer(int time, NetworkInstanceId playerID) {
        GameObject player = NetworkServer.FindLocalObject(playerID);
        CharController playerController = player.GetComponent<CharController>();
        playerController.isStunned = true;
        playerController.timer = time + Time.time;
    }

    [ClientRpc]
    private void RpcUnstunPlayer() {
        isStunned = false;
    }

    [Command]
    private void CmdDamageBarricade(int dmg, NetworkInstanceId barId) {
        Barricade bar = NetworkServer.FindLocalObject(barId).GetComponent<Barricade>();
        bar.health -= dmg;
        if (bar.health <= 0) {
            NetworkServer.Destroy(bar.gameObject);
        }
    }

    [Command]
    private void CmdSpawnBarricadeOnServer(float xPos, float yPos) {
        GameObject loadedGO = Resources.Load("BarricadeGO", typeof(GameObject)) as GameObject;
        GameObject barricadeGO = Instantiate(loadedGO) as GameObject;
        barricadeGO.transform.position = new Vector3(xPos, yPos);
        NetworkServer.Spawn(barricadeGO);
    }

    [Command]
    private void CmdSpawnItemOnServer(string _name, float xPos, float yPos) {
        GameObject loadedGO = Resources.Load(_name, typeof(GameObject)) as GameObject;
        GameObject instGO = (GameObject)Instantiate(loadedGO);
        instGO.transform.position = new Vector2(xPos, yPos);
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
        if (isStunned)
        {
            return;
        }
        //Bewegung Horizontal
        float x = 0;
        x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }



}
