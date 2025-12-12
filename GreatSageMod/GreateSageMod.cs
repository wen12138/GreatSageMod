using CSharpModBase;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GreatSageMod
{
    public struct GreatSageModConfig
    {
        public bool EnterGreatSageModeFromSmashStance;
        public bool EnterGreatSageModeFromPillarStance;
        public bool EnterGreatSageModeFromThrustStance;

        public GreatSageModConfig(bool enterGreatSageModeFromSmashStance, bool enterGreatSageModeFromPillarStance, bool enterGreatSageModeFromThrustStance)
        {
            EnterGreatSageModeFromSmashStance = enterGreatSageModeFromSmashStance;
            EnterGreatSageModeFromPillarStance = enterGreatSageModeFromPillarStance;
            EnterGreatSageModeFromThrustStance = enterGreatSageModeFromThrustStance;
        }
    }

    public class GreateSageMod : ICSharpMod
    {
        public string Name => "GreatSage";

        public string Version => "0.0.3";

        private Harmony m_Harmony;
        public static GreatSageModConfig Config;
        public static bool Stance2DaSheng = false;
        public static BUS_QiTianDaShengComp DaShengComp;

        public void Init()
        {
            Utils.Log("Init Greate Sage Mod!!!");
            m_Harmony = new Harmony("GreateSageMode.Patch");
            m_Harmony.PatchAll(Assembly.GetExecutingAssembly());
            InitConfig();
        }

        public static void InitConfig()
        {
            Config = new GreatSageModConfig(false, true, false);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string configPath = Path.Combine(baseDirectory, "CSharpLoader\\Mods\\GreatSageMod\\GreatSageConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                Config = json.FromJson<GreatSageModConfig>();

                Utils.Log($"Load GreatSageModConfig! EnterGreatSageModeFromSmashStance: {Config.EnterGreatSageModeFromSmashStance}, " +
                    $"EnterGreatSageModeFromPillarStance: {Config.EnterGreatSageModeFromPillarStance}, " +
                    $"EnterGreatSageModeFromThrustStance: {Config.EnterGreatSageModeFromThrustStance}");
            }
            else
            {
                Utils.Log("GreatSageMod.json Not Exist! Init Config Failed!");
            }
        }

        public void DeInit()
        {
            m_Harmony?.UnpatchAll();
            Utils.Log("Uninit Greate Sage Mod!!!");
        }

    }
}
