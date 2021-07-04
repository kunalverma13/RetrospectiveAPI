using System.Collections.Generic;

namespace RetrospectiveAPI.Models
{
    public class SendEmailRequestModel
    {
        public string meetingId { get; set; }
        public string[] recepients { get; set; }
    }
}
