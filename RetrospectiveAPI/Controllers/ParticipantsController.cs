//using Contracts;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using RetrospectiveAPI.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace RetrospectiveAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ParticipantsController : ControllerBase
//    {
//        private readonly ICRUD _CRUD;
//        public ParticipantsController(ICRUD CRUD)
//        {
//            _CRUD = CRUD;
//        }

//        [HttpGet]
//        public IEnumerable<ParticipantModel> GetParticipants()
//        {
//            return _CRUD.LoadRecords<ParticipantModel>("Participant");
//        }

//        [HttpPost]
//        public string SaveParticipant(ParticipantModel participant)
//        {
//            return _CRUD.InsertRecord<ParticipantModel>("Participant", participant).Id.ToString();
//        }

//        [HttpPost("SaveMeeting")]
//        public void SaveMeeting(MeetingModel meeting)
//        {
//            _CRUD.InsertRecord<MeetingModel>("Meeting", meeting);
//        }

//        [HttpGet("GetParticipantById")]
//        public ParticipantModel GetParticipantById(string Id)
//        {
//            return _CRUD.LoadRecordById<ParticipantModel>("Participant", new Guid(Id));
//        }

//        [HttpDelete]
//        public void DeleteParticipant(string Id)
//        {
//            _CRUD.DeleteRecordById<ParticipantModel>("Participant", new Guid(Id));
//        }

//        [HttpPut]
//        public void Upsert(ParticipantModel participant, string Id)
//        {
//            _CRUD.Upsert<ParticipantModel>("Participant", participant, new Guid(Id));
//        }
//    }
//}
