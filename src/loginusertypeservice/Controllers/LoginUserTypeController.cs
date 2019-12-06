using Microsoft.AspNetCore.Mvc;
using loginusertypeservice.Models;
using loginusertypeservice.Services;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;
using ExcelDataReader;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Collections.Generic;

namespace loginusertypeservice.Controllers
{
    [Route("api/[controller]")]
    public class LoginUserTypeController : Controller
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILoginUserTypeCloudantService _cloudantService;

        public LoginUserTypeController(HtmlEncoder htmlEncoder, ILoginUserTypeCloudantService cloudantService = null)
        {
            _cloudantService = cloudantService;
            _htmlEncoder = htmlEncoder;
        }

        /// <summary>
        /// Get all the records
        /// </summary>
        /// <returns>returns all records from database</returns>
        [HttpGet]
        public async Task<dynamic> Get()
        {
            if (_cloudantService == null)
            {
                return new string[] { "No database connection" };
            }
            else
            {
                return await _cloudantService.GetAllAsync();
            }
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id">ID to be selected</param>
        /// <returns>record for the given id</returns>
        [HttpGet("id")]
        public async Task<dynamic> Get(string id)
        {
            if (_cloudantService == null)
            {
                return new string[] { "No database connection" };
            }
            else
            {
                return await _cloudantService.GetByIdAsync(id);
            }
        }

        /// <summary>
        /// Create a new record
        /// </summary>
        /// <param name="employee">New record to be created</param>
        /// <returns>status of the newly added record</returns>
        [HttpPost]
        public async Task<dynamic> Post([FromBody]LoginUserTypeAddRequest employee)
        {
            if (_cloudantService != null)
            {
                return await _cloudantService.CreateAsync(employee);
                //Console.WriteLine("POST RESULT " + response);
                //return new string[] { employee.IBMEmailID, employee._id, employee._rev };
                //return JsonConvert.DeserializeObject<UpdateEmployeeResponse>(response.Result);
            }
            else
            {
                return new string[] { "No database connection" };
            }
        }

        /// <summary>
        /// Update an existing record by giving _id and _rev values
        /// </summary>
        /// <param name="employee">record to be updated for given _id and _rev</param>
        /// <returns>status of the record updated</returns>
        [HttpPut]
        public async Task<dynamic> Update([FromBody]LoginUserType loginUserType)
        {
            if (_cloudantService != null)
            {
                return await _cloudantService.UpdateAsync(loginUserType);
                //Console.WriteLine("Update RESULT " + response);
                //return new string[] { employee.IBMEmailID, employee._id, employee._rev };
                //return JsonConvert.DeserializeObject<UpdateEmployeeResponse>(response.Result);
            }
            else
            {
                return new string[] { "No database connection" };
            }
        }


        /// <summary>
        /// Delete the record for the given id
        /// </summary>
        /// <param name="id">record id to bb deleted</param>
        /// <param name="rev">revision number of the record to be deleted</param>
        /// <returns>status of the record deleted</returns>
        [HttpDelete]
        public async Task<dynamic> Delete(string id, string rev)
        {
            if (_cloudantService != null)
            {
                return await _cloudantService.DeleteAsync(id, rev);
                //Console.WriteLine("Update RESULT " + response);
                //return new string[] { employee.IBMEmailID, employee._id, employee._rev };
                //return JsonConvert.DeserializeObject<UpdateEmployeeResponse>(response.Result);
            }
            else
            {
                return new string[] { "No database connection" };
            }
        }

        [HttpGet("upload")]
        public async Task<dynamic> Upload()
        {
            if (_cloudantService != null)
            {                
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var inFilePath = @"C:\gith\pushed\employeeservice\src\employeeservice\bulkdata\master.xlsx";
                var outFilePath = @"C:\gith\pushed\employeeservice\src\employeeservice\bulkdata\master.json";
                                
                using (var inFile = System.IO.File.Open(inFilePath, FileMode.Open, FileAccess.Read))
                using (var outFile = System.IO.File.CreateText(outFilePath))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(inFile, new ExcelReaderConfiguration()
                    { FallbackEncoding = Encoding.GetEncoding(1252) }))
                    using (var writer = new JsonTextWriter(outFile))
                    {
                        writer.Formatting = Formatting.Indented; //I likes it tidy
                        writer.WriteStartArray();
                        reader.Read(); //SKIP FIRST ROW, it's TITLES.
                        do
                        {
                            while (reader.Read())
                            {
                                var UnformattedEmployeeId = "";
                                //peek ahead? Bail before we start anything so we don't get an empty object
                                try
                                {
                                    UnformattedEmployeeId = Convert.ToString(reader.GetValue(0));
                                }
                                catch
                                {

                                }
                                //if (string.IsNullOrEmpty(UnformattedEmployeeId)) break;

                                writer.WriteStartObject();
                                writer.WritePropertyName("UnformattedEmployeeId");
                                writer.WriteValue(UnformattedEmployeeId);

                                writer.WritePropertyName("Formatted Employeed Id");
                                writer.WriteValue(Convert.ToString(reader.GetValue(1)));

                                writer.WritePropertyName("HCAM - ID");
                                writer.WriteValue(Convert.ToString(reader.GetValue(2)));

                                writer.WritePropertyName("C - ID");
                                writer.WriteValue(Convert.ToString(reader.GetValue(3)));

                                writer.WritePropertyName("Employee Name");
                                writer.WriteValue(Convert.ToString(reader.GetValue(4)));

                                writer.WritePropertyName("IBM Email ID");
                                writer.WriteValue(Convert.ToString(reader.GetValue(5)));

                                writer.WritePropertyName("Nationwide Email ID");
                                writer.WriteValue(Convert.ToString(reader.GetValue(6)));

                                writer.WritePropertyName("Gender");
                                writer.WriteValue(Convert.ToString(reader.GetValue(7)));
                          
                                writer.WriteEndObject();
                            }
                        } while (reader.NextResult());
                        writer.WriteEndArray();
                    }
                }
               return new string[] { "done" };
            }
            else
            {
                return new string[] { "No database connection" };
            }
        }    
    }
}
