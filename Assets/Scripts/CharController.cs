using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CharacterStates {
    ducken,
    springen,
    normal
}


//CRITICAL: Synchronization on the network
[RequireComponent(typeof(Rigidbody2D))]
public class CharController : MonoBehaviour {

    public float speed;
    public float jumpPower;

    private bool grounded = true;
    public Transform groundCheck;
    public float radius;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private CharacterStates cs;


	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        cs = CharacterStates.normal;
	}
	

    /*
     * Vectoren 2d
     * Character muss sich bewegen links rechts springen ducken /// klettern
     * 
     * 
     * 
     * 
     * 
     * Character animation,
     * Character states isDucken true, isJumpen true, isBewegung true, isInteragieren true,
     * 
    */


	void Update () {
        //springen und ground check
        grounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);

        if (grounded == false)
        {
            cs = CharacterStates.springen;
        }
        else {
            cs = CharacterStates.normal;
        }

        if (Input.GetKeyUp(KeyCode.Space) && cs == CharacterStates.normal){
            Debug.LogError("Springen!");
            rb.AddForce(new Vector2(0, jumpPower));
            cs = CharacterStates.springen;
        }
        //TODO: Ducken
        

    }

    void FixedUpdate(){

        //Bewegung Horizontal
        float x = 0;
        float y = 0;
        x = Input.GetAxis("Horizontal");
        

        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }



}
