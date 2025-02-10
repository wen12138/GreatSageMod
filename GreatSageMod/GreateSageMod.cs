using CSharpModBase;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatSageMod
{
    public class GreateSageMod : ICSharpMod
    {
        public string Name => "GreatSage";

        public string Version => "0.0.1";

        private Harmony m_Harmony;

        public void Init()
        {
            Console.WriteLine("初始化齐天大圣Mod!!!");
            m_Harmony = new Harmony("GreateSageMode.Patch");
            m_Harmony.PatchAll();
        }

        public void DeInit()
        {
            m_Harmony?.UnpatchAll();
            Console.WriteLine("解除齐天大圣Mod!!!");
        }

    }
}
