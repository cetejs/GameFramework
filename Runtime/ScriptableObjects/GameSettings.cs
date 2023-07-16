using System.Collections.Generic;

namespace GameFramework
{
    public class GameSettings : ScriptableObjectSingleton<GameSettings>
    {
        public List<string> GlobalAssemblyNames = new List<string>()
        {
            "GameFramework",
            "Assembly-CSharp"
        };

        public List<string> RuntimeReloadAssemblyNames = new List<string>()
        {
        };

        public List<string> DevConsoleAssemblyNames = new List<string>()
        {
        };
    }
}