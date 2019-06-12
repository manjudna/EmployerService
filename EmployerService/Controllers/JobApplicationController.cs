using EmployerService.Models;
using JobApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployerService.Controllers
{
    public class JobApplicationController : ApiController
    {
        private AssessmentEntities db = new AssessmentEntities();
        
        /// <summary>
        /// Validates the Job Application against the questions
        /// </summary>
        /// <param name="applicantQualification"></param>
        /// <returns></returns>
        //[Route("api/ValidateApplication")]
        //[HttpPost]
        //public ApplicantQualification ValidateApplication(ApplicantQualification applicantQualification)
        //{
        //    //check request for null         
        //    if (applicantQualification == null)
        //    {
        //        var badRequest = new HttpResponseMessage(HttpStatusCode.BadRequest)
        //        {
        //            Content = new StringContent("No application data provided to validate.")
        //        };
        //        throw new HttpResponseException(badRequest);
        //    }
            
        //    //iterate thru jo application and check if appliction if valid?
        //    foreach (var appQuestion in applicantQualification.ApplicantQuestions)
        //    {
        //        var ans = db.EmployerQuestions.SingleOrDefault(q => q.Id == appQuestion.Id);

        //        if(ans==null)
        //        {
        //            var badRequest = new HttpResponseMessage(HttpStatusCode.NotFound)
        //            {
        //                Content = new StringContent("Invalid question id from the application, could not find the Answer for the question Id:"+ appQuestion.Id)
        //            };
        //            throw new HttpResponseException(badRequest);
        //        }

        //        bool isAnsAcceptable = IsAnswerAcceptable(appQuestion.Answer, ans.Answer);
        //        if (!isAnsAcceptable)//throws the exception even if one question is not answered and not acceptable answer input from job application
        //        {
        //            var badRequest = new HttpResponseMessage(HttpStatusCode.BadRequest)
        //            {
        //                Content = new StringContent("Application is Invalid or Unacceptable Answer.")
        //            };
                  
        //            throw new HttpResponseException(badRequest);                   
        //        }
        //    }

        //    return applicantQualification;
        //}

        /// <summary>
        /// Accepts the answer from application and real answer from employer and validates
        /// </summary>
        /// <param name="appAns"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        /// This is implemenated using specification pattern
        private bool IsAnswerAcceptable(string appAns,string answer)
        {
            int score = Compute(appAns, answer);

            //currently score is set as 5, we can increase the value if we want more felxibility in our answer/response.
            bool isAnsOk = !string.IsNullOrWhiteSpace(appAns.ToUpper()) && score <= 5;

            return isAnsOk;
        }

        /// <summary>
        /// Returns the score of the answers, high the score answer is not good , 0 is exact match to the answer
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        /// <summary>
        /// Validates the Job Application against the questions
        /// </summary>
        /// <param name="applicantQualification"></param>
        /// <returns></returns>
        [Route("api/ValidateApplications")]
        [HttpPost]
        public List<ApplicantQualification> ValidateApplications(List<ApplicantQualification> applicantQualifications)
        {
            List<ApplicantQualification> validApplications = null;
            //check request for null         
            if (applicantQualifications == null)
            {
                var badRequest = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("No application data provided to validate.")
                };
                throw new HttpResponseException(badRequest);
            }

            foreach (var applicantQualification in applicantQualifications)
            {
                bool isValidAns = false;
                //iterate thru job application and check if appliction if valid?
                foreach (var appQuestion in applicantQualification.ApplicantQuestions)
                {
                    var ans = db.EmployerQuestions.SingleOrDefault(q => q.Id == appQuestion.Id);

                    if (ans == null)
                    {
                        var badRequest = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("Invalid question id from the application, could not find the Answer for the question Id:" + appQuestion.Id)
                        };
                        throw new HttpResponseException(badRequest);
                    }

                    bool isAnsAcceptable = IsAnswerAcceptable(appQuestion.Answer, ans.Answer);
                    if (isAnsAcceptable)
                    {
                        isValidAns = true;
                        
                    }
                    else
                    {
                        isValidAns = false;
                        break;
                    }
                }

                if(isValidAns)
                {
                    validApplications = new List<ApplicantQualification>();
                    validApplications.Add(applicantQualification);
                }

            }
            return validApplications;
        }
    }
}
