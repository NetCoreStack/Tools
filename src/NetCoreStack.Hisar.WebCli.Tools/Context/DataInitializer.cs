using NetCoreStack.Hisar.WebCli.Tools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using System.IO;

namespace NetCoreStack.Hisar.WebCli.Tools.Context
{
    public class DataInitializer
    {
        private static void InsertInitialDataFromResource(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scope.ServiceProvider.GetService<HisarCliContext>())
            {
                var existPages = db.Set<Page>().ToList();
            }

            var pageContent = Properties.Resource.DefaultPageContent;
            if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                var layoutPagePath = PathUtility.GetLayoutPagePath(HostingHelper.MainAppDirectory);
                pageContent = File.ReadAllText(layoutPagePath);
            }

            var pages = new List<Page>()
            {
                new Page
                {
                    Content = pageContent,
                    Name = HostingConstants.LayoutPageName,
                    PageType = PageType.Layout,
                    UpdatedDate = DateTime.Now
                }
            };

            AddOrUpdateAsync(serviceProvider, p => p.Id, pages);
        }

        private static void AddOrUpdateAsync<TEntity>(
            IServiceProvider serviceProvider,
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            List<TEntity> existingData;

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scope.ServiceProvider.GetService<HisarCliContext>())
            {
                existingData = db.Set<TEntity>().ToList();
            }

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scope.ServiceProvider.GetService<HisarCliContext>())
            {
                foreach (var item in entities)
                {
                    db.Entry(item).State = existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
                        ? EntityState.Modified
                        : EntityState.Added;
                }

                db.SaveChanges();
            }
        }

        public static void InitializeDb(IServiceProvider serviceProvider)
        {
            using (var db = serviceProvider.GetService<HisarCliContext>())
            {
                if (db.Database.EnsureCreated())
                {
                    InsertInitialDataFromResource(serviceProvider);
                }
            }
        }
    }
}
