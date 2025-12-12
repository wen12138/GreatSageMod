using b1;
using BtlShare;
using CSharpModBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;

namespace GreatSageMod
{
    public class BUIASwitchWeaponPoseProp : BUIASwitchWeaponPoseBase
    {
        public BUIASwitchWeaponPoseProp()
        {
            this.InputActionType = EInputActionType.SwitchWeaponPoseProp;
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
                bool isAlreadyInProp = GreateSageMod.DaShengComp.RoleData.RoleCs.Actor.Wear.Stance == ArchiveB1.Stance.Prop;
                if (isAlreadyInProp)
                {
                    if (GreateSageMod.Stance2DaSheng)
                    {
                        GreateSageMod.Stance2DaSheng = false;
                        bus_GSEventCollection.Evt_ResetDaShengStatus.Invoke();
                    }
                    else if (GreateSageMod.Config.EnterGreatSageModeFromPillarStance)
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
            return 1;
        }
    }
}
