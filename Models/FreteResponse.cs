using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaulaPresentesWebMVC.Models
{
    public class FreteResponse
    {
        public string name { get; set; }

        public string price { get; set; }

        public int delivery_time { get; set; }

    }
}