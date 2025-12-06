using System.Collections;
using System.Management.Automation;
using System.Text;

namespace Dotenv {

	using KVPair = KeyValuePair<string, string>;
	public class PsenvrcParser {

		/**
		 * Execute psscript and return diff of current Environment and Env: in PowerShell object.
		 * */
		public List<KVPair> Parse(String psscript) {
			using (PowerShell ps = PowerShell.Create()) {
				ps.AddScript(psscript);
				ps.Invoke();

				if (ps.HadErrors) {
					throw BuildException(ps);
				}

				var ret = new List<KVPair>();
				var newEnv = ListEnv(ps);
				HashSet<string> found = new HashSet<string>();
				IDictionary oldEnv = Environment.GetEnvironmentVariables();
				foreach (DictionaryEntry kv in oldEnv) {
					string key = kv.Key.ToString() ?? "";
					string value = kv.Value?.ToString() ?? "";
					found.Add(key);
					string? newVal = newEnv[key];
					if (newVal == value)
						continue;
					ret.Add(new KVPair(key, newVal));
				}
				return ret;

			}
		}

		private static Exception BuildException(PowerShell ps) {
			StringBuilder builder = new StringBuilder();
			foreach (var error in ps.Streams.Error) {
				builder.AppendLine(error.ToString());
			}
			return new Exception(builder.ToString());
		}

		private static Dictionary<string, string> ListEnv(PowerShell ps) {
			var results = ps.AddScript("Get-ChildItem Env:").Invoke();
			var ret = new Dictionary<string, string>();

			foreach (var entry in results) {
				var name = entry.Properties["Name"].Value.ToString();
				var value = entry.Properties["Value"].Value?.ToString() ?? "";
				ret[name] = value;
			}
			return ret;
		}
	}
}
