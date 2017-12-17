using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadatak1
{
    /// <summary >
    /// Label describing the TodoItem .
    /// e.g. Critical , Personal , Work ...
    /// </ summary >
    public class TodoItemLabel
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        /// <summary >
        /// All TodoItems that are associated with this label
        /// </ summary >
        public List<TodoItem> LabelTodoItems { get; set; }

        public TodoItemLabel()
        {

        }

        public TodoItemLabel(string value)
        {
            Id = Guid.NewGuid();
            Value = value;
            LabelTodoItems = new List<TodoItem>();
        }

        
    }
}
