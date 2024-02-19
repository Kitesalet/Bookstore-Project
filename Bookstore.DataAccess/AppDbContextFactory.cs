using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookstore.DataAccess.DAL;
using Microsoft.Extensions.Configuration;

namespace Bookstore.DataAccess
{

        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Bookstore");

                var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json")
                .Build();

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

                return new AppDbContext(optionsBuilder.Options);
            }
        }

    
}
