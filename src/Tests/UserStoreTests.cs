using System;
using DocumentDB.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using System.Security.Claims;

namespace Tests {
    public class CustomIdentityUser : IdentityUser {
        public virtual string CustomProperty { get; set; }
    }
    public enum DivalTitle : ushort {
        Unknown = 0,
        M = 1,
        Mme,
        MmeM,
        Me
    }
    public class ApplicationUser : IdentityUser {
        public virtual string StoreName { get; set; }
        public virtual string Address { get; set; }

        public virtual string City { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual string Country { get; set; }

        public virtual DivalTitle OwnerTitle { get; set; }

        public virtual string OwnerFirstName { get; set; }

        public virtual string OwnerLastName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync( Microsoft.AspNet.Identity.UserManager<ApplicationUser> manager ) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UserStoreTests : IDisposable {
        DocumentClient client = null;
        Database database = null;
        DocumentCollection collection = null;

        readonly string endpoint = "https://pdt-franchises.documents.azure.com:443/";
        readonly string key = "TRhwH7brHYDZ8UA6Ij9mLO850OB8CG0lb45qa5JkR1qvC5cQt9q3RxZbMlnIj3oLWv68hPdbGxuWt1t5YQLu7A==";
        readonly string userdb = "testdb";
        readonly string usercoll = "testusers";

        public UserStoreTests( ITestOutputHelper output ) {
            client = new DocumentClient( new Uri( endpoint ), key );
            database = Utils.ReadOrCreateDatabase( client, userdb );
            collection = Utils.ReadOrCreateCollection( client, database.SelfLink, usercoll );
        }
        [Fact]
        [Trait( "Category", "User" )]
        public async Task CanUpgradeUserSchema( ) {
            string username = "test_user";
            DivalTitle title = DivalTitle.M;

            await CanCreateUser( );

            var userstore = new UserStore<ApplicationUser>(new Uri(endpoint), key, userdb, usercoll);
            var user = await userstore.FindByNameAsync(username);
            Assert.False( user == null );
            user.OwnerTitle = title;
            await userstore.UpdateAsync( user );
            user = await userstore.FindByNameAsync( username );
            Assert.True( user.OwnerTitle.Equals( title ) );
        }
        [Fact]
        [Trait( "Category", "User" )]
        public async Task CanUpdateUser( ) {
            string username = "test_user";
            string test_prop = "test_property";

            await CanCreateUser( );

            var userstore = new UserStore<CustomIdentityUser>(new Uri(endpoint), key, userdb, usercoll);
            var user = await userstore.FindByNameAsync(username);
            Assert.False( user == null );
            user.CustomProperty = test_prop;
            await userstore.UpdateAsync( user );
            user = await userstore.FindByNameAsync( username );
            Assert.True( user.CustomProperty.Equals( test_prop ) );
        }
        [Fact]
        [Trait( "Category", "User" )]
        public async Task CanCreateUser( ) {
            string username = "test_user";

            var userstore = new UserStore<IdentityUser>(new Uri(endpoint), key, userdb, usercoll);
            await userstore.CreateAsync( new IdentityUser( "test_user" ) );
            IdentityUser user = await userstore.FindByNameAsync(username);
            Assert.False( user == null );
            Assert.True( username.Equals( user.UserName ) );
        }

        public void Dispose( ) {
            client.DeleteDatabaseAsync( database.SelfLink ).Wait( );
            client.Dispose( );
        }
    }
}
