using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetrospectiveAPI.Models
{
    public class SaveParticipantRequestModel
    {
        public string meetingId { get; set; }
        public string participantName { get; set; }
        public string participantEmail { get; set; }
    }
}
