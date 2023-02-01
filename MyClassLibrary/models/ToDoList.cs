using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace function_app_entityFramework.models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; }
        [ForeignKey("DipendenteId")]
        [JsonIgnore]
        public Dipendenti Dipendente { get; set; } = null!;
        public int DipendenteId { get; set; }
    }
}
