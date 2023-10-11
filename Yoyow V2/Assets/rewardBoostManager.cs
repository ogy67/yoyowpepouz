using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rewardBoostManager : MonoBehaviour
{
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    /* TEST
    public bool test_btnGiveReward = false;
    public typeBoostGame test_typeBoost = typeBoostGame.bouclier;
    public int test_number = 10;
    public Transform test_posSpawn;

    // Update is called once per frame
    void Update()
    {
        if (test_btnGiveReward)
        {
            test_btnGiveReward = false;
            giveBoostReward(test_typeBoost, test_number, test_posSpawn);
        }
    }*/

    public GameObject instanceItemReward;
    public Transform contentSpawnItem;
    public popYowGemManager popCurrencyManager;
    public int maxInstancePop = 35;
    public int maxSfxCount = 5;

    private List<GameObject> instanceItem_list = new List<GameObject>();

    public void giveBoostReward(typeBoostGame type, int number, Transform posSpawn)
    {
        GameObject instance;
        float speedBlast = popCurrencyManager.getSpeedBlast_boost(number);
        int nb_2 = number > maxInstancePop ? maxInstancePop : number;
        currentIdAnimSfx++;
        animSfxIdEnabled.Add(currentIdAnimSfx);
        nbSfxPerIdAnim.Add(currentIdAnimSfx, number > maxSfxCount ? maxSfxCount : number);

        for (int i = 0; i < nb_2; i++)
        {
            instance = Instantiate(instanceItemReward, contentSpawnItem);
            instanceItem_list.Add(instance);
            instance.SetActive(true);
            instance.GetComponent<itemRewardBoost>().popItem(type, posSpawn, speedBlast, currentIdAnimSfx);
        }
    }

    private int currentIdAnimSfx = 0;
    private List<int> animSfxIdEnabled = new List<int>();
    private Dictionary<int, int> nbSfxPerIdAnim = new Dictionary<int, int>();
    public void itemTryAnimSfx(int idAnim)
    {
        if (animSfxIdEnabled.Contains(idAnim))
        {
            animSfxIdEnabled.Remove(idAnim);
            StartCoroutine(animSfx(idAnim));
        }
    }
    private IEnumerator animSfx(int idAnim)
    {
        for(int i = 0; i < nbSfxPerIdAnim[idAnim]; i++)
        {
            sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.itemBlast_end);
            yield return null;
        }
        nbSfxPerIdAnim.Remove(idAnim);
    }

    public void resetItems()
    {
        if (instanceItem_list.Count != 0)
        {
            foreach (GameObject o in instanceItem_list)
            {
                Destroy(o);
            }
            instanceItem_list = new List<GameObject>();
        }
    }
}
