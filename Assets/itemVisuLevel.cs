using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemVisuLevel : MonoBehaviour
{
    public Text numLevelText_unlock, numLevetText_lock;
    public GameObject difficultyGlow_contentGo;
    public GameObject[] difficultyFond;
    public GameObject fondLock, fondUnlock;
    public Transform crownBar;
    public Image starImg;
    public Image contourImg;
    public Color[] contourColor;
    public Color crownOnColor, crownOffColor;
    public Color starOnColor, starOffColor;
    public bool isLastItem;
    public int numLevel;

    private int prevCrown;
    private bool prevStar;
    public bool isLocked;

    public Transform contentTextResize_unlock;
    public Vector3 sizeText_over100_unlock;
    public Transform contentTextResize_lock;
    public Vector3 sizeText_over100_lock;
    // 1.572065

    public GameObject isSelectedImg;
    public static GameObject currentImgSelected;
    
    public void initApp(int _numLevel)
    {
        numLevel = _numLevel;
        numLevetText_lock.text = _numLevel.ToString();
        numLevelText_unlock.text = _numLevel.ToString();
        if (numLevel >= 100)
        {
            contentTextResize_unlock.localScale = sizeText_over100_unlock;
            contentTextResize_lock.localScale = sizeText_over100_lock;
        }
    }
    public void initItem(int _numLevel, int nbCrown, bool star, bool _isLocked, bool _isLastItem, bool _isSelectedItem)
    {
        numLevel = _numLevel;
        prevCrown = nbCrown;
        prevStar = star;
        isLocked = _isLocked;

        int difficultyLevel = SaveSystem.getDifficultyLevel(_numLevel - 1);
        isLastItem = _isLastItem;

        difficultyFond[difficultyLevel].SetActive(true);
        contourImg.color = contourColor[difficultyLevel];

        if(_isSelectedItem
            && currentImgSelected != isSelectedImg)
        {
            if(currentImgSelected != null)
                currentImgSelected.SetActive(false);
            isSelectedImg.SetActive(true);

            currentImgSelected = isSelectedImg;
        }


        if (_isLocked)
        {
            fondLock.SetActive(true);
            difficultyGlow_contentGo.SetActive(false);

            numLevetText_lock.text = _numLevel.ToString();
            numLevelText_unlock.text = _numLevel.ToString();
            if (numLevel >= 100)
            {
                contentTextResize_unlock.localScale = sizeText_over100_unlock;
                contentTextResize_lock.localScale = sizeText_over100_lock;
            }
        }
        else
        {
            fondUnlock.SetActive(true);

            numLevelText_unlock.text = _numLevel.ToString();
            if (numLevel >= 100)
            {
                contentTextResize_unlock.localScale = sizeText_over100_unlock;
                contentTextResize_lock.localScale = sizeText_over100_lock;
            }

            foreach (Transform t in crownBar)
            {
                t.GetComponent<Image>().color = t.GetSiblingIndex() < nbCrown ? crownOnColor : crownOffColor;
            }
            starImg.color = star ? starOnColor : starOffColor;
        }
    }

    public void updateItem(int nbCrown, bool star, bool _locked, bool _isSelectedItem)
    {
        if (_isSelectedItem
            && currentImgSelected != isSelectedImg)
        {
            currentImgSelected.SetActive(false);
            isSelectedImg.SetActive(true);

            currentImgSelected = isSelectedImg;
        }

        if (fondLock.activeSelf && !_locked) // passe de locked à unlocked
        {
            fondLock.SetActive(false);
            fondUnlock.SetActive(true);
            difficultyGlow_contentGo.SetActive(true);

            foreach (Transform t in crownBar)
            {
                t.GetComponent<Image>().color = t.GetSiblingIndex() < nbCrown ? crownOnColor : crownOffColor;
            }

            if(itemLockedStar)
                rewardGo_unlocked.SetActive(true);
        }
        else if (prevCrown != nbCrown)// juste update values
        {
            foreach (Transform t in crownBar)
            {
                t.GetComponent<Image>().color = t.GetSiblingIndex() < nbCrown ? crownOnColor : crownOffColor;
            }
        }
        isLocked = _locked;
        starImg.color = star ? starOnColor : starOffColor;
    }

    public void itemIsSelected()
    {
        if (currentImgSelected != null)
            currentImgSelected.SetActive(false);
        isSelectedImg.SetActive(true);

        currentImgSelected = isSelectedImg;
    }


    public GameObject pathImgUp, pathImgDown, pathImgRight;
    public void setPathImg(int dir)
    {
        // dir, 1 haut, 2 droite, 3 bas
        switch (dir)
        {
            case 3:
                pathImgUp.gameObject.SetActive(true);
                break;
            case 2:
                pathImgRight.gameObject.SetActive(true);
                break;
            case 1:
                pathImgDown.gameObject.SetActive(true);
                break;
        }
    }


    public bool itemLockedStar = false;
    public int nbCoinPalier;
    public Text nbCoinText_locked, nbCoinText_ombre_locked;
    public GameObject rewardGo_locked;
    public Text nbCoinText_unlocked, nbCoinText_ombre_unlocked;
    public GameObject rewardGo_unlocked;
    public void setLockedStar(int nbCoin, bool rewardTaken)
    {
        if (!rewardTaken)
        {
            itemLockedStar = true;
            nbCoinPalier = nbCoin;

            nbCoinText_locked.text = "+" + nbCoin;
            nbCoinText_ombre_locked.text = "+" + nbCoin;

            nbCoinText_unlocked.text = "+" + nbCoin;
            nbCoinText_ombre_unlocked.text = "+" + nbCoin;

            // si locked
            if (isLocked)
            {
                //      afficher nbCoinWin
                nbCoinText_locked.gameObject.SetActive(true);
                rewardGo_locked.SetActive(true);
            }
            // sinon
            else
            {
                rewardGo_unlocked.SetActive(true);

                //      si rewardTaken
            }
        }

        //setCoinTest();
    }

    public void getReward()
    {
        itemLockedStar = false;
        StartCoroutine(closeReward());
    }

    private IEnumerator closeReward()
    {
        yield return new WaitForSeconds(0.15f);
        rewardGo_unlocked.SetActive(false);
    }

    /*public Text test_textNbCoinPalier;
    private void setCoinTest()
    {
        test_textNbCoinPalier.gameObject.SetActive(true);
        test_textNbCoinPalier.text = "+" + nbCoinPalier;
    }*/
}
