using MonoMod.Cil;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HBT.Skills
{
    public class Desperado : TweakBase
    {
        public static float Damage;
        public static float StackDamage;
        public static float Cooldown;

        public override string Name => ": Special :: Desperado";

        public override string SkillToken => "special_alt";

        public override string DescText => "<style=cIsDamage>Slayer</style>. Fire a revolver shot for <style=cIsDamage>" + d(Damage) + " damage</style>. Kills grant <style=cIsDamage>stacking tokens</style> for <style=cIsDamage>" + d(StackDamage) + "</style> more Desperado damage.";

        public override void Init()
        {
            Cooldown = ConfigOption(6f, "Cooldown", "Vanilla is 4");
            Damage = ConfigOption(8f, "Damage", "Decimal. Vanilla is 6");
            StackDamage = ConfigOption(0.04f, "Desperado Total Damage Percent per Token", "Vanilla is 0.1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.OnEnter += BaseFireSidearmRevolverState_OnEnter;
            IL.EntityStates.Bandit2.Weapon.FireSidearmSkullRevolver.ModifyBullet += FireSidearmSkullRevolver_ModifyBullet;
            Changes();
        }

        private void FireSidearmSkullRevolver_ModifyBullet(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = StackDamage;
            }
            else
            {
                Main.HBTLogger.LogError("Failed to apply Desperado Stacking Damage hook");
            }
        }

        private void BaseFireSidearmRevolverState_OnEnter(On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.orig_OnEnter orig, EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState self)
        {
            if (self is EntityStates.Bandit2.Weapon.FireSidearmSkullRevolver)
            {
                self.damageCoefficient = Damage;
            }
            orig(self);
        }

        private void Changes()
        {
            var skullemoji = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SkullRevolver.asset").WaitForCompletion();
            skullemoji.baseRechargeInterval = Cooldown;
        }
    }
}