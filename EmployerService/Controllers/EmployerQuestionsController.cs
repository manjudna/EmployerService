using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EmployerService.Models;

namespace EmployerService.Controllers
{
    public class EmployerQuestionsController : ApiController
    {
        private AssessmentEntities db = new AssessmentEntities();

        // GET: api/EmployerQuestions
        public IQueryable<EmployerQuestion> GetEmployerQuestions()
        {
            return db.EmployerQuestions;
        }

        // GET: api/EmployerQuestions/5
        [ResponseType(typeof(EmployerQuestion))]
        public IHttpActionResult GetEmployerQuestion(int id)
        {
            EmployerQuestion employerQuestion = db.EmployerQuestions.Find(id);
            if (employerQuestion == null)
            {
                return NotFound();
            }

            return Ok(employerQuestion);
        }

        // PUT: api/EmployerQuestions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployerQuestion(int id, EmployerQuestion employerQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employerQuestion.Id)
            {
                return BadRequest();
            }

            db.Entry(employerQuestion).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployerQuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmployerQuestions
        [ResponseType(typeof(EmployerQuestion))]
        public IHttpActionResult PostEmployerQuestion(EmployerQuestion employerQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EmployerQuestions.Add(employerQuestion);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EmployerQuestionExists(employerQuestion.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = employerQuestion.Id }, employerQuestion);
        }

        // DELETE: api/EmployerQuestions/5
        [ResponseType(typeof(EmployerQuestion))]
        public IHttpActionResult DeleteEmployerQuestion(int id)
        {
            EmployerQuestion employerQuestion = db.EmployerQuestions.Find(id);
            if (employerQuestion == null)
            {
                return NotFound();
            }

            db.EmployerQuestions.Remove(employerQuestion);
            db.SaveChanges();

            return Ok(employerQuestion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployerQuestionExists(int id)
        {
            return db.EmployerQuestions.Count(e => e.Id == id) > 0;
        }
    }
}