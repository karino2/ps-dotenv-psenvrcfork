using Dotenv.MemoryBuffer;
using Dotenv.Logging;
using Dotenv.OSSpecific;

namespace Dotenv;

public class Daemon {
	public Daemon(bool enabled = false, bool safe = true, bool quiet = false) {
		this.Quiet = quiet;
		this._enabled = enabled;
		this._safe = safe;
		this._warned = new HashSet<string>(Platform.StrComparer);
		this.auth = Whitelist.FromConfigDir();
	}

	private bool _enabled = true;
	private string _name = ".psenvrc";
	private List<DotenvFile> _sourced = new List<DotenvFile>(32) { };
	public bool SkipErrors = false;
	private Logger log = new Logger();
	private string lastdir = "";
	public LoggingPreference LoggingPreference {
		get => this.log.Preference;
		set => this.log.Preference = value;
	}
	private Whitelist auth;
	public ICollection<string> WhitePaths => this.auth.WhitePaths;
	private bool _safe = true;
	public bool SafeMode {
		get => this._safe;
		set {
			if (value != this._safe) {
				this._safe = value;
				this.Update(this.lastdir);
			}
		}
	}
	public bool Quiet = false;
	private HashSet<string> _warned;


	public IList<DotenvFile> Sourced => this._sourced.AsReadOnly();
	public MemBuf<LogEntry> Logs => this.log.Logs;
	public bool Enabled {
		get => this._enabled;
		set {
			if (value) this.Enable();
			else this.Disable();
		}
	}

	private bool pathIsSourced(string p) => this._sourced.Exists(x => x.FilePath.Equals(p, Platform.StrComparison));

	public void Enable(string? pwd = null) {
		pwd ??= this.lastdir;
		if (this._enabled) return;
		this._enabled = true;
		this.Update(pwd);
	}

	public void Disable() {
		if (!this._enabled) return;
		this.Clear();
		this._enabled = false;
		this._warned.Clear();
	}

	public void Clear() {
		this._sourced.RemoveAll(x => {
			this.log.Info("unsourcing", x.FilePath);
			x.Unsource();
			return true;
		});
		this._warned.Clear();
	}

	public void Update(string pwd) {
		this.log.Debug($"update called in {pwd}");
		if(String.IsNullOrEmpty(pwd)) return;
		this.lastdir = pwd;
		if (!this._enabled) {
			this.log.Debug("nothing to do because the module is disabled");
			return;
		}

		this._sourced.RemoveAll(x => {
			if (pwd.StartsWith(x.Root, Platform.StrComparison)) return false;
			this.log.Debug($"unsourcing {x.FilePath}");
			x.Unsource();
			return true;
		});

		this._warned.RemoveWhere(x => {
			var parent = Path.GetDirectoryName(x);
			return !pwd.StartsWith(parent, Platform.StrComparison);
		});

		var files = this.findEnvFiles(pwd, true);
		this.sourceFiles(files);
	}

	public void ClearApprovedList() {
		Clear();
		auth.RemoveAll();
	}

	private List<string> findEnvFiles(string pwd, bool ignoreSourced) {
		var files = new List<string>(32) { };
		var dir = pwd;

		while (true) {
			var filepath = Path.Join(dir, _name);
			if (File.Exists(filepath) && (!ignoreSourced || !this.pathIsSourced(filepath)))
				files.Add(filepath);

			if (string.IsNullOrEmpty(dir) || dir.EndsInSeparator()) break;
			else dir = Path.GetDirectoryName(dir);
		}

		return files;
	}

	public List<string> FindEnvFiles(string pwd) => this.findEnvFiles(pwd, false);

	// psenvrc version.
	private void sourceFiles(List<string> files) {
		if (files is null || files.Count == 0) return;
		var warned = false;

		foreach (var f in files) {
			if (this._safe && !this.auth.IsMatch(f)) {
				if (!this.Quiet && this._warned.Add(f)) {
					this.log.Info("unauthorized file not sourced while safe mode is on", f);
					warned = true;
					System.Console.WriteLine($"dotenvrc info: {f} is not authorized, authorize it with `Approve-Dotenvrc` or disable the safe mode");
				}
				continue;
			}
			try {
				this.log.Info("sourcing file", f);
				var data = File.ReadAllText(f);
				this._sourced.Add(new DotenvFile(f, data));
			} catch (Exception e) {
				this.log.Exception(e, f);
			}
		}

		if (warned) System.Console.WriteLine("You can turn this message off by setting `$Dotenvrc.Quiet = $true`");
	}


	public bool AuthorizePattern(string path, bool update = false) {
		var fullpath = Path.GetFullPath(path);

		// When directory specified, use directory + .psenvrc as argument.
		// This is like the situation of  "Approve-Dotenvrc .".
		if (Directory.Exists(fullpath)) {
			fullpath = Path.Combine(fullpath, _name);
		}
		var ok = this.auth.Add(fullpath);
		this._warned.Remove(fullpath);
		if (ok && this.SafeMode) {
			this.Update(this.lastdir);
		}
		return ok;
	}

	public bool UnauthorizePattern(string path, bool update = false) {
		var fullpath = Path.GetFullPath(path);
		var ok = this.auth.Remove(fullpath);
		if (ok && this.SafeMode && update) {
			this.Update(this.lastdir);
		}
		return ok;
	}
}
