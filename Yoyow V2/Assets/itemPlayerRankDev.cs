using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemPlayerRankDev : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Text nameText, nameText_ombre;
    public Text rankText, rankText_ombre;
    public Text scoreText;

    public GameObject botGo;

    public void initItem(string name, int rank, int score, bool isABot)
    {
        nameText.text = name;
        nameText_ombre.text = name;

        rankText.text = rank.ToString();
        rankText_ombre.text = rank.ToString();

        scoreText.text = score.ToString();

        botGo.SetActive(isABot);
    }

}
