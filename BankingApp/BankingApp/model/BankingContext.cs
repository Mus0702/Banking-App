
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;
using System;

namespace BankingApp.Model {
    public class BankingContext : DbContextBase {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=banking")
           .EnableSensitiveDataLogging()
           .UseLazyLoadingProxies(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasMany(virement => virement.ListSentTransfers)
                .WithOne(compte => compte.OriginAccount)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Account>()
               .HasMany(virement => virement.ListReceivedTransfers)
               .WithOne(compte => compte.DestinationAccount)
               .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<ClientInternalAccount>()
                .HasKey(c => new { c.ClientId, c.InternalAccountId });

            modelBuilder.Entity<Account>()
        .HasDiscriminator(c => c.AccountType)
        .HasValue<CurrentAccount>(AccountType.CurrentAccount)
        .HasValue<SavingAccount>(AccountType.SavingAccount)
        .HasValue<ExternalAccount>(AccountType.ExternalAccount);

            modelBuilder.Entity<User>()
        .HasDiscriminator(m => m.Role)
        .HasValue<Client>(Role.Client)
        .HasValue<Manager>(Role.Manager)
        .HasValue<Admin>(Role.Admin);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<InternalAccount> InternalAccounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<ClientInternalAccount> ClientsInternalAccounts { get; set; }

        public void SeedData() {
            Database.BeginTransaction();

            var bruno = new Manager("Lacroix", "Bruno", "bruno@test.com", "bruno");
            var benoit = new Manager("Penelle", "Ben", "ben@test.com", "ben");
            var admin = new Admin("Istrator", "Admin", "admin@test.com", "admin");

            var agency1 = new Agency("Agency1", benoit);
            var agency2 = new Agency("Agency2", benoit);
            var agency3 = new Agency("Agency3", bruno);
            Agencies.AddRange(agency1, agency2, agency3);

            var bob = new Client("Marley", "bob", "bob@test.com", "bob", agency1);
            var caro = new Client("de Monaco", "caro", "caro@test.com", "caro", agency1);
            var louise = new Client("TheCross", "louise", "louise@test.com", "louise", agency2);
            var jules = new Client("TheCross", "jules", "jules@test.com", "jules", agency2);
            Users.AddRange(bruno, benoit, admin, bob, caro, louise, jules);

            var currentA = new CurrentAccount("BE02 9999 1017 8207", "AAA", -50);
            var currentB = new CurrentAccount("BE14 9996 1669 4306", "BBB", -10);
            var currentD = new CurrentAccount("BE55 9999 6717 9982", "DDD", -100);
            var savingC = new SavingAccount("BE71 9991 5987 4787", "CCC");
            var externalE = new ExternalAccount("BE23 0081 6870 0358", "EEE");

            var cia1 = new ClientInternalAccount(bob, currentA, TypeClient.Holder);
            var cia2 = new ClientInternalAccount(bob, currentB, TypeClient.Holder);
            var cia3 = new ClientInternalAccount(bob, savingC, TypeClient.Mandatory);
            var cia4 = new ClientInternalAccount(caro, currentA, TypeClient.Mandatory);
            var cia5 = new ClientInternalAccount(caro, currentD, TypeClient.Holder);
            var cia6 = new ClientInternalAccount(caro, savingC, TypeClient.Holder);
            ClientsInternalAccounts.AddRange(cia1, cia2, cia3, cia4, cia5, cia6);

            var cat1 = new Category("Category1");
            var cat2 = new Category("Category2");
            var cat3 = new Category("Category3");
            var cat4 = new Category("Category4");
            var cat5 = new Category("Category5");
            Categories.AddRange(cat1, cat2, cat3, cat4, cat5);


            var transfer14 = new Transfer(bob, 10, currentA, currentB, "Tx #001", new DateTime(2022, 01, 01));
            var transfer12 = new Transfer(bob, 15, currentB, savingC, "Tx #004", new DateTime(2022, 01, 02), new DateTime(2022, 01, 03));
            var transfer6 = new Transfer(bob, 50, currentA, currentB, "Tx #005", new DateTime(2022, 01, 02), new DateTime(2022, 01, 04));  //refusé
            var transfer7 = new Transfer(caro, 5, currentA, savingC, "Tx #002", new DateTime(2022, 01, 01), new DateTime(2022, 01, 05));
            var transfer9 = new Transfer(caro, 100, savingC, currentB, "Tx #008", new DateTime(2022, 01, 06)); //refusé
            var transfer5 = new Transfer(bob, 20, currentB, currentA, "Tx #006", new DateTime(2022, 01, 03), new DateTime(2022, 01, 07));   //refusé
            var transfer2 = new Transfer(null, 5, externalE, savingC, "Tx #007", new DateTime(2022, 01, 04), new DateTime(2022, 01, 08));
            var transfer8 = new Transfer(caro, 35, currentA, currentB, "Tx #003", new DateTime(2022, 01, 01), new DateTime(2022, 01, 09));
            var transfer13 = new Transfer(bob, 15, savingC, currentA, "Tx #010", new DateTime(2022, 01, 10));
            var transfer4 = new Transfer(bob, 10, savingC, externalE, "Tx #009", new DateTime(2022, 01, 07), new DateTime(2022, 01, 11));
            var transfer11 = new Transfer(caro, 100, currentA, savingC, "Tx #013", new DateTime(2022, 01, 13));                             //refusé
            var transfer3 = new Transfer(bob, 35, currentB, savingC, "Tx #012", new DateTime(2022, 01, 12), new DateTime(2022, 01, 14));
            var transfer1 = new Transfer(benoit, 15, currentA, currentB, "Tx #014", new DateTime(2022, 01, 15));
            var transfer10 = new Transfer(caro, 15, currentA, savingC, "Tx #011", new DateTime(2022, 01, 11), new DateTime(2022, 01, 16)); //refusé
            Transfers.AddRange(transfer1, transfer2, transfer3, transfer4, transfer5, transfer6, transfer7, transfer8,
                transfer9, transfer10, transfer11, transfer12, transfer13, transfer14);

            SaveChanges();
            Database.CommitTransaction();
        }
    }
}
