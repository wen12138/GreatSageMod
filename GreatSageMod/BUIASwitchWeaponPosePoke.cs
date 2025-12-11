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
            GreateSageMod.Prop2DaSheng = false;

            if (player == null)
            {
                Console.WriteLine("角色为空!");
                return;
            }

            if (player != null && player as ABGUCharacter != null && player.World != null)
            {
                BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(player);
                if (bus_GSEventCollection != null)
                {
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
