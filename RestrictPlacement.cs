using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Restrict Placement", "Goomig", "1.0.4")]
    [Description("Restrict or allow placing items")]
    public class RestrictPlacement : CovalencePlugin
    {
        #region // Fields/Variables \\
        private Configuration _config;
		private const string permissionBypass = "restrictplacement.bypass";
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
        #endregion

        #region // Hooks \\
        private void Init() 
        {
            permission.RegisterPermission(permissionBypass, this);
        }

        private object CanBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            return CheckBuild(planner, prefab, target);
        }
    
        private object CheckBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            var player = planner.GetOwnerPlayer();
            if (player == null) return null;

            if (_config.blacklist.Contains(prefab.fullName) && !permission.UserHasPermission(player.UserIDString, permissionBypass))
            {
				var msg = string.Format(lang.GetMessage("Restricted", this, player.UserIDString));
				player.ChatMessage(msg);
                return false;
            }
            return null;
        }
        #endregion
    }
};