using b1;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatSageMod
{
    [HarmonyPatch(typeof(BGUPlayerCharacterCS), "AfterInitAllComp")]
    public class PatchCharacterCS
    {
        private static void Postfix(BGUPlayerCharacterCS __instance)
        {
            bool flag = __instance != null;
            if (flag)
            {
                //BossRushV3.s_CustomPlayerResetComp = __instance.ActorCompContainerCS.AddComp<CustomPlayerResetComp>(new CustomPlayerResetComp(), int.MaxValue, 0);
                __instance.ActorCompContainerCS.AddComp<GreatSageMod.BUS_QiTianDaShengComp>(new BUS_QiTianDaShengComp(), int.MaxValue, 0);
                //__instance.ActorCompContainerCS
                Console.WriteLine("替换天命人齐天大圣组件!");
            }
        }

    }
}
