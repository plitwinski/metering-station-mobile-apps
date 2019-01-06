using NUnit.Framework;
using MeteringStation.Mobile.Extensions;

namespace MeteringStation.Mobile.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Test]
        public void WhenStringIsEnctrypted_DecrypionReturnsInitialValue()
        {
            const string unencryptedValue = "SeCrEt1@#";
            var encryptedValue = unencryptedValue.Encrypt();
            var result = encryptedValue.Decrypt();

            Assert.AreEqual(unencryptedValue, result);
        }
    }
}
