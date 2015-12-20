﻿using LawyersAdda.Entities;
using LawyersAdda.Models;
using LawyersAdda.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace LawyersAdda.Controllers
{
    public class LawyersController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Lawyers
        public ActionResult Index()
        {
            return View();
        }

        // GET: Lawyers/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Lawyers/Create
        public ActionResult RegisterAsLawyer()
        {
            var cityList = new CitiesController().GetCities();
            ViewBag.city = cityList;
            return View();
        }

        // POST: Lawyers/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsLawyer(LawyerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, FullName = model.FullName, Email = model.Email, PhoneNumber = model.PhoneNumber };
                //normal user registeration. Hence islawyer is set to false
                user.isLawyer = true;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    Lawyer lawyerToAdd = new Lawyer();
                    lawyerToAdd.Email = model.Email;
                    lawyerToAdd.Name = model.FullName;
                    lawyerToAdd.PhoneNumber = model.PhoneNumber;
                    lawyerToAdd.Bio = model.Bio;
                    lawyerToAdd.AlternatePhoneNumber = model.AlternatePhoneNumber;
                    lawyerToAdd.Sex = "Male";
                    lawyerToAdd.CreatedBy = "DJ";
                    lawyerToAdd.ModifiedBy = "DJ";
                    lawyerToAdd.CreatedDate = DateTime.Now;
                    lawyerToAdd.ModifiedDate = DateTime.Now;
                    lawyerToAdd.WebSiteUrl = model.BlogUrl;
                    lawyerToAdd.Id = user.Id;
                    lawyerToAdd.City = model.City;
                    // lawyerToAdd.User = user;
                    try
                    {
                        ApplicationDbContext c = new ApplicationDbContext();
                        c.Lawyers.Add(lawyerToAdd);
                        c.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            var str = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                var a = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                       ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                    }

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                }
                //TempData["LName"] = model.FullName;
                //TempData["LEmail"] = model.Email;
                //TempData["LPhoneNumber"] = model.PhoneNumber;
                //TempData["LUser"] = user;
                TempData["LUserId"] = user.Id;
                TempData["Lcity"] = model.City;
                TempData.Keep();
                return RedirectToAction("AddCourtToLawyer");
            }
            return View(model);
            //  AddErrors(result);
        }

        // GET: Lawyers/Create
        public ActionResult AddCourtToLawyer()
        {
         //   var cid = TempData["Lcity"].ToString();
           // TempData.Keep();
          //  ApplicationDbContext db = new ApplicationDbContext();
           // var ListofCities = (from r in db.Cities select r);
          //  var ListOfCourts = (from r in db.Courts where r.CityId == cid select r);
          //  ViewBag.cities = ListofCities;
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourtToLawyer(List<string> assignedCourts)
        {
            assignedCourts.Add("1");
            assignedCourts.Add("2");
            try
            {
                ApplicationDbContext c = new ApplicationDbContext();
                foreach(var assignedCourt in assignedCourts)
                {
                    var court = new Court { Id = assignedCourt };
                    Lawyer l = new Lawyer();
                    l.Courts.Add(court);
                    c.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var r = ex.InnerException.Message;

            }
            return RedirectToAction("Index", "Home");

        }



        //Fetching Cities as JSON
        public JsonResult GetCities()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var cities = from r in context.Cities select r;
            return Json(cities, JsonRequestBehavior.AllowGet);
          
        }

        // Fetching Courts of a city as JSON
        public JsonResult GetCourts(string cityId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var courts = from r in context.Courts where r.CityId==cityId select r;
            return Json(courts, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult AddCourtsToLawyers(List<>)


        // GET: Lawyers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Lawyers/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Lawyers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Lawyers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
    }
}
