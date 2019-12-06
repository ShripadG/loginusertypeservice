using loginusertypeservice.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace loginusertypeservice.Services
{
    public class UserTypeCloudantService : ILoginUserTypeCloudantService
    {
        private static readonly string _dbName = "loginusertype";
        private readonly Creds _cloudantCreds;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHttpClientFactory _factory;

        public UserTypeCloudantService(Creds creds, UrlEncoder urlEncoder, IHttpClientFactory factory)
        {
            _cloudantCreds = creds;
            _urlEncoder = urlEncoder;
            _factory = factory;
        }

        public async Task<dynamic> CreateAsync(LoginUserTypeAddRequest item)
        {
            string jsonInString = JsonConvert.SerializeObject(item);
            
            var _client = _factory.CreateClient("cloudant");

            var response = await _client.PostAsync(_client.BaseAddress+_dbName, new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return responseJson;
            }
            else if (Equals(response.ReasonPhrase,"Object Not Found")) //need to create database
            {
                var contents = new StringContent("", Encoding.UTF8, "application/json");
                response = await _client.PutAsync(_client.BaseAddress + _dbName, contents); //creating database using PUT request
                if (response.IsSuccessStatusCode) //if successful, try POST request again
                {
                    response = await _client.PostAsync(_client.BaseAddress+_dbName, new StringContent(jsonInString, Encoding.UTF8, "application/json"));
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        return responseJson;
                    }
                }

            }

            string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
            Console.WriteLine(msg);
            return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
        }

        public async Task<dynamic> UpdateAsync(LoginUserType item)
        {
            string jsonInString = JsonConvert.SerializeObject(item);

            var _client = _factory.CreateClient("cloudant");

            var response = await _client.PutAsync(_client.BaseAddress + _dbName + "/" + _urlEncoder.Encode(item._id) + "?rev=" + _urlEncoder.Encode(item._rev), new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return responseJson;
            }           

            string msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
            Console.WriteLine(msg);
            return JsonConvert.SerializeObject(new { msg = "Failure to POST. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase });
        }

        public async Task<dynamic> GetAllAsync()
        {
            var _client = _factory.CreateClient("cloudant");
            var response = await _client.GetAsync(_client.BaseAddress + _dbName+"/_all_docs?include_docs=true");            

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (Equals(response.ReasonPhrase,"Object Not Found")) //need to create database
            {
                var contents = new StringContent("", Encoding.UTF8, "application/json");
                response = await _client.PutAsync(_client.BaseAddress + _dbName, contents); //creating database using PUT request
                if (response.IsSuccessStatusCode) //if successful, try GET request again
                {
                    response = await _client.GetAsync(_client.BaseAddress + _dbName + "/_all_docs?include_docs=true");
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }

            }

            string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
            Console.WriteLine(msg);
            return JsonConvert.SerializeObject(new { msg = msg });
        }

        public async Task<dynamic> GetByIdAsync(string id)
        {
            var _client = _factory.CreateClient("cloudant");
            //var response = await _client.GetAsync(_client.BaseAddress + _dbName + "/" + _urlEncoder.Encode(id) + "?rev=" + _urlEncoder.Encode(rev));
            var response = await _client.GetAsync(_client.BaseAddress + _dbName + "/" + _urlEncoder.Encode(id));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }           

            string msg = "Failure to GET. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
            Console.WriteLine(msg);
            return JsonConvert.SerializeObject(new { msg = msg });
        }

        public async Task<dynamic> DeleteAsync(string id, string rev)
        {
            var _client = _factory.CreateClient("cloudant");
            var response = await _client.DeleteAsync(_client.BaseAddress + _dbName + "/" + _urlEncoder.Encode(id) + "?rev=" + _urlEncoder.Encode(rev));            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            string msg = "Failure to DELETE. Status Code: " + response.StatusCode + ". Reason: " + response.ReasonPhrase;
            Console.WriteLine(msg);
            return JsonConvert.SerializeObject(new { msg = msg });
        }
    }
}
