using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zadatak1;

namespace Zadatak2.Models.TodoViewModels
{
    public class TodoViewModel
    {

        public Guid Id { get; }

        public string Text { get; set; }

        public TodoViewModel()
        {

        }

        public TodoViewModel(TodoItem item)
        {
            Id = item.Id;
            Text = item.Text;
            DateDue = item.DateDue;
            DateCompleted = item.DateCompleted;
        }


        public string DeadlineText => DateDue.HasValue ? 
            ((DateDue - DateTime.Today).Value.TotalDays > 0 ? 
            " (za " + (DateDue - DateTime.Today).Value.TotalDays + " dana!)" : ((DateTime.Today-DateDue).Value.Days > 0 ?
            " (prije " + (DateTime.Today - DateDue).Value.Days + " dana!)" : "(Danas!)")) : null;


        public DateTime? DateDue { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}
