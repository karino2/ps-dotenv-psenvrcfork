using DotNet.Globbing;
using Dotenv.OSSpecific;
using System.Text;
using System.Security.Cryptography;

namespace Dotenv;
using FileHashPair = KeyValuePair<string, string>;

internal class Whitelist {

	public static string GetConfigDir() {
		string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		string configPath = Path.Combine(homeDir, ".config", "psdotenvrc");
		Directory.CreateDirectory(configPath);
		return configPath;
	}

	public static List<FileHashPair> ListAllows() {
		return Directory.GetFiles(GetConfigDir())
			.Where(path => Path.GetFileName(path).Length == 64)
			.Select(path => new FileHashPair(File.ReadAllText(path), Path.GetFileName(path)))
			.ToList();
	}

	public static Whitelist FromConfigDir() {
		return new Whitelist(ListAllows());
	}

	// key is path, value is hash value.
	private Dictionary<string, string> _allows = new Dictionary<string, string>();

	public Whitelist(List<FileHashPair> files) {
		foreach(var fpair in files) {
			_allows.Add(fpair.Key, fpair.Value);
		}
	}

	string Path2Hash(string fullPath) {
		string content = File.ReadAllText(fullPath);
		string combined = $"{fullPath}\n{content}";

		using (var sha256 = SHA256.Create()) {
			byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
		}
	}

	void SaveEntry(string fullPath) {
		string hash = Path2Hash(fullPath);
		string dest = Path.Combine(GetConfigDir(), hash);
		File.WriteAllText(dest, fullPath);
		_allows[fullPath] = hash;
	}

	public bool IsMatch(string path) {
		string? hash;
		if (!_allows.TryGetValue(path, out hash)) return false;

		return Path2Hash(path) == hash;
	}
	public ICollection<string> WhitePaths => this._allows.Keys;


	public bool Add(string fullPath) {
		try {
			SaveEntry(fullPath);
			return true;
		} catch (Exception) {
			return false;
		}
	}

	public bool Remove(string fullPath) {
		try {
			var paths = ListAllows()
			.Where(fpair => fpair.Key == fullPath)
			.Select(fpair => Path.Combine(GetConfigDir(), fpair.Value));


			foreach(var hashPath in paths) {
				File.Delete(hashPath);
			}
			_allows.Remove(fullPath);
			return true;

		} catch (Exception) {
			return false;
		}
	}

	internal void RemoveAll() {
		foreach(var fpair in ListAllows()) {
			File.Delete(Path.Combine(GetConfigDir(), fpair.Value));
		}
		_allows.Clear();
	}
}
