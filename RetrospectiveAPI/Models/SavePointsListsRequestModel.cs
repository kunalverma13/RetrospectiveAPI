using System.Collections.Generic;

namespace RetrospectiveAPI.Models
{
    public class SavePointsListsRequestModel
    {
        public string meetingId { get; set; }
        public List<PointsList> listOfPointLists { get; set; }
    }

    public class PointsList
    {
        public string listName;
        public List<Point> points;
    }

    public class Point
    {
        public string participantName;
        public string pointText;
    }
}
