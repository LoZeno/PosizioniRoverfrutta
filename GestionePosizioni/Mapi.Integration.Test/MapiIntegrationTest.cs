using MailWrapper;
using NUnit.Framework;

namespace Mapi.Integration.Test
{
    [TestFixture]
    public class MapiIntegrationTest
    {
        [Test]
        public void Test()
        {
            MAPI mailWrapper = new MAPI();
            mailWrapper.AddRecipientTo("lozeno1982@gmail.com");
            mailWrapper.SendMailPopup("Mail di prova", "Inviamo una mail di prova");
        }
    }
}
