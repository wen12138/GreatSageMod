using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveB1;
using b1;
using b1.Prediction;
using BtlShare;
using ResB1;
using UnrealEngine.Engine;
using UnrealEngine.Plugins.EnhancedInput;

namespace GreatSageMod
{
    public class BUIASwitchWeaponPoseBase : BUInputActionTemplate
    {
        // Token: 0x0601147A RID: 70778 RVA: 0x00498804 File Offset: 0x00496A04
        protected override bool OnTriggerInputAction(int InputActionID, ETriggerEvent TriggerEvent, ref b1.FInputActionValue Value, GSPredictionKey PredictionKey)
        {
            Console.WriteLine($"OnTriggerInputAction!! TriggerEvent: {TriggerEvent} Begin!!!");

            AActor owner = base.GetOwner();
            if (owner == null)
            {
                return false;
            }
            if (!BGUFuncLibInput.BGUIsCanReceiveBattleInput(owner) || !BGUFuncLibInput.BGUIsCanReceiveBattleInputByActionType(owner, this.InputActionType))
            {
                return false;
            }
            APlayerController firstLocalPlayerController = UGSE_EngineFuncLib.GetFirstLocalPlayerController(owner);
            
            if (firstLocalPlayerController == null)
            {
                return false;
            }
            int stanceType = this.GetStanceType();
            if (this.CanSwitchWeaponPose(firstLocalPlayerController, (Stance)stanceType))
            {
                BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(owner);
                if (bus_GSEventCollection != null)
                {
                    //BeforeSwitchWeaponPose(owner);
                    Console.WriteLine($"OnTriggerInputAction!! TriggerEvent: {TriggerEvent} End!! Next Step Is Delegate!");
                    bus_GSEventCollection.Evt_SwitchWeaponPoseByType.Invoke(stanceType);
                }
                return true;
            }
            return false;
        }

        // Token: 0x0601147B RID: 70779 RVA: 0x000304B5 File Offset: 0x0002E6B5
        protected virtual int GetStanceType()
        {
            return 0;
        }

        protected virtual void BeforeSwitchWeaponPose(AActor player)
        {

        }

        // Token: 0x0601147C RID: 70780 RVA: 0x00498878 File Offset: 0x00496A78
        protected bool CanSwitchWeaponPose(APlayerController Controller, Stance Stance)
        {
            if (Controller != null)
            {
                IBPC_PlayerTagData readOnlyData = BGU_DataUtil.GetReadOnlyData<IBPC_PlayerTagData, BPC_PlayerTagData>(Controller.PlayerState);
                if (readOnlyData == null || readOnlyData.HasTag(EBGPPlayerTag.Transforming))
                {
                    return false;
                }
                IBPC_PlayerRoleData readOnlyData2 = BGU_DataUtil.GetReadOnlyData<IBPC_PlayerRoleData, BPC_PlayerRoleData>(Controller);
                if (readOnlyData2 == null)
                {
                    return false;
                }
                switch (Stance)
                {
                    case Stance.Heavy:
                        return true;
                    case Stance.Prop:
                        {
                            int commLogicCfgValue = GameDBRuntime.GetCommLogicCfgValue(CommCfgType.PropStance);
                            return RoleDataHelper.IsTalentExist(readOnlyData2.RoleData.RoleCs, commLogicCfgValue);
                        }
                    case Stance.Poke:
                        {
                            int commLogicCfgValue2 = GameDBRuntime.GetCommLogicCfgValue(CommCfgType.PokeStance);
                            return RoleDataHelper.IsTalentExist(readOnlyData2.RoleData.RoleCs, commLogicCfgValue2);
                        }
                }
            }
            return false;
        }

    }
}
