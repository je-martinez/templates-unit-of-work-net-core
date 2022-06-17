    [Authorize]
    [Route("api/operations")]
    [ApiController]
    public class EVNT_EventController : ControllerBase
    {
        //EVNT_Event
        protected IEVNT_Event _logicController;
        public EVNT_EventController(IEVNT_Event logicController)
        {
            _logicController = logicController;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll(int? page_number, string search_param)
        {
            CustomHttpResponse content = null;
            if (page_number == null)
            {
                content = await _logicController.GetAll(search_param);
            }
            else
            {
                content = await _logicController.GetAllWithPagination((int)page_number, search_param);
            }
            return Ok(content);
        }

        [HttpGet]
        [Route("getById/{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var content = await _logicController.GetById(Id);
            return Ok(content);
        }

        [HttpPost]
        [AuthorizeOnlyAdmin]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] EVNT_EventDTO newRecord)
        {
            var content = await _logicController.Create(newRecord);
            return Ok(content);
        }

        [HttpPut]
        [AuthorizeOnlyAdmin]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] EVNT_EventDTO newRecord)
        {
            var content = await _logicController.Update(newRecord);
            return Ok(content);
        }
    }