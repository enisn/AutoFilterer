using AutoFilterer.Dynamics.Tests.Environment;
using AutoFilterer.Dynamics.Tests.Environment.Statics;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoFilterer.Dynamics
{
    public class ModelBinderTests : IClassFixture<WebApplicationFactory<WebApplication.API.Startup>>
    {
        private readonly WebApplicationFactory<WebApplication.API.Startup> _factory;
        public ModelBinderTests(WebApplicationFactory<WebApplication.API.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ModelBinder_ShouldBindCorrect_WithSingleParameter()
        {
            using (var client = _factory.CreateClient())
            {
                var query = new DynamicFilter { { "Page", "1" } };
                var expectedJson = "{ \"Page\": \"1\" }";

                var responseJson = await client.GetStringAsync("/_api/Tests/query-string-as-object?" + string.Join('&', query.Select(s => $"{s.Key}={s.Value}")));

                Assert.Equal(expectedJson, responseJson);
            }
        }
    }
}
