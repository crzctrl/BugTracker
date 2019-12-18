﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper tHelp = new TicketHelper();
        private NotificationHelper nHelp = new NotificationHelper();

        // GET: TicketAttachments
        //public ActionResult Index()
        //{
        //    //var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
        //    //return View(ticketAttachments.ToList());
        //    return View(tHelp.ListMyAttachments());
        //}

        // GET: TicketAttachments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketAttachment);
        //}

        // GET: TicketAttachments/Create
        //public ActionResult Create(int id)
        //{
        //    var attachment = new TicketAttachment
        //    {
        //        TicketId = id
        //    };
            
        //    return View(attachment);
        //}

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId")] TicketAttachment ticketAttachment, HttpPostedFileBase attach, string aDescription)
        {
            if (ModelState.IsValid)
            {               
                if (FileUploadValidator.IsWebFriendlyImage(attach) || FileUploadValidator.IsWebFriendlyFile(attach))
                {
                    var fileName = Path.GetFileName(attach.FileName);
                    var justFileName = Path.GetFileNameWithoutExtension(fileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    fileName = $"{justFileName}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                    attach.SaveAs(Path.Combine(Server.MapPath("~/Attachments"), fileName));
                    
                    ticketAttachment.FilePath = "/Attachments/" + fileName;
                    ticketAttachment.Created = DateTime.Now;
                    ticketAttachment.UserId = User.Identity.GetUserId();
                    ticketAttachment.Description = aDescription;
                    nHelp.AddAttachmentNotification(ticketAttachment);
                    db.TicketAttachments.Add(ticketAttachment);
                    db.SaveChanges();

                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }
            }

            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
        //    return View(ticketAttachment);
        //}

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,FilePath,Description,Created,Updated")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        //[Authorize(Roles = "Admin, Project_Manager, Developer, Submitter")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketAttachment);
        //}

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
