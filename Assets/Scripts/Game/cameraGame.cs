using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

using UnityEngine.Rendering;

public class cameraGame : MonoBehaviour
{
    private void Start()
    {
        pPP = pPV.profile;

        originalPos = transform.localPosition;
        cam = Camera.main;
        

        chooseScreenRatio();

        if (!expertMode.modeExpertOn)
        {
            animFond3DGame.getInstance().initializeColorFond(initColor_normal);
            initColor_current = initColor_normal;
        }

        currentColorFondInfinis = colorInfinis;
    }

    float minRatioToChange = 18/9f, maxRatio = 20/9f;
    float minScale = 6.8f, maxScale = 7.3f;

    private void chooseScreenRatio()
    {
        float newScale = minScale;
        float myRatio = (float)Screen.height / (float)Screen.width;

        if(myRatio >= minRatioToChange)
        {
            float rapport = (myRatio - minRatioToChange) / (maxRatio - minRatioToChange);
            newScale = minScale + (maxScale - minScale) * rapport;
        }
        // si ratio > 2 , 'dezommer' 
        //originalScale = (float)Screen.height / (float)Screen.width >= 2f ? 6.8f : 6.5f;
        originalScale = newScale;
        Camera.main.orthographicSize = originalScale;

        //foreach (Camera c in levelInfinisCameras)
        //    c.orthographicSize = originalScale * camLvlInfinis_factorDiminution;
    }

    private void LateUpdate()
    {
        if (changeZoom)
        {
            //float diff = Mathf.Abs(goal - cam.orthographicSize);
            if(Mathf.Abs(goal - cam.orthographicSize) / goal < 0.001)
            {
                cam.orthographicSize = goal;
            }
            else
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, goal, Time.deltaTime * speed);
            }
            //Debug.Log("Anim zoom");
        }
    }

    public static cameraGame getInstance()
    {
        return GameObject.Find("Main Camera").GetComponent<cameraGame>();
    }

    #region BLOOM EFFECT
    public float timeBloomEffect = 0.5f;
    public float goalBloomEffect = 5f;

    /*public void bloomCamEffect()
    {
        StartCoroutine(bloomEffect());
    }

    private IEnumerator bloomEffect()
    {
        Bloom b;
        if (pPP.TryGetSettings<Bloom>(out b))
        {
            //b.enabled.value = true;
            float p, t = 0f, range = timeBloomEffect / 2, currentBloomEffectVal = 0f;

            while (t < timeBloomEffect)
            {
                p = t <= range ?
                    t / range
                    : (t - range) / range;

                currentBloomEffectVal = t <= range ?
                     goalBloomEffect * p
                     : goalBloomEffect - goalBloomEffect * p;

                b.intensity.Override(currentBloomEffectVal);
                yield return null;
                t += Time.deltaTime;

            }
            b.intensity.Override(0f);
            //b.enabled.value = false;
        }
    }*/
    #endregion

    #region SPEED EFFECT
    /*public float minEff, maxEff, timeEff;
    private bool speedEffectOn = false, lockPrevAnim = false;
    private float currentSpeedEffectVal;

    
    public void speedCamEffect(float percentOfMax)
    {
        float effectValue = minEff + (maxEff - minEff) * percentOfMax;
        if (speedEffectOn) lockPrevAnim = true;
        speedEffectOn = true;
        StartCoroutine(speedEffect(timeEff, effectValue));
    }

    public void pivotCamEffect()
    {
        float g = 0.8f, t = 0.3f;
        if (speedEffectOn) lockPrevAnim = true;
        speedEffectOn = true;
        StartCoroutine(speedEffect(t, g));
    }*/

    /*private IEnumerator speedEffect(float time, float goal)
    {
        ChromaticAberration c;

        // si est appelé cash après changement de vitesse : coupe une anim
        float timeStart = 0f;
        if (currentSpeedEffectVal != 0)
        {
            // on determine a partir de quel moment on doit reprendre l'effet
            float progDone = currentSpeedEffectVal / goal;
            if (progDone > goal) progDone = goal;
            timeStart = (time / 2) * progDone;
        }

        if (lockPrevAnim) 
            yield return null; // on attend d'être unlock

        if (pPP.TryGetSettings<ChromaticAberration>(out c))
        {
            float p, t = timeStart, range = time / 2;

            while ( t < time && lockPrevAnim == false)
            {
                p = t <= range ? 
                    t / range 
                    : (t - range) / range;

                currentSpeedEffectVal = t <= range ?
                     goal * p
                     : goal - goal * p;

                c.intensity.Override(currentSpeedEffectVal);
                yield return null;
                t += Time.deltaTime;

            }

            if (!lockPrevAnim)
            {
                c.intensity.Override(0);
                currentSpeedEffectVal = 0;
                speedEffectOn = false;
            }
            else
            {
                lockPrevAnim = false;
            }
        }
    }*/
    #endregion

    #region PLAYER COL EFFECT

    float originalScale;
    Vector3 originalPos;

    float duration = 0.1f;
    public float zoomBounce = 1.03f;//1.03f;



    //private bool isAlreadyShaking = false;


    public void camEffectPlayerCollision(bool zoomEffect)
    {
        if (zoomEffect)
            StartCoroutine(zoomBouncePlayer());
        else
            StartCoroutine(shakePlayer());
    }
    //
    private IEnumerator zoomBouncePlayer()
    {
        float scaleUp = zoomBounce * originalScale - originalScale;
        float addScale;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (elapsed < duration / 2f)
            {
                addScale = scaleUp * (elapsed / duration);
                //pPV.profile
            }
            else
            {
                addScale = scaleUp * (1 - (elapsed / duration));
            }
            cam.orthographicSize = originalScale + addScale;
            elapsed += Time.deltaTime;

            yield return null;
        }
        cam.orthographicSize = originalScale;
    }

    float magnitude = 0.1f;
    private IEnumerator shakePlayer()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    #endregion

    #region EFFET DE ZOOM , TRANSITION LEVEL

    public float zoomGoalLevel = 1.1f;
    public float zoomGoalLevel_infinis = 1.07f;
    public float duration_2 = 0.3f;


    public float speedZoomTuto = 4f;
    public float scaleZoomTuto;

    public float speedZoomBetLvl = 3f;
    public float scaleZoomBetLvl = 1.1f;
    public void zoomBetweenNormalLvl(bool zoomOut)
    {
        if (zoomOut)
            StartCoroutine(zom(speedZoomBetLvl, originalScale * scaleZoomBetLvl));
        else
            StartCoroutine(zom(speedZoomBetLvl, originalScale));
    }
    public void zoomTuto(bool zoomOut)
    {
        if (zoomOut)
            StartCoroutine(zom(speedZoomTuto, originalScale * scaleZoomTuto));
        else
            StartCoroutine(zom(speedZoomTuto, originalScale));
    }
    public void zoomLevel(bool zoomOut, float sP, bool infinis)
    {
        if (sP == 0f) sP = 3f;
        if (zoomOut)
            StartCoroutine(zom(sP, originalScale * (infinis ? zoomGoalLevel_infinis : zoomGoalLevel)));
        else
            StartCoroutine(zom(sP, originalScale));
    }
    
    public float speed = 3f;

    private bool changeZoom = false;
    private float goal = 0f;

    private IEnumerator zom(float sP, float zoomVal)
    {
        changeZoom = true;
        goal = zoomVal;

        speed = sP;

        while (cam.orthographicSize != goal)
        {
            yield return null;
        }

        changeZoom = false;
        speed = 3f; // remet valeure par defaut si a été modifié
    }

    #endregion


    public float timeShade_levelChange = 1f;
    public Color32 initColor_normal; // couleure de fond de base
    public Color32 initColor_expert;
    private Color32 initColor_current;
    public Color32[] levelsColor; // couleure en fonction du niveau, [0]:initColor 
    private Color32 currentColor_adventure;

    public void adventureGameMode_activated()
    {
        // rien faire pour le moment
        currentColor_adventure = initColor_current;
        //animFond3DGame.getInstance().changeColorFond3D(initColor_current);
    }

    // appelé quand ferme mode jeu (aventure)
    public void adventureGameMode_desactivated()
    {
        // couleure actuelle->init     
        if (!Equals(currentColor_adventure, initColor_normal) && !Equals(currentColor_adventure, initColor_expert))
        {
            StartCoroutine(swtich(currentColor_adventure, initColor_current, timeShade_levelChange));
            animFond3DGame.getInstance().changeColorFond3D(initColor_current);
        }
    }

    // appelé quand player finit level (aventure)
    public void playerWin_changeDifficultyColor(int levelWon)
    {
        // level 1->2 : couleure init->2
        // level 2->3 : couleure 2->3
        // level 3->finit : rien

        // si levelWon = 1
        // switch( levelsColor[0], levelsColor[1])
        // si levelWon = 2
        // switch( levelsColor[1], levelsColor[2])
        Color32[] c = expertMode.modeExpertOn ? levelsColor_expert : levelsColor;
        if (levelWon < 3)
        {
            StartCoroutine(swtich(c[levelWon - 1], c[levelWon], timeShade_levelChange));
            currentColor_adventure = c[levelWon];
            animFond3DGame.getInstance().changeColorFond3D(currentColor_adventure);
        }
    }

    // anim couleure de current à init
    public void playerReplay()
    {
        Color32 c = expertMode.modeExpertOn ? initColor_expert : initColor_normal;
        //Debug.Log("currentColor_adventure : " + currentColor_adventure + ", initC : " + c);
        if (!currentColor_adventure.Equals(c))
        {
            //Debug.Log("Anim switch color");
            StartCoroutine(swtich(currentColor_adventure, c, timeShade_levelChange));
            currentColor_adventure = c;
            animFond3DGame.getInstance().changeColorFond3D(c);
        }
    }


    public Color32[] levelsColor_expert; // couleure en fonction du niveau, [0]:initColor 
    public float timeShade_modeExpert = 0.5f;
    public void modeExpertSwitched(bool expertActivated)
    {
        StartCoroutine(swtich(
            !expertActivated ? initColor_expert : initColor_normal,
            !expertActivated ? initColor_normal : initColor_expert,
            timeShade_modeExpert));

        if(expertActivated)
            changeBorderFadeColor(false, false, true, false, timeShade_modeExpert);
        else
            changeBorderFadeColor(false, false, false, true, timeShade_modeExpert);

        initColor_current = expertActivated ? initColor_expert : initColor_normal;
        animFond3DGame.getInstance().changeColorFond3D(initColor_current);
    }

    public void forceSetExpertMode_initApp()
    {
        cam = Camera.main;
        currentColor = initColor_expert;
        initColor_current = initColor_expert;
        currentColor_adventure = initColor_current;

        animFond3DGame.getInstance().changeColorFond3D(initColor_current);
        starModeOn = true;

        borderUp.color = new Color(starBordColor.r, starBordColor.g , starBordColor.b , 140 / 255f);
        borderDown.color = new Color(starBordColor.r, starBordColor.g , starBordColor.b , 140 / 255f);
        cam.backgroundColor = new Color(currentColor.r / 255f,currentColor.g / 255f,currentColor.b / 255f);
    }

    public static bool modeBonusInfinisOn = false;

    //. // modeInfinisChangeLevelColor()


    private Color32 currentColorFondInfinis;
    public void playerChangedLevelInfinis(Color32 newColor)
    {
        StartCoroutine(swtich(currentColorFondInfinis, newColor, timeShade_levelChange));
        currentColorFondInfinis = newColor;
        animFond3DGame.getInstance().changeColorFond3D(newColor);
    }
    public void modeInfinisBonus(bool activate)
    {
        /*StartCoroutine(swtich(
            activate ? currentColorFondInfinis : colorInfinisBonusPhase
            , activate ? colorInfinisBonusPhase : currentColorFondInfinis
            , timeShade_levelChange));*/
        modeBonusInfinisOn = activate;
    }




    #region CHANGEMENT COULEURE DE FOND DU JEU


    public Color gameBordColor, starBordColor, infinisBordColor;
    public Image borderUp, borderDown;
    private bool starModeOn = false;
    private void changeBorderFadeColor(bool toGame, bool toInfinis, bool toStar, bool starOff, float time)
    {
        Color goalColor = gameBordColor;

        if (toStar)
        {
            goalColor = starBordColor;
            starModeOn = true;
        }
        else if (starOff)
        {
            goalColor = gameBordColor;
            starModeOn = false;
        }

        else if (toGame)
        {
            if (starModeOn)
            {
                //toGame = false;
                goalColor = starBordColor;
            }
            else
            {
                goalColor = gameBordColor;
            }
        }
        else if (toInfinis)
        {
            goalColor = infinisBordColor;
        }

        //Color goalColor = toGame ? gameBordColor : toInfinis ? infinisBordColor : starBordColor;
        StartCoroutine(switch_border(borderUp.color, goalColor, time));
    }

    IEnumerator switch_border(Color32 initColor, Color32 endColor, float time)
    {
        float t = 0, p;
        Color c;
        while (t < time)
        {
            p = t / time;
            p = p > 1 ? 1 : p;

            c = new Color(
               (initColor.r + (endColor.r - initColor.r) * p) / 255f,
               (initColor.g + (endColor.g - initColor.g) * p) / 255f,
               (initColor.b + (endColor.b - initColor.b) * p) / 255f, 140/255f);

            borderUp.color = c;
            borderDown.color = c;

            //cam.backgroundColor = 
            yield return null;
            t += Time.deltaTime;
        }
        /*c = new Color(
               (initColor.r + (endColor.r - initColor.r) * 1) / 255f,
               (initColor.g + (endColor.g - initColor.g) *) / 255f,
               (initColor.b + (endColor.b - initColor.b) * p) / 255f, 140 / 255f);*/
        borderUp.color = new Color(endColor.r /255f, endColor.g/255f, endColor.b/255f, 140 / 255f);
        borderDown.color = new Color(endColor.r / 255f, endColor.g / 255f, endColor.b / 255f, 140 / 255f);
        //currentColor = endColor;
    }

    //public Color32 colorNormal;
    public Color32 colorInfinis;
    public Color32 colorInfinisBonusPhase;
    private float timeShade = 0.7f;
    private Camera cam;

    public void switchColor(bool toModeInf)
    {
        Color32 initColor = toModeInf ? initColor_current : colorInfinis;
        Color32 endColor = toModeInf ? colorInfinis : initColor_current;

        StartCoroutine(swtich(initColor, endColor, timeShade));
        if (toModeInf)
            changeBorderFadeColor(false, true, false, false, timeShade_modeExpert);
        else
            changeBorderFadeColor(true, false, false, false, timeShade_modeExpert);

        animFond3DGame.getInstance().changeColorFond3D(endColor);
    }

    IEnumerator swtich(Color32 initColor, Color32 endColor, float time)
    {
        float t = 0, p;
        while ( t < time)
        {
            p = t / time;
            p = p > 1 ? 1 : p;
            cam.backgroundColor = new Color(
               (initColor.r + (endColor.r - initColor.r) * p)/255f,
               (initColor.g + (endColor.g - initColor.g) * p)/255f,
               (initColor.b + (endColor.b - initColor.b) * p)/255f
               );
            yield return null;
            t += Time.deltaTime;
        }
        currentColor = endColor;
    }


    //public Color32[] lesFondsColor;
    private Color32 currentColor;
    private static Color32 menuColorBeforeGame;
    public static void changeColorGame(bool openGame, bool modeInfinis)
    {
        //if(!skinManager.fondBuyedOn)
            getInstance().changeC(openGame, modeInfinis, 0);
        
        /*else
        {
            getInstance().loadAnimArcEnCiel(openGame, modeInfinis);
            // fond special
            // openGame lancer l'anim
            // !openGame anim en fonction couleure actuelle
        }*/
    }

    public static Color32 colorChooseShop;
    private void changeC(bool openGame, bool modeInfinis, int indexC)
    {
        if (openGame)
        {
            menuColorBeforeGame = currentColor = modeInfinis ? colorInfinis : initColor_current;
        }
        if (openGame && indexC != 0)
        {
            Color32 toGo = colorChooseShop;
            currentColor = toGo;
            StartCoroutine(swtich(menuColorBeforeGame, toGo, timeShade));
        }
        else if(!openGame && !Equals(currentColor, menuColorBeforeGame))
        {
            StartCoroutine(swtich( currentColor, menuColorBeforeGame, timeShade));

            if (modeInfinis)
            {
                menuColorBeforeGame = colorInfinis;
                currentColor = colorInfinis;
                infinisManager.bonusPhase = false;
                animFond3DGame.getInstance().changeColorFond3D(colorInfinis);
                //bonusOn = false;
            }
        }
    }
    #endregion


    private static bool arcEnCielOn = false;
    public Color[] animFondArcEnCiel;
    private int curColor_index = 0, nextColor_index;
    public float time_betweenTwoColors = 5f;
    /*
     * void loadAnimArcEnCiel(bool openGame, bool modeInfinis)
     * IEnumerator animArcEnCiel(bool toModeInfinis)
     *  IEnumerator switch_ArcEnCiel(Color32 initColor, Color32 endColor, float time)
    */
    #region anim fond arc-en-ciel
    private void loadAnimArcEnCiel(bool openGame, bool modeInfinis)
    {
        arcEnCielOn = openGame;
        if (arcEnCielOn)
        {
            menuColorBeforeGame = currentColor = modeInfinis ? colorInfinis : initColor_current;
            StartCoroutine(animArcEnCiel(modeInfinis));
        }
    }
    private IEnumerator animArcEnCiel(bool toModeInfinis)
    {
        curColor_index = toModeInfinis ? 5 : 4;
        // en fonction to mode infinis, la premiere couleure du degrade est differente
        while (arcEnCielOn)
        {
            nextColor_index = (curColor_index + 1) % animFondArcEnCiel.Length;
            StartCoroutine(switch_ArcEnCiel(
                    animFondArcEnCiel[curColor_index]
                    , animFondArcEnCiel[nextColor_index]
                    , time_betweenTwoColors));
            float t = 0;
            while(t < time_betweenTwoColors && arcEnCielOn)
            {
                yield return null;
                t += Time.deltaTime;
            }
            curColor_index = nextColor_index;
        }
        // quand s'arrête : fin de game, retour a la couleure du menu
        StartCoroutine(swtich(
                     cam.backgroundColor
                    , menuColorBeforeGame
                    , timeShade));
    }

    private IEnumerator switch_ArcEnCiel(Color32 initColor, Color32 endColor, float time)
    {
        float t = 0, p;
        while (t < time && arcEnCielOn)
        {
            p = t / time;
            p = p > 1 ? 1 : p;
            cam.backgroundColor = new Color(
               (initColor.r + (endColor.r - initColor.r) * p) / 255f,
               (initColor.g + (endColor.g - initColor.g) * p) / 255f,
               (initColor.b + (endColor.b - initColor.b) * p) / 255f
               );
            yield return null;
            t += Time.deltaTime;
        }
        currentColor = endColor;
    }

    #endregion



    #region AIGLE EFFECT ( BLUR )
    public float maxEff_aigle;
    public float timeChange_aigle;
    private float currentSpeedEffectVal;
    public PostProcessVolume pPV;
    private PostProcessProfile pPP;

    public void activateAigle()
    {
        // on active la visee
        StartCoroutine(_aigle(true, timeChange_aigle));
    }

    public void desactivateAigle()
    {
        // on desactive la visee
        StartCoroutine(_aigle(false, timeChange_aigle));
    }

    private IEnumerator _aigle(bool setOn, float time)
    {
        ChromaticAberration c;

        if (pPP.TryGetSettings<ChromaticAberration>(out c))
        {
            float p, t = 0f;

            while (t < time)
            {
                
                yield return null;
                p = t / time;// setOn ? t / time : 1 - t / time;

                currentSpeedEffectVal = setOn ? maxEff_aigle * p : maxEff_aigle - maxEff_aigle * p;

                c.intensity.Override(currentSpeedEffectVal);
                t += Time.deltaTime;

            }

            c.intensity.Override(setOn ? maxEff_aigle : 0);
        }
    }
    #endregion
}
