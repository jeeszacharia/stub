using Microsoft.EntityFrameworkCore;

namespace CIAMstubAPI
{
    class PersonDb : DbContext
    {
        public PersonDb(DbContextOptions<PersonDb> options)
            : base(options) { }

        public DbSet<PersonModel> PersonData => Set<PersonModel>();
    }
}
