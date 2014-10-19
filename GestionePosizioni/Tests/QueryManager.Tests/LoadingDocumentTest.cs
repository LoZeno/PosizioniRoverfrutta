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

            var document = new LoadingDocument
            {
                Id = 10, 
                Notes = "Primo Inserimento"
            };

            var secondDocument = new LoadingDocument
            {
                Id = 5,
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
                var result = session.Load<LoadingDocument>(10);
                Assert.That(result.Notes, Is.EqualTo("Primo Inserimento"));

                var other = session.Load<LoadingDocument>(5);
                Assert.That(other.Notes, Is.EqualTo("secondo Inserimento"));
            }
        }
    }
}
