using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class testQualityScreen : MonoBehaviour
{
    public Text qualityText, resolutionText, editorModeText;
    public GameObject menuQualityGo;
    public GameObject waitQualityRefresh;
    public Dropdown m_DropdownResolution, m_DropDownQuality;

    private static Resolution[] allResolutionAvailable_Phone;

    // 0 : Very Low , 1 : Low , 2 : Medium , 3 : High , 4 :: Very High
    //private int defaultQuality = 2;

    //public GameObject makeGameCrash;
    /* void Start()
     * void fillDropDown()
     */
    #region INITIALISATION

    void Awake()
    {
        Application.targetFrameRate = 60;
        // On veut mettre la qualité "Medium" par défaut, qui après test est celle offrant une fluidité minimale
        //QualitySettings.SetQualityLevel(defaultQuality, true);




        //makeGameCrash.GetComponent<launcher>();
        /* Test manuel de la qualité avec le menu de test
        editorModeText.text = "EDITOR MODE : " + (Application.isEditor ? "On" : "Off");

        fillResolutionDropDown();
        fillDropQualityDown();*/
    }

    private void fillDropQualityDown()
    {
        // on remplit le dropDown de qualitées disponibles

        string[] qualityNames = QualitySettings.names; // les différentes qualité de résolution disponible
        int currentQualityIndex = QualitySettings.GetQualityLevel();

        List<string> allQuality = new List<string>();
        for (int i = 0; i < qualityNames.Length; i++)
        {
            allQuality.Add(qualityNames[i]);
        }

        m_DropDownQuality.ClearOptions();
        m_DropDownQuality.AddOptions(allQuality);
        m_DropDownQuality.value = currentQualityIndex;
        Debug.Log("Current quality name : " + qualityNames[currentQualityIndex]);
        showCurrentQuality(qualityNames[currentQualityIndex]);
    }



    // Nombre de choix possibles : 
    // 1 resolution inférieure à _maxScreenHeight
    //   OU , si defautRes est déja la résolution max ET que 2e resolution > _minScreenHeight
    // 2 resolution > _maxScreenHeight

    // Dans les test PC, trop de resolutions qui ne respectent pas le ratio, resolution très basses
    // -> verifier que PHONE ne fait pas ça
    // PHONE , verifier que waitQualityUpdated active image wait , sinon -> supprimer func car inutile

    // On met toutes les résolutions disponible ( même superieur à _maxScreenHeight )
    private void fillResolutionDropDown()
    {
        List<string> finalOptions = new List<string>();
        int currentResolutionIndex = 0;

        allResolutionAvailable_Phone = Screen.resolutions;

        int i = 0;
        foreach (Resolution res in allResolutionAvailable_Phone)
        {
            finalOptions.Add(res.height + " x " + res.width);
            if (Screen.currentResolution.width == Screen.resolutions[i].width
                && Screen.currentResolution.height == Screen.resolutions[i].height)
            {
                currentResolutionIndex = i;
            }
            i++;
        }
        m_DropdownResolution.ClearOptions();
        m_DropdownResolution.AddOptions(finalOptions);
        m_DropdownResolution.value = currentResolutionIndex;
        m_DropdownResolution.RefreshShownValue();
        showCurrentResolution();
    }

    #endregion

    /* void btnFullScreen(Toggle fullScreenToggle)
     * void resolution_DropdownValueChanged(Dropdown change)
     * void quality_DropdownValueChanged(Dropdown change)
     *  IEnumerator waitQualityUpdated(int indexValueWaited, string newQualityStr)
     *  
     * void showCurrentQuality(string newQuality)
     * void showCurrentResolution()
    */
    #region Interaction avec l'UI


    public void btnOpenMenuResolution()
    {
        menuQualityGo.SetActive(!menuQualityGo.activeSelf);
    }

    public void btnFullScreen(Toggle fullScreenToggle)
    {
        bool isOn = fullScreenToggle.isOn;
        Debug.Log("Toogle changed state to : " + isOn);
        Screen.fullScreen = isOn;
    }

    public void resolution_DropdownValueChanged(Dropdown change)
    {
        Resolution res = allResolutionAvailable_Phone[change.value];
        Debug.Log("Resolution choosen , y : " + res.height + " x : " + res.width);
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        showCurrentResolution();
    }

    private int idClick = 0; // verifier si n'a pas recliqué entre une transition de qualité
    public void quality_DropdownValueChanged(Dropdown change)
    {
        idClick++;
        QualitySettings.SetQualityLevel(change.value, true);
        StartCoroutine(waitQualityUpdated(change.value, change.options[change.value].text));
    }

    // La qualité ne se met pas directement à jour
    private IEnumerator waitQualityUpdated(int indexValueWaited, string newQualityStr)
    {
        int idClick_copy = idClick;
        waitQualityRefresh.SetActive(true);
        while (QualitySettings.GetQualityLevel() != indexValueWaited || idClick_copy != idClick)
            yield return null;
        // Si le joueur ne reclique pas sur une autre qualité avant qu'elle soit à jour
        if (idClick_copy == idClick)
        {
            waitQualityRefresh.SetActive(false);
            showCurrentQuality(newQualityStr);
        }
    }

    private void showCurrentQuality(string newQuality)
    {
        qualityText.text = "Current Quality : " + newQuality;
        Debug.Log("Quality is showned : " + newQuality + "("+QualitySettings.names[QualitySettings.GetQualityLevel()] + ")");
    }

    private void showCurrentResolution()
    {
        resolutionText.text = "Current Resolution : " + Screen.currentResolution.height + " x " + Screen.currentResolution.width;
    }

    #endregion

    /* Vector2 getPhoneScreenResolution()
    */
    #region GET

    private Vector2 getPhoneScreenResolution()
    {
        Resolution r = Screen.currentResolution;
        return new Vector2(r.width, r.height);
    }

    #endregion


    // pour une experience optimale et fluide, la resolution maximale en hauteur doit être fixée
    // la resolution choisit par défaut doit garantir une fluidité et haut fps
    private static int _maxScreenHeight = 1080; // Test pour ecran pc : 1600 , PHONE : 2000
    //private static int _minScreenHeight = 500; // Test pour ecran pc : 500 , PHONE : 800

    public void btn_setPreferedScreenResolution()
    {
        setPreferedScreenResolution();
    }

    public void setPreferedScreenResolution()
    {
        // On va parcourir toutes les résolution disponibles et prendre celle qui est la + proche de _maxScreenHeight
        int currentResolutionHeight = 0;
        Resolution finalResolution = Screen.currentResolution;

        foreach (Resolution res in Screen.resolutions)
        {
            int height = res.height;
            if (height > currentResolutionHeight && height < _maxScreenHeight)
            {
                currentResolutionHeight = height;
                finalResolution = res;
            }
        }

        Screen.SetResolution(finalResolution.width, finalResolution.height, Screen.fullScreen);
        Debug.Log("Resolution la + optimisé , width : " + finalResolution.width + " height : " + finalResolution.height);
        showCurrentResolution();
    }
}
