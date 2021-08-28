namespace SqlGenerator.DataTemplates
{
    public sealed class TableDBTemplate
    {
        public string TableName { get; set; }
        public ColumnDBTemplate[] TableColumns { get; set; }
    }
}
