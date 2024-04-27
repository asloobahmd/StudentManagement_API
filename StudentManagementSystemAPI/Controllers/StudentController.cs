using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystemAPI.Data;
using StudentManagementSystemAPI.Models;
using StudentManagementSystemAPI.Models.DTO;

namespace StudentManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StudentsController(AppDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            try
            {
                var students = await _db.Students.ToListAsync();

                var studentDtos = new List<StudentDto>();

                foreach (var student in students)
                {
                    studentDtos.Add(new StudentDto
                    {
                        Id = student.Id,
                        Name = student.Name,
                        Address = student.Address,
                        Age = student.Age,
                        Course = student.Course,
                        Email = student.Email,
                    });
                }

                return Ok(studentDtos);
            }catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(StudentDto studentPayload)
        {
            try
            {
                if(studentPayload == null)
                {
                    return BadRequest();
                }

                var studentDomain = new Student
                {
                    Name = studentPayload.Name,
                    Email = studentPayload.Email,
                    Course = studentPayload.Course,
                    Address = studentPayload.Address,
                    Age = studentPayload.Age
                };

                _db.Students.Add(studentDomain);
                await _db.SaveChangesAsync();

                var studentDtoObj = new StudentDto
                {
                    Id = studentDomain.Id,
                    Name = studentDomain.Name,
                    Email = studentDomain.Email,
                    Course = studentDomain.Course,
                    Address = studentDomain.Address,
                    Age = studentDomain.Age
                };
                
                return CreatedAtAction(nameof(GetStudent), new {id = studentDtoObj.Id}, studentDtoObj);
                
            }catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest();
                }

                var studentDomain = _db.Students.Where(u => u.Id == id).FirstOrDefault();
                if (studentDomain == null)
                {
                    return NotFound();
                }

                var studentDtoObj = new StudentDto()
                {
                    Id = studentDomain.Id,
                    Address = studentDomain.Address,
                    Age= studentDomain.Age,
                    Course= studentDomain.Course,
                    Email = studentDomain.Email,
                    Name = studentDomain.Name,
                };

                return Ok(studentDtoObj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentDto studentPayload)
        {
            try
            {
                if(studentPayload == null || id != studentPayload.Id)
                {
                    return BadRequest();
                }

                var studentDomain = _db.Students.Find(id);
                if (studentDomain == null)
                {
                    return NotFound();
                }

                var newStudent = new Student()
                {
                    Id = studentDomain.Id,
                    Address = studentPayload.Address,
                    Age= studentPayload.Age,
                    Course= studentPayload.Course,
                    Email = studentPayload.Email,
                    Name = studentPayload.Name,
                };

                _db.Entry(studentDomain).CurrentValues.SetValues(newStudent);
                await _db.SaveChangesAsync();

                return NoContent();
            }   
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var student = _db.Students.Where(u => u.Id == id).FirstOrDefault();
                if (student == null)
                {
                    return NotFound();
                }

                _db.Students.Remove(student);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
