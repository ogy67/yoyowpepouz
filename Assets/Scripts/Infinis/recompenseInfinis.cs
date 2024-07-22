using System.Collections;
using UnityEngine;

public class recompenseInfinis : MonoBehaviour
{
    public SpriteRenderer glowImg;
    public GameObject animRec;
    public GameObject ondeRec;
    private typeBonus myType;

    private GameObject toPop;
    public static recompenseInfinis instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = gameObject.GetComponent<recompenseInfinis>();
    }

    /*
     * static GameObject popRecompense(Vector3 pos, typeBonus bonus, Transform par)
     *  void pop(typeBonus bonus)
     * void OnTriggerEnter2D(Collider2D collision)
     *  IEnumerator dissapear()
     */
    #region Gestion recompense
    public static GameObject popRecompense(Vector3 pos, typeBonus bonus, Transform par)
    {
        GameObject toPop = Instantiate(instance.gameObject);
        toPop.transform.SetParent(par);
        toPop.transform.position = pos;
        toPop.GetComponent<recompenseInfinis>().pop(bonus);

        return toPop;
    }

    public void pop(typeBonus bonus)
    {
        glowImg.color = bonus == typeBonus.ARGENT ? argentC :
            bonus == typeBonus.BOOST ? boostC :
            bonus == typeBonus.GEMME ? gemmeC :
            vieC;
        // ordre : 0 vie : 1 boost : 2 argent
        animRec.SetActive(true);
        animRec.transform
            .GetChild(
            bonus == typeBonus.VIE ? 0 : bonus == typeBonus.BOOST ? 1 : bonus == typeBonus.ARGENT ? 2 : 3)
            .gameObject.SetActive(true);
        myType = bonus;
        GetComponent<Animation>().Play("bonusPop");
    }

    public IEnumerator cleanRecNotTaken()
    {
        GetComponent<Animation>().Play("bonusNotTaken");
        //animRec.GetComponent<Animation>().Play("bonusInfinisReccup");
        yield return new WaitForSeconds(7 / 60f);
        Destroy(gameObject);
    }

    public Color vieC, boostC, argentC, gemmeC;
    public ParticleSystem myParticles;
    public float maxTimeParticle = 1.5f;
    private bool reccup = false;


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Aimant" && !reccup)
        {
            playerGetReward(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.gameObject.tag == "Player"|| collision.gameObject.tag == "Player_ghost") && !reccup)
        {
            playerGetReward(false);
        }
    }

    private void playerGetReward(bool aimant)
    {
        sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_pickUpMap);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.catchBonus);
        reccup = true;
        Color ondeC = new Color();

        switch (myType)
        {
            case typeBonus.VIE:
                gameManager.getInstance().giveRecompense(recompense.VIE, true);
                ondeC = vieC;
                break;
            case typeBonus.ARGENT:
                gameManager.getInstance().pickUpYowInfinis(true);
                ondeC = argentC;
                break;
            case typeBonus.BOOST:
                boostIngameManager.getInstance().infiniteReccupBoostSP();
                //gameManager.getInstance().giveRecompense(recompense.BOOST);
                ondeC = boostC;
                break;
            case typeBonus.GEMME:
                gameManager.getInstance().giveRecompense(recompense.GEMME, true);
                ondeC = gemmeC;
                break;

        }

        if(aimant)
            boostIngameManager.getInstance().aimantUsed();

        ondeRec.GetComponent<SpriteRenderer>().color = ondeC;
        StartCoroutine(dissapear(ondeC));
    }

    

    private IEnumerator dissapear(Color colBlastParticle)
    {
        GetComponent<Animation>().Play("bonusInfinisReccup");
        yield return new WaitForSeconds(25 / 60f); // temps avant fin anim disapear
        // 230 / 110
        ParticleSystem.MainModule main = myParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(colBlastParticle.r, colBlastParticle.g, colBlastParticle.b, 230 / 255f)
            , new Color(colBlastParticle.r, colBlastParticle.g, colBlastParticle.b, 110 / 255f));
        myParticles.Play();

        yield return new WaitForSeconds(maxTimeParticle - 25 / 60f);
        Destroy(gameObject);
    }
    #endregion
}
