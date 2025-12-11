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
                Console.WriteLine("角色为空!");
                return;
            }

            if (player != null && player as ABGUCharacter != null && player.World != null && GreateSageMod.DaShengComp != null)
            {
                if (GreateSageMod.DaShengComp.RoleData.RoleCs.Actor.Wear.Stance == ArchiveB1.Stance.Prop)
                {
                    if (GreateSageMod.Prop2DaSheng)
                    {
                        GreateSageMod.Prop2DaSheng = false;
                        BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(player);
                        if (bus_GSEventCollection != null)
                        {
                            bus_GSEventCollection.Evt_ResetDaShengStatus.Invoke();
                        }
                    }
                    else
                    {
                        GreateSageMod.Prop2DaSheng = true;
                        BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(player);
                        if (bus_GSEventCollection != null)
                        {
                            bus_GSEventCollection.Evt_TriggerTrans2DaSheng.Invoke();
                        }
                    }
                }
            }
        }

        protected override int GetStanceType()
        {
            return 1;
        }
    }
}
