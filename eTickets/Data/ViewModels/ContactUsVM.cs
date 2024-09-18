using System.ComponentModel.DataAnnotations;

namespace eTickets.Data.ViewModels
{
    public class ContactUsVM
    {
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Message { get; set; }


    }
}
