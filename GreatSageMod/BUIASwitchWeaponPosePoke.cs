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
        // Token: 0x06011480 RID: 70784 RVA: 0x00498915 File Offset: 0x00496B15
        public BUIASwitchWeaponPosePoke()
        {
            this.InputActionType = EInputActionType.SwitchWeaponPosePoke;
        }

        protected override void BeforeSwitchWeaponPose(AActor player)
        {
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
                    bus_GSEventCollection.Evt_TriggerBanTrans2DaSheng.Invoke();
                }
            }
        }

        // Token: 0x06011481 RID: 70785 RVA: 0x00034E37 File Offset: 0x00033037
        protected override int GetStanceType()
        {
            return 2;
        }

    }
}
