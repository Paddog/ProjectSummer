using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour {
    
    public List<GameObject> uiElements = new List<GameObject>();


    void Start() {

    }

    //CharController->ShowDialog("KeyLock")->Zeigt Dialog KeyLock an-> 
    //Bitte Zeige den Dialog mit dem string(namen) "KeyLock" an.
    //Kontrolliere die Interaktion zwischen Ui und Spieler

    public void ShowDialog(string name) {
        foreach(GameObject element in uiElements)
        {
            if(element.name == name)
            {
                element.SetActive(true);
            }
        }
    }
    public void HideDialog(string name)
    {
        foreach(GameObject element in uiElements)
        {
            if(element.name == name)
            {
                element.SetActive(false);
            }
        }
    }

    public GameObject GetUiElement(string name) {
        foreach (GameObject element in uiElements) {
            if (element.name == name)
            {
                return element;
            }
            else {
                return null;
            }
        }
        return null;
    }
}
