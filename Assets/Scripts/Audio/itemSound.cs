using System.Collections;
using UnityEngine;

public class itemSound : MonoBehaviour
{
    public AudioSource player;
    public bool static_player; // bloquer autodestruction

    private void Start()
    {
        player = GetComponent<AudioSource>();
        if (!static_player)
            StartCoroutine(nonStatic_loadSound());
    }

    private static float timeDelayDeleteMin = 0.3f;
    private IEnumerator nonStatic_loadSound()
    {
        yield return new WaitForSeconds(player.clip.length + timeDelayDeleteMin);
        Destroy(gameObject);
        sfxGame.getInstance().endSfx();
    }


    public static float fadeOnTime =2f, fadeOffTime = 2f;
    public static float delayChangeMusic = 1f;
    private bool isFading = false;

    // apelé quand joueur désavtive la musique
    public void forceStopMusic()
    {
        player.volume = 0f;
        player.Pause();
        isFading = false;
    }

    public float forceReloadMusic() // renvoit le temps déja passé sur la musique
    {
        player.Play();
        StartCoroutine(fadeOn_enum(currentMusicItem.volume));
        return player.time;
    }

    private musicItem currentMusicItem;

    public void initMusic(musicItem mI)
    {
        currentMusicItem = mI;
        //Debug.Log("( "+gameObject.name + " ) Is fading ? " + isFading);
        //if (isFading)
        //    return;
        if(player == null)
            player = GetComponent<AudioSource>();
        //player.volume = mI.volume;
        player.clip = mI.sound;
        player.Play();
        StartCoroutine(fadeOn_enum(mI.volume));
        //Debug.Log("( " + gameObject.name + " ) Lis musique");
    }

    private IEnumerator fadeOn_enum(float soundGoal)
    {
        if (isFading)
            yield return new WaitUntil(predicate: () => isFading == false);
        isFading = true;
        float t = 0f;
        while(t < fadeOnTime && isFading)
        {
            yield return null;
            t += Time.deltaTime;
            player.volume = soundGoal * (t / fadeOnTime);
        }
        if (isFading) // btnMusic stop la routine
        {
            player.volume = soundGoal;
            isFading = false;
            //Debug.Log("<color=blue>Fade on end</color> ( " + gameObject.name + " )");
        }
    }

    public void fadeOffMusic()
    {
        //Debug.Log("<color=blue>Fade on start</color> ( " + gameObject.name + " )");
        StartCoroutine(fadeOff_enum());
    }

    private IEnumerator fadeOff_enum()
    {
        if (isFading)
            yield return new WaitUntil(predicate: () => isFading == false);
        isFading = true;
        float currentVolume = player.volume;
        float t = 0;
        while (t < fadeOffTime && isFading)
        {
            yield return null;
            t += Time.deltaTime;
            player.volume = currentVolume - currentVolume * (t / fadeOffTime);
        }
        if (isFading) // btnMusic stop la routine
        {
            //Debug.Log("( " + gameObject.name + " ) Fade off ends");
            player.volume = 0f;
            player.Stop();
            isFading = false;
        }
    }

    float timeFadeGameOver = 1.5f;
    float factorMinMusicGameOver = 0.5f; // volumeGoal = volume * 0.4f

    public void fadeMusicGameOver(bool replayGame)
    {
        StartCoroutine(fadeMusicGameOver_loop(replayGame));
    }
    private IEnumerator fadeMusicGameOver_loop(bool replayGame)
    {
        float t = 0f;
        float goalVolume;
        float diffVolume;

        if (replayGame)
        {
            goalVolume = currentMusicItem.volume;
            diffVolume = goalVolume - player.volume;
        }
        else
        {
            goalVolume = player.volume * factorMinMusicGameOver;
            diffVolume = player.volume - goalVolume;
        }

        while (!isFading && t < timeFadeGameOver) // si clique cash quit gameOver, fadePrincipal prend le devant et finit le fade en entier
        {
            yield return null;
            t += Time.deltaTime;
            player.volume = goalVolume + diffVolume * (t / timeFadeGameOver);
        }
        if (!isFading)
            player.volume = goalVolume;
    }
}
