using loginusertypeservice.Models;
using System.Threading.Tasks;

namespace loginusertypeservice.Services
{
    /// <summary>
    /// This is the interface for 
    /// </summary>
    public interface ILoginUserTypeCloudantService
    {
        /// <summary>
        /// Create a new record
        /// </summary>
        /// <param name="item">the record to be added.</param>
        /// <returns>returns the status of the add record</returns>
        Task<dynamic> CreateAsync(LoginUserTypeAddRequest item);

        /// <summary>
        /// Update given record
        /// </summary>
        /// <param name="item">the record to be updated</param>
        /// <returns>returns the status of the updated record</returns>
        Task<dynamic> UpdateAsync(LoginUserType item);

        /// <summary>
        /// Returns the list of all records in the database
        /// </summary>
        /// <returns>Returns the list of all records in the database</returns>
        Task<dynamic> GetAllAsync();

        /// <summary>
        /// Returns the record for the given id.
        /// </summary>
        /// <param name="id">id of the record to be retrieved</param>
        /// <returns>returns the record for given id</returns>
        Task<dynamic> GetByIdAsync(string id);

        /// <summary>
        /// Deletes the record for the given id
        /// </summary>
        /// <param name="id">id of the record to be deleted</param>
        /// <param name="rev">latest revision number of the record to be deleted</param>
        /// <returns>returns </returns>
        Task<dynamic> DeleteAsync(string id, string rev);
    }
}