using System;
using System.IO;

namespace Version
{
    class ProgramArgs
    {
        public string FullPath { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public string Namespace { get; set; }

        public static ProgramArgs Parse(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
                throw new InvalidOperationException("Invalid arguments");

            var programArgs = new ProgramArgs()
            {
                FullPath = Path.GetFullPath(args[0]),
                Major = Int32.Parse(args[1]),
                Minor = Int32.Parse(args[2]),
                Namespace = args.Length > 3 ? args[3] : "DefaultNamespace",
            };
            return programArgs;
        }
    }
}