using RDG;

public class vibrationManager
{
    private static bool vibeOn;
    static bool loaded = false;

    public static void vibe(int miliS)
    {
        if(getVibeVal())
            Vibration.Vibrate(miliS);
    }

    public static bool getVibeVal()
    {
        if (!loaded) // premier appel sur ce thread de l'app
        {
            vibeOn = paramManager.getParamState(typeParam.vibration);
            loaded = true;
        }
        return vibeOn;
    }

    public static bool vibeValChanged()
    {
        vibeOn = !getVibeVal();
        if (vibeOn)
        {
            Vibration.Vibrate(200);
        }

        //SaveSystem.Save(typeSave.VIBE, vibeOn);
        return vibeOn;
    }
}
