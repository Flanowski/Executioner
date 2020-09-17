using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Executioner
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    //Change these
    [BepInPlugin("Executioner", "WubWub", "1.0.0")]
    [R2APISubmoduleDependency(nameof(LoadoutAPI), nameof(SurvivorAPI), nameof(SoundAPI), nameof(LanguageAPI))]

    public class ExecutionerChar : BaseUnityPlugin
    {
        public void Awake()
        {
            var myCharacter = Resources.Load<GameObject>("prefabs/characterbodies/CommandoBody");

            LanguageAPI.Add("EXECUTIONER_DESCRIPTION" 
            ,@"The Executioner is a high risk high reward survivor that's all about racking up an endless kill count.

< ! > Use Service Pistol to score some kills, and use those to charge up Ion Burst for massive damage.

< ! > Saving up Ion Burst charges is a risky move, but can pay off if you can get a bunch of shots off on a boss.

< ! > If you find yourself getting swarmed, Crowd Dispersion can get enemies off your back fast.

< ! > Execution is a great crowd control AND single target tool, don't forget its damage depends on how many targets it hits!");

            var mySurvivorDef = new SurvivorDef
            {
                bodyPrefab = myCharacter,
                descriptionToken = "EXECUTIONER_DESCRIPTION",
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/CommandoDisplay"),
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(mySurvivorDef);

            LanguageAPI.Add("EXECUTIONER_SKILLSLOT_SKILLNAME_NAME", "Service Pistol");
            LanguageAPI.Add("EXECUTIONER_SKILLSLOT_SKILLNAME_DESCRIPTION", "Fire your pistol for 90% damage.");

            var mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(ExampleState));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0.09f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = true;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = true;
            mySkillDef.isCombatSkill = false;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0.5f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("");
            mySkillDef.skillDescriptionToken = "EXECUTIONER_SKILLSLOT_SKILLNAME_DESCRIPTION";
            mySkillDef.skillName = "EXECUTIONER_SKILLSLOT_SKILLNAME_NAME";
            mySkillDef.skillNameToken = "EXECUTIONER_SKILLSLOT_SKILLNAME_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            var skillLocator = myCharacter.GetComponent<SkillLocator>();
            var skillFamily = skillLocator.primary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

            };
        }
    }
}