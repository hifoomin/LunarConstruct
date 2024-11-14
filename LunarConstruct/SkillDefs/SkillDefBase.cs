using EntityStates;
using R2API;
using RoR2.Skills;
using System;

namespace LunarConstruct.SkillDefs
{
    public abstract class SkillDefBase<T> : SkillDefBase where T : SkillDefBase<T>
    {
        public static T Instance { get; private set; }

        public SkillDefBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting SkillDefBase was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class SkillDefBase
    {
        public abstract string NameToken { get; }
        public abstract string DescToken { get; }

        public abstract SerializableEntityStateType ActivationState { get; }
        public abstract string ActivationStateMachineName { get; }
        public abstract int BaseMaxStock { get; }
        public abstract float BaseRechargeInterval { get; }
        public abstract bool BeginSkillCooldownOnSkillEnd { get; }
        public abstract bool CanceledFromSprinting { get; }
        public abstract bool CancelSprintingOnActivation { get; }
        public abstract bool FullRestockOnAssign { get; }
        public abstract InterruptPriority SkillInterruptPriority { get; }
        public abstract bool IsCombatSkill { get; }
        public abstract bool MustKeyPress { get; }
        public abstract int RechargeStock { get; }
        public abstract Sprite Icon { get; }
        public abstract int StockToConsume { get; }
        public abstract int RequiredStock { get; }

        public SkillDef SkillDef;

        public virtual void Create()
        {
            SkillDef = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef.activationState = ActivationState;
            SkillDef.activationStateMachineName = ActivationStateMachineName;
            SkillDef.baseMaxStock = BaseMaxStock;
            SkillDef.baseRechargeInterval = BaseRechargeInterval;
            SkillDef.beginSkillCooldownOnSkillEnd = BeginSkillCooldownOnSkillEnd;
            SkillDef.canceledFromSprinting = CanceledFromSprinting;
            SkillDef.cancelSprintingOnActivation = CancelSprintingOnActivation;
            SkillDef.fullRestockOnAssign = FullRestockOnAssign;
            SkillDef.interruptPriority = SkillInterruptPriority;
            SkillDef.isCombatSkill = IsCombatSkill;
            SkillDef.mustKeyPress = MustKeyPress;
            SkillDef.rechargeStock = RechargeStock;
            SkillDef.icon = Icon;
            SkillDef.skillNameToken = NameToken;
            SkillDef.skillDescriptionToken = DescToken;
            SkillDef.stockToConsume = StockToConsume;
            SkillDef.requiredStock = RequiredStock;

            ContentAddition.AddSkillDef(SkillDef);
        }
    }
}