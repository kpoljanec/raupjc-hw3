using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todo.EntityFramework.Exceptions;

namespace Zadatak1
{
    public class TodoSqlRepository : ITodoRepository
    {

        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }

        public void Add(TodoItem todoItem)
        {
            if (_context.Items.Find(todoItem.Id) != null)
            {
                throw new DuplicateTodoItemException($"duplicate id: {todoItem.Id}");
            }
            _context.Items.Add(todoItem);
            _context.SaveChanges();
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            if (await _context.Items.FirstOrDefaultAsync(t => t.Id == todoItem.Id) != null)
            {
                throw new DuplicateTodoItemException($"duplicate id: {todoItem.Id}");
            }
            _context.Items.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public TodoItem Get(Guid todoId, Guid userId)
        {
            TodoItem item = _context.Items.Find(todoId);
            if (item == null)
            {
                return null;
            }
            if (item.UserId != userId)
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
            return item;
        }

        public async Task<TodoItem> GetAsync(Guid todoId, Guid userId)
        {
            TodoItem todo = await _context.Items.FirstOrDefaultAsync(t => t.Id == todoId);
            if (todo == null)
            {
                return null;
            }
            if (todo.UserId != userId)
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
            return todo;
        }

        public List<TodoItem> GetActive(Guid userId)
        {
            return _context.Items.Where(i => !i.DateCompleted.HasValue && i.UserId == userId).ToList();
        }

        public async Task<List<TodoItem>> GetActiveAsync(Guid userId)
        {
            return await _context.Items.Where(i => !i.DateCompleted.HasValue && i.UserId==userId).ToListAsync();
        }

        public List<TodoItem> GetAll(Guid userId)
        {
            return _context.Items.Where(i => i.UserId == userId).OrderByDescending(i => i.DateCreated).ToList();
        }

        public async Task<List<TodoItem>> GetAllAsync(Guid userId)
        {
            return await _context.Items.Where(i => i.UserId == userId).OrderByDescending(i => i.DateCreated).ToListAsync();
        }

        public List<TodoItem> GetCompleted(Guid userId)
        {
            return _context.Items.Where(i => i.DateCompleted.HasValue && i.UserId == userId).ToList();
        }

        public async Task<List<TodoItem>> GetCompletedAsync(Guid userId)
        {
            return await _context.Items.Where(i => i.DateCompleted.HasValue && i.UserId == userId).ToListAsync();
        }

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return _context.Items.Where(filterFunction).Where(i => i.UserId == userId).ToList();
        }

        public async Task<List<TodoItem>> GetFilteredAsync(Expression<Func<TodoItem, bool>> filterFunction, Guid userId)
        {
            return await _context.Items.Where(filterFunction).Where(i => i.UserId == userId).ToListAsync();
        }

        public bool MarkAsCompleted(Guid todoId, Guid userId)
        {
            TodoItem item = Get(todoId, userId);
            if (item != null)
            {
                item.MarkAsCompleted();
                Update(item, item.UserId);
                return true;
            }
            else
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
        }

        public async Task<bool> MarkAsCompletedAsync(Guid todoId, Guid userId)
        {
            TodoItem item = await GetAsync(todoId, userId);
           if(item != null)
            {
                item.MarkAsCompleted();
                await UpdateAsync(item, item.UserId);
                return true;
            }
            else
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
        }

        public bool Remove(Guid todoId, Guid userId)
        {
            TodoItem item = _context.Items.Find(todoId);
            if (item == null)
                return false;
            if (item.UserId != userId)
            {
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            }
            _context.Items.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(Guid todoId, Guid userId)
        {
            TodoItem item = await _context.Items.FirstOrDefaultAsync(t => t.Id == todoId);
            if (item == null)
                return false;
            if (item.UserId != userId)
            {
                throw new TodoAccessDeniedException("Access denied. User must own this item to access it.");
            }
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public void Update(TodoItem todoItem, Guid userId)
        {
            if (Get(todoItem.Id, userId) == null)
            {
                Add(todoItem);
            }
            else if (todoItem.UserId == userId)
            {
                _context.SaveChanges();
            }
            else
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
        }

        public async Task UpdateAsync(TodoItem todoItem, Guid userId)
        {
            if (await GetAsync(todoItem.Id, userId) == null)
            {
                await AddAsync(todoItem);
            }
            else if(todoItem.UserId==userId)
            {
                _context.SaveChanges();
            }
            else
            {
                throw new TodoAccessDeniedException("Access denied.");
            }
        }

        public async Task<List<TodoItemLabel>> GetLabelsAsync(List<string> names)
        {
            return await _context.Labels.Where(l => names.Contains(l.Value)).ToListAsync();
        }

    }
}
