using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MongoDB.Driver;

namespace InterWebs.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IUserStore<ApplicationUser>
    {
        public ApplicationDbContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            Database = server.GetDatabase("InterWebs");

//            var collection = Database.GetCollection<ApplicationUser>("entities");
//            var entity = new ApplicationUser {UserName = "sangUser"};
//            collection.Insert(entity);
        }

        private MongoDatabase Database { get; set; }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}