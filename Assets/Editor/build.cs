using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

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

        //...

        project.SetBuildProperty(project.ProjectGuid(), "ENABLE_BITCODE", "NO");

        //...

        project.WriteToFile(projectPath);
    }
}
