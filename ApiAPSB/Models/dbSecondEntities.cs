using System.Data.Entity;
namespace ApiAPSB.Models
{
    public class dbSecondEntities : DbContext
    {
        public dbSecondEntities()
            : base("name=dbSecondEntities") { }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PersonCustomer> PersonCustomers { get; set; }
       

    }
}