using LunarConstruct.Projectiles;
using LunarConstruct.SkillDefs;
using R2API;
using RoR2.Navigation;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace LunarConstruct.Enemies
{
    public class LunarConstruct : EnemyBase<LunarConstruct>
    {
        public override string PathToClone => "RoR2/DLC1/MajorAndMinorConstruct/MinorConstructBody.prefab";
        public override string CloneName => "LunarConstruct";
        public override string PathToCloneMaster => "RoR2/DLC1/MajorAndMinorConstruct/MinorConstructMaster.prefab";
        public CharacterBody body;
        public CharacterMaster master;

        public delegate Vector3 orig_aimOrigin(InputBankTest self);

        public override void CreatePrefab()
        {
            base.CreatePrefab();
            body = prefab.GetComponent<CharacterBody>();
            body.baseArmor = 0f;
            body.baseDamage = 13f;
            body.levelDamage = 2.6f;
            body.baseMaxHealth = 260f;
            body.levelMaxHealth = 78f;
            body.autoCalculateLevelStats = true;
            body.baseNameToken = "LUNARCONSTRUCT_NAME";
            body.portraitIcon = Main.lunarConstruct.LoadAsset<Texture2D>("Assets/LunarConstruct/texPortrait.png");
        }

        public override void AddSpawnCard()
        {
            base.AddSpawnCard();
            isc.hullSize = HullClassification.Human;
            isc.nodeGraphType = MapNodeGroup.GraphType.Ground;
            isc.requiredFlags = NodeFlags.None;
            isc.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            isc.directorCreditCost = 120;
            isc.occupyPosition = false;
            isc.eliteRules = SpawnCard.EliteRules.Lunar;
            isc.sendOverNetwork = true;
            isc.prefab = prefabMaster;
            isc.name = "cscLunarConstruct";
        }

        public override void AddDirectorCard()
        {
            base.AddDirectorCard();
            card.minimumStageCompletions = 0;
            card.selectionWeight = 1;
            card.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;
        }

        public override void Modify()
        {
            base.Modify();
            master = prefabMaster.GetComponent<CharacterMaster>();

            SkillLocator sl = prefab.GetComponentInChildren<SkillLocator>();
            ReplaceSkill(sl.primary, FireShards.Instance.SkillDef);

            LanguageAPI.Add("LUNARCONSTRUCT_NAME", "Lunar Chimera" + (Main.useWolfoNameScheme.Value ? " (Construct)" : ""));
            LanguageAPI.Add("LUNARCONSTRUCT_LORE", "Design driven. Cheap- and replicable. A shell made of two pyramids- I love triangles, after all.\r\n\r\nA central energy core capable of delivering a powerful blast within close quarters.\r\n\r\nThe energy core is heavy- the construct cannot move. Inefficient- speed is war.\r\n\r\nI have implemented a technology similar to my old gates. They can close distance to threats in the blink of an eye.\r\n\r\nTo see my brother bastardize this design- and for what? To act as security guards to keep his carnival of vermin trapped?\r\n\r\nIronic, then, that those very designs will be their destruction.");
            LanguageAPI.Add("LUNARCONSTRUCT_SUBTITLE", "Horde of Many");

            AISkillDriver shooty = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "Shooty"
                                    select x).First();
            shooty.requiredSkill = FireShards.Instance.SkillDef;
            shooty.requireSkillReady = true;
            shooty.maxDistance = 150f;

            body.gameObject.AddComponent<TeleportController>();

            var moonRamp = Main.lunarConstruct.LoadAsset<Texture2D>("Assets/LunarConstruct/texRamp.png");

            var matLunarConstruct = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/MajorAndMinorConstruct/matMinorConstructAlly.mat").WaitForCompletion());
            matLunarConstruct.SetTexture("_FresnelRamp", moonRamp);
            matLunarConstruct.SetTexture("_FlowHeightRamp", moonRamp);
            matLunarConstruct.SetTexture("_MainTex", Main.lunarConstruct.LoadAsset<Texture2D>("Assets/LunarConstruct/texLunarConstruct.png"));
            matLunarConstruct.SetColor("_EmColor", new Color32(73, 107, 229, 255));

            var mdl = body.gameObject.transform.GetChild(0).GetChild(0);
            var model = mdl.GetComponent<CharacterModel>();
            var baseRendererInfos = model.baseRendererInfos;
            for (int i = 0; i < baseRendererInfos.Length; i++)
            {
                baseRendererInfos[i].defaultMaterial = matLunarConstruct;
            }

            body.gameObject.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);

            var childLocator = mdl.GetComponent<ChildLocator>();
            var revealed = childLocator.transformPairs[1].transform;
            var peripheryLight = revealed.GetChild(0).GetComponent<Light>();
            peripheryLight.color = new Color32(132, 193, 255, 255);
            var peripheryLight2 = revealed.GetChild(1).GetComponent<Light>();
            peripheryLight2.color = new Color32(94, 145, 255, 255);
            revealed.GetChild(2).gameObject.SetActive(false);

            var newRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampRJMushroom.png").WaitForCompletion();
            var newShield = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/MajorAndMinorConstruct/matMinorConstructShield.mat").WaitForCompletion());
            newShield.SetColor("_TintColor", new Color32(12, 31, 63, 255));
            newShield.SetTexture("_RemapTex", newRamp);

            var hidden = childLocator.transformPairs[2].transform;
            var visual = hidden.GetChild(0);
            var meshRenderer = visual.GetComponent<MeshRenderer>();
            meshRenderer.material = newShield;

            master.bodyPrefab = prefab;
        }

        public override void PostCreation()
        {
            base.PostCreation();
            List<DirectorCardCategorySelection> stages = new()
            {
                Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/moon/dccsMoonMonsters.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/moon/dccsMoonMonstersDLC1.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itmoon/dccsITMoonMonsters.asset").WaitForCompletion(),
            };
            RegisterEnemy(prefab, prefabMaster, stages);
        }
    }

    public class TeleportController : MonoBehaviour
    {
        public HealthComponent hc;
        public CharacterBody cb;
        public float stopwatch = 0f;
        public GameObject teleportEffect = TeleportEffect.prefab;

        public void Start()
        {
            hc = GetComponent<HealthComponent>();
            cb = hc.body;
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= GetDelay())
            {
                stopwatch = 0f;
                HandleTeleport(PickTeleportPosition());
            }
        }

        public float GetDelay()
        {
            return 6f;
        }

        public void HandleTeleport(Vector3 pos)
        {
            if (cb.isPlayerControlled)
            {
                return;
            }
            Vector3 current = transform.position;
            EffectManager.SpawnEffect(teleportEffect, new EffectData
            {
                scale = 0.6f,
                origin = current
            }, true);
            TeleportHelper.TeleportBody(cb, pos + new Vector3(0, 1, 0));
        }

        public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes)
        {
            NodeGraph.Node[] validNodes = nodes.Where(x => Vector3.Distance(x.position, transform.position) > min && Vector3.Distance(x.position, transform.position) < max).ToArray();
            if (validNodes.Length <= 1)
            {
                return new Vector3[] { transform.position };
            }
            return validNodes.Select(node => node.position).ToArray();
        }

        public Vector3 PickTeleportPosition()
        {
            if (!SceneInfo.instance || !SceneInfo.instance.groundNodes)
            {
                return transform.position;
            }

            NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
            Vector3[] validPositions;
            validPositions = PickValidPositions(0, 35, nodes);
            return validPositions.GetRandom();
        }
    }
}