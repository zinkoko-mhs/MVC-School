﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcSchool.Data;
using MvcSchool.Models;
using MvcSchool.Models.Domain;
using System.Collections.Immutable;
using System.Linq;
using static MvcSchool.Models.ShowEnrollmentViewModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace MvcSchool.Controllers
{
    [Authorize]
    public class SchoolController : Controller
    {
        private readonly SchoolDbContext schoolDbContext;
        public SchoolController(SchoolDbContext schoolDbContext)
        {
            this.schoolDbContext = schoolDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg, int pageSize)
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = schoolDbContext.Students.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var students = await schoolDbContext.Students.Skip(recSkip).Take(pageSize).ToListAsync();

            

            //var data = students.Skip(recSkip).Take(pageSize).ToList();
            this.ViewBag.Pager = pager;

            //.Skip((int)(CurrentPage - 1) * (int)PageSize).Take((int)PageSize)
            return View(students);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel addStudentRequest)
        {
            var student = new Student() 
            {
                StudentName= addStudentRequest.StudentName,
                DateOfBirth=addStudentRequest.DateOfBirth,
                FatherName=addStudentRequest.FatherName,
                //Class=addStudentRequest.Class,
            };
            await schoolDbContext.Students.AddAsync(student);
            await schoolDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var student = await schoolDbContext.Students.FirstOrDefaultAsync(x=> x.StudentID ==id);

            if(student != null)
            {
                var studentViewModel = new UpdateStudentViewModel()
                {
                    StudentID = student.StudentID,
                    StudentName = student.StudentName,
                    DateOfBirth = student.DateOfBirth,
                    FatherName = student.FatherName,
                    //Class = student.Class,
                };
                return await Task.Run(()=>View(("View") ,studentViewModel));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateStudentViewModel updateStudentRequest)
        {
            var student = await schoolDbContext.Students.FindAsync(updateStudentRequest.StudentID);

            if(student != null) 
            {
                student.StudentName= updateStudentRequest.StudentName;
                student.DateOfBirth= updateStudentRequest.DateOfBirth;
                student.FatherName= updateStudentRequest.FatherName;
                //student.Class = updateStudentRequest.Class;

                await schoolDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await schoolDbContext.Students.FindAsync(id);

            if(student != null) 
            {
                schoolDbContext.Students.Remove(student);
                await schoolDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddClass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddClass(AddClassViewModel addClassRequest)
        {
            var classModel = new Class()
            {
                ClassName = addClassRequest.ClassName,
                ClassFee= addClassRequest.ClassFee,
                StartDate= addClassRequest.StartDate,
                EndDate= addClassRequest.EndDate,
            };
            await schoolDbContext.AddAsync(classModel);
            await schoolDbContext.SaveChangesAsync();
            return RedirectToAction("ViewClass");
        }

        [HttpGet]
        public async Task<IActionResult> ViewClass(int pg, int pageSize) 
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = schoolDbContext.Classes.Count();

            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var classModel = await schoolDbContext.Classes.Skip(recSkip).Take(pageSize).ToListAsync();

            //var data = classModel.Skip(recSkip).Take(pageSize).ToList();
            this.ViewBag.Pager = pager;

            //.Skip((int)(CurrentPage - 1) * (int)PageSize).Take((int)PageSize)
            return View(classModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateClass(int id)
        {
            var classModel = await schoolDbContext.Classes.FirstOrDefaultAsync(x=> x.ClassID== id);

            if(classModel != null)
            {
                var classViewModel = new UpdateClassViewModel()
                {
                    ClassID = classModel.ClassID,
                    ClassName = classModel.ClassName,
                    ClassFee = classModel.ClassFee,
                    StartDate = classModel.StartDate,
                    EndDate = classModel.EndDate,
                };
                return await Task.Run(() => View(("UpdateClass"), classViewModel));
            }
            return RedirectToAction("ViewClass");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClass(UpdateClassViewModel updateClassRequest)
        {
            var classModel = await schoolDbContext.Classes.FindAsync(updateClassRequest.ClassID);

            if(classModel!= null) 
            {
                classModel.ClassName = updateClassRequest.ClassName;
                classModel.ClassFee = updateClassRequest.ClassFee;
                classModel.StartDate = updateClassRequest.StartDate;
                classModel.EndDate = updateClassRequest.EndDate;

                await schoolDbContext.SaveChangesAsync();
                return RedirectToAction("ViewClass");
            }
            return RedirectToAction("ViewClass");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classModel = await schoolDbContext.Classes.FindAsync(id);
            if(classModel!= null ) 
            {
                schoolDbContext.Classes.Remove(classModel);
                await schoolDbContext.SaveChangesAsync();

                return RedirectToAction("ViewClass");
            }
            return RedirectToAction("ViewClass");
        }

        [HttpGet]
        public IActionResult AddEnrollment()
        {            
            AddEnrollmentViewModel addEnrollmentRequest = new AddEnrollmentViewModel();
            addEnrollmentRequest.StudentName = (from s in schoolDbContext.Students select s.StudentName).Distinct().ToList();
            addEnrollmentRequest.ClassName = (from c in schoolDbContext.Classes select c.ClassName).Distinct().ToList();
            return View(addEnrollmentRequest);
        }

        [HttpPost]
        public async Task<IActionResult> AddEnrollment(AddEnrollmentViewModel addEnrollmentRequest)
        {
            string name = addEnrollmentRequest.StudentName.First();
            string className = addEnrollmentRequest.ClassName.First();

            var enrollment = new Models.Domain.Enrollment()
            {
                StudentID = (from s in schoolDbContext.Students
                             where s.StudentName == name
                             select s.StudentID).First(),
                ClassID = (from c in schoolDbContext.Classes
                           where c.ClassName == className
                           select c.ClassID).First(),
                //db.Countries.First(a => a.DOTWInternalID == citee.CountryCode).ID
                PaymentStautus = addEnrollmentRequest.PaymentStautus,
                PaymentDeadline = addEnrollmentRequest.PaymentDeadline,
                CreatedDate = addEnrollmentRequest.CreatedDate,
                CreatedBy = addEnrollmentRequest.CreatedBy,
            };

            await schoolDbContext.AddAsync(enrollment);
            await schoolDbContext.SaveChangesAsync();

            return RedirectToAction("ViewEnrollment");
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnrollment(int pg,int pageSize)
        {
            if(pageSize < 1)
            {
                pageSize = 10;
            }
            if(pg < 1 )
            {
                pg = 1;
            }
            int resCount =  (from e in schoolDbContext.Enrollmentss
                                  join s in schoolDbContext.Students on e.StudentID equals s.StudentID
                                  join c in schoolDbContext.Classes on e.ClassID equals c.ClassID
                                  orderby (e.EnrollmentID)
                                  select new ShowEnrollmentViewModel
                                  {
                                      EnrollmentID = e.EnrollmentID,
                                      StudentName = s.StudentName,
                                      ClassName = c.ClassName,
                                      PaymentDeadline = e.PaymentDeadline,
                                      PaymentStautus = e.PaymentStautus,
                                      CreatedDate = e.CreatedDate,
                                      CreatedBy = e.CreatedBy,

                                  }).Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var enrollment = await (from e in schoolDbContext.Enrollmentss
                                        join s in schoolDbContext.Students on e.StudentID equals s.StudentID
                                        join c in schoolDbContext.Classes on e.ClassID equals c.ClassID
                                        orderby (e.EnrollmentID)
                                        select new ShowEnrollmentViewModel
                                        {
                                            EnrollmentID = e.EnrollmentID,
                                            StudentName = s.StudentName,
                                            ClassName = c.ClassName,
                                            PaymentDeadline = e.PaymentDeadline,
                                            PaymentStautus = e.PaymentStautus,
                                            CreatedDate = e.CreatedDate,
                                            CreatedBy = e.CreatedBy,
                                            
                                        }).Skip(recSkip).Take(pageSize).ToListAsync();

            //var data =enrollment.Skip(recSkip).Take(pageSize).ToList();
            this.ViewBag.Pager = pager;

            //.Skip((int)(CurrentPage - 1) * (int)PageSize).Take((int)PageSize)
            return View(enrollment);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateEnrollment(int id)
        {
            var enrollment = await schoolDbContext.Enrollmentss.FirstOrDefaultAsync(x => x.EnrollmentID == id);

            if(enrollment != null)
            {
                var enrollmentModel = new UpdateEnrollmentViewModel()
                {
                    EnrollmentID = enrollment.EnrollmentID,
                    StudentID = enrollment.StudentID,
                    ClassID = enrollment.ClassID,
                    PaymentDeadline = enrollment.PaymentDeadline,
                    PaymentStautus = enrollment.PaymentStautus,
                    CreatedDate = enrollment.CreatedDate,
                    CreatedBy = enrollment.CreatedBy
                };
                return await Task.Run(() => View(("UpdateEnrollment"), enrollmentModel));
        }
            return RedirectToAction("ViewEnrollment");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEnrollment(UpdateEnrollmentViewModel updateEnrollmentRequest)
        {
            var enrollment = await schoolDbContext.Enrollmentss.FindAsync(updateEnrollmentRequest.EnrollmentID);

            if(enrollment != null) 
            {
                enrollment.EnrollmentID = updateEnrollmentRequest.EnrollmentID;
                enrollment.StudentID= updateEnrollmentRequest.StudentID;
                enrollment.ClassID= updateEnrollmentRequest.ClassID;
                enrollment.PaymentDeadline = updateEnrollmentRequest.PaymentDeadline;
                enrollment.PaymentStautus = updateEnrollmentRequest.PaymentStautus;
                enrollment.CreatedDate = updateEnrollmentRequest.CreatedDate;
                enrollment.CreatedBy = updateEnrollmentRequest.CreatedBy;

                await schoolDbContext.SaveChangesAsync();
                return RedirectToAction("ViewEnrollment");
            }
            return RedirectToAction("ViewEnrollment");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await schoolDbContext.Enrollmentss.FindAsync(id);
            
            if(enrollment != null)
            {
                schoolDbContext.Enrollmentss.Remove(enrollment);
                await schoolDbContext.SaveChangesAsync();

                return RedirectToAction("ViewEnrollment");
            }
            return RedirectToAction("ViewEnrollment");

        }

        [HttpGet]
        public async Task<IActionResult> ClassSelect(string className)
        {
            int classId = (from c in schoolDbContext.Classes
                           where c.ClassName == className
                           select c.ClassID).First();

            var students = await (from e in schoolDbContext.Enrollmentss
                            join s in schoolDbContext.Students on e.StudentID equals s.StudentID
                            where (e.ClassID == classId)
                            orderby (s.StudentID)
                            select new ClassSelectViewModel
                            {
                                StudentID = s.StudentID,
                                StudentName= s.StudentName,
                                DateOfBirth= s.DateOfBirth,
                                FatherName  = s.FatherName,
                                Class=className
                            }).ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Access");
        }
    }
}
