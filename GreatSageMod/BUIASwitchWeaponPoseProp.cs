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

        protected override int GetStanceType()
        {
            return 1;
        }
    }
}
