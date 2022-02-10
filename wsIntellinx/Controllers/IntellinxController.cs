using Core.Api.Utils.Log;
using Core.Nuget.Security.ApiKey;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using wsIntellinx.BLL;
using wsIntellinx.ViewModels;
using wsIntellinx.ViewModels.Params;

namespace wsIntellinx.Controllers
{
    /// <summary>
    /// This controller deals with KbaMember related items.
    /// </summary>
    [Produces("application/json")] // HTTP header: Content-Type - UnsupportedMediaType (415)
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ApiKeyAuth]
    [ApiController]
    public class IntellinxController : ControllerBase
    {
        private readonly ILogger _log;
        private readonly IWebHostEnvironment _env;
        private readonly IIntellinxLogic _intellinxLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntellinxController"/> class.
        /// </summary>
        /// <param name="log"><see cref="ILogger"/>.</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/>.</param>
        /// <param name="intellinxLogic"><see cref="IIntellinxLogic"/>.</param>
        public IntellinxController(ILogger log,
                                   IWebHostEnvironment env,
                                   IIntellinxLogic intellinxLogic)
        {
            _log = log;
            _log.Enter();
            _intellinxLogic = intellinxLogic;
            _env = env;
            _log.Exit();
        }

        /// <summary>
        /// Initial get method to test service is up.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/ping")]
        public ActionResult<string> Ping()
        {
            _log.Enter("");
            return Ok("ping");
        }

        /// <summary>
        /// This endpoint returns a list of KbaMembers whose records have changed within the start and end parameters inclusive.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>list of KbaMembers if success, else empty list</returns>
        [HttpGet]
        [Route("kba/members")]
        public async Task<ActionResult<List<KbaMember>>> GetKbaMembers([Required, DataType(DataType.DateTime)]
                                                                        DateTime startDate,
                                                                       [DataType(DataType.DateTime)]
                                                                        DateTime? endDate)
        {
            var kbaMembers = await _intellinxLogic.GetKbaMembers(startDate, endDate);
            return Ok(kbaMembers);
        }

        /// <summary>
        /// This endpoint returns a KbaMember record matching the kbaNumber parameter.
        /// </summary>
        /// <param name="kbaMemberParam"></param>
        /// <returns>a KbaMember record if success</returns>
        [HttpPost]
        [Route("kba/members/verification")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KbaMember>> GetKbaMember([FromBody] KbaMemberParam kbaMemberParam)
        {
            var kbaMember = await _intellinxLogic.GetKbaMember(kbaMemberParam);
            if(kbaMember != null)
            {
                return Ok(kbaMember);
            }
            return NotFound("No member found matching with the requested kbaNumber");
        }
    }
}
