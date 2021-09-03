using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace webapi_azure_oci.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly TelemetryClient telemetryClient = new TelemetryClient();

        [HttpGet]
        public IEnumerable<Employee> Get()
        {

            string conString = "User Id=poc;Password=poc_OCIAzure_2k21;Data Source=10.1.0.250:1521/db0901_pdb1.clientsubnet.vncdatabase.oraclevcn.com;";
            List<Employee> empList = new List<Employee>();

            using OracleConnection con = new OracleConnection(conString);
            using OracleCommand cmd = con.CreateCommand();
            try
            {

                // Establish an operation context and associated telemetry item:
                using (var operation = telemetryClient.StartOperation<RequestTelemetry>("OracleRequest"))
                {
                    // Telemetry sent in here will use the same operation ID.
                    telemetryClient.TrackTrace("OracleQuery"); // or other Track* calls

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

                    while (reader.Read())
                    {
                        empList.Add(new Employee()
                        {
                            FirstName = reader.GetString(0),
                            LastName = reader.GetString(1)
                        }
                        );
                    }

                    // Set properties of containing telemetry item--for example:
                    operation.Telemetry.ResponseCode = "200";

                    // Optional: explicitly send telemetry item:
                    telemetryClient.StopOperation(operation);

                } // When operation is disposed, telemetry item is sent.
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
