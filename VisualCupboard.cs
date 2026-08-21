using System;
using Oxide.Core;
using UnityEngine;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("Visual Cupboard", "Orffyrus", "1.0.16")]
    [Description("Shows a visual claim sphere on each connected building block instead of one circle around the tool cupboard")]
    class VisualCupboard : RustPlugin
    {

        #region Loadup

        private void OnServerInitialized() { serverInitialized = true; }

        private void Loaded()
        {
            LoadVariables();
            serverInitialized = true;
            lang.RegisterMessages(messages, this);
            permission.RegisterPermission("visualcupboard.allowed", this);
            permission.RegisterPermission("visualcupboard.admin", this);
        }

        private void LoadDefaultConfig()
        {
            Puts("Creating a new config file");
            Config.Clear();
            LoadVariables();
        }

        private Dictionary<string, string> messages = new Dictionary<string, string>()
            {
            {"notallowed", "You are not allowed to access that command." }
            };

        #endregion

        #region Configuration

        private bool Changed;
        private static float UseCupboardRadius = 34f;
        private float DurationToShowRadius = 60f;
        private float ShowCupboardsWithinRangeOf = 50f;
        private int VisualDarkness = 1;

        private static bool serverInitialized = false;

        private void LoadConfigVariables()
        {
            CheckCfgFloat("My Privilege Radius per Block is (16 is default)", ref UseCupboardRadius);
            CheckCfgFloat("Show Visuals On Cupboards Withing Range Of", ref ShowCupboardsWithinRangeOf);
            CheckCfgFloat("Show Visuals For This Long", ref DurationToShowRadius);
            CheckCfg("How Dark to make Visual Cupboard", ref VisualDarkness);
        }

        private void LoadVariables()
        {
            LoadConfigVariables();
            SaveConfig();
        }

        private void CheckCfg<T>(string Key, ref T var)
        {
            if (Config[Key] is T)
                var = (T)Config[Key];
            else
                Config[Key] = var;
        }

        private void CheckCfgFloat(string Key, ref float var)
        {

            if (Config[Key] != null)
                var = Convert.ToSingle(Config[Key]);
            else
                Config[Key] = var;
        }

        private object GetConfig(string menu, string datavalue, object defaultValue)
        {
            var data = Config[menu] as Dictionary<string, object>;
            if (data == null)
            {
                data = new Dictionary<string, object>();
                Config[menu] = data;
                Changed = true;
            }

            object value;
            if (!data.TryGetValue(datavalue, out value))
            {
                value = defaultValue;
                data[datavalue] = value;
                Changed = true;
            }
            return value;
        }

        #endregion

        #region Sphere Entity

        private class ToolCupboardSphere : MonoBehaviour
        {
            private BaseEntity sphere;
            private BaseEntity entity;
            public bool showall;
            private Vector3 pos = new Vector3(0, 0, 0);
            private Quaternion rot = new Quaternion();
            private string strPrefab = "assets/prefabs/visualization/sphere.prefab";

            private void Awake()
            {
                SpawnSphere();
            }

            private void SpawnSphere()
            {
                entity = GetComponent<BaseEntity>();
                sphere = GameManager.server.CreateEntity(strPrefab, pos, rot, true);
                SphereEntity ball = sphere.GetComponent<SphereEntity>();
                ball.OwnerID = entity.OwnerID;
                ball.currentRadius = 1f;
                ball.lerpRadius = UseCupboardRadius;
                ball.lerpSpeed = 100f;
                showall = false;
                sphere.SetParent(entity);
                sphere.Spawn();
            }

            private void OnDestroy()
            {
                if (sphere == null) return;
                sphere.Kill(BaseNetworkable.DestroyMode.None);
            }

        }

        #endregion

        #region Hooks

        private object CanNetworkTo(SphereEntity sphereEntity, BasePlayer target)
        {
            var sphereobj = sphereEntity.GetComponentInParent<ToolCupboardSphere>();
            if (sphereobj == null) return null;
            if (sphereobj != null && sphereobj.showall == false)

            {
                if (target.userID != sphereEntity.OwnerID) return false;
            }
            return null;
        }

        #endregion

        #region Commands

        [ChatCommand("showsphere")]
        private void cmdChatShowSphere(BasePlayer player, string command)
        {
            AddSphere(player, false, false);
        }

        [ChatCommand("showsphereall")]
        private void cmdChatShowSphereAll(BasePlayer player, string command)
        {
            AddSphere(player, true, false);
        }

        [ChatCommand("showsphereadmin")]
        private void cmdChatShowSphereAdmin(BasePlayer player, string command)
        {
            if (isAllowed(player, "visualcupboard.admin"))
            {
                AddSphere(player, true, true);
                return;
            }
            else if (!isAllowed(player, "visualcupboard.admin"))
            {
                SendReply(player, lang.GetMessage("notallowed", this));
                return;
            }
        }

        [ChatCommand("killsphere")]
        private void cmdChatDestroySphere(BasePlayer player, string command)
        {
            if (isAllowed(player, "visualcupboard.admin"))
            {
                DestroyAll<ToolCupboardSphere>();
                return;
            }
            else if (!isAllowed(player, "visualcupboard.admin"))
            {
                SendReply(player, lang.GetMessage("notallowed", this));
                return;
            }
        }

        #endregion

        #region Helpers

        private bool IsRedundant(Vector3 pos, List<Vector3> positions, float r)
        {
            const int thetaStep = 30;
            const int phiStep = 30;
            bool allCovered = true;

            for (int theta = 0; theta < 360; theta += thetaStep)
            {
                for (int phi = -90; phi <= 90; phi += phiStep)
                {
                    float phiRad = phi * Mathf.Deg2Rad;
                    float thetaRad = theta * Mathf.Deg2Rad;
                    Vector3 dir = new Vector3(
                        Mathf.Cos(phiRad) * Mathf.Cos(thetaRad),
                        Mathf.Sin(phiRad),
                        Mathf.Cos(phiRad) * Mathf.Sin(thetaRad)
                    );
                    Vector3 p = pos + dir * r;

                    bool covered = false;
                    foreach (var other in positions)
                    {
                        if (Vector3.Distance(pos, other) < 0.01f) continue;
                        if (Vector3.Distance(p, other) <= r)
                        {
                            covered = true;
                            break;
                        }
                    }

                    if (!covered)
                    {
                        allCovered = false;
                        goto EndCheck;
                    }
                }
            }
        EndCheck:
            return allCovered;
        }

        private void AddSphere(BasePlayer player, bool showall, bool adminshow)
        {
            if (isAllowed(player, "visualcupboard.allowed") || isAllowed(player, "visualcupboard.admin"))
            {
                List<BaseCombatEntity> cblist = new List<BaseCombatEntity>();
                Vis.Entities<BaseCombatEntity>(player.transform.position, ShowCupboardsWithinRangeOf, cblist);

                foreach (BaseCombatEntity bp in cblist)
                {
                    if (bp is BuildingPrivlidge)
                    {
                        BuildingPrivlidge priv = bp as BuildingPrivlidge;
                        var building = BuildingManager.server.GetBuilding(priv.buildingID);
                        if (building == null) continue;

                        bool isOwner = !adminshow && player.userID == bp.OwnerID;
                        bool shouldShow = adminshow || isOwner;

                        if (shouldShow)
                        {
                            var entities = building.decayEntities;
                            List<Vector3> positions = entities.Select(e => e.transform.position).ToList();

                            foreach (var entity in entities)
                            {
                                Vector3 pos = entity.transform.position;
                                bool redundant = IsRedundant(pos, positions, UseCupboardRadius);
                                if (redundant) continue;

                                if (entity.GetComponent<ToolCupboardSphere>() == null)
                                {
                                    for (int i = 0; i < VisualDarkness; i++)
                                    {
                                        var sphereobj = entity.gameObject.AddComponent<ToolCupboardSphere>();
                                        if (showall) sphereobj.showall = true;
                                        GameManager.Destroy(sphereobj, DurationToShowRadius);
                                    }
                                }
                            }
                        }

                        if (adminshow)
                        {
                            Vector3 pos = bp.transform.position;
                            player.SendConsoleCommand("ddraw.text", 10f, Color.red, pos + Vector3.up, FindPlayerName(bp.OwnerID));
                            PrintWarning("Tool Cupboard Owner " + bp.OwnerID + " : " + FindPlayerName(bp.OwnerID));
                        }
                    }
                }
                return;
            }
            SendReply(player, lang.GetMessage("notallowed", this));
            return;
        }

        private string FindPlayerName(ulong userId)
        {
            BasePlayer player = BasePlayer.FindByID(userId);
            if (player)
                return player.displayName;

            player = BasePlayer.FindSleeping(userId);
            if (player)
                return player.displayName;

            var iplayer = covalence.Players.FindPlayer(userId.ToString());
            if (iplayer != null)
                return iplayer.Name;

            return "Unknown Entity Owner";
        }

        private void Unload()
        {
            DestroyAll<ToolCupboardSphere>();
        }

        static void DestroyAll<T>()
        {
            var objects = GameObject.FindObjectsOfType(typeof(T));
            if (objects != null)
                foreach (var gameObj in objects)
                    GameObject.Destroy(gameObj);
        }

        private bool isAllowed(BasePlayer player, string perm) => permission.UserHasPermission(player.UserIDString, perm);

        #endregion
    }
}