using System;
using Xunit;
using Maximus.Utilities;

namespace Maximus.Test.Utilities
{
    public class StringExtensionsFixture
    {

        #region IsNullOrWhiteSpace
        [Fact]
        public void IsNullOrWhiteSpace_TrueForNull()
        {
            string subject = null;
            var result = subject.IsNullOrWhiteSpace();

            // TODO: add user message describing test.
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrWhiteSpace_TrueForEmpty()
        {
            string subject = string.Empty; // same as "";
            var result = subject.IsNullOrWhiteSpace();

            // TODO: add user message describing test.
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrWhiteSpace_TrueForWhitespace()
        {
            Assert.True("\t".IsNullOrWhiteSpace());
        }

        [Fact]
        public void IsNullOrWhiteSpace_FalseForNominalString()
        {
            Assert.False("Test".IsNullOrWhiteSpace());
        }
        #endregion

        #region IsNullOrEmpty
        [Fact]
        public void IsNullOrEmpty_TrueForNull()
        {
            string subject = null;
            var result = subject.IsNullOrEmpty();

            // TODO: add user message describing test.
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_TrueForEmpty()
        {
            string subject = string.Empty; // same as "";
            var result = subject.IsNullOrEmpty();

            // TODO: add user message describing test.
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_FalseForWhitespace()
        {
            Assert.False("\t      ".IsNullOrEmpty());
        }

        [Fact]
        public void IsNullOrEmpty_FalseForNominalString()
        {
            Assert.False("Test".IsNullOrEmpty());
        }
        #endregion

        #region FormatWith

        [Fact]
        public void FormatWith_NormalUsage()
        {
            var result = "The answer to life, the universe, and {0} is {1}.".FormatWith("everything", 42);
            Assert.Equal("The answer to life, the universe, and everything is 42.", result);
        }

        [Fact]
        public void FormatWith_DoesNotThrowOnNull()
        {
            string template = null;
            var result = template.FormatWith("BOOM");
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("{0} {1}", new object[] { "Hello", "World" }, "Hello World")]
        [InlineData("{0:C2}", new object[] { 125 }, "$125.00")]
        public void FormatWithCases(string template, object[] parameters, string expected)
        {
            Assert.Equal(template.FormatWith(parameters), expected);
        }

        [Fact]
        public void FormatWith_AllowPartialApplication()
        {
            var template = "Number = {0}, String = {1}";
            var result = template.FormatWith(42);
            var expected = "Number = 42, String = {1}";

            Assert.Equal(expected, result);
        }


        #endregion

        [Fact]
        public void FormatEmptyArg()
        {
            var template = "";
            var result = template.Format(" ");
            var expected = "";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatNoArg()
        {
            var template = "{0} {2:yyyy-MM-dd} {1}";
            var result = template.Format();
            var expected = "{0} {2:yyyy-MM-dd} {1}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatNumberFirstPass()
        {
            var template = "{0} {2:yyyy-MM-dd} {1}";
            var result = template.Format(42);
            var expected = "42 {1:yyyy-MM-dd} {0}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatTimeSpanSecondPass()
        {
            var template = "42 {1:yyyy-MM-dd} {0}";
            var result = template.Format(TimeSpan.FromSeconds(50));
            var expected = "42 {0:yyyy-MM-dd} 00:00:50";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatDateTimeThirdPass()
        {
            var d = DateTime.Parse("1992-02-18");

            var template = "42 {0:yyyy-MM-dd} 00:00:50";
            var result = template.Format(d);
            var expected = string.Format(template, d);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatSinglePass()
        {
            var d = DateTime.Parse("1992-02-18");

            var template = "{0} {2:yyyy-MM-dd} {1}";
            var result = template.Format(42, TimeSpan.FromSeconds(50), d);
            var expected = string.Format(template, 42, TimeSpan.FromSeconds(50), d);

            Assert.Equal(expected, result);
        }
    }
}
