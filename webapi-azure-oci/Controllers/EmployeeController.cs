using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Microsoft.ApplicationInsights;

namespace webapi_azure_oci.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        //private static readonly string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        //private readonly ILogger<EmployeeController> _logger;

        //public EmployeeController(ILogger<EmployeeController> logger)
        //{
        //    _logger = logger;
        //}

        private TelemetryClient telemetryClient = new TelemetryClient();

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new Employee
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();

            string conString = "User Id=poc;Password=poc_OCIAzure_2k21;Data Source=10.1.0.250:1521/db0901_pdb1.clientsubnet.vncdatabase.oraclevcn.com;";
            List<Employee> empList = new List<Employee>();

            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;

                        //Use the command to display employee names from 
                        // the EMPLOYEES table
                        cmd.CommandText = "select first_name, last_name from employees where department_id = :id";

                        // Assign id to the department number 50 
                        OracleParameter id = new OracleParameter("id", 100);
                        cmd.Parameters.Add(id);

                        //Execute the command and use DataReader to display the data
                        OracleDataReader reader = cmd.ExecuteReader();
                        timer.Stop();
                        telemetryClient.TrackDependency("OracleRequest", "OracleQuery", "OracleQueryStatus", startTime, timer.Elapsed, true);

                        while (reader.Read())
                        {
                            empList.Add(new Employee()
                            {
                                FirstName = reader.GetString(0),
                                LastName = reader.GetString(1)
                            }
                            );
                        }
                        return empList.ToArray();
                    }
                    catch (Exception ex)
                    {
                        //await context.Response.WriteAsync(ex.Message);
                        empList.Add(new Employee()
                        {
                            FirstName = null,
                            LastName = null
                        });
                        return empList.ToArray();
                    }
                    finally
                    {

                    }
                }
            }
        }
    }
}
