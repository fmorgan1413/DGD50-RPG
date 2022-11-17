using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    // TODO: transition characters on screen
    public GameObject fadingSquare;

    public GameObject player1;
    public GameObject player2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeSquare());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeSquare(bool fadeToNotBlack = true, int fadeSpeed = 20)
    {
        Color objectColor = fadingSquare.GetComponent<Image>().color;
        float fadeAmount;

        if(fadeToNotBlack)
        {
            while(fadingSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                fadingSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }

    }
}
