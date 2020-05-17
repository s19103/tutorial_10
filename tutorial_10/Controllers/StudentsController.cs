using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tutorial_10.Entities;
using tutorial_10.Models;

namespace tutorial_10.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _studentContext;

        public StudentsController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = _studentContext.Student
                                          .Include(s => s.IdEnrollmentNavigation)
                                          .ThenInclude(s => s.IdStudyNavigation)
                                          .Select(st => new 
                                          {
                                              IndexNumber = st.IndexNumber,
                                              FirstName = st.FirstName,
                                              LastName = st.LastName,
                                              BirthDate = st.BirthDate.ToShortDateString(),
                                              IdEnrol = st.IdEnrollment,
                                              Semester = st.IdEnrollmentNavigation.Semester,
                                              Studies = st.IdEnrollmentNavigation.IdStudyNavigation.Name
                                          }).ToList();

            return Ok(students);
        }

        [HttpPost("insert")]
        public IActionResult InsertStudent(InsertStudent insertStudent)
        {
            var st = new Student
            {
                IndexNumber = insertStudent.IndexNumber,
                FirstName = insertStudent.FirstName,
                LastName = insertStudent.LastName,
                BirthDate = insertStudent.BirthDate,
                IdEnrollment = insertStudent.IdEnrollment
            };

            _studentContext.Student.Add(st);
            _studentContext.SaveChanges();

            return Ok("New student added to database!");
        }

        [HttpPost("modify")]
        public IActionResult ModifyStudent(Student student)
        {
            _studentContext.Attach(student);
            _studentContext.SaveChanges();

            return Ok("Hello");
        }

        [HttpGet("delete")]
        public IActionResult DeleteStudent(Student student)
        {
            _studentContext.Attach(student);
            _studentContext.Remove(student);
            _studentContext.SaveChanges();

            return Ok("Student removed!");
        }




    }
}