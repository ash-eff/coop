using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviour {

    #region Private Fields
    public GameObject target;

    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;

    [Tooltip("UI Image to display Player's Health")]
    [SerializeField]
    private Image playerHealthSlider;

    [Tooltip("UI Image to display Player's Image")]
    [SerializeField]
    private Image playerImage;


    #endregion


    #region MonoBehaviour Callbacks

    private void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void Update()
    {
        // Reflect the Player Health
        if (playerHealthSlider != null)
        {
            //playerHealthSlider.fillAmount = target.GetComponent<PlayerCharacter>().health;
        }
        
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    #endregion
    
    
    #region Public Methods
    
    public void SetTarget(GameObject _target)
    {
        target = _target;
        
        if (playerNameText != null)
        {
            playerNameText.text = target.GetPhotonView().Owner.NickName;
        }

        if (playerImage != null)
        {
            playerImage.sprite = target.GetComponent<SpriteRenderer>().sprite;
        }
    }
    
    #endregion
}