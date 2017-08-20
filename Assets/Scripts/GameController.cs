using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//TODO: Start, Restart
public class GameController : MonoBehaviour {
    public List<string> finishingOrder = new List<string>();

    private bool timerStarted = false;

    public float finishingTime = 30f;
    public float timeStamp;
    private void Update(){
        if (timerStarted) {
            timeStamp = Time.time + finishingTime;
        }
        if (timeStamp < Time.time)
        {
            //TODO: Update UI timer;
            if (finishingOrder.Count > 3) {
                //TODO: Finish Game
            }
        }
        else {
            //TODO: Finish Game
        }
    }

    public void AddFinisher(string name) {
        finishingOrder.Add(name);
        if (finishingOrder.Count > 1) {
            timerStarted = true;
        }
    }
    
}
