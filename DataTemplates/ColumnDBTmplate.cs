namespace SqlGenerator.DataTemplates
{
    public sealed class ColumnDBTemplate
    {
        public string ColumnName { get; set; }
        public string DBType { get; set; }
        public bool IsPrimaryKey { get; set; } = false;
        public bool IsNullable { get; set; } = true;
        public long? MaxValue { get; set; }
        public string DefaultValue { get; set; }
        public ForeignDataDBTemplate ForeignData { get; set; }
    }
}
