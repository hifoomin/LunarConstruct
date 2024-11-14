using EntityStates;
using LunarConstruct.States;
using RoR2.Skills;

namespace LunarConstruct.SkillDefs
{
    public class FireShards : SkillDefBase<FireShards>
    {
        public override string NameToken => "LUNARCONSTRUCT_FIRESHARDS_NAME";

        public override string DescToken => "LUNARCONSTRUCT_FIRESHARDS_DESC";

        public override SerializableEntityStateType ActivationState => new(typeof(ChargeShardsState));

        public override string ActivationStateMachineName => "Weapon";

        public override int BaseMaxStock => 5;

        public override float BaseRechargeInterval => 0.25f;

        public override bool BeginSkillCooldownOnSkillEnd => true;

        public override bool CanceledFromSprinting => false;

        public override bool CancelSprintingOnActivation => true;

        public override bool FullRestockOnAssign => true;

        public override InterruptPriority SkillInterruptPriority => InterruptPriority.Any;

        public override bool IsCombatSkill => true;

        public override bool MustKeyPress => false;

        public override int RechargeStock => 1;

        public override Sprite Icon => null;

        public override int StockToConsume => 1;
        public override int RequiredStock => 5;
    }
}