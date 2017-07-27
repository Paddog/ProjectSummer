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

    public float speed;
    public float jumpPower;
    public float noMovementThreshold = 0.0001f;

    private bool grounded = true;
    public Transform groundCheck;
    public float radius;
    public LayerMask groundLayer;
    
    [SerializeField]
    public DialogController DC;

    private Rigidbody2D rb;
    private CharacterStates cs;

    private bool isDuck = false;


    private float cooldown = 1;
    private float timestamp = 0;

    private const int noMovementFrames = 3;
    private Vector3[] previousLocations = new Vector3[noMovementFrames];


    private CameraController mainCamera;

    public GameObject interactableGO;

    void Awake() {
        if(!isLocalPlayer)
            return;




        for(int i = 0; i < previousLocations.Length; i++) {
            previousLocations[i] = Vector3.zero;
        }
    }

    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        cs = CharacterStates.normal;
        mainCamera = GameObject.Find("SceneCamera").GetComponent<CameraController>();

        if(isLocalPlayer) {
            mainCamera.mainChar = this.gameObject;
            DC = GameObject.Find("Canvas").GetComponent<DialogController>();
        }
	}


    //CRITICAL: If you are host and client this dosnt work for some reason!
    void EnterCode() {
        if (!isLocalPlayer) {
            return;
        }

        InputField inputF = DC.GetUiElement(interactableGO.tag).transform.GetChild(2).GetComponent<InputField>();
        //If the Code is correct
        if (interactableGO.GetComponent<KeyLock>().EnterCode(int.Parse(inputF.text))) {
            if (interactableGO.GetComponent<KeyLock>().isLocked == true)
            {
                Debug.LogError("Unlocking!");
                CmdSendKeyLockState(false, interactableGO.GetComponent<NetworkIdentity>().netId);
                
                DC.HideDialog(interactableGO.tag);
            }
            else {
                Debug.LogError("Locking!");
                CmdSendKeyLockState(true, interactableGO.GetComponent<NetworkIdentity>().netId);
                
                DC.HideDialog(interactableGO.tag);
            }

        }
        cs = CharacterStates.normal;
    }

    [Command]
    public void CmdSendKeyLockState(bool state, NetworkInstanceId id) {
        Debug.LogError("Sending Command!");
        NetworkServer.FindLocalObject(id).GetComponent<KeyLock>().isLocked = state;
        NetworkServer.FindLocalObject(id).GetComponent<KeyLock>().conDoorScript.isLocked = state;
        NetworkServer.FindLocalObject(id).GetComponent<KeyLock>().SyncKeyLockState();
    }

	void Update () {

        if (!isLocalPlayer) { return; }

        if (interactableGO != null && Input.GetKeyUp(KeyCode.E)) {
            Debug.LogError("Interacting!");
            DC.ShowDialog(interactableGO.tag);
            GameObject uiInteractGO = DC.GetUiElement(interactableGO.tag);
            GameObject buttonOK = uiInteractGO.transform.GetChild(0).gameObject;

            buttonOK.GetComponent<Button>().onClick.AddListener(() => { EnterCode(); });
            cs = CharacterStates.interagieren;
        }


        for(int i = 0; i < previousLocations.Length - 1; i++) {
            previousLocations[i] = previousLocations[i + 1];
        }
        previousLocations[previousLocations.Length - 1] = this.transform.position;

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
    public void CmdCrouchToggle(bool id) {
        Debug.LogError("Server: CrouchToggle()");
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

    //CRITICAL: Not working lastpos and curpos is the same position!
    public bool? isMoving() {
        for(int i = 0; i < previousLocations.Length - 1; i++) {
            if(Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold) {
                //The minimum movement has been detected between frames
                return true;
            } else {
                return false;
            }
        }

        return null;
    }


}
