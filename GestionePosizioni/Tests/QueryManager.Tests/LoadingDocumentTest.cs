using Models.DocumentTypes;
using NUnit.Framework;

namespace QueryManager.Tests
{
    [TestFixture]
    public class LoadingDocumentTest
    {
        [Test]
        public void when_saving_a_loading_document_it_is_possible_to_determine_the_id()
        {
            var storage = new RavenDataStorage();
            storage.Initialize();
            const string firstId = "LoadingDocuments/10";
            const string secondId = "LoadingDocuments/21";
            var document = new LoadingDocument
            {
                Id = firstId,
                Notes = "Primo Inserimento"
            };

            var secondDocument = new LoadingDocument
            {
                Id = secondId,
                Notes = "secondo Inserimento"
            };

            using (var session = storage.CreateSession())
            {
                session.Store(document);
                session.Store(secondDocument);
                session.SaveChanges();
            }

            using (var session = storage.CreateSession())
            {
                var result = session.Load<LoadingDocument>(firstId);
                Assert.That(result.Notes, Is.EqualTo("Primo Inserimento"));

                var other = session.Load<LoadingDocument>(secondId);
                Assert.That(other.Notes, Is.EqualTo("secondo Inserimento"));
            }
        }
    }
}
