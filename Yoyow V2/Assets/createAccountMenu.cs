using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class createAccountMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*public bool test_btnInitMenu = false;

    // Update is called once per frame
    void Update()
    {
        if (test_btnInitMenu)
        {
            test_btnInitMenu = false;
            initMenu();
        }
    }*/

    public void initMenu()
    {
        inputName.characterLimit = maxCharInName;
        accountCreated_closing = false;
        openMenu();
    }


    public Animation openMenuAnim;
    public void openMenu()
    {
        openMenuAnim.Play("openMenuCreateAccount");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.createAccount_open);
    }

    public void closeMenu()
    {
        if (errorNameOpen)
        {
            errorNameOpen = false;
            errorNameAnim.Play("errorNameUsedClose");
        }

        openMenuAnim.Play("closeMenuCreateAccount");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.createAccount_close);

        // soit fermé par btn quit
        // soit fermé par noConnection
        // soit fermé par goNextPhaseConnection
    }

    public Animation btnOkAnim;
    public InputField inputName;
    public Text ombreText;
    private bool btnOkOn = false;
    public int minCharInName = 3;
    public int maxCharInName = 10;
    public void inputNameValueChanged()
    {
        if (errorNameOpen)
        {
            errorNameOpen = false;
            errorNameAnim.Play("errorNameUsedClose");
        }
        if(inputName.text.Length < minCharInName)
        {
            if (btnOkOn)
            {
                // desactiver btnOk
                btnOkAnim.Play("btnOkCreateAccountDisabled");

                btnOkOn = false;
            }
        }
        else
        {
            if (!btnOkOn)
            {
                // activer btnOk
                btnOkAnim.Play("btnOkCreateAccountEnabled");

                btnOkOn = true;
            }
        }
        ombreText.text = inputName.text;
    }


    public void btnOk()
    {
        if (btnOkOn && !accountCreated_closing)
        {
            StartCoroutine(waitCreateAccount());
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.createAccount_btnOk);
        }
    }

    public rankingInfinis rankManager;
    public onlineRankingManager onlineManager;
    public Animation waitAnim;
    public float timeCreateAccountTest = 0.7f;

    private string nameTested = "";
    private IEnumerator waitCreateAccount()
    {
        nameTested = inputName.text;
        waitAnim.Play("waitCreateAccountOpen");
        yield return new WaitForSeconds(timeCreateAccountTest);

        // check si base contient deja un joueur avec le meme nom
        onlineManager.checkIfNamePlayerAlreadyUsed(nameTested);

        yield return new WaitForSeconds(0.3f);
        waitAnim.Play("waitCreateAccountClose");
    }

    private bool accountCreated_closing = false;
    public IEnumerator waitCreateAccount_resultNameUsed(bool nameAlreadyUsed)
    {
        //Debug.Log("waitCreateAccount_resultNameUsed");
        // si oui, message erreur nom deja utilisé
        // test nameUsed
        if (nameAlreadyUsed) // nameAlreadyUsed
        {
            //Debug.Log("(nameAlreadyUsed) waitCreateAccount_resultNameUsed");
            //Debug.Log("Name already used, retry");
            openErrorNameAlreadyUsed();
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.createAccount_errorNameUsed);
        }

        // si non, fermer menu create account et passer à l'ouverture du ranking
        else
        {
            //Debug.Log("Name valid");
            accountCreated_closing = true;
            //Debug.Log("(close) waitCreateAccount_resultNameUsed");
            yield return new WaitForSeconds(0.3f);
            // on ferme le menu
            //onlineRankingManager.initAutoLogSucced = true;
            closeMenu();

            // creer un compte avec mdp aléatoire, save data
            //createAccountIds(nameTested);

            //Debug.Log("(close2) waitCreateAccount_resultNameUsed");
            yield return new WaitForSeconds(0.2f);
            managerRankInfinis.getInstance().playerPassedCreateAccountPhase(true);

            //Debug.Log("(close) waitCreateAccount_resultNameUsed");
        }
    }



    public Animation errorNameAnim;
    private bool errorNameOpen = false;
    private void openErrorNameAlreadyUsed()
    {
        inputName.text = "";
        inputNameValueChanged();
        errorNameAnim.Play("errorNameUsedOpen");
        errorNameOpen = true;
    }

    /*private static string aleatoryString = "abcdefghijklmnopqrstuvwxyz" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "1234567890" +
        "éêèàâôîûù" +
        "&@$£";*/
    //private string rejectChar = "<>/.,;:!)(\"\'*-{}[]£";

    public int lengthMdp = 15;
    /*private void createAccountIds(string namePlayer)
    {
        string mdpStr = "";
        while (mdpStr.Length != lengthMdp)
            mdpStr += aleatoryString[Random.Range(0, aleatoryString.Length)];
        Debug.Log("New mdp : " + mdpStr+ " , new id : "+namePlayer);

        SaveSystem.Save(typeSave.ONLINE_AUTH_VALS, new List<string>() { namePlayer, mdpStr });
    }*/
}
