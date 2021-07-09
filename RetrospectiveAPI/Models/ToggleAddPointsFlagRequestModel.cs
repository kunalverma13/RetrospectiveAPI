using System.Collections.Generic;

namespace RetrospectiveAPI.Models
{
    public class ToggleAddPointsFlagRequestModel
    {
        public string meetingId { get; set; }
        public bool value { get; set; }
    }
}
