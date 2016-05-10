﻿using System;
using System.Threading.Tasks;
using DocumentDB.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Tests
{
    [TestClass]
    public class CustomUserPropertiesTests : TestBase
    {
        private readonly UserStore<TestUserType> _userStore;

        public CustomUserPropertiesTests()
        {
            _userStore = new UserStore<TestUserType>(new Uri(Endpoint), Key, Userdb, Usercoll);
        }

        private TestUserType _testUser = new TestUserType
        {
            UserName = "customtest@test.com",
            Email = "customtest@test.com",
            IsAwesome = true,
            TestTest = "this just some some text..."
        };

        [TestMethod]
        public async Task CanCreateUserWithCustomProperties()
        {
            await _userStore.CreateAsync(_testUser);

            var savedUser = await _userStore.FindByEmailAsync(_testUser.Email);

            Assert.IsNotNull(savedUser);
            Assert.IsTrue(savedUser.IsAwesome);
            Assert.IsFalse(string.IsNullOrEmpty(savedUser.TestTest));
            Assert.AreEqual(_testUser.Email, savedUser.Email);
        }

        [TestMethod]
        public async Task UpdatesAreAppliedToUserWithCustomProperties()
        {
            await CanCreateUserWithCustomProperties();

            var savedUser = await _userStore.FindByEmailAsync(_testUser.Email);
            if (savedUser == null)
            {
                throw new NullReferenceException("savedUser");
            }

            savedUser.IsAwesome = false;
            savedUser.TestTest = "test test text";

            await _userStore.UpdateAsync(savedUser);

            savedUser = await _userStore.FindByEmailAsync(_testUser.Email);

            Assert.IsNotNull(savedUser);
            Assert.IsFalse(savedUser.IsAwesome);
            Assert.AreSame(savedUser.TestTest, "test test text");
        }

    }

    public class TestUserType : IdentityUser
    {
        public bool IsAwesome { get; set; }
        public string TestTest { get; set; }
    }
}