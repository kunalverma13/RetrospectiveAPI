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
                meeting.meetingCreationDate = DateTime.Now;
                var Id = _CRUD.InsertRecord<MeetingModel>(Constants.MEETING_TABLE_NAME, meeting).id.ToString();
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
                var meeting = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(saveParticipantRequest.meetingId));
                if (meeting.participants == null)
                {
                    meeting.participants = new List<ParticipantModel>();
                }
                var participantId = meeting.participants.Count + 1;
                meeting.participants.Add(new ParticipantModel() { 
                    id = participantId, 
                    participantName = saveParticipantRequest.participantName,
                    participantEmail = saveParticipantRequest.participantEmail
                });

                _CRUD.Upsert<MeetingModel>(Constants.MEETING_TABLE_NAME, meeting, meeting.id).id.ToString();
                return new JsonResult(participantId);
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

                var meeting = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(requestModel.meetingId));
                if (!meeting.canAddPoints)
                {
                    return new JsonResult(new { saved = false, error = "Adding points is not allowed now." });
                }
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
                                    (p, i) => new PointModel() { 
                                        id = i + 1, 
                                        participantName = p.participantName, 
                                        pointText = p.pointText, 
                                        participantId = p.participantId 
                                    }).ToList()
                            });
                    }
                    else
                    {
                        var points = meeting.pointsLists.Find(x => x.listName == pl.listName).points;
                            points.AddRange(pl.points.Select(
                                    (p, i) => new PointModel() { 
                                        id = points.Count() + i + 1, 
                                        participantName = p.participantName, 
                                        pointText = p.pointText, 
                                        participantId = p.participantId 
                                    }).ToList());
                    }
                }

                _CRUD.Upsert<MeetingModel>(Constants.MEETING_TABLE_NAME, meeting, meeting.id).id.ToString();
                return new JsonResult(new { saved = true, error = "" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { saved = false, error = e.Message });
            }
        }

        //[HttpDelete]
        //public void DeleteMeeting(string Id)
        //{
        //    _CRUD.DeleteRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(Id));
        //}

        [HttpGet("GetMeetingData")]
        public MeetingModel GetMeetingData(string Id)
        {
            return _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(Id));
        }

        [HttpPost("SaveActionItem")]
        public JsonResult SaveActionItem(SaveActionItemRequestModel saveActionItemRequest)
        {
            var Id = "";
            try
            {
                var meeting = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(saveActionItemRequest.meetingId));
                var pointsList = meeting.pointsLists.Find(pl => pl.id == saveActionItemRequest.listId);
                if(pointsList != null)
                {
                    var point = pointsList.points.Find(p => p.id == saveActionItemRequest.pointId);
                    if(point != null)
                    {
                        point.actionItem = saveActionItemRequest.actionItem;
                    }
                }

                Id = _CRUD.Upsert<MeetingModel>(Constants.MEETING_TABLE_NAME, meeting, meeting.id).id.ToString();
                return new JsonResult(Id);
            }
            catch (Exception e)
            {
                return new JsonResult("");
            }
        }

        [HttpGet("GetMeetingPointsByParticipant")]
        public JsonResult GetMeetingPointsByParticipant(string meetingId, int participantId)
        {
            try
            {
                var meeting = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(meetingId));
                var points = meeting.pointsLists.SelectMany(p => p.points.FindAll(pt => pt.participantId == participantId));
                return new JsonResult(points);
            } catch (Exception e)
            {
                return new JsonResult("");
            }
            
        }

        [HttpPost("ToggleAddPointsFlag")]
        public JsonResult ToggleAddPointsFlag(object toggleAddPointsFlagRequest)
        {
            var response = "";
            try
            {
                var requestJsonString = toggleAddPointsFlagRequest.ToString();
                var request = JsonConvert.DeserializeObject<ToggleAddPointsFlagRequestModel>(requestJsonString);

                var meeting = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(request.meetingId));
                meeting.canAddPoints = request.value;
                response = _CRUD.Upsert<MeetingModel>(Constants.MEETING_TABLE_NAME, meeting, meeting.id).id.ToString();
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                return new JsonResult("");
            }
        }

        [HttpGet("GetMeetings")]
        public JsonResult GetMeetings()
        {
            var meetings = _CRUD.LoadRecords<MeetingModel>(Constants.MEETING_TABLE_NAME);
            var response = meetings.Select(m => new { meetingId = m.id, meetingName = m.meetingName, meetingDate = m.meetingCreationDate });
            return new JsonResult(response);
        }
    }
}
