using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kariaji.WebApi.Helpers
{
    public static class DbContextHelper
    {
        public static IMutableForeignKey WithDisabledCascadeDelete(this IMutableForeignKey fk)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
            return fk;
        }

        public static ModelBuilder WithDisabledCascadeDeleteOnForeignKeys(this ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.WithDisabledCascadeDelete();
            return modelBuilder;
        }
    }
}
