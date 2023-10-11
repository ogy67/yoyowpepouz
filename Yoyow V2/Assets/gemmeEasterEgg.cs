using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct eggClickEvent
{
    public float[] rangeTime_fromPrevClick;
    public int btn_indexRequired;
}



public class gemmeEasterEgg : MonoBehaviour
{
    public eggClickEvent[] codeInfos;
    public int startBtnCode;
    private static int nbGemmeEasterEgg = 300; 


    private bool waitingForCode = false;
    private int currentClickIndex;
    public void btnEasterEgg(int index)
    {
        if(!waitingForCode && index == startBtnCode)
        {
            currentClickIndex = -1;
            StartCoroutine(waitCode_enum());
            return;
        }
        currentClickIndex = index;
    }

    private int currentPhaseCode;
    private bool forceFinish = false; // appelé si clique trop tard
    private IEnumerator waitCode_enum()
    {
        waitingForCode = true;
        currentPhaseCode = 0;
        eggClickEvent e;

        while (currentPhaseCode < codeInfos.Length && !forceFinish)
        {
            e = codeInfos[currentPhaseCode];
            // on attend un clique sur un bouton egg
            float t_max = e.rangeTime_fromPrevClick[1];
            float t = 0f;
            while(t < t_max && currentClickIndex == -1)
            {
                yield return null;
                t += Time.deltaTime;
            }
            // clique trop tard OU trop tôt, fin
            if(t >= t_max || t < e.rangeTime_fromPrevClick[0]
                // clique ne correspond pas au clique attendus
                || currentClickIndex != e.btn_indexRequired)
            {
                forceFinish = true;
                Debug.Log("Error at time " + t+ " range : ("+ e.rangeTime_fromPrevClick[0]+" - "+ e.rangeTime_fromPrevClick[1]+")");
            }
            // on attend la phase suivante
            else
            {
                // remet état initial, sinon on attend pas de clique
                currentClickIndex = -1;
            }

            currentPhaseCode++;
            
        }
        if (forceFinish) // code pas bon
        {
            forceFinish = false;
            Debug.Log("<color=red> CODE MAUVAIS</color>");
        }
        else
        {
            // code bon
            Debug.Log("<color=blue> BON CODE</color>");
            popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbGemmeEasterEgg, transform.position);
            //rewardMenu.getInstance().giveReward(typeReward.gemmes, new object[] { nbGemmeEasterEgg });
        }
        waitingForCode = false;
    }
}
