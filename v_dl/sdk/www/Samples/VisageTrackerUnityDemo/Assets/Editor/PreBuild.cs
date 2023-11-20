using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

#if UNITY_ANDROID
class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void CopyFileIfNotExists(string src, string dst)
    {
        if (!File.Exists(dst))
            FileUtil.CopyFileOrDirectory(src, dst);
    }

    public void CopyDirIfNotExists(string src, string dst)
    {
        if (!Directory.Exists(dst))
            FileUtil.CopyFileOrDirectory(src, dst);
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        //copy data files
        if (!Directory.Exists("../../data"))
            Debug.Log("Directory visageSDK-Android/Samples/data not found - skipping copying data files");
        else
        {
            if (!Directory.Exists("Assets/StreamingAssets"))
                Directory.CreateDirectory("Assets/StreamingAssets");

            if (!Directory.Exists("Assets/StreamingAssets/Visage Tracker"))
                Directory.CreateDirectory("Assets/StreamingAssets/Visage Tracker");

            CopyDirIfNotExists("../../data/vfa", "Assets/StreamingAssets/Visage Tracker/vfa");
            CopyDirIfNotExists("../../data/vfr", "Assets/StreamingAssets/Visage Tracker/vfr");
            CopyDirIfNotExists("../../data/vft", "Assets/StreamingAssets/Visage Tracker/vft");
            CopyFileIfNotExists("../../data/Facial Features Tracker.cfg", "Assets/StreamingAssets/Visage Tracker/Facial Features Tracker.cfg");
            CopyFileIfNotExists("../../data/Head Tracker.cfg", "Assets/StreamingAssets/Visage Tracker/Head Tracker.cfg");
            CopyFileIfNotExists("../../data/NeuralNet.cfg", "Assets/StreamingAssets/Visage Tracker/NeuralNet.cfg");
        }

        //copy libraries
        if (!Directory.Exists("../../../lib/armeabi-v7a"))
            Debug.Log("Directory visageSDK-Android/lib not found - skipping copying libraries");
        else
        {
            if (!Directory.Exists("Assets/Plugins"))
                Directory.CreateDirectory("Assets/Plugins");

            if (!Directory.Exists("Assets/Plugins/Android"))
                  Directory.CreateDirectory("Assets/Plugins/Android");

            CopyFileIfNotExists("../../../lib/armeabi-v7a/libVisageVision.so", "Assets/Plugins/Android/libVisageVision.so");
            CopyFileIfNotExists("../../../lib/armeabi-v7a/libVisageAnalyser.so", "Assets/Plugins/Android/libVisageAnalyser.so");
            CopyFileIfNotExists("../../../lib/armeabi-v7a/libVisageTrackerUnityPlugin.so", "Assets/Plugins/Android/libVisageTrackerUnityPlugin.so");
            CopyFileIfNotExists("../../../lib/armeabi-v7a/libomp.so", "Assets/Plugins/Android/libomp.so");
            CopyFileIfNotExists("../../../lib/armeabi-v7a/libc++_shared.so", "Assets/Plugins/Android/libc++_shared.so");
            CopyFileIfNotExists("../../../lib/armeabi-v7a/libTFPlugin.so", "Assets/Plugins/Android/libTFPlugin.so");
        }
        if (!Directory.Exists("../../Android/AndroidCameraPlugin/app/release"))
            Debug.Log("Directory visageSDK-Android/Samples/Android/AndroidCameraPlugin/app/release not found - skipping copying AndroidCameraPlugin.jar and AndroidManifest.xml");
        else
        {
            CopyFileIfNotExists("../../Android/AndroidCameraPlugin/app/release/AndroidCameraPlugin.jar", "Assets/Plugins/Android/AndroidCameraPlugin.jar");
            CopyFileIfNotExists("../../Android/AndroidCameraPlugin/app/release/AndroidManifest.xml", "Assets/Plugins/Android/AndroidManifest.xml"); 
        }
    AssetDatabase.Refresh();
	}
}
#endif