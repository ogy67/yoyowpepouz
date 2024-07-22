using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onlineMenuPreview : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool onlinePreviewOpen = false;
    public void btnOpenOnline()
    {
        if (managerNiveau.gameOn)
            return;
        //sfxManager.getInstance().playSound_online(typeSound_online.btn_ranking);
        onlinePreviewOpen = true;
        GetComponent<Animation>().Play("openOnlinePreview");
    }

    public void btnCloseOnline()
    {
        onlinePreviewOpen = false;
        //sfxManager.getInstance().playSound_online(typeSound_online.btn_close);
        GetComponent<Animation>().Play("closeOnlinePreview");
    }
}
