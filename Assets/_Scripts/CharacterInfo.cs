using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInfo : MonoBehaviourPunCallbacks
{
    public Sprite baseSprite;
    public Sprite selectedSprite;
    public string characterName;
    public string characterInfo;
    public bool playable;
    public bool isSelectable = true;

    private SpriteRenderer spr;

    public void WakeUp()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = baseSprite;
        if(name != "Default")
        {
            characterName = name + " Dude";
            characterInfo = name + " Dude has a " + name + " weapon that does " + name + " damage. " + name + " Dude's favorite color is not " + name + ".";
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
            spr.sprite = selectedSprite;
            Debug.Log("Selectable? " + isSelectable);
            // change sprite to selected sprite
        }
        else if (!isSelectable)
        {
            Debug.Log(name + " has been de-selected.");
            isSelectable = true;
            spr.sprite = baseSprite;
            Debug.Log("Selectable? " + isSelectable);
            // change sprite to base sprite
        }
    }
}