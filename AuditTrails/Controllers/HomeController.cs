﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuditTrails.Models;
using PagedList;

namespace AuditTrails.Controllers
{
    public class HomeController : Controller
    {
        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        [Audit(AuditingLevel = 3)]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Audit(AuditingLevel = 3)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateFoo()
        {
            return View();
        }

        [HttpPost]
        [Audit(AuditingLevel = 3)]
        public ActionResult CreateFoo(Foo foo)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            db.Foos.Add(foo);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AuditTrails(int ? page)
        {
            var db = new ApplicationDbContext();

            var pageNumber = page ?? 1;

            var audits = db.AuditRecords.OrderByDescending(x => x.Id).ToPagedList(pageNumber, 20);

            return View(audits);
        }

    }
}