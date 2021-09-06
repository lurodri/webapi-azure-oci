using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;

namespace webapi_azure_oci.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private TelemetryClient telemetryClient;
        
        public EmployeeController()
        {
            //TelemetryConfiguration configuration = new TelemetryConfiguration();
            var configuration = new TelemetryConfiguration();
            configuration.InstrumentationKey = "6bca0969-e2e2-4dbe-914a-5aa25a48f56c";
            telemetryClient = new TelemetryClient(configuration);
        }

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            string conString = "User Id=poc;Password=poc_OCIAzure_2k21;Data Source=exacs-vcp-rex4h-scan.exaprivvcp.vncdatabase.oraclevcn.com:1521/pdb_b3.exaprivvcp.vncdatabase.oraclevcn.com;";
            List<Employee> empList = new List<Employee>();
            OracleDataReader reader = null;

            OracleConnection con = new OracleConnection(conString);
            OracleCommand cmd = con.CreateCommand();
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
                    reader = cmd.ExecuteReader();

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
            }
            catch (Exception ex)
            {
                //await context.Response.WriteAsync(ex.Message);
                empList.Add(new Employee()
                {
                    FirstName = null,
                    LastName = null
                });
            }
            if (reader != null)
            {
                reader.Dispose();
            }
            if (cmd != null) 
            {
                cmd.Dispose();
            }
            if (con != null)
            {
                con.Close();
            }
            return empList.ToArray();
        }
    }
}
