using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QueryManager;

namespace IntegrationTests
{
    [TestFixture]
    public class RavenDataStorageTest
    {
        private string path;

        private RavenDataStorage storage;

        [TearDown]
        public void DeleteDatabase()
        {
            storage.DocumentStore.Dispose();
            var di = new DirectoryInfo(path);
            di.GetFiles().ToList().ForEach(f => f.Delete());
            di.GetDirectories().ToList().ForEach(d => d.Delete(true));
            path = null;
        }

        [Test]
        public void Test_Should_create_an_instance_of_RavenDB_when_folder_is_empty()
        {
            string DbPath = "Data";
            storage = new RavenDataStorage();
            storage.ConnectionString = @"~\" + DbPath;
            storage.Initialize();

            path = GetExecutingPath(DbPath);
            Assert.IsTrue(File.Exists(path + @"\Data"));
        }

        [Test]
        public void Test_Should_create_instance_of_RavenDB_in_MyDocuments_when_no_path_is_provided()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Posizioni\Archive");
            storage = new RavenDataStorage();
            storage.Initialize();

            Assert.IsTrue(File.Exists(path + @"\Data"));
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
