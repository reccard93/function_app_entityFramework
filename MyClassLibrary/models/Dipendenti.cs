using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace function_app_entityFramework.models
{
    public class Dipendenti
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ToDoList> Tasks { get; set; } = new List<ToDoList>();
    }
}
