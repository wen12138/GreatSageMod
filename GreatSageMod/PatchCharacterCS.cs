using b1;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using b1.ECS;

namespace GreatSageMod
{
    [HarmonyPatch(typeof(BGUPlayerCharacterCS), "InitAllComp")]
    public class PatchCharacterCS
    {
        private static void Postfix(BGUPlayerCharacterCS __instance)
        {
            if (__instance != null)
            {
                EntityManager entMgr = null;
                {
                    Type worldType = __instance.ActorCompContainerCS.ECSWorld.GetType();
                    var nonPublicFields = worldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var field in nonPublicFields)
                    {
                        if (field.Name == "EntMgr")
                        {
                            entMgr = field.GetValue(__instance.ActorCompContainerCS.ECSWorld) as EntityManager;
                            break;
                        }
                    }
                }

                var oriComp = entMgr?.GetObject<b1.BUS_QiTianDaShengComp>(__instance.ECSEntity);
                if (oriComp != null)
                {
                    Type compContainerType = __instance.ActorCompContainerCS.GetType();
                    int removeCount = 0;

                    {
                        var compCSs = compContainerType.GetField("CompCSs", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (compCSs != null)
                        {
                            var compList = compCSs.GetValue(__instance.ActorCompContainerCS) as List<UActorCompBaseCS>;
                            compList.Remove(oriComp);
                            removeCount++;
                            Utils.Log("Remove Origin QTDSComp Form CompCSs"); 
                        }
                    }

                    {
                        var compCSs = compContainerType.GetField("CompCSsToBeginPlay", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (compCSs != null)
                        {
                            var compList = compCSs.GetValue(__instance.ActorCompContainerCS) as List<UActorCompBaseCS>;
                            compList.Remove(oriComp);
                            removeCount++;
                            Utils.Log("Remove Origin QTDSComp From CompCSsToBeginPlay");
                        }
                    }

                    if (removeCount == 2)
                    {
                        entMgr.RemoveObject(__instance.ECSEntity, oriComp);
                        Utils.Log("Remove Origin QTDSComp From EntityManager");

                        if (oriComp.IsNetActive())
                        {
                            oriComp.OnNetDeActive();
                        }

                        oriComp.OnEndPlay(UnrealEngine.Engine.EEndPlayReason.Destroyed);
                        __instance.ActorCompContainerCS.RegisterUnitComp<BUS_QiTianDaShengComp>(int.MinValue, (EActorCompAlterFlag)0L, (EActorCompRejectFlag)0L, int.MaxValue, 0);
                        Utils.Log("Replace QTDS Comp Successfully!");
                    }

                }
                else
                    Utils.Log("Replace QTDS Comp Failed");
            }
        }

    }
}
