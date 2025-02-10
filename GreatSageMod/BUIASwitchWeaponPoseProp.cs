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
        // Token: 0x06011482 RID: 70786 RVA: 0x00498925 File Offset: 0x00496B25
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

            if (player != null && player as ABGUCharacter != null && player.World != null)
            {
                BUS_GSEventCollection bus_GSEventCollection = BUS_EventCollectionCS.Get(player);
                if (bus_GSEventCollection != null)
                {
                    bus_GSEventCollection.Evt_TriggerTrans2DaSheng.Invoke();
                }
            }
        }

        // Token: 0x06011483 RID: 70787 RVA: 0x00002858 File Offset: 0x00000A58
        protected override int GetStanceType()
        {
            return 1;
        }
    }
}
