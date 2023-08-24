using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GameFramework
{
    internal class PreprocessBuildProject : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 1; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            new AssetBundleWindow().BuildAssetBundles();
        }
    }
}