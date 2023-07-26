using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InputSettingNameAttribute : BundleAssetNameAttribute
    {
        public InputSettingNameAttribute() : base(GameSettings.Instance.InputSettingAssetName, "asset")
        {
        }
    }
}