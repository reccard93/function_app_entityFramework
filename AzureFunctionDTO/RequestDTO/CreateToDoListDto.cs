using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionDTO.RequestDTO
{
    public class CreateToDoListDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int DipendenteId { get; set; }
    }
}
