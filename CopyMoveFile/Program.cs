using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CopyMoveFile.Properties;


namespace CopyMoveFile
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var oArgs  = new Arguments(args);

			if (args.Length == 0) {
				Console.WriteLine("\nInvalid usage! Use -? to see help.");
				Environment.Exit(1); 
			}

			if (oArgs["?"] != null || oArgs["h"] != null || oArgs["help"] != null) {
				Console.WriteLine(Settings.Default.helpTxt);
				Environment.Exit(0);
			}

			string action = oArgs["action"] ?? args[0];
			string inPath = oArgs["inPath"] ?? (oArgs["inFile"] ?? (args.Length > 1 && args[1] != null ? args[1] : ""));
			string outPath = null;
			bool selectedPath = false;

			if (oArgs["outPath"] == null && args.Length < 3) {
				var dlg = new SaveFileDialog();
				dlg.InitialDirectory = new FileInfo(inPath).DirectoryName;
				dlg.OverwritePrompt = true;
				dlg.FileName = new FileInfo(inPath).Name;
				if (dlg.ShowDialog() == DialogResult.OK) {
					outPath = dlg.FileName;
					selectedPath = true;
				}
			}
			else
				outPath = oArgs["outPath"] ?? (args.Length > 2 && args[2] != null ? args[2] : "");

			if (string.IsNullOrEmpty(inPath) || string.IsNullOrEmpty(outPath)) {
				Console.WriteLine("\nInvalid or missing file path(s).\n\nUse -? to view available options");
				Console.ReadLine();
				Environment.Exit(1);
			}
			if (string.IsNullOrEmpty(action)) {
				Console.WriteLine("\nInvalid action parameter \"{0}\".\n\nUse -? to view available options", action);
				Environment.Exit(1);
			}

			// Verify that source file exists
			var inFile = new FileInfo(inPath);
			var outFile = new FileInfo(outPath);
			var inAttrs = inFile.Attributes;
			var outAttrs = outFile.Attributes;

			if (!inFile.Exists) {
				Console.WriteLine("\nInvalid input file. Unable to locate \"{0}\"", inPath);
				Environment.Exit(1);
			}

			// Determine whether the paths are files or folders
			bool outIsDir = !selectedPath && (outAttrs & FileAttributes.Directory) != 0 ? true : false;
			bool inIsDir = (inAttrs & FileAttributes.Directory) != 0 ? true : false;

			if (inIsDir) {
				Console.WriteLine("\nInvalid input file path. Currently only functional for single files.");
				Environment.Exit(1);
			}

			switch (action) {
				case "copy":
					if (!outIsDir) {	// Copy file to a given file name						
						if (outFile.Exists) {
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite != "Y" && overWrite != "YES")
								Environment.Exit(1);
						}
					}
					else {				// Copy file to a given directory
						outPath = Path.Combine(outPath, inFile.Name);
						if (new FileInfo(outPath).Exists) {
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite != "Y" && overWrite != "YES")
								Environment.Exit(1);
						}
					}
					inFile.CopyTo(outPath, true);
					break;

				case "move":
					if (!outIsDir) {	// Move file to a given file name
						if (outFile.Exists) {
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite == "Y" || overWrite == "YES") {
								new FileInfo(outFile.FullName + ".bak").Delete();
								outFile.MoveTo(outFile.FullName + ".bak");
							}
							else
								Environment.Exit(1);
						}
					}
					else {				// Move file to a given directory
						outPath = Path.Combine(outPath, inFile.Name);
						outFile = new FileInfo(outPath);
						if (outFile.Exists) {
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite == "Y" || overWrite == "YES") {
								new FileInfo(outFile.FullName + ".bak").Delete();
								outFile.MoveTo(outFile.FullName + ".bak");
							}
							else
								Environment.Exit(1);
						}
					}
					inFile.MoveTo(outPath);
					break;

				default:
					Console.WriteLine("Invalid action parameter.\n\nUse -? to view available options");
					Environment.Exit(1);
					break;
			}
		}
	}
}
