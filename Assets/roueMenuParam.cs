using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum typeBtnParam
{
    sideHand,
    sfx,
    music,
    vibration,
    twitter,
    help
}
public class roueMenuParam : MonoBehaviour, IPointerDownHandler,
    IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerUpHandler
{
    public Transform initPosDrag_transform, endPosDrag_transform, roue_transform;

    private float initZRotation;
    private float currentZRotation = 0;
    private bool clickInterruptedByDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        btnLikeAnim.Play("btnLikeParamBig");
        initZRotation = currentZRotation;
        clickInterruptedByDrag = true;
        snapFinished = true;

        initPosDrag_transform.position = eventData.position;
        endPosDrag_transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        bool dragRight = isDraggingRight(eventData);
        float factorRotation = !dragRight ? -1 : 1;

        // calculer angle de rotation
        Vector2 initPosToCenter = roue_transform.position - initPosDrag_transform.position;
        Vector2 currentPosToCenter = (Vector2)roue_transform.position - eventData.position;
        float angle = Vector2.Angle(initPosToCenter, currentPosToCenter); // /!\ TOUJOURS POSITIF
        angle = (angle * factorRotation) + initZRotation;

        // on rotate la roue par rapport au drag
        roue_transform.rotation = Quaternion.Euler(0, 0, angle);


        endPosDrag_transform.position = eventData.position;
        currentZRotation = angle;
        itemsFollowRoue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        btnLikeAnim.Play("btnLikeParamLittle");
        endPosDrag_transform.position = eventData.position;
    }

    public float speedSnap = 100f;
    public Transform targetParent, targetTr;
    public Transform targetStatic; // target qui tourne avec roue, et qui doit atteindre targetTr
    private bool snapFinished = true;
    private IEnumerator snapItem()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.roueEndRotation);

        snapFinished = false;
        // on snap cash btn le + prêt 
        // les angles font -120 -60 0 60 120 180 
        // on regarde quel angle est le + proche
        float a = Mathf.Abs(currentZRotation);
        float m = a % 60;
        float targetAngleRoue = m < 30 ? (a - m) : (a - m) + 60;
        float factor = currentZRotation > 0 ? 1 : -1;
        targetAngleRoue *= factor;

        targetParent.rotation = Quaternion.Euler(0, 0, targetAngleRoue);
        Vector3 targetDirection = targetTr.position - roue_transform.position;

        bool distanceUnderMin = false;
        float progVal = 0f;
        while(!clickDown && !distanceUnderMin)
        {
            progVal += speedSnap * Time.deltaTime / 180;
            roue_transform.right = Vector3.Lerp(roue_transform.right, targetDirection, progVal);
            distanceUnderMin = roundFloat(targetTr.position.x, 2) == roundFloat(targetStatic.position.x, 2)
                && roundFloat(targetTr.position.y, 2) == roundFloat(targetStatic.position.y, 2);
            // evite bug : angle = 180 , tr.right met rot.z à 180 mais modifie aussi rotation x et y
            if (distanceUnderMin && Mathf.Abs(targetAngleRoue) == 180)
                roue_transform.rotation = Quaternion.Euler(0, 0, 180);
            itemsFollowRoue();
            yield return null;
        }

        if (!clickDown)
        {
            if (Mathf.Abs(targetAngleRoue) != 180)
                roue_transform.right = targetDirection;

            currentZRotation = targetAngleRoue;
            snapFinished = true;
            itemsFollowRoue();

            //Quaternion targetRotationQuaternion = Quaternion.Euler(0, 0, targetAngleRoue);
            //newAngleWheel_checkCheatCode(Mathf.RoundToInt(targetRotationQuaternion.eulerAngles.z));
        }
    }

    private float roundFloat(float f, int nbDigit)
    {
        int i = (int)(f * Mathf.Pow(10, nbDigit));
        return (float)i / Mathf.Pow(10, nbDigit);
    }
    private bool isDraggingRight(PointerEventData eventData)
    {
        return ((roue_transform.position.x - initPosDrag_transform.position.x) * (eventData.position.y - initPosDrag_transform.position.y)
            - (roue_transform.position.y - initPosDrag_transform.position.y) * (eventData.position.x - initPosDrag_transform.position.x))
            < 0; ;
    }

    bool clickDown = false;
    public paramManager paramM;
    public void OnPointerDown(PointerEventData eventData)
    {
        clickDown = true;
        clickInterruptedByDrag = false;
        currentZRotation = roue_transform.rotation.eulerAngles.z;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        clickDown = false;
        if (!clickInterruptedByDrag)
        {
            if(!snapFinished)
                StartCoroutine(snapItem());
            else
            {
                // on ferme le menu param
                paramM.btnCloseParam();
            }
        }
        else
        {
            StartCoroutine(snapItem());
        }    
    }

    public Animation btnLikeAnim;
    public Transform[] posItemRoue_tab;
    public Transform[] btnAssocieItem_tab;

    public void itemsFollowRoue()
    {
        int i = 0;
        foreach(Transform t in posItemRoue_tab)
        {
            btnAssocieItem_tab[i].position = t.position;
            i++;
        }
    }


    // CACHE AU JOUEUR
    // cheat code, gems données quand roue tournée avec bonne combinaison
    // Droitier - Vibration - Aide - Aide - Aide - Aide - Twitter - Twitter - Droitier - Vibration - Aide - Son

    private readonly Dictionary<int, typeBtnParam> typeBtnByAngle = new Dictionary<int, typeBtnParam>()
    {
        {0, typeBtnParam.sideHand},

        {300, typeBtnParam.sfx},
        {240, typeBtnParam.music},
        {180, typeBtnParam.vibration},
        {120, typeBtnParam.twitter},
        {60, typeBtnParam.help}
    };
    // code de 9 inconus
    public int cheatCodeLength = 9;
    public typeBtnParam[] cheatCodeGem;
    public Transform posSpawnGem_cheat;
    public int numberGemCheat = 500;
    private List<typeBtnParam> currentCheatCode = new List<typeBtnParam>();

    // angle 60; quand rotate dessus, newAngleWheel_checkCheatCode appelé quand r = 59.X et pas 60.X -> (int)59.X = 59
    private int roundFloatToNearestAngle(float angle)
    {
        if( (int)angle != angle)
        {
            bool roundUp = (angle % 1) > 0.5f;
            //Debug.Log("Angle int and float not same, int : " + (int)angle + " , float : " + angle);
            return (int)angle + (roundUp ? 1 : 0);
        }
        return (int)angle;
    }
    private void newAngleWheel_checkCheatCode(float angle)
    {
        int angle_2 = roundFloatToNearestAngle(angle);
        //Debug.Log("Angle wheel " + angle_2);
        //Debug.Log("CheatCode, type " + typeBtnByAngle[angle_2]);

        currentCheatCode.Add(typeBtnByAngle[angle_2]);

        if (currentCheatCode.Count == cheatCodeLength + 1)
        {
            currentCheatCode.RemoveAt(0);
        }
        if (currentCheatCode.Count == cheatCodeLength)
        {
            bool codeSame = true;
            // on compare le cheat code avec le code composé
            for (int i = 0; i < cheatCodeLength; i++)
            {
                if (currentCheatCode[i] != cheatCodeGem[i])
                    codeSame = false;
            }
            if (codeSame)
            {
                //Debug.Log("CHEAT CODE GEM");
                popYowGemManager.getInstance().popRec_main(typeReward_game.gem, numberGemCheat, posSpawnGem_cheat.position);
            }

        }
    }
}

// 240l -> 138l
