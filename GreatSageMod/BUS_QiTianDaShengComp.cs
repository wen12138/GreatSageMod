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
    // Token: 0x020007FC RID: 2044
    public class BUS_QiTianDaShengComp : UActorCompBaseCS
    {
        // Token: 0x0600513B RID: 20795 RVA: 0x0013E8FC File Offset: 0x0013CAFC
        public override void OnAttach()
        {
            TryGetRoleData();

            this.QiTianDaShengData = base.RequireWritableData<BUC_QiTianDaShengData>();
            this.EquipData = base.RequireReadOnlyData<IBUC_EquipData, BUC_EquipData>();
            this.BuffData = base.RequireReadOnlyData<IBUC_BuffData, BUC_BuffData>();
            this.PassiveSkillData = base.RequireReadOnlyData<IBUC_PassiveSkillData, BUC_PassiveSkillData>();
            this.SimpleStateData = base.RequireReadOnlyData<IBUC_SimpleStateData, BUC_SimpleStateData>();
            APawn apawn = base.GetOwner() as APawn;
            this.RoleBaseData = BGU_DataUtil.GetReadOnlyData<IBPC_RoleBaseData, BPC_RoleBaseData>((apawn != null) ? apawn.PlayerState : null);
            this.LevelData = base.RequireReadonlyGameInstanceData<IBIC_LevelData, BIC_LevelData>();
            base.BUSEventCollection.Evt_TriggerTrans2DaSheng += this.OnTriggerTrans2DaSheng;
            base.BUSEventCollection.Evt_TriggerBanTrans2DaSheng += this.OnTriggerBanTrans2DaSheng;
            base.BUSEventCollection.Evt_ResetDaShengStatus += this.OnResetDaShengStatus;
            base.BUSEventCollection.Evt_AfterUnitRebirth += this.OnAfterUnitRebirth;

            Console.WriteLine("齐天大圣组件 OnAttach 成功!");
        }

        private void TryGetRoleData()
        {
            var gameplayer = GSGBtl.GetLocalPlayerContainer().GamePlayer;
            if (gameplayer == null)
            {
                Console.WriteLine("齐天大圣组件获取玩家失败!");
                return;
            }

            Console.WriteLine("齐天大圣组件获取玩家成功!");
            var type = gameplayer.GetType();
            var field = type.GetField("RootData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                RoleData = field.GetValue(gameplayer) as DSRoleData;
                if (RoleData != null)
                {
                    Console.WriteLine("齐天大圣组件获取玩家数据成功!");
                }
                else
                {
                    Console.WriteLine("齐天大圣组件获取玩家数据失败!");
                }
            }
        }

        // Token: 0x0600513C RID: 20796 RVA: 0x0013E9FC File Offset: 0x0013CBFC
        public override void OnBeginPlay()
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
        }

        // Token: 0x0600513D RID: 20797 RVA: 0x00002858 File Offset: 0x00000A58
        public override int GetTickGroupMask()
        {
            return 1;
        }

        // Token: 0x0600513E RID: 20798 RVA: 0x0013EA1C File Offset: 0x0013CC1C
        public override void OnTickWithGroup(float DeltaTime, int TickGroup)
        {
            if (!this.bHasInit)
            {
                this.bHasInit = true;
                int configID = this.IsInHGSLevel() ? BUS_QiTianDaShengComp.HGS_DASHENG_CONFIG_ID : BUS_QiTianDaShengComp.NORMAL_DASHENG_CONFIG_ID;
                this.InitDaShengConfig(configID);
                Console.WriteLine("齐天大圣组件 OnTickWithGroup Init 成功!");
            }
            if (this.bHasInit)
            {
                if (this.SimpleStateData.HasSimpleState(EBGUSimpleState.BanTrans2DaSheng) || this.QiTianDaShengData.bIsBanTrans2DaSheng)
                {
                    this.Reset2LittleMonkey();
                    return;
                }
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
                            //bool flag2 = RoleData != null && RoleData.RoleCs.Actor.Wear.Stance == Stance.Prop;
                            if (!this.IsInHGSLevel())
                            {
                                this.QiTianDaShengData.DaShengDurationTimer -= DeltaTime;
                                if (this.QiTianDaShengData.DaShengDurationTimer <= 0f)
                                {
                                    flag = false;
                                }
                            }
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

        // Token: 0x0600513F RID: 20799 RVA: 0x0013EB24 File Offset: 0x0013CD24
        private void InitDaShengConfig(int ConfigID)
        {
            FUStTransQiTianDaShengConfigDesc transQiTianDaShengConfigDesc = BGW_GameDB.GetTransQiTianDaShengConfigDesc(ConfigID);
            if (transQiTianDaShengConfigDesc != null)
            {
                this.QiTianDaShengData.PreDaSheng_BeginTriggerEffectIDList = transQiTianDaShengConfigDesc.PreDaShengBeginTriggerEffectIDList.ToList<int>();
                this.QiTianDaShengData.PreDaSheng_BeginTriggerBuffIDList = transQiTianDaShengConfigDesc.PreDaShengBeginTriggerBuffIDList.ToList<int>();
                this.QiTianDaShengData.PreDaSheng_SustainTriggerBuffIDList = transQiTianDaShengConfigDesc.PreDaShengSustainTriggerBuffIDList.ToList<int>();
                this.QiTianDaShengData.DaSheng_BeginTriggerEffectIDList = transQiTianDaShengConfigDesc.DaShengBeginTriggerEffectIDList.ToList<int>();
                this.QiTianDaShengData.DaSheng_BeginTriggerBuffIDList = transQiTianDaShengConfigDesc.DaShengBeginTriggerBuffIDList.ToList<int>();
                this.QiTianDaShengData.DaSheng_SustainTriggerBuffIDList = transQiTianDaShengConfigDesc.DaShengSustainTriggerBuffIDList.ToList<int>();
                this.QiTianDaShengData.RelatedTalentIDList = transQiTianDaShengConfigDesc.RelatedTalentIDList.ToList<int>();
                this.QiTianDaShengData.RelatedEquipIDList = transQiTianDaShengConfigDesc.RelatedEquipIDList.ToList<int>();
                if (this.QiTianDaShengData.RelatedTalentIDList.Count > 0 || this.QiTianDaShengData.RelatedEquipIDList.Count > 0)
                {
                    this.QiTianDaShengData.HasValidDescInfo = true;
                }
            }
        }

        // Token: 0x06005140 RID: 20800 RVA: 0x0013EC20 File Offset: 0x0013CE20
        private bool CheckCanKeepDaShengMode()
        {
            if (this.IsInHGSLevel())
            {
                bool result = false;
                if (this.QiTianDaShengData.RelatedEquipIDList.Count > 0)
                {
                    result = this.QiTianDaShengData.RelatedEquipIDList.All((int item) => this.EquipData.SelfEquipMap.Values.Contains(item));
                }
                return result;
            }
            bool flag = false;
            bool flag2 = false;
            if (this.QiTianDaShengData.RelatedEquipIDList.Count > 0)
            {
                flag = this.QiTianDaShengData.RelatedEquipIDList.All((int item) => this.EquipData.SelfEquipMap.Values.Contains(item));
            }
            if (this.QiTianDaShengData.RelatedTalentIDList.Count > 0)
            {
                flag2 = this.QiTianDaShengData.RelatedTalentIDList.All((int item) => this.RoleBaseData.TalenList.Keys.Contains(item));
            }
            return flag && flag2;
            //return true;
        }

        // Token: 0x06005141 RID: 20801 RVA: 0x0013ECD0 File Offset: 0x0013CED0
        private bool IsInHGSLevel()
        {
            return this.LevelData.CurrentLevelID == BUS_QiTianDaShengComp.HGS_LEVEL_ID || this.LevelData.CurrentLevelID == BUS_QiTianDaShengComp.HGS_SHI_ZHONG_JING_LEVEL_ID;
        }

        // Token: 0x06005142 RID: 20802 RVA: 0x0013ECFC File Offset: 0x0013CEFC
        private void TrySwitch2LittleMonkey(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.LittleMonkey;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
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

        // Token: 0x06005143 RID: 20803 RVA: 0x0013EED0 File Offset: 0x0013D0D0
        private void TrySwitch2PreStage(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.PreStage;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
            if (LastStage == EDaShengStage.LittleMonkey)
            {
                this.EnterPreDaSheng();
            }
        }

        // Token: 0x06005144 RID: 20804 RVA: 0x0013EEF7 File Offset: 0x0013D0F7
        private void TrySwitch2DaShengMode(EDaShengStage LastStage)
        {
            this.QiTianDaShengData.DaShengStage = EDaShengStage.DaShengMode;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
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

        // Token: 0x06005145 RID: 20805 RVA: 0x0013EF2C File Offset: 0x0013D12C
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

        // Token: 0x06005146 RID: 20806 RVA: 0x0013F0EC File Offset: 0x0013D2EC
        private void EnterDaShengMode(bool NeedRemovePreStageBuff)
        {
            base.BUSEventCollection.Evt_ComboGraphReset.Invoke();
            if (!this.IsInHGSLevel())
            {
                BGUGlobalConfigInfo bguglobalConfigInfo;
                if (BGW_GameDB.GetGlobalConfigByAlias(B1GlobalFNames.NORMAL_DASHENG_DURATION, out bguglobalConfigInfo))
                {
                    this.QiTianDaShengData.DaShengDurationTimer = bguglobalConfigInfo.FloatValue;
                }
                else
                {
                    this.QiTianDaShengData.DaShengDurationTimer = BUS_QiTianDaShengComp.NORMAL_DASHENG_DURATION;
                }
            }
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

        // Token: 0x06005147 RID: 20807 RVA: 0x0013F3C0 File Offset: 0x0013D5C0
        private void Reset2LittleMonkey()
        {
            this.TrySwitch2LittleMonkey(this.QiTianDaShengData.DaShengStage);
        }

        // Token: 0x06005148 RID: 20808 RVA: 0x0013F3D3 File Offset: 0x0013D5D3
        private void OnTriggerTrans2DaSheng()
        {
            if (this.QiTianDaShengData.DaShengStage == EDaShengStage.PreStage)
            {
                this.TrySwitch2DaShengMode(this.QiTianDaShengData.DaShengStage);
            }
            else
            {
                this.TrySwitch2DaShengMode(EDaShengStage.LittleMonkey);
            }
        }

        // Token: 0x06005149 RID: 20809 RVA: 0x0013F3F4 File Offset: 0x0013D5F4
        private void OnTriggerBanTrans2DaSheng()
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
            this.Reset2LittleMonkey();
        }

        // Token: 0x0600514A RID: 20810 RVA: 0x0013F418 File Offset: 0x0013D618
        private void OnResetDaShengStatus()
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
            this.Reset2LittleMonkey();
        }

        // Token: 0x0600514B RID: 20811 RVA: 0x0013F418 File Offset: 0x0013D618
        private void OnAfterUnitRebirth(ERebirthType RebirthType)
        {
            this.QiTianDaShengData.bIsBanTrans2DaSheng = false;
            this.QiTianDaShengData.DaShengDurationTimer = -1f;
            this.Reset2LittleMonkey();
        }

        // Token: 0x04003FD8 RID: 16344
        private static int NORMAL_DASHENG_CONFIG_ID = 1;

        // Token: 0x04003FD9 RID: 16345
        private static int HGS_DASHENG_CONFIG_ID = 2;

        // Token: 0x04003FDA RID: 16346
        private static int HGS_LEVEL_ID = 98;

        // Token: 0x04003FDB RID: 16347
        private static int HGS_SHI_ZHONG_JING_LEVEL_ID = 61;

        // Token: 0x04003FDC RID: 16348
        private static float NORMAL_DASHENG_DURATION = 86400f;

        // Token: 0x04003FDD RID: 16349
        private BUC_QiTianDaShengData QiTianDaShengData;

        // Token: 0x04003FDE RID: 16350
        private IBUC_EquipData EquipData;

        // Token: 0x04003FDF RID: 16351
        private IBUC_BuffData BuffData;

        // Token: 0x04003FE0 RID: 16352
        private IBUC_PassiveSkillData PassiveSkillData;

        // Token: 0x04003FE1 RID: 16353
        private IBUC_SimpleStateData SimpleStateData;

        // Token: 0x04003FE2 RID: 16354
        private IBPC_RoleBaseData RoleBaseData;

        // Token: 0x04003FE3 RID: 16355
        private IBIC_LevelData LevelData;

        // Token: 0x04003FE4 RID: 16356
        private bool bHasInit;

        private DSRoleData RoleData;
    }
}
