using System;
using System.Globalization;
using System.IO;
using Moq;
using NUnit.Framework;
using NUnit.Util;
using Version.Templates;

namespace Version.Test
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void Program_Main_CallsParseAndCreateMethodsForCSFile()
        {
            const string outputFile = "TestOutput.cs";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            Program.Main(new[] { outputFile, "1", "0", "Foo" });

            Assert.True(File.Exists(outputFile), "Program created a file named 'TestOutput.cs'");
            Console.WriteLine("File output:\r\n{0}", File.ReadAllText(outputFile));
        }

        [Test]
        public void Program_Main_CallsParseAndCreateMethodsForJSFile()
        {
            const string outputFile = "TestOutput.js";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            Program.Main(new[] { outputFile, "1", "0", "Foo" });

            Assert.True(File.Exists(outputFile), "Program created a file named 'TestOutput.js'");
            Console.WriteLine("File output:\r\n{0}", File.ReadAllText(outputFile));
        }

        [Test]
        public void Program_Ctor_HasPropertiesDefaulted()
        {
            var program = new Program();

            Assert.IsAssignableFrom<SystemDateTimeService>(program.DateTimeService);
            Assert.AreEqual(2, program.VersionEditors.Count);
            Assert.IsAssignableFrom<CsAssemblyInfoTemplate>(program.VersionEditors[".cs"]());
            Assert.IsAssignableFrom<JsVersionInfoTemplate>(program.VersionEditors[".js"]());
        }

        [Test]
        public void Program_CreateVersionFile_ThrowsForUnknownExtension()
        {
            const string outputFile = "TestOutput.UNK";
            var programArgs = new ProgramArgs()
            {
                FullPath = Path.GetFullPath(outputFile),
            };
            var program = new Program();

            Assert.Throws<InvalidOperationException>(() => program.CreateVersionFile(programArgs));
        }

        [Test]
        public void Program_CreateVersionFile_VersionEditorCalled()
        {
            const string outputFile = "TestOutput.test";
            var testVersionEditor = new Mock<IVersionEditor>();
            testVersionEditor.SetupAllProperties();

            IVersionEditor versionEditor = testVersionEditor.Object;
            var programArgs = new ProgramArgs()
            {
                FullPath = Path.GetFullPath(outputFile),
                Namespace = "FOO",
                Major = 3,
                Minor = 9,
            };
            var program = new Program();
            program.VersionEditors.Add(".test", () => versionEditor);

            program.CreateVersionFile(programArgs);

            Assert.True(File.Exists(outputFile), "Program created a file named 'TestOutput.test'");
            Assert.AreEqual(3, versionEditor.Version.Major);
            Assert.AreEqual(9, versionEditor.Version.Minor);
            Assert.AreNotEqual(0, versionEditor.Version.Build);
            Assert.AreNotEqual(0, versionEditor.Version.Revision);
            Assert.AreEqual("FOO", versionEditor.Namespace);
            testVersionEditor.Verify(v => v.TransformText());
        }

        [TestCase("1/1/0001 12:00:00 AM", 10101)]
        [TestCase("12/31/9999 11:59:59.9999999 PM", 31231)]
        [TestCase("12/31/2008 11:59:59.9999999 PM", 61231)]
        [TestCase("1/1/2009 12:00:00 AM", 101)]
        public void Program_GetDateRev(string date, int expectedResult)
        {
            DateTime inputDate = DateTime.Parse(date, CultureInfo.InvariantCulture);
            var dateTimeService = Mock.Of<IDateTimeService>((s) => s.GetDateTimeEst() == inputDate);
            var program = new Program()
            {
                DateTimeService = dateTimeService
            };

            int result = program.GetDateRev();
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("1/1/0001 12:00:00 AM", 0)]
        [TestCase("1/1/0001 11:59:59.9999999 PM", 2359)]
        public void Program_GetTimeRev(string date, int expectedResult)
        {
            DateTime inputDate = DateTime.Parse(date, CultureInfo.InvariantCulture);
            var dateTimeService = Mock.Of<IDateTimeService>((s) => s.GetDateTimeEst() == inputDate);
            var program = new Program()
            {
                DateTimeService = dateTimeService
            };

            int result = program.GetTimeRev();
            Assert.AreEqual(expectedResult, result);
        }
    }
}
