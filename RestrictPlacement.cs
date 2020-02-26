using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Oxide.Plugins {
	[Info("Restrict Placement", "ItzNathaniel", "1.0.5")]
	[Description("Restrict Users from placing certain items.")]
	public class RestrictPlacement : RustPlugin {
		#region // Fields/Variables \\
		private Configuration _config;
		private const string permissionBypass = "restrictplacement.bypass";
		#endregion

		#region // Config \\
		protected override void LoadConfig() {
			base.LoadConfig();
			_config = Config.ReadObject<Configuration>();
			SaveConfig();
		}

		protected override void SaveConfig() => Config.WriteObject(_config);

		protected override void LoadDefaultConfig() {
			_config = new Configuration {
				blacklist = new HashSet<string> {
					"fullName.deployed.prefab"
				}
			};

			SaveConfig();
		}

		private class Configuration {
			[JsonProperty("Blacklist")]
			public HashSet<string> blacklist;
		}
		#endregion

		#region // Language \\
		protected override void LoadDefaultMessages() {
			lang.RegisterMessages(new Dictionary<string, string> {
				["Restricted"] = "<color=#d63031>Restrict Placement</color><color=#ffeaa7>: You're not allowed to place this item.</color>"
			}, this);
		}
		#endregion

		#region // Hooks \\
		private void Init() {
			permission.RegisterPermission(permissionBypass, this);
		}

		private object CanBuild(Planner planner, Construction prefab, Construction.Target target) {
			return CheckBuild(planner, prefab, target);
		}

		private object CheckBuild(Planner planner, Construction prefab, Construction.Target target) {
			var player = planner.GetOwnerPlayer();
			if (player == null) return null;
			
			if (_config.blacklist.Contains(prefab.fullName) && !permission.UserHasPermission(player.UserIDString, permissionBypass)) {
				var msg = string.Format(lang.GetMessage("Restricted", this, player.UserIDString));
				player.ChatMessage(msg);
				return true;
			}
			return null;
		}
		#endregion
	}
}
