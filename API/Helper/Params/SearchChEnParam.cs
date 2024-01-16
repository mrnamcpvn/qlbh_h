using System;

namespace Glossary_API.Helpers.Params
{
    public class SearchChEnParam
    {
        public string System { get; set; }
        public string SubSystem { get; set; }
        public string Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? Activate { get; set; }
    }
}