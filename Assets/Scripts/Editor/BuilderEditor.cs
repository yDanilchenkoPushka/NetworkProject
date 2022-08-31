using UnityEditor;

namespace Editor
{
    public class BuilderEditor
    {
        [MenuItem("Build/Standalone/With Rewired Input")]
        public static void BuildStandaloneWithRewiredInput()
        {
            string defines = "REWIRED_INPUT";
            
            RunStandaloneBuild(defines);
        }
        
        [MenuItem("Build/Standalone/With Unity Input")]
        public static void BuildStandaloneWithUnityInput()
        {
            string defines = "UNITY_INPUT";
            
            RunStandaloneBuild(defines);
        }

        private static void RunStandaloneBuild(string defines)
        {
            string fileName = "TestBuild";
            string fileExtension = ".exe";
            
            string dataPath = EditorUtility.SaveFolderPanel("Windows Build", "", "");
            
            string locationStringPathName = dataPath + "/" + fileName + fileExtension;
            
            string[] scenePaths = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Level.unity" };
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
            BuildPipeline.BuildPlayer(scenePaths, locationStringPathName, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
    }
}