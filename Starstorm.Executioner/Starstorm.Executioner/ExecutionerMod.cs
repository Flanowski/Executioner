using System;
using System.Collections.Generic;
using BepInEx;
using EntityStates;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Executioner
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    //Change these
    [BepInPlugin("Executioner", "WubWub", "1.0.0")]
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(SurvivorAPI), nameof(SoundAPI), nameof(LanguageAPI))]

    public class ExecutionerMod : BaseUnityPlugin
    {
        public static GameObject characterPrefab;
        public GameObject myCharacterDisplay;

        public void Awake()
        {
            InitPrefab();
            RegisterCharacter();            
            SkillSetup();
        }

        void InitPrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "ExecutionerBody");
        }

        void RegisterCharacter()
        {
            myCharacterDisplay = PrefabAPI.InstantiateClone(characterPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "ExecutionerDisplay", true);
            myCharacterDisplay.AddComponent<NetworkIdentity>();

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(characterPrefab);
            };

            CharacterBody component = characterPrefab.GetComponent<CharacterBody>();
            component.baseDamage = 10f;
            component.baseCrit = 1f;
            component.levelCrit = 0f;
            component.baseMaxHealth = 400f;
            component.levelMaxHealth = 40f;
            component.baseArmor = 20f;
            component.baseRegen = 2f;
            component.levelRegen = 0.2f;
            component.baseMoveSpeed = 8f;
            component.levelMoveSpeed = 0.25f;
            component.baseAttackSpeed = 1f;
            component.name = "Executioner";

            characterPrefab.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;
            LanguageAPI.Add("EXECUTIONER_DESCRIPTION"
            , @"The Executioner is a high risk high reward survivor that's all about racking up an endless kill count.

< ! > Use Service Pistol to score some kills, and use those to charge up Ion Burst for massive damage.

< ! > Saving up Ion Burst charges is a risky move, but can pay off if you can get a bunch of shots off on a boss.

< ! > If you find yourself getting swarmed, Crowd Dispersion can get enemies off your back fast.

< ! > Execution is a great crowd control AND single target tool, don't forget its damage depends on how many targets it hits!");

            var mySurvivorDef = new SurvivorDef
            {
                bodyPrefab = characterPrefab,
                descriptionToken = "EXECUTIONER_DESCRIPTION",
                displayPrefab = myCharacterDisplay,
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                name = "Executioner",
                unlockableName = "",
            };

            SurvivorAPI.AddSurvivor(mySurvivorDef);
        }

        void SkillSetup()
        {
            // get rid of the original skills first, otherwise we'll have commando's loadout and we don't want that
            foreach (GenericSkill obj in characterPrefab.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(obj);
            }
            
            PrimarySetup();
        }

        private void PrimarySetup()
        {
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("EXECUTIONER_SKILLSLOT_SKILLNAME_NAME", "Service Pistol");
            LanguageAPI.Add("EXECUTIONER_SKILLSLOT_SKILLNAME_DESCRIPTION", "Fire your pistol for 90% damage.");

            var mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(ExampleState));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("");
            mySkillDef.skillDescriptionToken = "EXECUTIONER_SKILLSLOT_SKILLNAME_DESCRIPTION";
            mySkillDef.skillName = "EXECUTIONER_SKILLSLOT_SKILLNAME_NAME";
            mySkillDef.skillNameToken = "EXECUTIONER_SKILLSLOT_SKILLNAME_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);
           
            component.primary = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.primary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.primary.skillFamily;


            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }
    }
}