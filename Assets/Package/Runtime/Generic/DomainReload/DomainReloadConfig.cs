using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DomainReloadConfig : ScriptableObject
    {
        public List<string> reloadAssemblies = new List<string>();

        private static DomainReloadConfig instance;
        
        public static DomainReloadConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            DomainReloadConfig config = Resources.Load<DomainReloadConfig>("DomainReloadConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }
}