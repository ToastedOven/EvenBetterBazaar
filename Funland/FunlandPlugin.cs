using BepInEx;
using BepInEx.Configuration;
using EmotesAPI;
using Funland;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

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
        int busInt = -1;
        GameObject funland;
        GameObject cube;
        GameObject vengaBus;
        internal static VengaBus Venga;
        JoinSpot busDoor = new JoinSpot("VengaBusDoor", new Vector3(1.741f, 0, 3.534f));
        internal static Shader defaultShader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
        public void Awake()
        {
            Assets.PopulateAssets();
            Assets.LoadSoundBanks();
            CustomEmotesAPI.AddNonAnimatingEmote("SpawnFunLand");
            CustomEmotesAPI.BlackListEmote("SpawnFunLand");
            CustomEmotesAPI.AddNonAnimatingEmote("SpawnBus");
            CustomEmotesAPI.BlackListEmote("SpawnBus");
            funLandInt = CustomEmotesAPI.RegisterWorldProp(Assets.Load<GameObject>($"assets/funlandtest2.prefab"), new JoinSpot[] { new JoinSpot("LeftSpot", new Vector3(12.539f, -2.088f, 2.62f), new Vector3(90, 0, 0), Vector3.one), new JoinSpot("RightSpot", new Vector3(12.539f, -5.235f, 2.62f), new Vector3(90, 0, 0), Vector3.one) });
            GameObject g = Assets.Load<GameObject>($"Assets/bussy/vengabus14.prefab");
            busInt = CustomEmotesAPI.RegisterWorldProp(g, new JoinSpot[] { busDoor });
            g.AddComponent<VengaBus>();
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            CustomEmotesAPI.emoteSpotJoined_Prop += CustomEmotesAPI_emoteSpotJoined_Prop;
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>($"Assets/bussy/vengaBussySit.anim"), true, visible: false);
            NetworkingAPI.RegisterMessageType<SyncBusToServer>();
            NetworkingAPI.RegisterMessageType<SyncBusToClient>();
            g = Assets.Load<GameObject>($"assets/sign/newsign 1.prefab");
            foreach (var item in g.GetComponentsInChildren<Renderer>())
            {
                item.material.shader = defaultShader;
            }
            RopeController controller = g.transform.Find("RopeController").gameObject.AddComponent<RopeController>();
            controller.anchorPoint = g.transform.Find("Capsule").gameObject;
            controller.attachPoint = g.transform.Find("funlandsign").Find("Point1").gameObject;
            controller.ropeSegmentPrefab = Assets.Load<GameObject>($"assets/sign/ropesegment.prefab");

            controller = g.transform.Find("RopeController (1)").gameObject.AddComponent<RopeController>();
            controller.anchorPoint = g.transform.Find("Capsule (1)").gameObject;
            controller.attachPoint = g.transform.Find("funlandsign").Find("Point2").gameObject;
            controller.ropeSegmentPrefab = Assets.Load<GameObject>($"assets/sign/ropesegment.prefab");
            On.RoR2.SceneCatalog.OnActiveSceneChanged += (orig, self, newScene) =>
            {
                orig(self, newScene);
                if (newScene.name == "bazaar")
                {
                    GameObject g = Assets.Load<GameObject>($"assets/terrain/bazaarpath.prefab");
                    g.GetComponentInChildren<Renderer>().material.shader = defaultShader;
                    GameObject.Instantiate(g);
                    g = Assets.Load<GameObject>($"assets/terrain/bazaarwalls.prefab");
                    GameObject.Instantiate(g);
                    GameObject wall = GameObject.Find("CaveMeshMain");
                    Object.DestroyImmediate(wall.GetComponent<MeshCollider>());
                    wall.GetComponent<MeshFilter>().mesh = Assets.Load<Mesh>($"assets/terrain/cavemeshmain.mesh");
                    g = Assets.Load<GameObject>($"assets/sign/newsign 1.prefab");
                    GameObject.Instantiate(g);
                }
            };
        }

        private void CustomEmotesAPI_emoteSpotJoined_Prop(GameObject emoteSpot, BoneMapper joiner, BoneMapper host)
        {
            string emoteSpotName = emoteSpot.name;
            if (emoteSpotName == "LeftSpot")
            {
                int spot = joiner.props.Count;
                joiner.props.Add(GameObject.Instantiate(Assets.Load<GameObject>($"assets/funlandminecart.prefab")));
                joiner.props[spot].transform.parent = host.transform;
                joiner.props[spot].transform.localPosition = Vector3.zero;
                joiner.props[spot].GetComponentInChildren<SkinnedMeshRenderer>().material.shader = defaultShader;
                GameObject g = new GameObject();
                g.transform.parent = joiner.props[spot].transform.GetChild(0).GetChild(0);
                g.transform.localPosition = Vector3.zero;
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.localEulerAngles = new Vector3(0, 180, 0);
                joiner.AssignParentGameObject(g, true, true, true);
            }
            if (emoteSpotName == "RightSpot")
            {
                int spot = joiner.props.Count;
                joiner.props.Add(GameObject.Instantiate(Assets.Load<GameObject>($"assets/funlandminecart.prefab")));
                joiner.props[spot].transform.parent = host.transform;
                joiner.props[spot].transform.localPosition = Vector3.zero;
                joiner.props[spot].GetComponentInChildren<SkinnedMeshRenderer>().material.shader = defaultShader;
                joiner.props[spot].GetComponent<Animator>().Play("Minecart", -1, .992f);
                GameObject g = new GameObject();
                g.transform.parent = joiner.props[spot].transform.GetChild(0).GetChild(0);
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.localEulerAngles = new Vector3(0, 180, 0);
                joiner.AssignParentGameObject(g, true, true, true);
            }
            if (emoteSpotName.StartsWith("VengaBusDoor"))
            {
                joiner.PlayAnim("vengaBussySit", 0);
                Venga.JoinBus(joiner);
            }
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (newAnimation == "SpawnFunLand" && NetworkServer.active)
            {
                if (funland)
                {
                    GameObject.Destroy(funland);
                }
                funland = CustomEmotesAPI.SpawnWorldProp(funLandInt);
                funland.layer = 11;
                funland.transform.position = mapper.transform.parent.position + new Vector3(33, 0, 0);
                funland.GetComponent<MeshRenderer>().material.shader = defaultShader;
                NetworkServer.Spawn(funland);
            }
            if (newAnimation == "SpawnTestCube")
            {
                if (cube)
                {
                    GameObject.Destroy(cube);
                }
                cube = GameObject.Instantiate(Assets.Load<GameObject>($"Assets/cube.prefab"));
                cube.transform.position = mapper.transform.parent.position + new Vector3(0, 5, 0);
                mapper.AssignParentGameObject(cube, true, true, true);
            }
            if (newAnimation == "SpawnBus")
            {
                if (NetworkServer.active)
                {
                    if (vengaBus)
                    {
                        NetworkServer.Destroy(vengaBus);
                    }
                    vengaBus = CustomEmotesAPI.SpawnWorldProp(busInt);
                    vengaBus.transform.SetParent(mapper.transform.parent);
                    vengaBus.transform.localPosition = new Vector3(0, 0, 7);
                    vengaBus.transform.SetParent(null);
                    vengaBus.layer = 11;
                    NetworkServer.Spawn(vengaBus);
                }
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Keypad8))
            {
                funland.transform.localEulerAngles += new Vector3(0 + 10 * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                funland.transform.localEulerAngles -= new Vector3(0 + 10 * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.Keypad4))
            {
                funland.transform.localEulerAngles += new Vector3(0, 0, 0 + 10 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                funland.transform.localEulerAngles -= new Vector3(0, 0, 0 + 10 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad7))
            {
                funland.transform.localEulerAngles += new Vector3(0, 0 + 10 * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.Keypad9))
            {
                funland.transform.localEulerAngles -= new Vector3(0, 0 + 10 * Time.deltaTime, 0);
            }
        }
    }
}
