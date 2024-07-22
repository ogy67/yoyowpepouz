using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class writeTextAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // vider les lignes
        foreach (Transform t in contentItemText_line1)
            Destroy(t.gameObject);
        foreach (Transform t in contentItemText_line2)
            Destroy(t.gameObject);
    }

    public bool debug_btnRewriteText = false;
    private string debug_currentText;
    // Update is called once per frame
    void Update()
    {
        if (debug_btnRewriteText)
        {
            debug_btnRewriteText = false;
            writeText(debug_currentText);
        }
    }

    private static writeTextAnim instance;
    public static writeTextAnim getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("writeTextAnim").GetComponent<writeTextAnim>();
        return instance;
    }

    public void deleteText()
    {
        foreach (Transform t in contentItemText_line1)
            Destroy(t.gameObject);
        foreach (Transform t in contentItemText_line2)
            Destroy(t.gameObject);

        listItemText = new List<itemTextAnim>();
    }

    public Transform contentItemText_line1, contentItemText_line2;
    public HorizontalLayoutGroup layout_1, layout_2;
    public static float totalTimeAnimText;
    public void writeText(string s)
    {
        debug_currentText = s;
        // vider les lignes
        deleteText();

        // on remplit les lignes avant d'animer le text
        fillLinesWithItems(s);
        //layout_1.SetLayoutHorizontal();
        //layout_2.SetLayoutHorizontal();
        /*layout_1.enabled = false;
        layout_2.enabled = false;
        layout_1.enabled = true;
        layout_2.enabled = true;*/
        // on anim le text
        animateTextOn();
    }

    public float initDelay = 0.2f;
    public int nbCharPerSec = 10;
    private void animateTextOn()
    {
        float timeBetweenChar = (float)1 / nbCharPerSec;
        float currentTime = Time.time;
        totalTimeAnimText = initDelay;

        foreach(itemTextAnim item in listItemText)
        {
            item.animItemText_on(currentTime + initDelay);
            currentTime += timeBetweenChar;
            totalTimeAnimText += timeBetweenChar;
        }
        totalTimeAnimText += itemTextAnim.timeAnimFadeColor;
    }

    // appelé quand finit level et change texte
    public void animateTextOff()
    {
        //float timeBetweenChar = (float)1 / nbCharPerSec;
        //float currentTime = Time.time;

        foreach (itemTextAnim item in listItemText)
        {
            item.animItemText_off();
            //currentTime += timeBetweenChar;
        }
    }

    // appelé par tutoManager
    public void btnOkHelp(bool toGame)
    {
        // on anim texte fade invisible -> alpha=0.5
        // on anim texte fade visible -> alpha=1
        foreach(itemTextAnim item in listItemText)
        {
            item.btnOkHelp(toGame);
        }
    }


    public GameObject itemText;
    public char changeLineChar = '+';
    public char spaceHiddentChar = '-';
    private List<itemTextAnim> listItemText;
    private void fillLinesWithItems(string s)
    {
        listItemText = new List<itemTextAnim>();

        char c;
        string currentString = "";
        string currentString_black = "";
        string itemTextString;
        Transform currentLine = contentItemText_line1;

        for (int i = 0; i < s.Length; i++)
        {
            c = s[i];
            if (c == '+')
            {
                // changement de ligne
                currentLine = contentItemText_line2;
            }
            else
            {
                if (c == '<')
                {
                    string imgString = "" + c;
                    int i2 = i;
                    do
                    {
                        i2++;
                        imgString += s[i2];
                    } while (s[i2] != '>');

                    i = i2;
                    currentString += imgString;
                    currentString_black += imgString;
                    //infoTMP.SetText(currentString + "|");
                    //infoTMPShadow.SetText(currentString_black + "|");
                    itemTextString = imgString;
                }
                else if (c == ' ')
                {
                    itemTextString = "" + spaceHiddentChar;
                }
                else
                {
                    itemTextString = "" + c;
                }

                // creer un nouvel item et le mettre dans le content associé
                // le mettre dans currentLine
                itemTextAnim item = Instantiate(itemText, currentLine).GetComponent<itemTextAnim>();
                listItemText.Add(item);
                item.gameObject.SetActive(true);
                item.setItemText(itemTextString, c == ' ');
            }
        }
    }
}
