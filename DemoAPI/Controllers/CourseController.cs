﻿using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public CourseController(StudentDbContext context)
        {
            _context = context;
        }
        private async void LoadCoursetData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/CourseInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var courseList = JsonConvert.DeserializeObject<List<Course>>(jsonData);

            foreach (var course in courseList)
            {
                _context.Courses.Add(course);
            }

            await _context.SaveChangesAsync();
        }
        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            List<Course> courseData = await _context.Courses.ToListAsync();
            if (courseData.Count() == 0)
            {
                LoadCoursetData();
                courseData = await _context.Courses.ToListAsync();
            }
            return await _context.Courses.ToListAsync();
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course Courses)
        {
            if (id != Courses.Id)
            {
                return BadRequest();
            }

            if (Courses is null)
            {
                throw new ArgumentNullException(nameof(Courses));
            }

            _context.Entry(Courses).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> Post(Course Courses)
        {
            _context.Courses.Add(Courses);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseExists(Courses.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourse", new { id = Courses.Id }, Courses);
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var Course = await _context.Courses.FindAsync(id);
            if (Course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(Course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: credit find
        [HttpGet("findByCredit")]
        public async Task<ActionResult<IEnumerable<Course>>> findByCredit(int credit)
        {
            var allCourse = await _context.Courses.ToListAsync();
            var selectedCourse = allCourse.FindAll(s => s.Credit > credit);
            return selectedCourse;

        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}