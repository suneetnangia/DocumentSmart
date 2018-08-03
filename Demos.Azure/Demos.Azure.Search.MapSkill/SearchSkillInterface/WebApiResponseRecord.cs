using System.Collections.Generic;

namespace Demos.Azure.Search.MapSkill.SearchSkillInterface
{
    internal class WebApiResponseRecord
    {
        public string recordId { get; set; }
        public Dictionary<string, object> data { get; set; }
        public List<WebApiResponseError> errors { get; set; }
        public List<WebApiResponseWarning> warnings { get; set; }
    }
}