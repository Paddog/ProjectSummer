using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    public float dotCooldown = 1;
    public int dotDamage = 1;


    [SyncVar(hook = "CheckHealth")][Range(0, 100)]
    public int health = 100;
    [SyncVar(hook = "CheckBreath")][Range(0, 100)]
    public int breathAmount = 100;

    [SyncVar(hook = "Dead")]
    public bool isAlive = true;
    [SyncVar]
    public bool isChoking = false;

    private float timestamp = 0f;
    void Update() {
        if(!isLocalPlayer)
            return;


        if(Input.GetKey(KeyCode.K)) {
            CmdTakeBreath(3);
        }

        if(isChoking && timestamp < Time.time) {
            CmdTakeDamage(dotDamage);
            timestamp = Time.time + dotCooldown;
        }
    }


    public void CheckHealth(int _health) {
        if(health <= 0) {
            isAlive = false;
            CmdSendIsAliveStatus(false);
            
        } else {
            isAlive = true;
            CmdSendIsAliveStatus(true);
            
        }
    }

    public void CheckBreath(int _breath) {
        if(breathAmount <= 0) {
            isChoking = true;
            CmdSendIsChokingStatus(true);
            
        } else {
            isChoking = false;
            CmdSendIsChokingStatus(false);
            
        }
    }

    [Command]
    public void CmdSendIsAliveStatus(bool _isAlive) {
        isAlive = _isAlive;
    }

    [Command]
    public void CmdSendIsChokingStatus(bool _isChoking) {
        isChoking = _isChoking;
    }

    [Command]
    public void CmdTakeDamage(int _amount) {
        health -= _amount;
    }

    [Command]
    public void CmdTakeBreath(int _amount) {
        breathAmount -= _amount;
    }

    private void Dead(bool _isAlive) {
        if(_isAlive) {
            Debug.Log("Player Alive!");
        } else {
            Debug.Log("Player Dead!");
            //TODO: Disable components go to scene view spectate etc
        }
    }
    
    
}
