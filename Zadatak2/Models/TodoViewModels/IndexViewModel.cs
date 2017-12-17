using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zadatak2.Models.TodoViewModels
{
    public class IndexViewModel
    {
        public List<TodoViewModel> TodoViewModels { get; set; } = new List<TodoViewModel>();
        public string Title { get; set; }
    }
}
