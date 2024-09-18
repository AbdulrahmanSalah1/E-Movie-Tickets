using System;

namespace eTickets.Models
{
    // Represents the model for error details in the application
    public class ErrorViewModel
    {
        // Unique identifier for the request that caused the error
        public string RequestId { get; set; }
        // Determines if the RequestId should be shown in the error view
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
