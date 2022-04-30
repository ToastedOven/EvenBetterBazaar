using BepInEx;
using BepInEx.Configuration;
using EmotesAPI;
using R2API;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency("SoundAPI", "PrefabAPI", "CommandHelper", "ResourcesAPI")]
    public class FunlandPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.weliveinasociety.evenbetterbazaar";
        public const string PluginAuthor = "Nunchuk";
        public const string PluginName = "EvenBetterBazaar";
        public const string PluginVersion = "1.0.0";
        int funLandInt = -1;
        GameObject funland;
        Shader defaultShader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
        public void Awake()
        {
            Assets.PopulateAssets();
            //Assets.AddSoundBank("caramelldeeznuts.bnk");
            Assets.LoadSoundBanks();
            CustomEmotesAPI.AddNonAnimatingEmote("SpawnFunLand");
            funLandInt = CustomEmotesAPI.RegisterWorldProp(Assets.Load<GameObject>($"assets/funlandtest2.prefab"), new JoinSpot[] { new JoinSpot("LeftSpot", new Vector3(12.539f, -2.088f, 2.62f), new Vector3(90, 0, 0), Vector3.one), new JoinSpot("RightSpot", new Vector3(12.539f, -5.235f, 2.62f), new Vector3(90, 0, 0), Vector3.one) });
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            CustomEmotesAPI.emoteSpotJoined_Prop += CustomEmotesAPI_emoteSpotJoined_Prop;
        }

        private void CustomEmotesAPI_emoteSpotJoined_Prop(string emoteSpotName, BoneMapper joiner, BoneMapper host)
        {
            if (emoteSpotName == "LeftSpot")
            {
                //joiner.PlayAnim("MinecartSit", -1);
                int spot = joiner.props.Count;
                joiner.props.Add(GameObject.Instantiate(Assets.Load<GameObject>($"assets/funlandminecart.prefab")));
                joiner.props[spot].transform.parent = host.transform;
                joiner.props[spot].transform.localPosition = Vector3.zero;
                joiner.props[spot].GetComponentInChildren<SkinnedMeshRenderer>().material.shader = defaultShader;
                GameObject g = new GameObject();
                g.transform.parent = joiner.props[spot].transform.GetChild(0).GetChild(0);
                g.transform.localPosition = Vector3.zero;
                g.transform.localEulerAngles = new Vector3(0, 180, 0);
                joiner.AssignParentGameObject(g, true, true, false);
            }
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (newAnimation == "SpawnFunLand")
            {
                if (funland)
                {
                    GameObject.Destroy(funland);
                }
                funland = CustomEmotesAPI.SpawnWorldProp(funLandInt);
                funland.transform.position = mapper.transform.parent.position + new Vector3(33, 0, 0);
                funland.GetComponent<MeshRenderer>().material.shader = defaultShader;
            }
        }
    }
}
