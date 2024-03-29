﻿using EventManagementFPT.Model;
using EventManagementFPT.Modules.CategoryModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventManagementFPT.Modules.CategoryModule
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public ICollection<Category> GetAll()
        {
            return _categoryRepository.GetAll().ToList();
        }
        public ICollection<Category> GetCategoriesBy(Expression<Func<Category, bool>> filter = null,
            Func<IQueryable<Category>, ICollection<Category>> options = null,
            string includeProperties = null)
        {
            return _categoryRepository.GetCategoriesBy(filter);
        }
        public async Task AddNewCategory(Category newCategory)
        {
            newCategory.CategoryId = Guid.NewGuid();
            await _categoryRepository.AddAsync(newCategory);
        }
        public async Task UpdateCategory(Category categoryUpdate)
        {
            await _categoryRepository.UpdateAsync(categoryUpdate);
        }
        public async Task DeleteCategory(Guid? id)
        {
            Category categoryDelete = _categoryRepository.GetFirstOrDefaultAsync(x => x.CategoryId.Equals(id) && x.Status == true).Result;
            if (categoryDelete == null) return;
            categoryDelete.Status = false;
            await _categoryRepository.UpdateAsync(categoryDelete);
        }
        public Category GetCategoryByID(Guid? cateID)
        {
            return _categoryRepository.GetFirstOrDefaultAsync(x => x.CategoryId.Equals(cateID)).Result;
        }
    }
}
