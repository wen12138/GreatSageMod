using b1;
using BtlShare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;

namespace GreatSageMod
{
    public class BUIASwitchWeaponPosePoke : BUIASwitchWeaponPoseBase
    {
        public BUIASwitchWeaponPosePoke()
        {
            this.InputActionType = EInputActionType.SwitchWeaponPosePoke;
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
                bool isAlreadyInPoke = GreateSageMod.DaShengComp.RoleData.RoleCs.Actor.Wear.Stance == ArchiveB1.Stance.Poke;
                if (isAlreadyInPoke)
                {
                    if (GreateSageMod.Stance2DaSheng)
                    {
                        GreateSageMod.Stance2DaSheng = false;
                        bus_GSEventCollection.Evt_ResetDaShengStatus.Invoke();
                    }
                    else if (GreateSageMod.Config.EnterGreatSageModeFromThrustStance)
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
            return 2;
        }

    }
}
