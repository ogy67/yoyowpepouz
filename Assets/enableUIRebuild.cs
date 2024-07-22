using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enableUIRebuild : MonoBehaviour
{
    public RectTransform toRebuild;
    //private bool initRebuildDone = false;

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(toRebuild);
    }
}
