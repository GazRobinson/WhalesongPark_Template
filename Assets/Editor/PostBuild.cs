using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;
using UnityEngine.Device;
using UnityEngine.UIElements;

public static class PostBuild
{

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Creating batch file at: " + pathToBuiltProject);
        int lastSlash = pathToBuiltProject.LastIndexOf('/');
        string path = pathToBuiltProject.Substring(0, lastSlash);

        FileStream f = File.Create(path + "/RunBorderless.bat");
        TextWriter w = new StreamWriter(f);
        w.WriteLine(": TV setup");
        w.WriteLine("WhalesongPark.exe -popupwindow -screen-width 2880 -screen-height 1280");
        w.Close(); 
        f.Close();

        f = File.Create(path + "/RunOnTVSetup.bat");
        w = new StreamWriter(f);
        w.WriteLine(": Screen");
        w.WriteLine("WhalesongPark.exe -popupwindow -screen-width 1280 -screen-height 720");
        w.Close();
        f.Close();

        f = File.Create(path + "/RunOnWhalesongSetup.bat");
        w = new StreamWriter(f);
        w.WriteLine(": Whalesong Park");
        w.WriteLine("WhalesongPark.exe -popupwindow -screen-width 1344 -screen-height 672");
        w.Close();
        f.Close();

        CreateDefaultButtons(path);
    }

    static void CreateDefaultButtons(string path)
    {
        if (!File.Exists(path + "/Buttons.txt"))
        {
            FileStream f = File.Create(path + "/Buttons.txt");
            TextWriter w = new StreamWriter(f);
            for (int i = 0; i < 25; i++)
                w.WriteLine(i.ToString());
            w.Close();
            f.Close();
        }
    }
}
