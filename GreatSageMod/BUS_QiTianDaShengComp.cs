using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveB1;
using b1;
using BtlB1;
using BtlShare;
using CommB1;
using UnrealEngine.Engine;

namespace GreatSageMod
{
    public class BUS_QiTianDaShengComp : b1.BUS_QiTianDaShengComp
    {
        private DSRoleData RoleData;
        private Stance m_PreStance;
        private bool bHasInit = false;
        private BUC_QiTianDaShengData QiTianDaShengData;
        private IBUC_BuffData BuffData;
        private IBUC_PassiveSkillData PassiveSkillData;

        public override void OnAttach()
        {
            QiTianDaShengData = RequireWritableData<BUC_QiTianDaShengData>();
            this.BuffData = base.RequireReadOnlyData<IBUC_BuffData, BUC_BuffData>();
            this.PassiveSkillData = base.RequireReadOnlyData<IBUC_PassiveSkillData, BUC_PassiveSkillData>();
            TryGetRoleData();

            base.BUSEventCollection.Evt_TriggerTrans2DaSheng += this.OnTriggerTrans2DaSheng;
            base.BUSEventCollection.Evt_TriggerBanTrans2DaSheng += this.OnTriggerBanTrans2DaSheng;
            base.BUSEventCollection.Evt_ResetDaShengStatus += this.OnResetDaShengStatus;
            base.BUSEventCollection.Evt_AfterUnitRebirth += this.OnAfterUnitRebirth;
        }

        public override void OnBeginPlay()
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            this.QiTianDaShengData.DaShengDurationTimer = 86400f;
        }

        private void TryGetRoleData()
        {
            var gameplayer = GSGBtl.GetLocalPlayerContainer().GamePlayer;
            if (gameplayer == null)
            {
                Utils.Log("New QTSD Comp Get Gameplayer Failed!");
                return;
            }

            Utils.Log("New QTSD Comp Get Gameplayer Successfully!!");
            var type = gameplayer.GetType();
            var field = type.GetField("RootData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                RoleData = field.GetValue(gameplayer) as DSRoleData;
                if (RoleData != null)
                {
                    m_PreStance = RoleData.RoleCs.Actor.Wear.Stance;

                    Utils.Log($"New QTSD Comp Get Roleplayer Data Successfully!! Current Stance: {m_PreStance}");
                }
                else
                {
                    Utils.Log("New QTSD Comp Get Roleplayer Data Failed!!!");
                }
            }
        }

        public override void OnTickWithGroup(float DeltaTime, int TickGroup)
        {
            if (!this.bHasInit)
            {
                this.bHasInit = true;
                this.InitDaShengConfig();
            }
            if (this.bHasInit)
            {
                switch (this.QiTianDaShengData.DaShengStage)
                {
                    case EDaShengStage.LittleMonkey:
                        if (!this.QiTianDaShengData.HasValidDescInfo)
                        {
                            return;
                        }
                        if (this.CheckCanKeepDaShengMode())
                        {
                            if (this.IsInHGSLevel())
                            {
                                this.TrySwitch2DaShengMode(EDaShengStage.LittleMonkey);
                                return;
                            }
                            this.TrySwitch2PreStage(EDaShengStage.LittleMonkey);
                            return;
                        }
                        break;
                    case EDaShengStage.PreStage:
                        if (!this.CheckCanKeepDaShengMode())
                        {
                            this.TrySwitch2LittleMonkey(EDaShengStage.PreStage);
                            return;
                        }
                        break;
                    case EDaShengStage.DaShengMode:
                        {
                            bool flag = this.CheckCanKeepDaShengMode();
                            if (!flag)
                            {
                                this.TrySwitch2LittleMonkey(EDaShengStage.DaShengMode);
                            }
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        private bool IsInHGSLevel()
        {
            return RoleData.RoleCs.Actor.Wear.Stance == Stance.Prop;
        }

        private void InitDaShengConfig()
        {
            Utils.Log("InitDaShengConfig Begin!");

            this.QiTianDaShengData.PreDaSheng_BeginTriggerEffectIDList = new List<int>();
            this.QiTianDaShengData.PreDaSheng_BeginTriggerBuffIDList = new List<int>();
            this.QiTianDaShengData.PreDaSheng_SustainTriggerBuffIDList = new List<int>() { 1999 };
            this.QiTianDaShengData.DaSheng_BeginTriggerEffectIDList = new List<int>();
            this.QiTianDaShengData.DaSheng_BeginTriggerBuffIDList = new List<int>();
            this.QiTianDaShengData.DaSheng_SustainTriggerBuffIDList = new List<int>() { 601, 602 };
            this.QiTianDaShengData.HasValidDescInfo = true;

            Utils.Log("InitDaShengConfig Finish!");
        }

        private void OnTriggerTrans2DaSheng()
        {
            if (this.QiTianDaShengData.DaShengStage == EDaShengStage.PreStage)
            {
                this.TrySwitch2DaShengMode(this.QiTianDaShengData.DaShengStage);
            }
        }

        private void OnTriggerBanTrans2DaSheng()
        {
            //this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            //this.Reset2LittleMonkey();
        }

        private void OnResetDaShengStatus()
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            this.Reset2LittleMonkey();
        }

        private void OnAfterUnitRebirth(ERebirthType RebirthType)
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            this.Reset2LittleMonkey();
        }

        private bool CheckCanKeepDaShengMode()
        {
            return RoleData.RoleCs.Actor.Wear.Stance == Stance.Prop;
        }

        private void TrySwitch2LittleMonkey(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.LittleMonkey;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            if (LastStage != EDaShengStage.PreStage)
            {
                if (LastStage != EDaShengStage.DaShengMode)
                {
                    return;
                }
            }
            else
            {
                foreach (int buffID in this.QiTianDaShengData.PreDaSheng_BeginTriggerBuffIDList)
                {
                    if (this.BuffData.HasBuff(buffID))
                    {
                        base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID, EBuffEffectTriggerType.None, true);
                    }
                }
                using (List<int>.Enumerator enumerator = this.QiTianDaShengData.PreDaSheng_SustainTriggerBuffIDList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        int buffID2 = enumerator.Current;
                        if (this.BuffData.HasBuff(buffID2))
                        {
                            base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID2, EBuffEffectTriggerType.None, true);
                        }
                    }
                    return;
                }
            }
            base.BUSEventCollection.Evt_ComboGraphReset.Invoke();
            foreach (int buffID3 in this.QiTianDaShengData.DaSheng_BeginTriggerBuffIDList)
            {
                if (this.BuffData.HasBuff(buffID3))
                {
                    base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID3, EBuffEffectTriggerType.None, true);
                }
            }
            foreach (int buffID4 in this.QiTianDaShengData.DaSheng_SustainTriggerBuffIDList)
            {
                if (this.BuffData.HasBuff(buffID4))
                {
                    base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID4, EBuffEffectTriggerType.None, true);
                }
            }
        }

        private void TrySwitch2PreStage(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.PreStage;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            if (LastStage == EDaShengStage.LittleMonkey)
            {
                this.EnterPreDaSheng();
            }
        }

        private void TrySwitch2DaShengMode(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.DaShengMode;
            //this.QiTianDaShengData.DaShengDurationTimer = -1f;
            if (LastStage == EDaShengStage.LittleMonkey)
            {
                this.EnterDaShengMode(false);
                return;
            }
            if (LastStage != EDaShengStage.PreStage)
            {
                return;
            }
            this.EnterDaShengMode(true);
        }

        private void EnterPreDaSheng()
        {
            if (this.QiTianDaShengData.PreDaSheng_BeginTriggerEffectIDList != null && this.QiTianDaShengData.PreDaSheng_BeginTriggerEffectIDList.Count > 0)
            {
                FEffectInstReq effectInstReq = new FEffectInstReq(this.Owner)
                {
                    HitLocation = this.Owner.BGUGetActorLocation(),
                    HitPointNormalDir = this.Owner.BGUGetActorRotation(),
                    HitActionDir = EHitActionDir.Default
                };
                foreach (int effectID in this.QiTianDaShengData.PreDaSheng_BeginTriggerEffectIDList)
                {
                    base.BUSEventCollection.Evt_TriggerSkillEffect.Invoke(effectID, effectInstReq, null, true);
                }
            }
            foreach (int buffID in this.QiTianDaShengData.PreDaSheng_BeginTriggerBuffIDList)
            {
                BuffDescRuntime buffDescRuntime = BGW_GameDB.GetBuffDescRuntime(buffID, this.PassiveSkillData);
                if (buffDescRuntime != null)
                {
                    base.BUSEventCollection.Evt_BuffAdd.Invoke(buffID, this.Owner, this.Owner, (float)buffDescRuntime.GetDuration(), EBuffSourceType.Trans2DaSheng, false, default(FBattleAttrSnapShot));
                }
            }
            foreach (int buffID2 in this.QiTianDaShengData.PreDaSheng_SustainTriggerBuffIDList)
            {
                base.BUSEventCollection.Evt_BuffAdd.Invoke(buffID2, this.Owner, this.Owner, -1f, EBuffSourceType.Trans2DaSheng, false, default(FBattleAttrSnapShot));
            }
        }

        private void EnterDaShengMode(bool NeedRemovePreStageBuff)
        {
            base.BUSEventCollection.Evt_ComboGraphReset.Invoke();
            this.QiTianDaShengData.DaShengDurationTimer = 86400f;

            if (NeedRemovePreStageBuff)
            {
                foreach (int buffID in this.QiTianDaShengData.PreDaSheng_BeginTriggerBuffIDList)
                {
                    if (this.BuffData.HasBuff(buffID))
                    {
                        base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID, EBuffEffectTriggerType.None, true);
                    }
                }
                foreach (int buffID2 in this.QiTianDaShengData.PreDaSheng_SustainTriggerBuffIDList)
                {
                    if (this.BuffData.HasBuff(buffID2))
                    {
                        base.BUSEventCollection.Evt_BuffRemoveImmediately.Invoke(buffID2, EBuffEffectTriggerType.None, true);
                    }
                }
            }
            if (this.QiTianDaShengData.DaSheng_BeginTriggerEffectIDList != null && this.QiTianDaShengData.DaSheng_BeginTriggerEffectIDList.Count > 0)
            {
                FEffectInstReq effectInstReq = new FEffectInstReq(this.Owner)
                {
                    HitLocation = this.Owner.BGUGetActorLocation(),
                    HitPointNormalDir = this.Owner.BGUGetActorRotation(),
                    HitActionDir = EHitActionDir.Default
                };
                foreach (int effectID in this.QiTianDaShengData.DaSheng_BeginTriggerEffectIDList)
                {
                    base.BUSEventCollection.Evt_TriggerSkillEffect.Invoke(effectID, effectInstReq, null, true);
                }
            }
            foreach (int buffID3 in this.QiTianDaShengData.DaSheng_BeginTriggerBuffIDList)
            {
                BuffDescRuntime buffDescRuntime = BGW_GameDB.GetBuffDescRuntime(buffID3, this.PassiveSkillData);
                if (buffDescRuntime != null)
                {
                    base.BUSEventCollection.Evt_BuffAdd.Invoke(buffID3, this.Owner, this.Owner, (float)buffDescRuntime.GetDuration(), EBuffSourceType.Trans2DaSheng, false, default(FBattleAttrSnapShot));
                }
            }
            foreach (int buffID4 in this.QiTianDaShengData.DaSheng_SustainTriggerBuffIDList)
            {
                base.BUSEventCollection.Evt_BuffAdd.Invoke(buffID4, this.Owner, this.Owner, -1f, EBuffSourceType.Trans2DaSheng, false, default(FBattleAttrSnapShot));
            }
        }

        private void Reset2LittleMonkey()
        {
            this.TrySwitch2LittleMonkey(this.QiTianDaShengData.DaShengStage);
        }
    }
}
