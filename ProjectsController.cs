using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Mvc;

namespace Estates.Controllers
{
    public class ProjectsController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult ShowProjects()
        {
            //  Project project = db.Projects.Find(Session["userID"]);
            //  Account account = db.Accounts.Find(Session["userID"]);
            //  var tuple = new Tuple<Account,Project>(account,project);

            var model = new ProjectViewModel();
            model.Projects = db.Projects.ToList();
            model.Accounts = db.Accounts.ToList();
            model.Comments = db.Comments.ToList();

            return View(model);
        }


        public ActionResult ApproveProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project != null)
            {

                project.admin_approve = true;
                db.SaveChanges();
            }

            return RedirectToAction("ShowProjects");
        }


        public ActionResult Delete(int? id)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return Content("Id Not Found @@@@");
            }
            return View(project);



        }


        public ActionResult ShowPosts(int id)
        {
            ProjectViewModel model = new ProjectViewModel();
            model.Comments = db.Comments.ToList();
            model.Projects = db.Projects.ToList();
            Project project = db.Projects.Find(id);
            if (project != null)
            {


                model.Project = project;
            }
            return View(model);
        }





        [HttpPost]

        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                Project project = db.Projects.Find(id);
                if (project != null)
                { var result = (from Comment in db.Comments
                                where Comment.p_id == id
                                select new
                                {
                                    Comment.Id



                                }).ToList();

                    foreach (var item in result)
                    {
                        Comment comment = db.Comments.Find(item.Id);
                        db.Comments.Remove(comment);
                    }

                    db.Projects.Remove(project);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ShowProjects");
        }



        public ActionResult Create()
        {

            if (Session["userID"] == null)
            {

                return HttpNotFound();
            }

            return View();
        }

        public ActionResult showHomeProjects()
        {






            return View(db.Projects.ToList());
        }








        [HttpPost]
        public ActionResult writeComment(Comment comment)
        {


            Project project = db.Projects.Find(Session["projectid"]);
            if (project != null)
            {


                comment.projectmanager_id = (int)Session["userID"];


                comment.p_id = project.Id;
                db.Comments.Add(comment);
                db.SaveChanges();

            }


            return RedirectToAction("ShowPosts", new { id = Session["projectid"] });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(Project project)
        {
            

            if (Session["userID"] != null)
            {
                if (project != null)
                {





                    project.admin_approve = false;
                    project.customer_id = (int)Session["userID"];
                    project.project_progress = "notprogressed";




                    db.Projects.Add(project);
                    db.SaveChanges();

                    return RedirectToAction("ShowProjects");
                }
            }
            return View();
        }










                
                    
                    

        public ActionResult deleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment != null)
            {

                db.Comments.Remove(comment);
                db.SaveChanges();
               
            }
            return RedirectToAction("ShowPosts", new { id = Session["projectid"] });
        }


    }
}