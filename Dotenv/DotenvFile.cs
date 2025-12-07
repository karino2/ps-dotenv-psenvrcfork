namespace Dotenv;

public readonly struct EnvVar {
	public string Name { get; }
	public string Value { get; }
	public string? Replaced { get; }
	public EnvVar(string name, string val, string? replaced) => (this.Name, this.Value, this.Replaced) = (name, val, replaced);

	public override string ToString() => $"{this.Name} = {this.Value}";
	internal void unset() => Environment.SetEnvironmentVariable(this.Name, this.Replaced);
}

public class DotenvFile {
	public string FilePath { get; internal set; }
	public string Name { get; internal set; }
	public string Root { get; internal set; }
	List<EnvVar> _vars;

	public IList<EnvVar> Vars => this._vars.AsReadOnly();

	internal DotenvFile(string path, string psscript) {
		this.FilePath = path;
		this.Root = Path.GetDirectoryName(path) ??　"";
		this.Name = Path.GetFileName(path);

		this._vars = new List<EnvVar>();


		var parser = new PsenvrcEvaluator();
		this._vars = parser.Eval(psscript);
		if (this._vars.Count != 0) {
			Console.WriteLine("Load:");
			foreach (var v in this._vars) {
				Console.WriteLine(v.Name);
			}
		}
	}



	internal void Unsource() {
		if (this._vars.Count != 0) {
			Console.WriteLine("Unload:");
			foreach (var v in this._vars) {
				Console.WriteLine(v.Name);
			}
		}
		foreach (var v in this._vars) {
			v.unset();
		}
	}
}
