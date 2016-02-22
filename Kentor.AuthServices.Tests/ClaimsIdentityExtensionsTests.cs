﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ClaimsIdentityExtensionsTests
    {
        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIdentity()
        {
            ClaimsIdentity identity = null;

            Action a = () => identity.ToSaml2Assertion(new EntityId("foo"));

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("identity");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIssuer()
        {
            var subject = new ClaimsIdentity();

            Action a = () => subject.ToSaml2Assertion(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("issuer");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Subject()
        {
            var subject = "JohnDoe";

            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, subject)
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            a.Subject.NameId.Value.Should().Be(subject);
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Sets_SessionIndex()
        {
            var subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "NameId"),
                new Claim(AuthServicesClaimTypes.SessionIndex, "SessionID"),
                new Claim(ClaimTypes.Email, "me@example.com")
            });

            var issuer = new EntityId("http://idp.example.com");
            var actual = subject.ToSaml2Assertion(issuer);

            actual.Statements.OfType<Saml2AuthenticationStatement>()
                .Single().SessionIndex.Should().Be("SessionID");

            var attributes = actual.Statements.OfType<Saml2AttributeStatement>()
                .Single().Attributes;

            attributes.Should().HaveCount(1);

            attributes.Single().ShouldBeEquivalentTo(
                new Saml2Attribute(ClaimTypes.Email, "me@example.com"));
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Attributes()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test"),
                new Claim(ClaimTypes.Email, "me@example.com"),
            });

            var actual = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            actual.Statements.OfType<Saml2AttributeStatement>().Should().HaveCount(1);
            actual.Statements.OfType<Saml2AttributeStatement>().Single().Attributes[0].Values[0].Should().Be("Test");
            actual.Statements.OfType<Saml2AttributeStatement>().Single().Attributes[1].Values[0].Should().Be("me@example.com");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_MultipleValuesForSameKey_CombinesTo_OneAttribute()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test1"),
                new Claim(ClaimTypes.Role, "Test2"),
            });

            var assertion = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            assertion.Statements.OfType<Saml2AttributeStatement>().Should().HaveCount(1);
            var actual = assertion.Statements.OfType<Saml2AttributeStatement>().Single();

            var expected = new Saml2AttributeStatement(
                new Saml2Attribute(ClaimTypes.Role, new[] { "Test1", "Test2" }));

            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_DefaultCondition()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe")
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            // Default validity time is hearby defined to two minutes.
            a.Conditions.NotOnOrAfter.Value.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(2));
        }
    }
}
