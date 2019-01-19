using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInfo : MonoBehaviourPunCallbacks
{
    public string characterName;
    public string characterInfo;
    public bool playable;
    public bool isSelectable;

    public void WakeUp()
    {
        isSelectable = true;
        if (name != "Default")
        {
            characterName = name + " Dude";
            characterInfo = name + " Dude has a " + name + " weapon that does " + name + " damage. " + name + " Dude's favorite color is not " + name + ".";
            Debug.Log(characterName + " Slectable? " + isSelectable);
        }
        else
        {
            characterName = "";
            characterInfo = "Select a character!";
        }        
    }

    public void SelectChamp()
    {
        if (isSelectable)
        {
            Debug.Log(name + " has been selected.");
            isSelectable = false;
            Debug.Log("Selectable? " + isSelectable);
            // change sprite to selected sprite
        }
        else if (!isSelectable)
        {
            Debug.Log(name + " has been de-selected.");
            isSelectable = true;
            Debug.Log("Selectable? " + isSelectable);
            // change sprite to base sprite
        }
    }
}