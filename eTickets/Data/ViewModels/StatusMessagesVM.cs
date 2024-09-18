using eTickets.Models;
using System.Collections.Generic;

namespace eTickets.Data.ViewModels
{
    public class StatusMessagesVM
    {
        public List<ContactUs> UnreadMessages { get; set; }
        public List<ContactUs> ReadMessages { get; set; }
    }
}
