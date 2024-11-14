using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using LunarConstruct.Enemies;
using LunarConstruct.Projectiles;
using LunarConstruct.SkillDefs;
using R2API;
using R2API.ContentManagement;
using RoR2.ExpansionManagement;
using System.Reflection;

namespace LunarConstruct
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "LunarConstruct";
        public const string PluginVersion = "1.0.2";

        public static ExpansionDef SOTVExpansionDef;

        public static ManualLogSource LCLogger;

        public static AssetBundle lunarConstruct;

        public static ConfigEntry<bool> useWolfoNameScheme { get; set; }

        public void Awake()
        {
            LCLogger = base.Logger;
            SOTVExpansionDef = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();

            useWolfoNameScheme = Config.Bind("Name", "Use WolfoQoL name scheme?", false, "Changes Lunar Construct's in-game name from Lunar Chimera to Lunar Chimera (Construct).");

            lunarConstruct = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("LunarConstruct.dll", "lunarconstruct"));
            LunarShard.Init();
            TeleportEffect.Init();

            var skillTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SkillDefBase)));

            foreach (var skillType in skillTypes)
            {
                SkillDefBase skill = (SkillDefBase)System.Activator.CreateInstance(skillType);
                skill.Create();
            }

            var enemyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase)));

            foreach (var enemyType in enemyTypes)
            {
                EnemyBase enemy = (EnemyBase)System.Activator.CreateInstance(enemyType);
                enemy.Create();
            }
        }
    }
}