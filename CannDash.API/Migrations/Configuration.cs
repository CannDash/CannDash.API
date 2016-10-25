namespace CannDash.API.Migrations
{
    using EfficientlyLazy.IdentityGenerator;
    using EfficientlyLazy.IdentityGenerator.Entity;
    using Infrastructure;
    using RandomNameGenerator;
    using RandomNameGeneratorLibrary;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CannDash.API.Infrastructure.CannDashDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CannDash.API.Infrastructure.CannDashDataContext context)
        {
            
            var placeGenerator = new PlaceNameGenerator();
            string city = "San Diego";
            string companyName = string.Empty;
            string state = "CA";
            int zipcode = 92101;

            if (!context.Dispensaries.Any())
            {
                for (int i = 0; i < 20; i++)
                {
                    companyName = placeGenerator.GenerateRandomPlaceName();

                    context.Dispensaries.Add(new Models.Dispensary
                    {
                        CompanyName = companyName,
                        Street = Generator.GenerateAddress().AddressLine,
                        City = city,
                        State = state,
                        ZipCode = zipcode,
                        Email = companyName + "@me.com",
                        Phone = "619 123 5678"
                    });
                    context.SaveChanges();
                }
            }
            
            var personGenerator = new PersonNameGenerator();
            string firstName = String.Empty;
            string lastName = String.Empty;
            DateTime dob = DateTime.MinValue;

            if (!context.Customers.Any())
            {
                for (int i = 0; i < 20; i++)
                {
                    dob = Generator.GenerateDOB(21, 90);
                    firstName = personGenerator.GenerateRandomFirstName();
                    lastName = personGenerator.GenerateRandomLastName();

                    context.CustomerAddresses.Add(
                        new Models.CustomerAddress
                        {
                            CustomerAddressId = i,
                            Street = Generator.GenerateAddress().AddressLine,
                            City = city,
                            State = state,
                            ZipCode = zipcode,
                        });

                    context.Customers.Add(new Models.Customer
                    {
                        DispensaryId = 266,
                        FirstName = firstName,
                        LastName = lastName,
                        CustomerAddressId = i,
                        Email = firstName + '.' + lastName + "@me.com",
                        Phone = "619 123 8798",
                        DateOfBirth = dob,
                        Age = (DateTime.Today).Year - dob.Year
                    });
                    context.SaveChanges();
                }
            }

            if (!context.Drivers.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    dob = Generator.GenerateDOB(21, 90);
                    firstName = personGenerator.GenerateRandomFirstName();
                    lastName = personGenerator.GenerateRandomLastName();

                    context.Drivers.Add(new Models.Driver
                    {
                        DispensaryId = 14,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = firstName + '.' + lastName + "@me.com",
                        Phone = "619 123 2345",
                    });
                    context.SaveChanges();
                }
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
