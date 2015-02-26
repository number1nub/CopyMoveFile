using System;
using System.IO;


namespace CopyMoveFile
{
	class Program
	{
		static void Main(string[] args)
		{
			var oArgs  = new Arguments(args);

			string action = oArgs["action"] == null ? args[0] : oArgs["action"];
			string inPath = oArgs["inPath"] != null ? oArgs["inPath"] : (oArgs["inFile"] != null ? oArgs["inFile"] : (args[1] != null ? args[1] : ""));
			string outPath = oArgs["outPath"] != null ? oArgs["outPath"] : (oArgs["outFile"] != null ? oArgs["outFile"] : (args[2] != null ? args[2] : ""));

			if (string.IsNullOrEmpty(inPath) || string.IsNullOrEmpty(outPath))
			{
				Console.WriteLine("\nInvalid or missing file path(s).\n\nUse -? to view available options");
				Console.ReadLine();
				Environment.Exit(1);
			}
			if (string.IsNullOrEmpty(action))
			{
				Console.WriteLine("\nInvalid action parameter \"{0}\".\n\nUse -? to view available options", action);				
				Environment.Exit(1);
			}

			// Verify that source file exists
			var inFile = new FileInfo(inPath);
			var outFile = new FileInfo(outPath);
			var inAttrs = inFile.Attributes;
			var outAttrs = outFile.Attributes;

			if (!inFile.Exists)
			{
				Console.WriteLine("\nInvalid input file. Unable to locate \"{0}\"", inPath);
				Environment.Exit(1);
			}
			
			// Determine whether the paths are files or folders
			bool outIsDir = (outAttrs & FileAttributes.Directory) != 0 ? true : false;
			bool inIsDir = (inAttrs & FileAttributes.Directory) != 0 ? true : false;

			if (inIsDir)
			{
				Console.WriteLine("\nInvalid input file path. Currently only functional for single files.");
				Environment.Exit(1);				
			}



			switch (action)
			{
				case "copy":					
					if (!outIsDir)
					{	//
						// Copy file to a given file name
						//
						if (outFile.Exists)
						{
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite != "Y" && overWrite != "YES")
								Environment.Exit(1);
						}
					}
					else
					{	//
						// Copy file to a given directory
						//
						outPath = Path.Combine(outPath, inFile.Name);
						if (new FileInfo(outPath).Exists)
						{ 
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite != "Y" && overWrite != "YES")
								Environment.Exit(1);
						}
					}
					inFile.CopyTo(outPath, true);
					break;

				case "move":
					if (!outIsDir)
					{	//
						// Move file to a given file name
						//
						if (outFile.Exists)
						{
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite == "Y" || overWrite == "YES")
							{
								new FileInfo(outFile.FullName + ".bak").Delete();
								outFile.MoveTo(outFile.FullName + ".bak");
							}
							else
								Environment.Exit(1);
						}
					}
					else
					{	//
						// Move file to a given directory
						//
						outPath = Path.Combine(outPath, inFile.Name);
						outFile = new FileInfo(outPath);
						if (outFile.Exists)
						{
							Console.WriteLine("The specified output file already exists. Overwrite? [y/n]: ");
							string overWrite = Console.ReadLine().ToUpper();
							if (overWrite == "Y" || overWrite == "YES")
							{
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
