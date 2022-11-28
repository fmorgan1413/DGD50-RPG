using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public GameObject fadingSquare;

    public GameObject player1;
    public GameObject player2;

    private Vector3 target1 = new Vector3(2.63f, 2.31f, 0.0f);
    private Vector3 target2 = new Vector3(3.35f, -1.18f, 0.0f);

    private float time = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartScreen(player1, player2, target1, target2, time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeSquare(bool fadeToNotBlack = true, int fadeSpeed = 15)
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

    public IEnumerator StartScreen(GameObject player1, GameObject player2, Vector3 targetPos1, Vector3 targetPos2, float duration) 
    {
        StartCoroutine(FadeSquare());
        yield return new WaitForSeconds(.5f);

        float time = 0;
        Vector3 startPos1 = player1.transform.position;
        Vector3 startPos2 = player2.transform.position;

        while (time < duration)
        {
            player1.transform.position = Vector3.Lerp(startPos1, targetPos1, time / duration);
            player2.transform.position = Vector3.Lerp(startPos2, targetPos2, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fadingSquare.SetActive(false);
        player1.transform.position = targetPos1;
        player2.transform.position = targetPos2;
    }
}
