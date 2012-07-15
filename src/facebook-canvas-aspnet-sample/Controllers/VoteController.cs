using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nokatak.Models;

namespace Nokatak.Controllers
{ 
    public class VoteController : Controller
    {
        private NokatDB db = new NokatDB();

        //
        // GET: /Vote/

        public ViewResult Index()
        {
            var votes = db.Votes.Include(v => v.nokta);
            return View(votes.ToList());
        }

        //
        // GET: /Vote/Details/5

        public ViewResult Details(long id)
        {
            Vote vote = db.Votes.Find(id);
            return View(vote);
        }

        //
        // GET: /Vote/Create

        public ActionResult Create()
        {
            ViewBag.NoktaID = new SelectList(db.Noktas, "ID", "Body");
            return View();
        } 

        //
        // POST: /Vote/Create

        [HttpPost]
        public ActionResult Create(Vote vote)
        {
            if (ModelState.IsValid)
            {
                db.Votes.Add(vote);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.NoktaID = new SelectList(db.Noktas, "ID", "Body", vote.NoktaID);
            return View(vote);
        }
        
        //
        // GET: /Vote/Edit/5
 
        public ActionResult Edit(long id)
        {
            Vote vote = db.Votes.Find(id);
            ViewBag.NoktaID = new SelectList(db.Noktas, "ID", "Body", vote.NoktaID);
            return View(vote);
        }

        //
        // POST: /Vote/Edit/5

        [HttpPost]
        public ActionResult Edit(Vote vote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NoktaID = new SelectList(db.Noktas, "ID", "Body", vote.NoktaID);
            return View(vote);
        }

        //
        // GET: /Vote/Delete/5
 
        public ActionResult Delete(long id)
        {
            Vote vote = db.Votes.Find(id);
            return View(vote);
        }

        //
        // POST: /Vote/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Vote vote = db.Votes.Find(id);
            db.Votes.Remove(vote);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}