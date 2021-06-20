﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public JsonResult SendEmail(string Id)
        {
            try
            {
                var meetingData = _CRUD.LoadRecordById<MeetingModel>("Meeting", new Guid(Id));
                var recepientList = getRecepientList(meetingData);
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
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Point Type" + htmlTdEnd;
                messageBody += htmlTdStart + "Retro point" + htmlTdEnd;
                messageBody += htmlTdStart + "Action item" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                var flattenedList = meetingData.pointsLists.SelectMany(lp => lp.points).ToList();
                foreach (PointsModel pl in meetingData.pointsLists)
                {
                    foreach (PointModel p in pl.points)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + pl.listName + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + p.pointText + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + p.actionItem + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
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
