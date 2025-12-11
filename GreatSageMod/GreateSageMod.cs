using CSharpModBase;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GreatSageMod
{
    public class GreateSageMod : ICSharpMod
    {
        public string Name => "GreatSage";

        public string Version => "0.0.2";

        private Harmony m_Harmony;

        public static bool Prop2DaSheng = false;
        public static BUS_QiTianDaShengComp DaShengComp;

        public void Init()
        {
            Utils.Log("Init Greate Sage Mod!!!");
            m_Harmony = new Harmony("GreateSageMode.Patch");
            m_Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void DeInit()
        {
            m_Harmony?.UnpatchAll();
            Utils.Log("Uninit Greate Sage Mod!!!");
        }

    }
}
