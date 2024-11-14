using EntityStates;

namespace LunarConstruct.States
{
    public class ChargeShardsState : BaseState
    {
        [SerializeField]
        public string enterSoundString = "Play_minorConstruct_attack_chargeUp";

        [SerializeField]
        public string exitSoundString = "Stop_minorConstruct_attack_chargeUp";

        [SerializeField]
        public string animationLayerName = "Weapon";

        [SerializeField]
        public string animationStateName = "ChargeConstructBeam";

        [SerializeField]
        public string animationPlaybackRateParam = "ConstructBeam.playbackRate";

        [SerializeField]
        public string chargeEffectMuzzle = "Muzzle";

        [SerializeField]
        public GameObject chargeEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/ChargeLunarGolemTwinShot.prefab").WaitForCompletion();

        [SerializeField]
        public float baseDuration = 0.2f;

        private float duration;

        private GameObject chargeInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation(animationLayerName, animationStateName, animationPlaybackRateParam, duration);
            Util.PlaySound(enterSoundString, gameObject);
            Transform transform = FindModelChild(chargeEffectMuzzle);
            if (transform && chargeEffectPrefab)
            {
                chargeInstance = Object.Instantiate(chargeEffectPrefab, transform.position, transform.rotation);
                chargeInstance.transform.parent = gameObject.transform;
                ScaleParticleSystemDuration component = chargeInstance.GetComponent<ScaleParticleSystemDuration>();
                if (component)
                {
                    component.newDuration = duration;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (chargeInstance)
            {
                Ray aimRay = GetAimRay();
                chargeInstance.transform.forward = aimRay.direction;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration)
            {
                outer.SetNextState(new FireShardsState());
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(exitSoundString, gameObject);
            if (chargeInstance)
            {
                Destroy(chargeInstance);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}