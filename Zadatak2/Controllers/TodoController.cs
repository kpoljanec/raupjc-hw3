using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zadatak1;
using Microsoft.AspNetCore.Identity;
using Zadatak2.Models;
using Zadatak2.Models.TodoViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Zadatak2.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {

        private readonly ITodoRepository _todoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoRepository todoRepository, UserManager<ApplicationUser> signInManager)
        {
            _todoRepository = todoRepository;
            _userManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var activeItems = await _todoRepository.GetActiveAsync(new Guid(user.Id));

            IndexViewModel model = new IndexViewModel();

            foreach(TodoItem item in activeItems)
            {
                model.TodoViewModels.Add(new TodoViewModel(item));
            }

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel todoModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return Forbid("User not found.");
                }
                List<string> labels = todoModel.Labels
                    .Split(',')
                    .Select(l => l.Trim())
                    .Where(l => !string
                    .IsNullOrWhiteSpace(l))
                    .Distinct()
                    .ToList();

                List<TodoItemLabel> labelList = await _todoRepository.GetLabelsAsync(labels);
                labels.Except(labelList.Select(l => l.Value)).ToList().ForEach(l2 =>
                {
                    labelList.Add(new TodoItemLabel(l2));
                });
                TodoItem todoItem = new TodoItem(todoModel.Text, Guid.NewGuid())
                {
                    UserId = new Guid(user.Id),
                    DateDue = todoModel.DateDue,
                    Labels = labelList
                };
                await _todoRepository.AddAsync(todoItem);
                return RedirectToAction(nameof(Index));
            }
            return View(todoModel);
        }

        public async Task<IActionResult> GetCompleted()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var completedItems = await _todoRepository.GetCompletedAsync(new Guid(user.Id));

            CompletedViewModel model = new CompletedViewModel();

            foreach (TodoItem item in completedItems)
            {
                model.TodoViewModels.Add(new TodoViewModel(item));
            }

            return View(model);
        }

        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            await _todoRepository.MarkAsCompletedAsync(id, new Guid(user.Id));
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromCompleted(Guid id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            await _todoRepository.RemoveAsync(id, new Guid(user.Id));

            return RedirectToAction(nameof(GetCompleted));
        }
    }
}
