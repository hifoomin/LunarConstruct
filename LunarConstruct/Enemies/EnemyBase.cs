using EntityStates;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using R2API;
using RoR2.Skills;
using static R2API.DirectorAPI;

namespace LunarConstruct.Enemies
{
    public abstract class EnemyBase<T> : EnemyBase where T : EnemyBase<T>
    {
        public static T Instance { get; private set; }

        public EnemyBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting EnemyBase was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class EnemyBase
    {
        public DirectorCard card;
        public DirectorAPI.DirectorCardHolder cardHolder;
        public CharacterSpawnCard isc;
        public virtual string PathToClone { get; } = null;
        public virtual string CloneName { get; } = null;
        public virtual string PathToCloneMaster { get; } = null;
        public virtual bool local { get; } = false;
        public virtual bool localMaster { get; } = false;
        public GameObject prefab;
        public GameObject prefabMaster;
        public virtual ExpansionDef RequiredExpansionHolder { get; } = Main.SOTVExpansionDef;

        public virtual void Create()
        {
            if (PathToClone != null && CloneName != null && PathToCloneMaster != null)
            {
                CreatePrefab();
                var req = prefab.AddComponent<ExpansionRequirementComponent>();
                var req2 = prefabMaster.AddComponent<ExpansionRequirementComponent>();
                req.requiredExpansion = RequiredExpansionHolder;
                req2.requiredExpansion = RequiredExpansionHolder;
            }
            Modify();
            AddSpawnCard();
            AddDirectorCard();
            PostCreation();
        }

        public virtual void Modify()
        {
        }

        public virtual void PostCreation()
        {
        }

        public virtual void AddSpawnCard()
        {
            isc = ScriptableObject.CreateInstance<CharacterSpawnCard>();
        }

        public virtual void AddDirectorCard()
        {
            card = new DirectorCard();
            card.spawnCard = isc;

            cardHolder = new DirectorAPI.DirectorCardHolder
            {
                Card = card,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
                MonsterCategorySelectionWeight = 2f
            };
        }

        public void RegisterEnemy(GameObject bodyPrefab, GameObject masterPrefab, List<DirectorCardCategorySelection> stages)
        {
            // bodyPrefab.GetComponent<CharacterBody>()._masterObject = masterPrefab;
            PrefabAPI.RegisterNetworkPrefab(bodyPrefab);
            PrefabAPI.RegisterNetworkPrefab(masterPrefab);
            ContentAddition.AddBody(bodyPrefab);
            ContentAddition.AddMaster(masterPrefab);
            foreach (DirectorCardCategorySelection stage in stages)
            {
                DirectorAPI.AddCard(stage, cardHolder);
            }
        }

        public void DestroyModelLeftovers(GameObject prefab)
        {
            GameObject.Destroy(prefab.GetComponentInChildren<ModelLocator>().modelBaseTransform.gameObject);
        }

        public virtual void CreatePrefab()
        {
            if (!local)
            {
                prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(PathToClone).WaitForCompletion(), CloneName + "Body");
            }
            else
            {
            }

            if (!localMaster)
            {
                prefabMaster = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(PathToCloneMaster).WaitForCompletion(), CloneName + "Master");
            }
            else
            {
            }

            // prefab.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        }

        /// <summary>
        /// A method to destroy the previous skill family of a slot and replace it with a new one
        /// </summary>
        public void ReplaceSkill(GenericSkill slot, SkillDef replaceWith, string familyName = "temp")
        {
            SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
            ((ScriptableObject)family).name = familyName;
            // family.variants = new SkillFamily.Variant[1];
            slot._skillFamily = family;
            slot._skillFamily.variants = new SkillFamily.Variant[1];

            slot._skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = replaceWith
            };
        }
    }
}