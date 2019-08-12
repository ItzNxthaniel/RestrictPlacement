using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Restrict Placement", "Goomig", "1.0.1")]
    [Description("A simple Rust plugin that allows you to restrict/allow players from placing items.")]
    public class RestrictPlacement : CovalencePlugin
    {
        #region // Fields/Variables \\
        private Configuration _config;
        #endregion

        #region // Config \\
        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<Configuration>();
            SaveConfig();
        }

        protected override void SaveConfig() => Config.WriteObject(_config);

        protected override void LoadDefaultConfig()
        {
            _config = new Configuration
            {
                blacklist = new HashSet<string>
                {
                    "fullname.prefab",
                }
            };

            SaveConfig();
        }

        private class Configuration
        {
            [JsonProperty("Blacklist")]
            public HashSet<string> blacklist;
        }
        #endregion

        #region // Language \\
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["Restricted"] = "<color=#ff7675>Placement Restriction</color>: You are not allowed to place this item."
            }, this);
        }

        private void Message(BasePlayer player, string key, params object[] args)
        {
            var message = string.Format(lang.GetMessage(key, this, player.UserIDString), args);
            player.ChatMessage(message);
        }
        #endregion

        #region // Hooks \\
        private void Init() 
        {
            permission.RegisterPermission("restrictplacement.bypass", this);
        }

        private object CanBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            return CheckBuild(planner, prefab, target);
        }
    
        private object CheckBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            var player = planner.GetOwnerPlayer();
            if (player == null) return null;

            var name = prefab.fullName;
            if (_config.blacklist.Contains(name) && !permission.UserHasPermission(player.UserIDString, "restrictplacement.bypass"))
            {
                Message(player, "Restricted");
                return false;
            }
            return null;
        }
        #endregion
    }
};