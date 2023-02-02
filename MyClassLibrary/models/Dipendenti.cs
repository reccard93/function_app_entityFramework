using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary.models
{
    public class Dipendenti
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ToDoList> Tasks { get; set; } = new List<ToDoList>();
    }
}
