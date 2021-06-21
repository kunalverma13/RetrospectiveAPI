using Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RetrospectiveAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetrospectiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly ICRUD _CRUD;
        public MeetingController(ICRUD CRUD)
        {
            _CRUD = CRUD;
        }

        [HttpPost("SaveMeeting")]
        public JsonResult SaveMeeting(MeetingModel meeting)
        {
            try
            {
                var Id = _CRUD.InsertRecord<MeetingModel>("Meeting", meeting).id.ToString();
                return new JsonResult(Id);
            }
            catch
            {
                return new JsonResult("");
            }
        }

        [HttpPost("SaveParticipant")]
        public JsonResult SaveParticipant(SaveParticipantRequestModel saveParticipantRequest)
        {
            try
            {
                var meeting = _CRUD.LoadRecordById<MeetingModel>("Meeting", new Guid(saveParticipantRequest.meetingId));
                if (meeting.participants == null)
                {
                    meeting.participants = new List<ParticipantModel>();
                }
                meeting.participants.Add(new ParticipantModel() { 
                    id = meeting.participants.Count + 1, 
                    participantName = saveParticipantRequest.participantName,
                    participantEmail = saveParticipantRequest.participantEmail
                });

                var Id = _CRUD.Upsert<MeetingModel>("Meeting", meeting, meeting.id).id.ToString();
                return new JsonResult(Id);
            }
            catch (Exception e)
            {
                return new JsonResult("");
            }
        }

        [HttpPost("SavePointsLists")]
        public JsonResult SavePointsLists(object savePointsListsRequest)
        {
            try
            {
                var requestJsonString = savePointsListsRequest.ToString();
                SavePointsListsRequestModel requestModel = JsonConvert.DeserializeObject<SavePointsListsRequestModel>(requestJsonString);

                var meeting = _CRUD.LoadRecordById<MeetingModel>("Meeting", new Guid(requestModel.meetingId));
                if (meeting.pointsLists == null)
                {
                    meeting.pointsLists = new List<PointsModel>();
                }
                foreach (PointsList pl in requestModel.listOfPointLists)
                {
                    if (meeting.pointsLists.FindIndex(x => x.listName == pl.listName) == -1)
                    {
                        meeting.pointsLists.Add(
                            new PointsModel()
                            {
                                id = meeting.pointsLists.Count + 1,
                                listName = pl.listName,
                                points = pl.points.Select(
                                    (p, i) => new PointModel() { id = i + 1, participantName = p.participantName, pointText = p.pointText }
                                    ).ToList()
                            });
                    }
                    else
                    {
                        meeting.pointsLists.Find(x => x.listName == pl.listName).points
                            .AddRange(pl.points.Select(
                                    (p, i) => new PointModel() { id = i + 1, participantName = p.participantName, pointText = p.pointText }
                                    ).ToList());
                    }
                }

                _CRUD.Upsert<MeetingModel>("Meeting", meeting, meeting.id).id.ToString();
                return new JsonResult(true);
            }
            catch (Exception e)
            {
                return new JsonResult(false);
            }
        }

        [HttpDelete]
        public void DeleteParticipant(string Id)
        {
            _CRUD.DeleteRecordById<ParticipantModel>("Participant", new Guid(Id));
        }

        [HttpGet("GetMeetingData")]
        public MeetingModel GetMeetingData(string Id)
        {
            return _CRUD.LoadRecordById<MeetingModel>("Meeting", new Guid(Id));
        }

        [HttpPost("SaveActionItem")]
        public JsonResult SaveActionItem(SaveActionItemRequestModel saveActionItemRequest)
        {
            var Id = "";
            try
            {
                var meeting = _CRUD.LoadRecordById<MeetingModel>("Meeting", new Guid(saveActionItemRequest.meetingId));
                var pointsList = meeting.pointsLists.Find(pl => pl.id == saveActionItemRequest.listId);
                if(pointsList != null)
                {
                    var point = pointsList.points.Find(p => p.id == saveActionItemRequest.pointId);
                    if(point != null)
                    {
                        point.actionItem = saveActionItemRequest.actionItem;
                    }
                }

                Id = _CRUD.Upsert<MeetingModel>("Meeting", meeting, meeting.id).id.ToString();
                return new JsonResult(Id);
            }
            catch (Exception e)
            {
                return new JsonResult("");
            }
        }
    }
}
