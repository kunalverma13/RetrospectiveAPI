using System.Collections.Generic;

namespace RetrospectiveAPI.Models
{
    public class SaveActionItemRequestModel
    {
        public string meetingId { get; set; }
        public int listId { get; set; }
        public int pointId { get; set; }
        public string actionItem { get; set; }
    }
}
