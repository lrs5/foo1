using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LRS {
	/// <summary>
	/// Summary description for SLOC.
	/// </summary>
	class SLOC {

		static LineCounts	TotalLines;

		static Regex re;

//---------------------------------------------------------------------------------------


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)	{
			if (args.Length <= 1) {
				Console.Error.WriteLine("Usage: SLOC dirname filenamepattern+"
					+ "\n       (e.g. SLOC . *.c *.cpp)");
					return 1;
			}
			DecodeSymbols();
			re = new Regex(@"^\s*}\s*(//.*)?", RegexOptions.Compiled);
			TotalLines = new LineCounts();
			string	dir;
			dir = Path.GetFullPath(args[0]);
			Console.WriteLine("Processing files in {0}", dir);
			for (int i=1; i<args.Length; ++i) 
				ProcessFileSpec(dir, args[i]);
			Console.WriteLine("\nGrand total: {0} lines", TotalLines);
			DecodeSymbols();
			return 0;
		}

//---------------------------------------------------------------------------------------

		static void ProcessFileSpec(string dir, string filespec) {
			string []	filenames  = Directory.GetFiles(dir, filespec);
			LineCounts  ThisFile   = new LineCounts(),
						DirFiles   = new LineCounts();
			// int			nLines, DirLines = 0;
			Console.WriteLine("\nProcessing {0}\\{1}", dir, filespec);
			foreach (string fn in filenames) {
				ThisFile   =  CountLines(fn);
				DirFiles   += ThisFile;
				TotalLines += ThisFile;
				Console.WriteLine("{0,5} - {1}", ThisFile, Path.GetFileName(fn));
			}
			Console.WriteLine("-----");
			Console.WriteLine("{0,5} - Total", DirFiles);
			string [] DirNames = Directory.GetDirectories(dir, "*");
			foreach (string DirName in DirNames) {
				ProcessFileSpec(DirName, filespec);
			}
		}

//---------------------------------------------------------------------------------------

		static LineCounts CountLines(string filename) {
			var lines = new LineCounts();
			using (StreamReader rdr = new StreamReader(filename, Encoding.ASCII)) {
				string s;
				while ((s = rdr.ReadLine()) != null) {
					++lines.nLines;				// Count total lines
					s = s.Trim();
					if (s.Length == 0) {
						++lines.nBlankLines;	// Count totally blank lines
					} else {
						if (re.Match(s).Success) {
							++lines.nJustClosingBrace;
						} else {
							if (s.Length > 1 && s.Substring(0, 2) == "//") {
								++lines.nCommentLines;
							} else {
								// TODO: The following is only a rough approximation to /* comments
								//		 a line is inside a /* block iff the leftmost non-blank
								//		 characters are either /* or *
								// TODO: Another shortcut. Blank lines inside /* comments are
								//		 counted as blank lines, not as comments.
								// TODO: We really need to set up a small FSM here.
								if ((s.Length > 1 && s.Substring(0, 2) == "/*") || s[0] == '*') {
									++lines.nCommentLines;
								}
							}
						}
					}
				}
			}
			return lines;
		}

//---------------------------------------------------------------------------------------

		private static void DecodeSymbols() {
			Console.WriteLine("L=Total lines, B=Blank lines, }=Just closing brace, //=Comment lines, T=Text lines");
		}
	}
}
