using System.Collections.Generic;

namespace GameFramework
{
    public class DefineSymbolsSetting : ScriptableObjectSingleton<DefineSymbolsSetting>
    {
        public List<string> DefineSymbols = new List<string>()
        {
            "ENABLE_LOG",
            "MOBILE_INPUT",
            "ENABLE_CONSOLE"
        };
    }
}