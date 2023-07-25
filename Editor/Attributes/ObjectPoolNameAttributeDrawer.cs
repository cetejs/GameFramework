using UnityEditor;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ObjectPoolNameAttribute))]
    internal class ObjectPoolNameAttributeDrawer : BundleAssetNameAttributeDrawer
    {
    }
}