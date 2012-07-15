using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nokatak.Models;
using MySampleApp;
using Facebook;

namespace Nokatak.Controllers
{ 
    public class NoktaController : Controller
    {
        private NokatDB db = new NokatDB();
        private readonly FacebookClient _fb;
        private readonly IMyFacebookCanvasContext _fbCanvasContext;
        private readonly string[] ExtendedPermissions = new[] { "user_about_me", "publish_stream", "read_stream" };

        public NoktaController(FacebookClient fb, IMyFacebookCanvasContext fbCanvasContext)
        {
            _fb = fb;
            _fbCanvasContext = fbCanvasContext;
             
            
        }


        //
        // GET: /Nokta/

        public ViewResult Index(string id)
        {
            var perms = _fbCanvasContext.Permissions;
            ViewBag.perms = perms;
            if (perms == null)
            {
                ViewBag.fbLoginUrl = _fbCanvasContext.GetLoginUrl(_fbCanvasContext.GetCanvasUrl(Request) + Request.Url.AbsolutePath, ExtendedPermissions);
                dynamic r = _fb.Get("fql", new
                {
                    q = new
                    {
                        name = "SELECT name from user where uid=" + Int64.Parse(id),
                        pic = "SELECT pic_small from user where uid=" + Int64.Parse(id)
                    }

                });

                ViewBag.name = r.data[0].fql_result_set[0].name;
                ViewBag.picture = r.data[0].fql_result_set[0].pic_small;
                ViewBag.id = id;
            }
            else
            {

                dynamic result = _fb.Get("me", new { fields = new[] { "name", "picture", "id" } });

                // dynamic result1 = _fb.Get("fql", new
                //{
                //  q = new{id="SELECT name from user where uid=100000814655661"
                //    }
                //});

                ViewBag.name = result.name;
                ViewBag.picture = result.picture;
                ViewBag.id = result.id;
            }
            List<Nokta> Model = db.Noktas.ToList();


            Model.Reverse();
            List<string> pictures = new List<string>();
            List<string> users = new List<string>();
            List<List<string>> list = new List<List<string>>();
            List<int> votesup = new List<int>();
            List<int> votesdown = new List<int>();
            foreach (Nokta n in Model)
            {
                dynamic r = _fb.Get("fql", new
                {
                    q = new
                    {
                        id = "SELECT name from user where uid=" + n.userID,
                        pic_small = "SELECT pic_square from user where uid=" + n.userID
                    }

                });
                pictures.Add(r.data[0].fql_result_set[0].pic_square);
                // pictures.Add("TryUpdateModel"); 
                List<string> sublist = new List<string>();
                sublist.Add(r.data[0].fql_result_set[0].name);
                sublist.Add(n.Body);
                list.Add(sublist);

                int up = 0;
                int down = 0;
                var votes = from v in db.Votes
                            where v.NoktaID.Equals(n.ID)
                            select v;
                foreach (var v in votes)
                {
                    if (v.up)
                    {
                        up++;
                    }
                    else
                    {
                        down++;
                    }
                }
                votesup.Add(up);
                votesdown.Add(down);

            }
            ViewBag.pictures = pictures;
            ViewBag.up = votesup;
            ViewBag.down = votesdown;
            ViewBag.list = list;
            return View(Model);
        }

   

        public ViewResult IndexUser(long uid,long myid)
        {
            ViewBag.linkId = myid;
            var Model = from n in db.Noktas
                        where n.userID.Equals(uid)
                        select n;
                


            Model.Reverse();
            List<string> pictures = new List<string>();
            List<string> users = new List<string>();
            List<List<string>> list = new List<List<string>>();
            List<int> votesup = new List<int>();
            List<int> votesdown = new List<int>();
            foreach (Nokta n in Model)
            {
                dynamic r = _fb.Get("fql", new
                {
                    q = new
                    {
                        id = "SELECT name from user where uid=" + n.userID,
                        pic_small = "SELECT pic_square from user where uid=" + n.userID
                    }

                });
                pictures.Add(r.data[0].fql_result_set[0].pic_square);
                // pictures.Add("TryUpdateModel"); 
                List<string> sublist = new List<string>();
                sublist.Add(r.data[0].fql_result_set[0].name);
                sublist.Add(n.Body);
                list.Add(sublist);

                int up = 0;
                int down = 0;
              /*  var votes1 = from v in db.Votes
                            where v.NoktaID.Equals(n.ID)
                            select v;
                foreach (var v1 in votes1)
                {
                    if (v1.up)
                    {
                        up++;
                    }
                    else
                    {
                        down++;
                    }
                }*/
                votesup.Add(up);
                votesdown.Add(down);

            }
            ViewBag.pictures = pictures;
            ViewBag.up = votesup;
            ViewBag.down = votesdown;
            ViewBag.list = list;
            return View(Model);
        }

        

        public ActionResult Vote(long NoktaID,Boolean up, long userID)
        {
            Vote v = new Vote();
            v.NoktaID = NoktaID;
            v.userID = userID;
            v.up = up;
            db.Votes.Add(v);
            db.SaveChanges();


            return RedirectToRoute("Nokat", new { Controller = "Nokta", Action = "Index", id = userID });
        }
        //
        // GET: /Nokta/Details/5


        public ViewResult Details(long id)
        {
            Nokta nokta = db.Noktas.Find(id);
            return View(nokta);
        }

        //
        // GET: /Nokta/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Nokta/Create

        [HttpPost]
        public ActionResult Create(Nokta nokta, String userID)
        {
            if (ModelState.IsValid)
            {
                if (nokta.Body.Length < 200)
                {
                    long uid = Int64.Parse(userID);
                    nokta.userID = uid;
                    db.Noktas.Add(nokta);
                    db.SaveChanges();
                    return RedirectToRoute("Nokat", new { Controller = "Nokta", Action = "Index", id = nokta.userID });
                    
                    //return new EmptyResult();
                }
               // return RedirectToAction("Index");  
            }
            return RedirectToRoute("Nokat", new { Controller = "Nokta", Action = "Index", id = nokta.userID });
        }
        
        //
        // GET: /Nokta/Edit/5
 
        public ActionResult Edit(long id)
        {
            Nokta nokta = db.Noktas.Find(id);
            return View(nokta);
        }

        //
        // POST: /Nokta/Edit/5

        [HttpPost]
        public ActionResult Edit(Nokta nokta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nokta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nokta);
        }

        //
        // GET: /Nokta/Delete/5
 
        public ActionResult Delete(long id)
        {
            Nokta nokta = db.Noktas.Find(id);
            return View(nokta);
        }

        //
        // POST: /Nokta/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Nokta nokta = db.Noktas.Find(id);
            long ID1 = nokta.userID;
            db.Noktas.Remove(nokta);
            db.SaveChanges();
            return RedirectToRoute("Nokat", new { Controller = "Nokta", Action = "Index", id = ID1 });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}