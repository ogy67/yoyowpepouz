using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemTranche : MonoBehaviour
{
    public developerBddManager devBddManager;
    public Text valueTrancheText;
    private int currentValueTranche = 0;

    public void itemInstantiated(int valueTranche)
    {
        valueTrancheText.text = valueTranche == -1 ? "0" : valueTranche.ToString();
        currentValueTranche = valueTranche != -1 ? valueTranche : 0;
    }
    public void btnUpTranche()
    {
        currentValueTranche++;
        updateValueItem();
    }
    public void btnDownTranche()
    {
        currentValueTranche--;
        // si tranche en dessou de 0, supprimer l'item
        if(currentValueTranche < 0)
        {
            devBddManager.itemTrancheDeleted(GetComponent<itemTranche>());
        }
        else
        {
            updateValueItem();
        }
    }

    private void updateValueItem()
    {
        valueTrancheText.text = currentValueTranche.ToString();
    }

    public int getValueTranche()
    {
        return currentValueTranche;
    }
}
