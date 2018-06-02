using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estates.Controllers
{
    public class RequestsController : Controller
    {
        private Model1 db = new Model1();


      public ActionResult sendRequest()
        {
            if (Session["userID"] == null)
            {
                
                return HttpNotFound();
            }

            return View();
        }

        public ActionResult sendTeamRequest(int id)
        {
            Account account = db.Accounts.Find(id);
            if (account != null)
            {
                Request request =new Request();
                request.Sender_ID = (int) Session["userID"];
                request.reciver_id = id;
                string projectname =(string) Session["projectname"];
                var result = (from Project in db.Projects
                    where Project.subject ==projectname
                    select new
                    {
                        Project.Id



                    }).ToList();

                request.project_id = result.FirstOrDefault().Id;
                db.Requests.Add(request);
                db.SaveChanges();

                return View();

            }

            return HttpNotFound();

        }
        
        [HttpPost]
        public ActionResult sendRequest(Request request)
        {
            Account account = db.Accounts.Find(Session["projectOwner"]);
            if (account != null)
            {

     //           Project project = db.Projects.Find(Session["projectId"]);
                Project project = db.Projects.Find(Session["projectId"]);
                if (project != null)
                {
                    request.startdate = request.startdate;
                    request.enddate = request.enddate;
            //        request.startdate = request.startdate.ToString("yyyy-MM-dd");
                    request.Sender_ID = (int)Session["userID"];
                    request.reciver_id =(int) Session["projectOwner"];
             //       Account account1 = db.Accounts.Find(Session["userID"]);
                  
                    request.project_id = project.Id;
                    db.Requests.Add(request);
                    db.SaveChanges();

                    return RedirectToAction("ShowRequests");
                }
               
            }

            return HttpNotFound();
        }



        public ActionResult acceptTeam(int id,int pid)
        {
            Account account = db.Accounts.Find(id);

            if(account != null)
            {
                Project project = db.Projects.Find(pid);

                if (project != null)
                {
                    
                    Project_Team team=new Project_Team();
                    Account account1 = db.Accounts.Find(Session["userID"]);
                    if (account1.user_role == 3)
                    {   
                        team.team_leader_id = account1.Id;
                        team.project_manager_id = account.Id;
                        team.project_id =project.Id;
                        team.project_name = project.subject;
                        team.junior_engineer_id =account1.Id;
                        
                        db.Project_Team.Add(team);
                        db.SaveChanges();
                        return RedirectToAction("showTeamRequests");
                    }

                    if (account1.user_role == 4)
                    {
                        


                    }

                }


            }

            return HttpNotFound();
        }


        public ActionResult acceptManager(int id,int pid)
        {
            Account account = db.Accounts.Find(id);
            if (account != null)
            {
                Project project = db.Projects.Find(pid);
                
                if (project != null)
                {

                    project.project_manager_id = id;
                    project.project_progress = "on progress";
                    


                    var result = (from Request in db.Requests
                        where Request.project_id==pid
                        select new
                        {
                            Request.Id



                        }).ToList();



                    foreach (var item in result)
                    {
                        Request request = db.Requests.Find(item.Id);
                        db.Requests.Remove(request);
                    }
                 
                    



                    db.SaveChanges();


                    return RedirectToAction("ShowRequests");
                }

            }
            return HttpNotFound();
        }

        public ActionResult showTeamRequests()
        {
            Account account = db.Accounts.Find(Session["userID"]);

            if (account != null)
            {

                return View(db.Requests.ToList());
            }

            return HttpNotFound();
        }

        public ActionResult ShowRequests()
        {
         //   ProjectViewModel model = new ProjectViewModel();
            Account account = db.Accounts.Find(Session["userID"]);
            if (account != null)
            {
                if (account.user_role == 1)
                {
                    return View(db.Requests.ToList());
                    
                }
                else if (account.user_role == 3 || account.user_role == 4)

                {

                    return RedirectToAction("showTeamRequests");
                }
            }

            return HttpNotFound();
        }
    }
}