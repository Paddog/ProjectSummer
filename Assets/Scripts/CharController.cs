using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public enum CharacterStates {
    ducken,
    springen,
    normal
}


//CRITICAL: Synchronization on the network
[RequireComponent(typeof(Rigidbody2D))]
public class CharController : NetworkBehaviour {

    public float speed;
    public float jumpPower;

    private bool grounded = true;
    public Transform groundCheck;
    public float radius;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private CharacterStates cs;

    private bool isDuck = false;


    private float cooldown = 1;
    private float timestamp = 0;

    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        cs = CharacterStates.normal;
	}
	

    /*
     * 
     * Character muss sich bewegen links rechts springen ducken x /// klettern
     * 
     * 
     * 
     * 
     * 
     * Character animation,
     * Character states isDucken true, isJumpen true, isBewegung true, x         isInteragieren true, 
     * 
    */


	void Update () {

        if (!isLocalPlayer) { return; }


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
        //TODO: Ducken
        
        if (Input.GetKeyDown(KeyCode.C) && timestamp <= Time.time)
        {
            timestamp = Time.time + cooldown;
            isDuck = !isDuck;
            CmdCrouchToggle(isDuck);
        }

    }

    [Command]
    public void CmdCrouchToggle(bool id) {
        Debug.LogError("Server: CrouchToggle()");
        BoxCollider2D bc2d = this.GetComponent<BoxCollider2D>();
        if (id == true)
        {
            //CRITICAL: Isnt relative to size, be careful if you have another size then 4!
            bc2d.size = new Vector2(bc2d.size.x, bc2d.size.y / 2);
            bc2d.offset = new Vector2(bc2d.offset.x, bc2d.size.y / -2);
            cs = CharacterStates.ducken;
        }
        else
        {
            bc2d.size = new Vector2(bc2d.size.x, bc2d.size.y * 2);
            bc2d.offset = new Vector2(0, 0);
        }
        Debug.LogError("Sending info to client!");
        RpcSendColliderInfo(bc2d.size.x, bc2d.size.y, bc2d.offset.x, bc2d.offset.y);
    }

    [ClientRpc]
    public void RpcSendColliderInfo(float x, float y, float offsetx, float offsety) {
        Debug.LogError("Recieving Info!");
        this.GetComponent<BoxCollider2D>().size = new Vector2(x, y);
        this.GetComponent<BoxCollider2D>().offset = new Vector2(offsetx, offsety);
    }


    void FixedUpdate(){
        if (!isLocalPlayer) { return; }
        //Bewegung Horizontal
        float x = 0;
        float y = 0;
        x = Input.GetAxis("Horizontal");
        

        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }



}
