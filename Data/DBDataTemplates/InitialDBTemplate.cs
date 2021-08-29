namespace Data.DBDataTemplates
{
    public sealed class InitialDBTemplate
    {
        public string DbName { get; set; }
        public string Collation { get; set; } = "utf8_general_ci";
        public TableDBTemplate[] Tables { get; set; }
    }
}
