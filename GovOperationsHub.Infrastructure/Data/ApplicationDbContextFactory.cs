using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // نضع سلسلة الاتصال (Connection String) الخاصة ببيئة التطوير فقط ليتمكن المحرك من قراءة المزود (SQL Server)
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GovOperationsHubDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            // نمرر null للخدمات الإضافية لأن محرك الـ Migrations لا يحتاجها إطلاقاً لمعرفة شكل الجداول
            return new ApplicationDbContext(optionsBuilder.Options, null!, null!);
        }
    }
}
