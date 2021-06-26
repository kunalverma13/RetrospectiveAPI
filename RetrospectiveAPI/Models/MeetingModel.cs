using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetrospectiveAPI.Models
{
    public class MeetingModel
    {
        [BsonId]
        public Guid id { get; set; }
        public string meetingName { get; set; }
        public List<ParticipantModel> participants { get; set; }
        public List<PointsModel> pointsLists { get; set; }
        public DateTime meetingCreationDate { get; set; }
        public bool canAddPoints { get; set; }
    }

    public class ParticipantModel
    {
        public int id { get; set; }
        public string participantName { get; set; }
        public string participantEmail { get; set; }
    }

    public class PointsModel
    {
        public int id { get; set; }
        public string listName { get; set; }
        public List<PointModel> points { get; set; }
    }

    public class PointModel
    {
        public int id { get; set; }
        public string participantName { get; set; }
        public int participantId { get; set; }
        public string pointText { get; set; }
        public string actionItem { get; set; }
    }
}
