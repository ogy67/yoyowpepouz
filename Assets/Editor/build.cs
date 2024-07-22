using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class build : MonoBehaviour
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string builtProjectPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        var projectPath = PBXProject.GetPBXProjectPath(builtProjectPath);
        var project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Désactiver Bitcode
        project.SetBuildProperty(project.ProjectGuid(), "ENABLE_BITCODE", "NO");

        // Modifier le fichier Info.plist
        string plistPath = Path.Combine(builtProjectPath, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Ajouter les capacités nécessaires
        var backgroundModes = rootDict.CreateArray("UIBackgroundModes");
        backgroundModes.AddString("audio");
        backgroundModes.AddString("fetch");
        backgroundModes.AddString("remote-notification");

        rootDict.SetBoolean("UIRequiresFullScreen", true);

        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        // Enregistrer les modifications dans Info.plist
        File.WriteAllText(plistPath, plist.WriteToString());

        // Activer In-App Purchase
        var capabilities = project.GetUnityMainTargetGuid();
        project.AddCapability(capabilities, PBXCapabilityType.InAppPurchase);

        // Enregistrer les modifications dans le projet Xcode
        project.WriteToFile(projectPath);
    }
}

