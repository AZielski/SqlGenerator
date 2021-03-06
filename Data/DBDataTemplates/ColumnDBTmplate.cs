namespace Data.DBDataTemplates
{
    public sealed class ColumnDBTemplate
    {
        public string ColumnName { get; set; }
        public string DbType { get; set; }
        public bool IsPrimaryKey { get; set; } = false;
        public bool IsNullable { get; set; } = true;
        public long? MaxValue { get; set; }
        public string DefaultValue { get; set; }
        public ForeignDataDBTemplate ForeignData { get; set; }
    }
}
