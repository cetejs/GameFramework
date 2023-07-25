using UnityEditor;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(InputSettingNameAttribute))]
    internal class InputSettingNameAttributeDrawer : BundleAssetNameAttributeDrawer
    {
    }
}