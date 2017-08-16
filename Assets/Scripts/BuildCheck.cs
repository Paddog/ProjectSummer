using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCheck : MonoBehaviour {

    public GameObject character;

    public Vector3 curPos;
    public Vector3 lastPos;


	void Start () {
        character = this.transform.parent.gameObject;
	}
	

	void Update () {
        curPos = character.transform.position;
        if (curPos.x != lastPos.x)
        {
            if (curPos.x > lastPos.x)
            {
                this.transform.localPosition = new Vector2(2f, this.transform.localPosition.y);
            }
            else
            {
                this.transform.localPosition = new Vector2(-2f, this.transform.localPosition.y);
            }
        }

        lastPos = curPos;
	}


}
