using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiusTest : MonoBehaviour
{
    public float smoothSpeed = 0f;
    public Image smoothSlider;
    public Text smoothNum;
    public float cameraRadius = 0.5f;
    public Image cameraSlider;
    public Text cameraNum;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && cameraRadius != 20)
        {
            if(cameraRadius < 20)
            {
                cameraRadius += .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && cameraRadius != 0.5)
        {
            if (cameraRadius > 0.5)
            {
                cameraRadius -= .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && smoothSpeed != 1)
        {
            if (smoothSpeed < 1f)
            {
                smoothSpeed += 0.1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && smoothSpeed != 0)
        {
            if (smoothSpeed - 0.1f < 0)
            {
                smoothSpeed = 0f;
            }

            if (smoothSpeed > 0)
            {
                smoothSpeed -= 0.1f;
            }
        }

        cameraSlider.fillAmount = cameraRadius / 20;
        cameraNum.text = cameraRadius.ToString();
        smoothSlider.fillAmount = smoothSpeed / 1;
        smoothNum.text = smoothSpeed.ToString();
    }
}
