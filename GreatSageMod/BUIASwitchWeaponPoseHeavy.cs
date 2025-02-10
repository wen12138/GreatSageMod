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
        // Token: 0x0601147E RID: 70782 RVA: 0x00498905 File Offset: 0x00496B05
        public BUIASwitchWeaponPoseHeavy()
        {
            this.InputActionType = EInputActionType.SwitchWeaponPoseHeavy;
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

        // Token: 0x0601147F RID: 70783 RVA: 0x000304B5 File Offset: 0x0002E6B5
        protected override int GetStanceType()
        {
            return 0;
        }
    }
}
