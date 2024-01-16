using System;

namespace Glossary_API.Helpers.Params
{
    public class SearchLocalParam
    {
        public string Country { get; set; }
        public string System { get; set; }
        public string SubSystem { get; set; }
        public string Name { get; set; }
        public bool?  Active { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}