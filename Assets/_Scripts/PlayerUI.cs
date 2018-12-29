using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    #region Public Fields

    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    #endregion

    #region Private Fields
    private PlayerConnectionManager target;

    //float yOffset = 2f;
    Vector3 targetPosition;
    public Transform targetTransform;

    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;


    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;


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
            playerHealthSlider.value = target.Health;
        }
    
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void LateUpdate()
    {
        if(targetPosition != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += 1f;
            transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
    
    #endregion
    
    
    #region Public Methods
    
    public void SetTarget(PlayerConnectionManager _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;
        targetTransform = target.GetComponent<Transform>();
    
        // TODO use bounds here for when art changes
        //characterHeight = 1;
    
        if (playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }
    }
    
    #endregion
}