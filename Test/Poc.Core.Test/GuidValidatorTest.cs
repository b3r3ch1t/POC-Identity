using POC.Domain.Core.Extensions;
using System;
using Xunit;

namespace Poc.Core.Test
{
    public class GuidValidatorTest
    {

        [Fact]
        public void Guid_Valid()
        {
            var guid = Guid.NewGuid();

            var result = guid.ToString().IsValidGuid();

            Assert.True(result);
        }

        [Fact]
        public void Guid_Empty_Failed()
        {
            var guid = string.Empty;

            var result = guid.IsValidGuid();

            Assert.False(result);
        }


        [Fact]

        public void Guid_Default_Failed()
        {
            var guid = default(Guid).ToString();

            var result = guid.IsValidGuid();

            Assert.False(result);
        }
    }
}
