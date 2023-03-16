using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly HttpClient _client;

        public UnitTest1()
        {   
            
        }

        [TestMethod]
        public async Task Test1()
        {
            var response = await _client.GetAsync("/headers");
            var stringResult = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello World!", stringResult);
        }
    }
}