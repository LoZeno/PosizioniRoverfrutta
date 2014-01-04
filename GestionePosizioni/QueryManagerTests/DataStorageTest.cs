using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using QueryManager;

namespace QueryManagerTests
{
    [TestFixture]
    public class DataStorageTest
    {
        private readonly string DbPath = "Data";

        private DataStorage storage;

        [TearDown]
        public void DeleteDatabase()
        {
            storage.DocumentStore.Dispose();
            var di = new DirectoryInfo(GetExecutingPath(DbPath));
            di.GetFiles().ToList().ForEach(f => f.Delete());
            di.GetDirectories().ToList().ForEach(d => d.Delete(true));
            di.Delete(true);
        }

        [Test]
        public void Test_Should_create_an_instance_of_RavenDB_when_folder_is_empty()
        {
            storage = new DataStorage();
            storage.ConnectionString = @"~\Data";
            storage.Initialize();

            string path = GetExecutingPath(DbPath);
            Assert.IsTrue(Directory.Exists(path));
        }
        private string GetExecutingPath(string lastPart)
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(Path.GetDirectoryName(path), lastPart);
        }
    }
}
