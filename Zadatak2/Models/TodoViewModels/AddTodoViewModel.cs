using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zadatak2.Models.TodoViewModels
{
    public class AddTodoViewModel
    {
        [Required]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy", ConvertEmptyStringToNull = true)]
        public DateTime? DateDue { get; set; }

        public string Labels { get; set; }

    }
}
