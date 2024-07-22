using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeMusic
{
    tuto,
    menuP,
    ingame
}

[System.Serializable]
public struct musicItem
{
    public AudioClip sound;
    public typeMusic _typeMusic;
    public float volume;
    public float lengthSecond;
}
public class musicManager : MonoBehaviour
{

    // les rang 0 sont les rangs de la music principale de la liste
    public musicItem[] tutoMusic;
    public musicItem[] menuPMusic;
    public musicItem[] ingameMusic;

    public itemSound tutoPlayer;
    public itemSound menuPPlayer;
    public itemSound ingamePlayer;

    public static bool musicOn = true;

    private static musicManager instance;
    public static musicManager getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("MUSIC").GetComponent<musicManager>();
        }
        return instance;
    }
    
    public void musicStateChanged()
    {
        // unpause si on
        if (musicOn)
        {
            if (currentItemSound != null)
            {
                float time_spent = currentItemSound.forceReloadMusic();
                // on lance le bouclage sans les secondes déjà passées
                StartCoroutine(waitMusicEndsToLoadAnother(currentItemSound.player.clip.length - time_spent));
            }
            // si musique etteinte de base, on doit en lancer une nouvelle
            else
                initMusic_menuP();
        }
        // arrête la musique en court si off
        else
        {
            currentItemSound.forceStopMusic();
        }
    }

    private itemSound currentItemSound;
    private typeMusic currentTypeMusic;
    // la musique commence une fois menu P ouvert
    public void initMusic_menuP()
    {
        musicOn = paramManager.getParamState(typeParam.music);
        if (!musicOn)
            return;
        readMusic(menuPMusic[0]);
        nbMusicPlayed[typeMusic.menuP] = 1;
        StartCoroutine(waitMusicEndsToLoadAnother(menuPMusic[0].lengthSecond));
        //Debug.Log("Init music menu P");
    }

    public void initMusic_tuto()
    {
        musicOn = paramManager.getParamState(typeParam.music);
        if (!musicOn)
            return;
        readMusic(tutoMusic[0]);
        nbMusicPlayed[typeMusic.tuto] = 1;
        StartCoroutine(waitMusicEndsToLoadAnother(tutoMusic[0].lengthSecond));
    }

    private Dictionary<typeMusic, int> nbMusicPlayed = new Dictionary<typeMusic, int>() {
        { typeMusic.ingame, 0 },
        {typeMusic.tuto, 0 },
        {typeMusic.menuP, 0 },
    };

    public void switchMusicType(typeMusic next)
    {
        if (!musicOn)
            return;

        if (currentItemSound != null)
        {
            // fade sound music
            currentItemSound.fadeOffMusic();
        }
        StartCoroutine(switchDelay(next));
    }

    private IEnumerator switchDelay(typeMusic next)
    {
        yield return new WaitForSeconds(itemSound.delayChangeMusic);
        loadNewMusic(next);
    }

    private void loadNewMusic(typeMusic next)
    {
        musicItem[] currentList = next == typeMusic.tuto ? tutoMusic :
            next == typeMusic.menuP ? menuPMusic : ingameMusic;


        // on choisit une nouvelle music
        // si on a pas encore joué de musique pour le type, on joue la musique principale de la liste
        if (nbMusicPlayed[next] == 0)
        {
            readMusic(currentList[0]);
            nbMusicPlayed[next] = 1;
        }
        else
        {
            // si une seule musique
            // on loop sur la première musique
            if (currentList.Length == 1)
            {
                readMusic(currentList[0]);
            }
            // si plusieurs musique
            // on prend une musique aléatoire autre que précédente
            else
            {
                int index;
                do
                {
                    index = Random.Range(0, currentList.Length);
                } while (index + 1 == nbMusicPlayed[next]);
                nbMusicPlayed[next] = index + 1;
                readMusic(currentList[index]);
                StartCoroutine(waitMusicEndsToLoadAnother(currentList[index].lengthSecond));
            }

        }
    }

    private IEnumerator waitMusicEndsToLoadAnother(float timeMusic)
    {
        float t = 0f;
        float wait = timeMusic - itemSound.fadeOffTime;
        int nbM = nbMusicRode;
        while(musicOn && nbM == nbMusicRode && t < wait)
        {
            yield return null;
            t += Time.deltaTime;
        }

        //Debug.Log("Fin musique");

        if (!musicOn)
            yield break;

        // on doit regarder si musicOn, si pas musicOn rien faire

        // si on a depassé le temps
        if (t >= wait)
        {
            //Debug.Log("Fade off musique");
            // fade sound music
            currentItemSound.fadeOffMusic();
            yield return new WaitForSeconds(itemSound.fadeOffTime);
            // on load la prochaine music sans changer de thème
            if(nbM == nbMusicRode)
            {
                //Debug.Log("Cherche une nouvelle musique");
                loadNewMusic(currentTypeMusic);
            }
        }
    }

    private static int nbMusicRode = 0;
    private void readMusic(musicItem mI)
    {
        nbMusicRode++;
        itemSound _player = mI._typeMusic == typeMusic.ingame ? ingamePlayer :
            mI._typeMusic == typeMusic.tuto ? tutoPlayer : menuPPlayer;
        _player.initMusic(mI);
        currentItemSound = _player;
        currentTypeMusic = mI._typeMusic;

        //Debug.Log("READ NEW MUSIC of type : "+mI._typeMusic);
    }
    public void gameOverOpenned()
    {
        if(musicOn)
            currentItemSound.fadeMusicGameOver(false);
    }

    public void gameOverReplay()
    {
        if (musicOn)
            currentItemSound.fadeMusicGameOver(true);
    }

}
