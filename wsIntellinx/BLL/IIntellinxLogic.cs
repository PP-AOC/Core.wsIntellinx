using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wsIntellinx.ViewModels;
using wsIntellinx.ViewModels.Params;

namespace wsIntellinx.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIntellinxLogic
    {
        /// <summary>
        /// Return a list of KbaMembers whose records have changed within the start and end parameters inclusive.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of KbaMembers if success, else empty List</returns>
        Task<List<KbaMember>> GetKbaMembers(DateTime startDate, DateTime? endDate);

        /// <summary>
        /// Return a KbaMember record matching the kbaNumber parameter.
        /// </summary>
        /// <param name="kbaMemberParam"></param>
        /// <returns>KbaMember record if success</returns>
        Task<KbaMember> GetKbaMember(KbaMemberParam kbaMemberParam);
    }
}
