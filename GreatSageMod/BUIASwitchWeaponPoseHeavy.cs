using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using b1;
using BtlShare;
using UnrealEngine.Engine;

namespace GreatSageMod
{
    public class BUIASwitchWeaponPoseHeavy : BUIASwitchWeaponPoseBase
    {
        public BUIASwitchWeaponPoseHeavy()
        {
            this.InputActionType = EInputActionType.SwitchWeaponPoseHeavy;
        }

        protected override void BeforeSwitchWeaponPose(AActor player)
        {
            if (player == null)
            {
                Utils.Log("player is null!");
                return;
            }

            BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(player);

            if (bus_GSEventCollection == null)
            {
                Utils.Log("bus_GSEventCollection is null!");
                return;
            }

            if (player != null && player as ABGUCharacter != null && player.World != null && GreateSageMod.DaShengComp != null)
            {
                bool isAlreadyInHeavy = GreateSageMod.DaShengComp.RoleData.RoleCs.Actor.Wear.Stance == ArchiveB1.Stance.Heavy;
                if (isAlreadyInHeavy)
                {
                    if (GreateSageMod.Stance2DaSheng)
                    {
                        GreateSageMod.Stance2DaSheng = false;
                        bus_GSEventCollection.Evt_ResetDaShengStatus.Invoke();
                    }
                    else if (GreateSageMod.Config.EnterGreatSageModeFromSmashStance)
                    {
                        GreateSageMod.Stance2DaSheng = true;
                        bus_GSEventCollection.Evt_TriggerTrans2DaSheng.Invoke();
                    }
                }
                else
                {
                    GreateSageMod.Stance2DaSheng = false;
                    bus_GSEventCollection.Evt_ResetDaShengStatus.Invoke();
                }
            }
        }

        protected override int GetStanceType()
        {
            return 0;
        }
    }
}
