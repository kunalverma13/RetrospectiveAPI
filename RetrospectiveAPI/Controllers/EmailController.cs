using Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RetrospectiveAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace RetrospectiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ICRUD _CRUD;

        public EmailController(ICRUD CRUD)
        {
            _CRUD = CRUD;
        }

        [HttpPost]
        public JsonResult SendEmail(object request)
        {
            try
            {
                var requestJsonString = request.ToString();
                SendEmailRequestModel sendEmailRequest = JsonConvert.DeserializeObject<SendEmailRequestModel>(requestJsonString);
                var meetingId = sendEmailRequest.meetingId;
                var recepients = sendEmailRequest.recepients;

                var meetingData = _CRUD.LoadRecordById<MeetingModel>(Constants.MEETING_TABLE_NAME, new Guid(meetingId));
                var recepientList = recepients.ToList();
                if(recepientList.Count == 0)
                {
                    recepientList = getRecepientList(meetingData);
                }
                var emailBody = getEmailBody(meetingData);

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("kvdevwork@gmail.com");
                foreach (string email in recepientList)
                {
                    message.To.Add(new MailAddress(email));
                }
                message.Subject = meetingData.meetingName;
                message.IsBodyHtml = true;
                message.Body = getEmailBody(meetingData);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("kvdevwork@gmail.com", "qdfyldmjjjairvhh");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

                return new JsonResult(true);
            }
            catch (Exception e) {
                return new JsonResult(false);
            }
        }

        private List<string> getRecepientList(MeetingModel meetingData)
        {
            return meetingData.participants.Select(p => p.participantEmail).ToList();
        }

        private string getEmailBody(MeetingModel meetingData)
        {
            try
            {
                string messageBody = "<font>The following are the actionItems: </font><br><br>";
                if (meetingData.pointsLists.Count == 0) return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTd2Start = "<td colspan=\"2\" style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Retro point" + htmlTdEnd;
                messageBody += htmlTdStart + "Action item" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                var flattenedList = meetingData.pointsLists.SelectMany(lp => lp.points).ToList();
                foreach (PointsModel pl in meetingData.pointsLists)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTd2Start + pl.listName + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                    foreach (PointModel p in pl.points)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + p.pointText + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + p.actionItem + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTd2Start + " " + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                }

                messageBody = messageBody + htmlTableEnd;
                return messageBody;
            }
            catch
            {
                return null;
            }
        }
    }
}
