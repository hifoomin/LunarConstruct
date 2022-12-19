using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.DamageAPI;

namespace HBT.Skills
{
    public class LightsOut : TweakBase
    {
        public static float Damage;
        public static float Cooldown;
        public static ModdedDamageType cooldownReset = ReserveDamageType();
        public static float CooldownReduction;

        public override string Name => ": Special : Lights Out";

        public override string SkillToken => "special";

        public override string DescText => "<style=cIsDamage>Slayer</style>. Fire a revolver shot for <style=cIsDamage>" + d(Damage) + " damage</style>. Kills <style=cIsUtility>reset Lights Out's cooldown</style> and <style=cIsUtility>reduce other cooldowns</style> by <style=cIsUtility>" + d(CooldownReduction) + "</style>.";

        public override void Init()
        {
            Cooldown = ConfigOption(6f, "Cooldown", "Vanilla is 4");
            Damage = ConfigOption(8f, "Damage", "Decimal. Vanilla is 6");
            CooldownReduction = ConfigOption(0.12f, "Non-Special Cooldown Reduction on Kill", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.OnEnter += BaseFireSidearmRevolverState_OnEnter;
            On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.ModifyBullet += FireSidearmResetRevolver_ModifyBullet;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            Changes();
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var di = damageReport.damageInfo;

            if (DamageAPI.HasModdedDamageType(di, cooldownReset))
            {
                Debug.LogError("HAS MODDED DAMAGE TYPE");
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/Bandit2ResetEffect"), new EffectData
                {
                    origin = di.position
                }, true);

                var sl = damageReport.attacker.GetComponent<SkillLocator>();
                if (sl)
                {
                    Debug.LogError("SKILL LOCATOR FOUND");
                    var primary = sl.primary;
                    var secondary = sl.secondary;
                    var utility = sl.utility;
                    var special = sl.special;
                    if (primary && primary.stock < primary.maxStock)
                    {
                        Debug.LogError("HAS PRIMARY AND PRIMARY STOCK IS BELOW MAX STOCK");
                        primary.rechargeStopwatch += primary.finalRechargeInterval * CooldownReduction;
                    }
                    if (secondary && secondary.stock < secondary.maxStock)
                    {
                        Debug.LogError("HAS SECONDARY AND UTILITY STOCK IS BELOW MAX STOCK");
                        secondary.rechargeStopwatch += secondary.finalRechargeInterval * CooldownReduction;
                    }
                    if (utility && utility.stock < utility.maxStock)
                    {
                        Debug.LogError("HAS UTILITY AND UTILITY STOCK IS BELOW MAX STOCK");
                        utility.rechargeStopwatch += utility.finalRechargeInterval * CooldownReduction;
                    }
                    if (special && special.stock < special.maxStock)
                    {
                        Debug.LogError("HAS SPECIAL AND SPECIAL STOCK IS BELOW MAX STOCK");
                        special.rechargeStopwatch += special.finalRechargeInterval * 1f;
                    }
                }
            }
            orig(self, damageReport);
        }

        private void FireSidearmResetRevolver_ModifyBullet(On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.orig_ModifyBullet orig, EntityStates.Bandit2.Weapon.FireSidearmResetRevolver self, RoR2.BulletAttack bulletAttack)
        {
            orig(self, bulletAttack);
            DamageAPI.AddModdedDamageType(bulletAttack, cooldownReset);
            bulletAttack.damageType &= ~DamageType.ResetCooldownsOnKill;
        }

        private void BaseFireSidearmRevolverState_OnEnter(On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.orig_OnEnter orig, EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState self)
        {
            if (self is EntityStates.Bandit2.Weapon.FireSidearmResetRevolver)
            {
                self.damageCoefficient = Damage;
            }
            orig(self);
        }

        private void Changes()
        {
            var reset = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/ResetRevolver.asset").WaitForCompletion();
            reset.baseRechargeInterval = Cooldown;
        }
    }
}