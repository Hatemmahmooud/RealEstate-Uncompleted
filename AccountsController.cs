using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Estates.Models;
using System.Runtime.InteropServices;

namespace Estates.Controllers
{
    public class AccountsController : Controller
    { 
        private AccountDBContext db1 = new AccountDBContext();
        private Model1 db = new Model1();



        



        public ActionResult ShowUsers()
        {
           
            
            return View(db.Accounts.ToList());
        }
        public ActionResult Register()
        {


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Register(Account account, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                
                byte[] bytes;

                int BytestoRead;

                int numBytesRead;

                if (file != null)

                {



                    account.photo = new byte [file.ContentLength];
                    file.InputStream.Read(account.photo,0,file.ContentLength);
                 

                }
                
                
            }


            try
            {

                if (account.job_description == "Customer")
                {
                    account.user_role = 1;
                }
                else
                if (account.job_description == "Team Leader")
                {
                    account.user_role = 3;
                }
                if (account.job_description == "Project Manager")
                {
                    account.user_role = 2;
                }
                if (account.job_description == "Junior Engineer")
                {
                    account.user_role = 4;
                }

                db.Accounts.Add(account);
                db.SaveChanges();

                return RedirectToAction("Welcome");
            }
            


                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

        


            return View(account);
        }

        public ActionResult Login()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account account)
        { 

            var result = (from Account in db.Accounts
                where Account.email == account.email && Account.password == account.password
                select new
                {   Account.Id,
                    Account.email,
                    Account.password,
                    Account.first_name,
                    Account.last_name,
                    Account.age,
                    Account.photo,
                    Account.username,
                    Account.mobile,
                    Account.user_role,
                    
                    

                }).ToList();

            

            if (result.FirstOrDefault() != null)
            {
                Session["userID"] = result.FirstOrDefault().Id;
                Session["email"] = result.FirstOrDefault().email;
                Session["password"] = result.FirstOrDefault().password;
                Session["firstname"] = result.FirstOrDefault().first_name;
                Session["lastname"] = result.FirstOrDefault().last_name;
                Session["age"] = result.FirstOrDefault().age;
                Session["mobile"] = result.FirstOrDefault().mobile;
                Session["username"] = result.FirstOrDefault().username;
                Session["user_role"] = result.FirstOrDefault().user_role;
                Session["photo"] = result.FirstOrDefault().photo;
                
              

               

                return RedirectToAction("Welcome", "Accounts");

            }

            else
            {
                ModelState.AddModelError("", "Wrong Username or Password yala !");
            }

            return View(account);
        }


        public ActionResult Edit()
        {
            if (Session["userID"] == null)
            {

                return HttpNotFound();

            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {  
                   
                   Session["username"] = account.username;
                   Session["password"] = account.password;
                   Session["firstname"] = account.first_name;
                   Session["lastname"] = account.last_name;

              
                Session["email"] = account.email;

                Session["age"] = account.age;
                Session["mobile"] = account.mobile;


                var ins= db.Accounts.Find(Session["userID"]);
           
                ins.username = account.username;
                ins.password = account.password;
                ins.first_name = account.first_name;
                ins.last_name = account.last_name;
                ins.email = account.email;
                ins.age = account.age;
                ins.mobile = account.mobile;
                ins.job_description = account.job_description;

                if (account.job_description == "Customer")
                {
                    ins.user_role=account.user_role = 1;
                }
                else
                if (account.job_description == "Team Leader")
                {
                    ins.user_role=account.user_role = 3;
                }
                if (account.job_description == "Project Manager")
                {
                    ins.user_role=account.user_role = 2;
                }
                if (account.job_description == "Junior Engineer")
                {
                    ins.user_role=account.user_role = 4;
                }
                string filename = "";

                byte[] bytes;

                int BytestoRead;

                int numBytesRead;

                if (file != null)

                {



                    account.photo = new byte[file.ContentLength];
                    file.InputStream.Read(account.photo, 0, file.ContentLength);


                }


                ins.photo = account.photo;
                db.SaveChanges();

                return RedirectToAction("ShowUsers");
            }

            return View(account);
        }

        public ActionResult search()
        {


            return View(db.Accounts.ToList());
        }

        [HttpPost]
        public ActionResult search(string search,string project)
        {
            Session["searchuser"] =search;
            Session["projectname"]=project;
            
            
            return View(db.Accounts.ToList());
            
        }

       


        public ActionResult Welcome()
        {
            ProjectViewModel model=new ProjectViewModel();


            model.Projects = db.Projects.ToList();
            Account account = db.Accounts.Find(Session["userID"]);
            if (account != null)
            {

                if (account.photo !=null)
                    Session["photo"] = account.photo;
            }

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
          
            Account account = db.Accounts.Find(id);
            if (account== null)
            {
                return Content("Id Not Found @@@@");
            }
            return View(account);

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                Account account = db.Accounts.Find(id);
                db.Accounts.Remove(account);
                db.SaveChanges();
                return RedirectToAction("ShowUsers");

            }

            return View(id);
        }

        


    }
}