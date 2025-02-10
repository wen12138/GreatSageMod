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
    [HarmonyPatch(typeof(BGUPlayerCharacterCS), "AfterInitAllComp")]
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
                var newComp = new BUS_QiTianDaShengComp();
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
                            compList.Add(newComp);
                            Console.WriteLine("从CompCSs移除了原始的齐天大圣组件"); 
                        }
                    }

                    {
                        var compCSs = compContainerType.GetField("CompCSsToBeginPlay", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (compCSs != null)
                        {
                            var compList = compCSs.GetValue(__instance.ActorCompContainerCS) as List<UActorCompBaseCS>;
                            compList.Remove(oriComp);
                            removeCount++;
                            compList.Add(newComp);
                            Console.WriteLine("从CompCSsToBeginPlay移除了原始的齐天大圣组件");
                        }
                    }

                    if (removeCount == 2)
                    {
                        entMgr.RemoveObject(__instance.ECSEntity, oriComp);
                        Console.WriteLine("从EntityManager中移除了原始的齐天大圣组件");

                        if (oriComp.IsNetActive())
                        {
                            oriComp.OnNetDeActive();
                        }

                        oriComp.OnEndPlay(UnrealEngine.Engine.EEndPlayReason.Destroyed);

                        newComp.Init(__instance.ActorCompContainerCS);
                        newComp.OnAttach();
                        entMgr.SetObject(__instance.ECSEntity, TypeManager.GetTypeIndex<b1.BUS_QiTianDaShengComp>(), newComp);
                    }

                    Console.WriteLine("成功替换天命人齐天大圣组件!");
                }
                else
                    Console.WriteLine("无法替换天命人齐天大圣组件!");
            }
        }

    }
}
