using Microsoft.EntityFrameworkCore;
using Talkie.Models;

namespace Talkie.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Message> Messages => Set<Message>();

        public DbSet<Text> Texts => Set<Text>();

        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<SharedFile> SharedFiles => Set<SharedFile>();

        public DbSet<Contact> Contacts => Set<Contact>();
    }
}