using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

namespace webapi_azure_oci.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private TelemetryClient telemetryClient;
        
        public EmployeeController()
        {
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            QuickPulseTelemetryProcessor quickPulseProcessor = null;
            configuration.DefaultTelemetrySink.TelemetryProcessorChainBuilder
                .Use((next) =>
                {
                    quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
                    return quickPulseProcessor;
                })
                .Build();

            var quickPulseModule = new QuickPulseTelemetryModule
            {
                AuthenticationApiKey = "6ece7b96-a7a0-4de1-8039-8d7537893c73"
            };
            quickPulseModule.Initialize(configuration);
            quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);

            telemetryClient = new TelemetryClient(configuration);

        }

        [HttpGet]
        public IEnumerable<Employee> Get()
        {

            DateTime HttpRequestTime = DateTime.Now;
            string conString = "User Id=poc;Password=poc_OCIAzure_2k21;Data Source=10.1.0.250:1521/db0901_pdb1.clientsubnet.vncdatabase.oraclevcn.com;";
            List<Employee> empList = new List<Employee>();
            
            DateTime OracleRequestTime = DateTime.Now;
            using OracleConnection con = new OracleConnection(conString);
            using OracleCommand cmd = con.CreateCommand();
            try
            {
                using (var operation = telemetryClient.StartOperation<DependencyTelemetry>("Oracle Request"))
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
                    //telemetryClient.TrackDependency("Oracle Request", "Oracle Query", "select first_name, last_name from employees where department_id = :id",
                    //    OracleRequestTime, (DateTime.Now - OracleRequestTime), true);

                    while (reader.Read())
                    {
                        empList.Add(new Employee()
                        {
                            FirstName = reader.GetString(0),
                            LastName = reader.GetString(1)
                        }
                        );
                    }
            }
                telemetryClient.TrackRequest("Employee Request", HttpRequestTime, (DateTime.Now - HttpRequestTime), "200", true);
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
