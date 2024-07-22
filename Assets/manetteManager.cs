using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  OK : quand gagne / perd avec boost activé, couleur btnA garde le vert
// => reinitialiser color btn quand activate
//  OK : meurt avec boost bombe activé : manette pas disabled
//  OK : meurt avec boost superVisee activé : lineVisee pas clean
// => est que desactivé quand clique ( doit aussi quand meurt ) 
// OK : player utilise super boost dans checkP ( 2e click )
// (zoneDetec.cs) OnpointerDown : check si checkP <PUIS> (manetteM.cs) OnpointerUp : check si SuperBoost
// => (zoneDetec.cs) ne gère pas pointerDown et Up avec playerMovment, btnManette le fait

public class manetteManager : MonoBehaviour
{
    private void Start()
    {
        int side = (int)SaveSystem.loadData(typeSave.MANETTE_SIDE_HAND);
        setSideButton(side == 1 || side == -1000 ? true : false);

        List<string> listPosString = (List<string>)SaveSystem.loadData(typeSave.MANETTE_POS_BTN);
        /*Debug.Log("Data pos : " + listPosString);
        foreach(string s in listPosString)
        {
            Debug.Log("Data : " + s);
        }*/
        //object dataPosBtns = SaveSystem.loadData(typeSave.MANETTE_POS_BTN);
        if (listPosString.Count != 0)
        {
            List<Vector2> posBtns2 = posBtnStringToVector(listPosString);
            grpA.localPosition = posBtns2[0];
            grpB.localPosition = posBtns2[1];
        }
    }

    public Transform zoneClick;
    public Transform grpA, grpB;
    public Transform reverseA, reverseB; // posBound
    public static bool manetteModeRightHandled = true;// par défaut

    public void setSideButton(bool RightHandled)
    {
        // si est déja dans le bon sens, on passe
        if (RightHandled == manetteModeRightHandled)
            return;
        //Debug.Log("ChangeSideButtons");
        // SOLUTION 2
        // on inverse zoneClick, puis on retourne les grpButton sur eux même ( inversement scale x )
        zoneClick.localScale = new Vector2(-zoneClick.localScale.x, zoneClick.localScale.y);
        grpA.localScale = new Vector2(-grpA.localScale.x, grpA.localScale.y);
        grpB.localScale = new Vector2(-grpB.localScale.x, grpB.localScale.y);
        reverseA.localScale = new Vector2(-reverseA.localScale.x, reverseA.localScale.y);
        reverseB.localScale = new Vector2(-reverseB.localScale.x, reverseB.localScale.y);
        manetteModeRightHandled = !manetteModeRightHandled;
        /* inverser côté switch btnA
        btnManetteA.zoneDetectionAssocie.changeSwitchSide_fromMenu();*/
    }

    // appelé en fin de game : infinis - online - aventure
    public void savePosBtnManette()
    {
        //Debug.Log("<color=red>Save pos btns</color>");
        SaveSystem.Save(typeSave.MANETTE_POS_BTN, posBtnsToString(grpA.localPosition, grpB.localPosition));
    }

    private List<string> posBtnsToString(Vector2 posBtnA, Vector2 posBtnB)
    {
        List<string> ret = new List<string>
        {
            posBtnA.x + "|" + posBtnA.y,
            posBtnB.x + "|" + posBtnB.y
        };
        return ret;
    }

    private List<Vector2> posBtnStringToVector(List<string> posBtns)
    {
        string[] pos1 = posBtns[0].Split('|'), pos2 = posBtns[1].Split('|');

        List<Vector2> ret = new List<Vector2>
        {
            new Vector2(float.Parse(pos1[0]), float.Parse(pos1[1])),
            new Vector2(float.Parse(pos2[0]), float.Parse(pos2[1]))
        };
        return ret;
    }


    // manetteManager donne un retour visuel des intéractions de player dans une partie

    public btnManette btnManetteA, btnManetteB;
    public zoneDetectionManette zoneDetectionA, zoneDetectionB;
    private bool manetteIsEnabled = false;
    public Animation animFondInfinis, animFondInfinis_up;

    // appelé quand player hang alors qu'il est sur le premier checkP
    public void playerOnfirstCheckP()
    {
        // activer de suite les autres contrôles
        playerDraggedACheckP(true);
    }

    // appelé par gameManager quand joueur peut controller le palet
    public void manetteEnabled()
    {
        // on active btnA
        zoneClick.gameObject.SetActive(true);
        btnManetteA.manetteEnabled();

        // on reset btnB (pas de preclickB a init)
        btnManetteB.preshotOn = false;
        zoneDetectionA.manetteActivated_resetClickZone();
        zoneDetectionB.manetteActivated_resetClickZone();

        manetteIsEnabled = true;

        if(infinisManager.infinis_Mode_On
            && infinisManager.SCORE > 0)
        {
            animFondInfinis.Play("activateFondManetteInfinis");
            animFondInfinis_up.Play("activateFondManetteInfinis");
        }
    }

    // appelé par gameManager quand joueur ne peut plus controller le palet
    public void manetteDisabled()
    {
        // pierreTombale : si gagne après mort
        // => manette est déja disabled
        if (!manetteIsEnabled)
            return;
        StartCoroutine(disableManette());
        manetteIsEnabled = false;

        if (infinisManager.infinis_Mode_On
    && infinisManager.SCORE > 0)
        {
            animFondInfinis.Play("DesactivateFondManetteInfinis");
            animFondInfinis_up.Play("DesactivateFondManetteInfinis");
        }
    }

    private IEnumerator disableManette()
    {
        // on desactive btnA
        // on desactive btnB si encore activé
        if (btnManetteA.btnActivated)
        {
            // on doit repositionner btn dans safe zone
            zoneDetectionA.checkIfBtnIsInSafeArea();


            btnManetteA.manetteDisabled();
            zoneDetectionA.playerDraggingBtn = false;
            zoneDetectionA.pointerDown = false;
        }
        if (btnManetteB.btnActivated)
        {
            zoneDetectionB.checkIfBtnIsInSafeArea();

            btnManetteB.manetteDisabled();
        }
        yield return new WaitForSeconds(30/60f);
        zoneClick.gameObject.SetActive(false);
    }

    public menuTutoAventure menuTuto;
    // appelé par gameManager quand player attrape un checkPoint
    public void playerDraggedACheckP(bool firstCheckP)
    {
        if (menuTuto.canDisplayBtnB())
        {
            btnManetteB.playerDragCheckPoint(firstCheckP);

            if (menuTuto.canDisplaySwitchBtnA())
            {
                btnManetteA.playerDragCheckPoint(firstCheckP);
            }
        }

        /* afficher la flèche
        // afficher btn B
        if (TutoManager.tutoOn)
        {
            // afficher btn b si phase presentation passée
            if (TutoManager.getInstance().canDisplayBtnB_manette())
            {
                btnManetteB.playerDragCheckPoint(firstCheckP);
            }
            // afficher fleche si phase en court
            if (TutoManager.getInstance().isPlayerInSwitchPhase())
            {
                btnManetteA.playerDragCheckPoint(firstCheckP);
            }
        }
        else
        {
            btnManetteB.playerDragCheckPoint(firstCheckP);
            btnManetteA.playerDragCheckPoint(firstCheckP);
        }*/
    }

    // appelé par gameManager quand player lache un checkP ( OU meurt dessus )
    public void playerUndraggedACheckP()
    {
        if (menuTuto.canDisplayBtnB())
        {
            if (btnManetteA.btnActivated)
                btnManetteB.playerUndragCheckPoint();

            if (menuTuto.canDisplaySwitchBtnA())
            {
                btnManetteA.playerUndragCheckPoint();
            }
        }

        /*if (TutoManager.tutoOn)
        {
            // Des-afficher btn b si phase presentation passée
            if (TutoManager.getInstance().canDisplayBtnB_manette())
            {
                btnManetteB.playerUndragCheckPoint();
            }
            // Des-afficher fleche si phase en court
            if (TutoManager.getInstance().isPlayerInSwitchPhase())
            {
                btnManetteA.playerUndragCheckPoint();
            }
        }
        else
        {
            // Des-afficher la flèche
            // Des-afficher btn B
            if (btnManetteA.btnActivated)
                btnManetteB.playerUndragCheckPoint();

            btnManetteA.playerUndragCheckPoint();
        }*/
    }
}
