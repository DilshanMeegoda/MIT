using System.Collections.Generic;

namespace WorkFlowManagement.Model
{
    public class ReportElement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TemplateType { get; set; }
        public string Type { get; set; }
        public string TypeAlias { get; set; }
        public string Value { get; set; }
        public string Info { get; set; }
        public string FilledBy { get; set; }
        public int SortOrder { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsVertical { get; set; }
        public bool AddNew { get; set; }
        public List<KeyValue> Values { get; set; }
        public List<List<ReportElement>> Child { get; set; }
        public List<List<ReportElement>> Accepted { get; set; }
        public List<List<ReportElement>> Rejected { get; set; }
        public List<OptionManager> Options { get; set; }
    }

    public class KeyValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Condition { get; set; }
        public List<List<ReportElement>> Child { get; set; }
    }

    public class OptionManager
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}