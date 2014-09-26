using System;
using System.Collections.Generic;
using System.IO;
using Version.Templates;

namespace Version
{
    class Program
	{
        // TODO: Make extensible (MEF?)
        private readonly Dictionary<string, Func<IVersionEditor>> _versionEditors = new Dictionary<string, Func<IVersionEditor>>();

        public IDateTimeService DateTimeService { get; set; }

        public Dictionary<string, Func<IVersionEditor>> VersionEditors
        {
            get { return _versionEditors; }
        }

        public Program()
        {
            DateTimeService = new SystemDateTimeService();
            VersionEditors.Add(".cs", () => new CsAssemblyInfoTemplate());
            VersionEditors.Add(".js", () => new JsVersionInfoTemplate());
        }

		public static void Main(string[] args)
		{
		    var program = new Program();
		    var programArgs = ProgramArgs.Parse(args);

            program.CreateVersionFile(programArgs);
		}

        public void CreateVersionFile(ProgramArgs args)
        {
            string extension = Path.GetExtension(args.FullPath).ToLowerInvariant();
            int build = GetDateRev();
            int rev = GetTimeRev();

            Func<IVersionEditor> factory;

            if (!VersionEditors.TryGetValue(extension, out factory))
            {
                throw new InvalidOperationException(string.Format("No template for extension '{0}'", extension));
            }

            IVersionEditor versionEditor = factory();
            versionEditor.Version = new System.Version(args.Major, args.Minor, build, rev);
            versionEditor.Namespace = args.Namespace;

            var contents = versionEditor.TransformText();
            File.WriteAllText(args.FullPath, contents);
        }

        public int GetTimeRev()
	    {
			// Rev in HHMM
            DateTime eastern = DateTimeService.GetDateTimeEst();
		    return eastern.Hour*100
		           + eastern.Minute;
	    }

        public int GetDateRev()
	    {
            // Rev in YMMDD
            DateTime eastern = DateTimeService.GetDateTimeEst();
            return (eastern.Year % 7) * 10000
				   + eastern.Month * 100
				   + eastern.Day;
	    }
	}
}
