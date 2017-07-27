using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject mainChar;
    public float velModifier = 2f;

    void Start() {

    }


    //TODO: Finish this system!
    void FixedUpdate() {
        Vector3 v1 = new Vector3(this.transform.position.x, this.transform.position.y, -10);
        Vector3 v2 = new Vector3(mainChar.transform.position.x, mainChar.transform.position.y, -10);
        this.transform.position = Vector3.Lerp(v1, v2, Time.deltaTime * velModifier);
    }
}
