using SqlGenerator.DataTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlGenerator.Services
{
    public static class ScriptCreator
    {
        public static Dictionary<string, string> DBMysqlTypes = new Dictionary<string, string>
        {
            {"varchar", "VARCHAR" },
            {"text", "TEXT" },
            {"int", "INT" },
            {"decimal", "DECIMAL" },
            {"datetime", "DATETIME" },
            {"date", "DATE" },
        };

        private static string CreateWithEndLine(string data) => $"{data}\n";
        public static string GenerateDB(InitialDBTemplate DBToGenerate)
        {
            try
            {
                string result = CreateWithEndLine("SET SQL_MODE = \"NO_AUTO_VALUE_ON_ZERO\";");
                result += CreateWithEndLine("START TRANSACTION;");
                result += CreateWithEndLine("SET time_zone = \"+00:00\";");
                result += CreateWithEndLine("/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;");
                result += CreateWithEndLine("/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;");
                result += CreateWithEndLine("/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;");
                result += CreateWithEndLine("/*!40101 SET NAMES utf8mb4 */;");

                foreach (var table in DBToGenerate.Tables)
                {
                    Console.WriteLine($"[INFO] Starting create {table.TableName} table");
                    result += GenerateTables(table);
                    Console.WriteLine($"[INFO] End of create {table.TableName} table");
                }

                result += CreateWithEndLine("COMMIT;");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex}");
                throw;
            }
        }

        private static string GenerateTables(TableDBTemplate table)
        {
            try
            {
                var result = CreateWithEndLine($"CREATE TABLE `{table.TableName}` (");

                foreach (var column in table.TableColumns)
                {
                    Console.WriteLine($"[INFO] Starting create {table.TableName} column");
                    result += GenerateColumn(column);
                    Console.WriteLine($"[INFO] End of create {table.TableName} column");
                }

                var primaryKeyColumn = table.TableColumns.Where(x => x.IsPrimaryKey).FirstOrDefault();
                if (primaryKeyColumn != null)
                {
                    Console.WriteLine($"[INFO] Adding primary key {primaryKeyColumn.ColumnName} to {table.TableName} table");
                    result += CreateWithEndLine($"PRIMARY KEY(`{primaryKeyColumn.ColumnName}`),");
                }

                var foreignColumnList = table.TableColumns.Where(x => x.ForeignData != null).ToList();
                foreach (var item in foreignColumnList)
                {
                    Console.WriteLine($"[INFO] Adding foreign key {item.ColumnName} to {item.ForeignData.TableName} table");
                    result += CreateWithEndLine($"FOREIGN KEY ({item.ColumnName}) REFERENCES {item.ForeignData.TableName}({item.ForeignData.ColumnName}),");
                }

                result = CreateWithEndLine(result.Remove(result.Length - 2));
                result += CreateWithEndLine(") ENGINE = InnoDB;");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex}");
                throw;
            }
        }

        private static string GenerateColumn(ColumnDBTemplate column)
        {
            try
            {
                var result = $"`{column.ColumnName}` {DBMysqlTypes[column.DBType]}";

                if (column.MaxValue.HasValue)
                {
                    switch (column.DBType)
                    {
                        case "decimal":
                            result += $" ({column.MaxValue.Value - 2}, 2)";
                            break;
                        default:
                            result += $" ({column.MaxValue.Value})";
                            break;
                    }
                }

                result += $"{(column.IsNullable ? " NULL" : " NOT NULL")}";
                result += $"{(column.IsPrimaryKey ? " AUTO_INCREMENT" : string.Empty)}";

                if (!string.IsNullOrEmpty(column.DefaultValue))
                {
                    result += $" DEFAULT '{column.DefaultValue}'";
                }

                return CreateWithEndLine($"{result},");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: {ex}");
                throw;
            }
        }
    }
}
