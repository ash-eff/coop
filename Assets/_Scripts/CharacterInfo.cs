using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour {

    public string characterName;
    public string characterInfo;

    private void Awake()
    {
        characterName = this.name + " Dude.";
        characterInfo = this.name + " Dude has a " + this.name + " weapon that does " + this.name + " damage. " + this.name + "'s favorite color is not " + this.name + ".";
    }
}
