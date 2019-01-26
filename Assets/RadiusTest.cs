using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiusTest : MonoBehaviour
{
    public float cursorRadius = 0.5f;
    public float cameraRadius = 0.5f;
    public Image cursorSlider;
    public Image cameraSlider;
    public Text cursorNum;
    public Text cameraNum;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && cursorRadius != 20)
        {
            if(cursorRadius < 20)
            {
                cursorRadius += .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && cursorRadius != 0.5)
        {
            if (cursorRadius > 0.5)
            {
                cursorRadius -= .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && cameraRadius != 20)
        {
            if (cameraRadius < 20)
            {
                cameraRadius += .5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && cameraRadius != 0.5)
        {
            if (cameraRadius > 0.5)
            {
                cameraRadius -= .5f;
            };
        }

        cursorSlider.fillAmount = cursorRadius / 20;
        cursorNum.text = cursorRadius.ToString();
        cameraSlider.fillAmount = cameraRadius / 20;
        cameraNum.text = cameraRadius.ToString();
    }
}
