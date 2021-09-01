using System;
using System.Collections.Generic;
using System.Linq;
using Data.DBDataTemplates;
using Helpers;

namespace Service
{
    public static class ScriptCreator
    {
        private static readonly Dictionary<string, string> DbMysqlTypes = new Dictionary<string, string>
        {
            {"varchar", "VARCHAR" },
            {"text", "TEXT" },
            {"int", "INT" },
            {"decimal", "DECIMAL" },
            {"datetime", "DATETIME" },
            {"date", "DATE" },
        };

        private static string CreateWithEndLine(string data) => $"{data}\n";

        /// <summary>
        /// Main method which handles process of creating db.
        /// </summary>
        /// <param name="dbToGenerate">Data to generate from.</param>
        /// <returns>MySql script in string to save.</returns>
        public static string CreateDb(InitialDBTemplate dbToGenerate)
        {
            try
            {
                var result = CreateWithEndLine("SET SQL_MODE = \"NO_AUTO_VALUE_ON_ZERO\";");
                result += CreateWithEndLine("START TRANSACTION;");
                result += CreateWithEndLine("SET time_zone = \"+00:00\";");
                result += CreateWithEndLine("/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;");
                result += CreateWithEndLine("/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;");
                result += CreateWithEndLine("/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;");
                result += CreateWithEndLine("/*!40101 SET NAMES utf8mb4 */;");

                foreach (var table in dbToGenerate.Tables)
                {
                    result += CreateTables(table);
                }

                result += CreateWithEndLine("COMMIT;");
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Error thrown in method {nameof(CreateDb)}", ex);
                throw;
            }
        }

        /// <summary>
        /// Generates table.
        /// </summary>
        /// <param name="table">Data to create from.</param>
        /// <returns>Return MySql table data in string.</returns>
        private static string CreateTables(TableDBTemplate table)
        {
            try
            {
                var result = CreateWithEndLine($"CREATE TABLE `{table.TableName}` (");

                result = table.TableColumns.Aggregate(result, (current, column) => current + CreateColumn(column));

                var primaryKeyColumn = table.TableColumns.FirstOrDefault(x => x.IsPrimaryKey);
                if (primaryKeyColumn != null)
                {
                    result += CreateWithEndLine($"PRIMARY KEY(`{primaryKeyColumn.ColumnName}`),");
                }

                var foreignColumnList = table.TableColumns.Where(x => x.ForeignData != null).ToList();

                foreach (var item in foreignColumnList)
                {
                    result += CreateWithEndLine($"FOREIGN KEY ({item.ColumnName}) REFERENCES {item.ForeignData.TableName}({item.ForeignData.ColumnName}),");
                }

                result = CreateWithEndLine(result.Remove(result.Length - 2));
                result += CreateWithEndLine(") ENGINE = InnoDB;");
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Error thrown in method {nameof(CreateTables)}", ex);
                throw;
            }
        }

        /// <summary>
        /// Generates columns for tables.
        /// </summary>
        /// <param name="column">Column data to create from.</param>
        /// <returns>MySql script in string.</returns>
        private static string CreateColumn(ColumnDBTemplate column)
        {
            try
            {
                var result = $"`{column.ColumnName}` {DbMysqlTypes[column.DbType]}";

                if (column.MaxValue.HasValue)
                {
                    result += column.DbType switch
                    {
                        "decimal" => $" ({column.MaxValue.Value - 2}, 2)",
                        _ => $" ({column.MaxValue.Value})"
                    };
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
                LogHelper.LogError($"Error thrown in method {nameof(CreateColumn)}", ex);
                throw;
            }
        }
    }
}
