using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace webapi_azure_oci.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

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

            string conString = "User Id=hr;Password=<password>;Data Source=<ip or hostname>:1521/<service name>;";
            List<Employee> empList = new List<Employee>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    //try
                    //{
                        con.Open();
                        cmd.BindByName = true;

                        //Use the command to display employee names from 
                        // the EMPLOYEES table
                        cmd.CommandText = "select first_name, last_name from employees where department_id = :id";

                        // Assign id to the department number 50 
                        OracleParameter id = new OracleParameter("id", 50);
                        cmd.Parameters.Add(id);

                        //Execute the command and use DataReader to display the data
                        OracleDataReader reader = cmd.ExecuteReader();
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
                    reader.Dispose();
                    //}
                    //catch (Exception ex)
                    //{
                    //    //await context.Response.WriteAsync(ex.Message);
                    //    return Enumerable.Range(1, 1).Select(index => new Employee
                    //    {
                    //        FirstName = null,
                    //        LastName = null
                    //    }).ToArray();
                    //}
                }
            }
        }
    }
}
