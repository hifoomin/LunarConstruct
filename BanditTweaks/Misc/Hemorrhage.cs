using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;

namespace HBT.Misc
{
    public class Hemorrhage : MiscBase
    {
        public static float Interval;
        public static float DamagePerTick;
        public static float TotalDuration;

        public override string Name => ": Secondary ::: Hemorrhage";

        public override void Init()
        {
            Interval = ConfigOption(0.25f, "Tick Interval", "Decimal. Vanilla is 0.25");
            DamagePerTick = ConfigOption(0.5f, "Tick Damage", "Vanilla is 0.333");
            TotalDuration = ConfigOption(4f, "Total Duration", "Vanilla is 15");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DotController.InitDotCatalog += DotController_InitDotCatalog1;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            Changes();
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(6),
                x => x.MatchLdcR4(15f)))
            {
                c.Index += 1;
                c.Next.Operand = TotalDuration;
            }
            else
            {
                Main.HBTLogger.LogError("Failed to apply Hemorrhage Total Duration hook");
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport dr)
        {
            var di = dr.damageInfo;

            if (di == null) return;

            var attacker = di.attacker;

            if (attacker == null) return;

            var body = attacker.GetComponent<CharacterBody>();
            if (body != null)
            {
                if (di.crit && di.procCoefficient == 0 && (di.damageType & DamageType.SuperBleedOnCrit) != DamageType.Generic)
                {
                    DotController.InflictDot(dr.victim.gameObject, di.attacker, DotController.DotIndex.SuperBleed, TotalDuration, 1f, uint.MaxValue);
                }
            }
        }

        private void DotController_InitDotCatalog1(On.RoR2.DotController.orig_InitDotCatalog orig)
        {
            orig();
            DotController.dotDefs[6].interval = Interval;
            DotController.dotDefs[6].damageCoefficient = DamagePerTick;
        }

        private void Changes()
        {
            LanguageAPI.Add("KEYWORD_SUPERBLEED", "<style=cKeywordName>Hemorrhage</style><style=cSub>Deal <style=cIsDamage>" + d(DamagePerTick / Interval * TotalDuration + (TotalDuration * DamagePerTick)) + "</style> base damage over " + TotalDuration + "s. <i>Hemorrhage can stack.</i></style>");
        }
    }
}