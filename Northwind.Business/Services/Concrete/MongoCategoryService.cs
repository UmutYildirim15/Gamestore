using AutoMapper;
using Business.Services.Dto;
using Northwind.Business.Services.Abstract;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Northwind.Data.Entities;

namespace Northwind.Business.Services.Concrete
{
    public class MongoCategoryService(IMongoUnitOfWork unitOfWork, IMapper mapper, IEntityLogger entityLogger) : IMongoCategoryService
    {
        public async Task<GenreResponse> GetGenreByIdAsync(string id, string culture)
        {
            var category = await unitOfWork.Categories.GetOneAsync(c => c.Id == id);
            return mapper.Map<GenreResponse>(category);
        }

        public async Task<IList<GenreResponse>> GetAllGenresAsync(string culture)
        {
            var categories = await unitOfWork.Categories.GetAllAsync();
            return mapper.Map<IList<GenreResponse>>(categories);
        }

        public async Task<IList<GenreResponse>> GetSubgenresByParentAsync(string parentId, string culture)
        {
            var categories = await unitOfWork.Categories.GetAllAsync(c => c.ParentId == parentId);
            return mapper.Map<IList<GenreResponse>>(categories);
        }

        public async Task<IList<GameResponse>> GetGamesByGenreIdAsync(string id, string culture)
        {
            var games = await unitOfWork.Categories.GetProductsByCategoryIdAsync(id);
            return mapper.Map<IList<GameResponse>>(games);
        }

        public async Task<GenreResponse> CreateGenreAsync(Category category)
        {
            await unitOfWork.Categories.AddAsync(category);
            await unitOfWork.SaveChangesAsync();
            await entityLogger.LogCreateAsync(category);
            return mapper.Map<GenreResponse>(category);
        }

        public async Task<GenreResponse> UpdateGenreAsync(string genreId, Category updatedCategory)
        {
            var existingCategory = await unitOfWork.Categories.GetOneAsync(c => c.Id == genreId);
            if(existingCategory == null)
            {
                throw new KeyNotFoundException("Genre not found.");
            }
            await entityLogger.LogUpdateAsync(existingCategory, updatedCategory);
            existingCategory.CategoryName = updatedCategory.CategoryName;
            existingCategory.Description = updatedCategory.Description;
            existingCategory.Picture = updatedCategory.Picture;
            existingCategory.ParentId = updatedCategory.ParentId;
            await unitOfWork.Categories.UpdateAsync(existingCategory);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<GenreResponse>(existingCategory);
        }

        public async Task DeleteGenreAsync(string? genreId)
        {
            var category = await unitOfWork.Categories.GetOneAsync(c => c.Id == genreId);
            await entityLogger.LogDeleteAsync(category);
            await unitOfWork.Categories.DeleteAsync(genreId);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
