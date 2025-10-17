using MongoDB.Bson;
using MongoDB.Driver;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Shared.Models;
using Shared.Util;
using Shared.Exceptions;

namespace Northwind.Data.Repositories.Concrete
{
    public class ProductRepository(IMongoContext context) : GenericRepository<Product>(context), IProductRepository
    {
        private static FilterDefinition<Product> BuildFilters(GameFilter filter)
        {
            var builder = Builders<Product>.Filter;
            var filters = new List<FilterDefinition<Product>>();

            if (filter.Blacklist.Count > 0)
                filters.Add(builder.Nin(p => p.Id, filter.Blacklist));

            if (filter.MaxPrice.HasValue)
                filters.Add(builder.Lte(p => p.UnitPrice, filter.MaxPrice.Value));

            if (filter.MinPrice.HasValue)
                filters.Add(builder.Gte(p => p.UnitPrice, filter.MinPrice.Value));

            if (IsFilterSet(filter.Name))
                filters.Add(builder.Regex(p => p.ProductName, new BsonRegularExpression($".*{filter.Name}.*", "i")));

            if (!filter.ShowDeleted)
                filters.Add(builder.Eq(p => p.Deleted, false));

            var categoryIds = (filter.MongoCategories ?? new List<string>())
                .Concat(filter.Genres?.Select(g => g.ToString()) ?? Enumerable.Empty<string>())
                .ToList();
            if (categoryIds.Count > 0)
                filters.Add(builder.In<string>(p => p.CategoryId.ToString(), categoryIds));

            var supplierIds = (filter.MongoSuppliers ?? new List<string>())
                .Concat(filter.Publishers?.Select(p => p.ToString()) ?? Enumerable.Empty<string>())
                .ToList();
            if (supplierIds.Count > 0)
                filters.Add(builder.In<string>(p => p.SupplierId.ToString(), supplierIds));

            return filters.Count > 0 ? builder.And(filters) : builder.Empty;
        }

        private static SortDefinition<Product>? BuildSort(string? sort)
        {
            if (string.IsNullOrEmpty(sort)) return null;

            return sort switch
            {
                SortingFilter.PriceAsc => Builders<Product>.Sort.Ascending(p => p.UnitPrice),
                SortingFilter.PriceDesc => Builders<Product>.Sort.Descending(p => p.UnitPrice),
                SortingFilter.MostCommented => Builders<Product>.Sort.Descending(p => p.CommentCount),
                SortingFilter.MostPopular => Builders<Product>.Sort.Descending(p => p.Popularity),
                SortingFilter.Newest => Builders<Product>.Sort.Descending(p => p.CreatedDate),
                _ => throw new EntityNotFoundException($"Invalid sorting option: {sort}")
            };
        }

        public async Task<EntityFilter<Product>> GetFilteredProductsAsync(GameFilter filter)
        {
            if (filter.Stores != null && !filter.Stores.Contains(StoreType.Northwind))
                return new EntityFilter<Product>(new List<Product>(), 0);

            var finalFilter = BuildFilters(filter);
            var sortDefinition = BuildSort(filter.Sort);

            var total = await DbSet.CountDocumentsAsync(finalFilter);
            var findFluent = DbSet.Find(finalFilter);

            if (sortDefinition != null)
                findFluent = findFluent.Sort(sortDefinition);

            findFluent = findFluent.Limit(filter.Limit);
            var products = await findFluent.ToListAsync();

            return new EntityFilter<Product>(products, (int)total);
        }

        public async Task<IList<Category>> GetCategoriesByProductKeyAsync(string key)
        {
            var product = await DbSet.Find(p => p.Key == key).FirstOrDefaultAsync();
            if (product == null)
            {
                return new List<Category>();
            }
            var categories = await Context.GetCollection<Category>()
                                          .Find(c => c.CategoryId == product.CategoryId)
                                          .ToListAsync();
            return categories;
        }

        public async Task<Supplier> GetSupplierByProductKeyAsync(string key)
        {
            var product = await DbSet.Find(p => p.Key == key).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new EntityNotFoundException($"Product not found: {key}");
            }
            var supplier = await Context.GetCollection<Supplier>().Find(s => s.SupplierId == product.SupplierId)
                                        .FirstOrDefaultAsync();
            return supplier;
        }

        private static bool IsFilterSet(string? filterBy)
        {
            return !string.IsNullOrEmpty(filterBy);
        }
    }
}
