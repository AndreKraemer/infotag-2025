using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using MiracleListVueHybridDemo.Dtos;
using System.IO;

namespace MiracleListVueHybridDemo.Utils
{
    /// <summary>
    /// Provides functionality to export a category with its tasks to Word (DOCX) format
    /// </summary>
    public static class WordExporter
    {
        /// <summary>
        /// Exports a category to a Word document
        /// </summary>
        /// <param name="category">The category to export</param>
        /// <param name="filePath">The file path where to save the document. If null, a temp file will be created</param>
        /// <returns>The file path of the generated document</returns>
        public static async Task<string> ExportCategoryToWordAsync(Dtos.Category category, string? filePath = null)
        {
            // If no file path is provided, create a temp file
            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = $"MiracleList_{category.Name}_{DateTime.Now:yyyy-MM-dd_HHmmss}.docx";
                filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            }

            // Create and prepare the document in memory
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create the document
                using (WordprocessingDocument doc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    // Add a main document part
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Add document content
                    AddDocumentContent(body, category);
                }

                // Save to the specified file
                memoryStream.Position = 0;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Exports all categories to a Word document
        /// </summary>
        /// <param name="categories">The list of categories to export</param>
        /// <param name="filePath">The file path where to save the document. If null, a temp file will be created</param>
        /// <returns>The file path of the generated document</returns>
        public static async Task<string> ExportAllCategoriesToWordAsync(List<Dtos.Category> categories, string? filePath = null)
        {
            // If no file path is provided, create a temp file
            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = $"MiracleList_AllCategories_{DateTime.Now:yyyy-MM-dd_HHmmss}.docx";
                filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            }

            // Create and prepare the document in memory
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create the document
                using (WordprocessingDocument doc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    // Add a main document part
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Add document title
                    var titlePara = new Paragraph();
                    var titleRun = new Run();
                    var titleProps = new RunProperties();
                    titleProps.AppendChild(new Bold());
                    titleProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "48" });
                    titleRun.AppendChild(titleProps);
                    titleRun.AppendChild(new Text("MiracleList - All Categories"));
                    titlePara.AppendChild(titleRun);
                    body.AppendChild(titlePara);

                    // Add document details
                    AddParagraphWithBoldLabel(body, "Total Categories:", categories.Count.ToString());
                    AddParagraphWithBoldLabel(body, "Total Tasks:", categories.Sum(c => c.Tasks?.Length ?? 0).ToString());
                    AddParagraphWithBoldLabel(body, "Export Date:", DateTime.Now.ToString("g"));

                    // Add a separator
                    var separatorPara = new Paragraph();
                    var separatorProps = new ParagraphProperties();
                    var separatorBorders = new ParagraphBorders();
                    separatorBorders.AppendChild(new BottomBorder()
                    {
                        Val = BorderValues.Single,
                        Size = 8,
                        Space = 1,
                        Color = "4F81BD"
                    });
                    separatorProps.AppendChild(separatorBorders);
                    separatorPara.AppendChild(separatorProps);
                    body.AppendChild(separatorPara);

                    // Add categories summary table
                    body.AppendChild(CreateCategoriesSummaryTable(categories));

                    // Add detailed content for each category
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (var category in categories)
                        {
                            // Add page break before each category
                            var pageBreakPara = new Paragraph();
                            var pageBreakRun = new Run();
                            pageBreakRun.AppendChild(new Break() { Type = BreakValues.Page });
                            pageBreakPara.AppendChild(pageBreakRun);
                            body.AppendChild(pageBreakPara);

                            // Add category content
                            AddCategoryContent(body, category);
                        }
                    }
                    else
                    {
                        // No categories message
                        var noContentPara = new Paragraph();
                        var noContentRun = new Run();
                        noContentRun.AppendChild(new Text("No categories available."));
                        noContentPara.AppendChild(noContentRun);
                        body.AppendChild(noContentPara);
                    }

                    // Add footer
                    var footerPara = new Paragraph();
                    var footerRun = new Run();
                    var footerProps = new RunProperties();
                    footerProps.AppendChild(new Italic());
                    footerRun.AppendChild(footerProps);
                    footerRun.AppendChild(new Text("Generated by MiracleList"));
                    footerPara.AppendChild(footerRun);
                    body.AppendChild(footerPara);
                }

                // Save to the specified file
                memoryStream.Position = 0;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Creates a summary table of all categories
        /// </summary>
        private static Table CreateCategoriesSummaryTable(List<Dtos.Category> categories)
        {
            var table = new Table();

            // Table properties
            var tableProps = new TableProperties();
            var tableBorders = new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 1 },
                new BottomBorder() { Val = BorderValues.Single, Size = 1 },
                new LeftBorder() { Val = BorderValues.Single, Size = 1 },
                new RightBorder() { Val = BorderValues.Single, Size = 1 },
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 1 },
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 1 }
            );
            tableProps.AppendChild(tableBorders);
            tableProps.AppendChild(new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });
            table.AppendChild(tableProps);

            // Header row
            var headerRow = new TableRow();
            headerRow.AppendChild(CreateHeaderCell("Category ID"));
            headerRow.AppendChild(CreateHeaderCell("Category Name"));
            headerRow.AppendChild(CreateHeaderCell("Tasks Count"));
            headerRow.AppendChild(CreateHeaderCell("Completed Tasks"));
            headerRow.AppendChild(CreateHeaderCell("Open Tasks"));
            table.AppendChild(headerRow);

            // Data rows
            foreach (var category in categories)
            {
                var completedTasks = category.Tasks?.Count(t => t.Done) ?? 0;
                var openTasks = (category.Tasks?.Length ?? 0) - completedTasks;

                var row = new TableRow();
                row.AppendChild(CreateDataCell(category.CategoryID.ToString()));
                row.AppendChild(CreateDataCell(category.Name));
                row.AppendChild(CreateDataCell((category.Tasks?.Length ?? 0).ToString()));
                row.AppendChild(CreateDataCell(completedTasks.ToString()));
                row.AppendChild(CreateDataCell(openTasks.ToString()));
                table.AppendChild(row);
            }

            return table;
        }

        /// <summary>
        /// Adds category content to the document
        /// </summary>
        private static void AddCategoryContent(Body body, Dtos.Category category)
        {
            // Add category header with larger font
            var categoryHeaderPara = new Paragraph();
            var categoryHeaderRun = new Run();
            var categoryHeaderProps = new RunProperties();
            categoryHeaderProps.AppendChild(new Bold());
            categoryHeaderProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "36" });
            categoryHeaderRun.AppendChild(categoryHeaderProps);
            categoryHeaderRun.AppendChild(new Text($"Category: {category.Name}"));
            categoryHeaderPara.AppendChild(categoryHeaderRun);
            body.AppendChild(categoryHeaderPara);

            // Add category details
            AddParagraphWithBoldLabel(body, "Category ID:", category.CategoryID.ToString());
            AddParagraphWithBoldLabel(body, "Tasks:", (category.Tasks?.Length ?? 0).ToString());

            // Add a separator
            var separatorPara = new Paragraph();
            var separatorProps = new ParagraphProperties();
            var separatorBorders = new ParagraphBorders();
            separatorBorders.AppendChild(new BottomBorder()
            {
                Val = BorderValues.Single,
                Size = 6,
                Space = 1,
                Color = "4F81BD"
            });
            separatorProps.AppendChild(separatorBorders);
            separatorPara.AppendChild(separatorProps);
            body.AppendChild(separatorPara);

            // Add tasks section if tasks exist
            if (category.Tasks != null && category.Tasks.Length > 0)
            {
                // Add tasks heading
                var tasksHeadingPara = new Paragraph();
                var tasksHeadingRun = new Run();
                var tasksHeadingProps = new RunProperties();
                tasksHeadingProps.AppendChild(new Bold());
                tasksHeadingProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" });
                tasksHeadingRun.AppendChild(tasksHeadingProps);
                tasksHeadingRun.AppendChild(new Text("Tasks"));
                tasksHeadingPara.AppendChild(tasksHeadingRun);
                body.AppendChild(tasksHeadingPara);

                // Add tasks table
                body.AppendChild(CreateTasksTable(category.Tasks));

                // Add detailed task information
                foreach (var task in category.Tasks)
                {
                    AddTaskDetails(body, task);
                }
            }
            else
            {
                // No tasks message
                var noTasksPara = new Paragraph();
                var noTasksRun = new Run();
                noTasksRun.AppendChild(new Text("No tasks available in this category."));
                noTasksPara.AppendChild(noTasksRun);
                body.AppendChild(noTasksPara);
            }
        }

        /// <summary>
        /// Adds the document content
        /// </summary>
        private static void AddDocumentContent(Body body, Dtos.Category category)
        {
            // Add document title
            var titlePara = new Paragraph();
            var titleRun = new Run();
            var titleProps = new RunProperties();
            titleProps.AppendChild(new Bold());
            titleProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "36" });
            titleRun.AppendChild(titleProps);
            titleRun.AppendChild(new Text($"Category: {category.Name}"));
            titlePara.AppendChild(titleRun);
            body.AppendChild(titlePara);

            // Add category details
            AddParagraphWithBoldLabel(body, "Category ID:", category.CategoryID.ToString());
            AddParagraphWithBoldLabel(body, "Tasks:", (category.Tasks?.Length ?? 0).ToString());
            AddParagraphWithBoldLabel(body, "Export Date:", DateTime.Now.ToString("g"));

            // Add a separator
            var separatorPara = new Paragraph();
            var separatorProps = new ParagraphProperties();
            var separatorBorders = new ParagraphBorders();
            separatorBorders.AppendChild(new BottomBorder()
            {
                Val = BorderValues.Single,
                Size = 6,
                Space = 1,
                Color = "4F81BD"
            });
            separatorProps.AppendChild(separatorBorders);
            separatorPara.AppendChild(separatorProps);
            body.AppendChild(separatorPara);

            // Add tasks section if tasks exist
            if (category.Tasks != null && category.Tasks.Length > 0)
            {
                // Add tasks heading
                var tasksHeadingPara = new Paragraph();
                var tasksHeadingRun = new Run();
                var tasksHeadingProps = new RunProperties();
                tasksHeadingProps.AppendChild(new Bold());
                tasksHeadingProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" });
                tasksHeadingRun.AppendChild(tasksHeadingProps);
                tasksHeadingRun.AppendChild(new Text("Tasks"));
                tasksHeadingPara.AppendChild(tasksHeadingRun);
                body.AppendChild(tasksHeadingPara);

                // Add tasks table
                body.AppendChild(CreateTasksTable(category.Tasks));

                // Add detailed task information
                foreach (var task in category.Tasks)
                {
                    AddTaskDetails(body, task);
                }
            }
            else
            {
                // No tasks message
                var noTasksPara = new Paragraph();
                var noTasksRun = new Run();
                noTasksRun.AppendChild(new Text("No tasks available in this category."));
                noTasksPara.AppendChild(noTasksRun);
                body.AppendChild(noTasksPara);
            }

            // Add footer
            var footerPara = new Paragraph();
            var footerRun = new Run();
            var footerProps = new RunProperties();
            footerProps.AppendChild(new Italic());
            footerRun.AppendChild(footerProps);
            footerRun.AppendChild(new Text("Generated by MiracleList"));
            footerPara.AppendChild(footerRun);
            body.AppendChild(footerPara);
        }

        /// <summary>
        /// Adds a paragraph with a bold label and normal text value
        /// </summary>
        private static void AddParagraphWithBoldLabel(Body body, string label, string value)
        {
            var para = new Paragraph();

            // Label part
            var labelRun = new Run();
            var labelProps = new RunProperties();
            labelProps.AppendChild(new Bold());
            labelRun.AppendChild(labelProps);
            labelRun.AppendChild(new Text(label + " "));
            para.AppendChild(labelRun);

            // Value part
            var valueRun = new Run();
            valueRun.AppendChild(new Text(value));
            para.AppendChild(valueRun);

            body.AppendChild(para);
        }

        /// <summary>
        /// Creates a table with all tasks
        /// </summary>
        private static Table CreateTasksTable(MiracleTask[] tasks)
        {
            var table = new Table();

            // Table properties
            var tableProps = new TableProperties();
            var tableBorders = new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 1 },
                new BottomBorder() { Val = BorderValues.Single, Size = 1 },
                new LeftBorder() { Val = BorderValues.Single, Size = 1 },
                new RightBorder() { Val = BorderValues.Single, Size = 1 },
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 1 },
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 1 }
            );
            tableProps.AppendChild(tableBorders);
            tableProps.AppendChild(new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });
            table.AppendChild(tableProps);

            // Header row
            var headerRow = new TableRow();
            headerRow.AppendChild(CreateHeaderCell("Title"));
            headerRow.AppendChild(CreateHeaderCell("Due Date"));
            headerRow.AppendChild(CreateHeaderCell("Status"));
            headerRow.AppendChild(CreateHeaderCell("Priority"));
            headerRow.AppendChild(CreateHeaderCell("Subtasks"));
            table.AppendChild(headerRow);

            // Data rows
            foreach (var task in tasks)
            {
                var row = new TableRow();
                row.AppendChild(CreateDataCell(task.Title));
                row.AppendChild(CreateDataCell(task.Due.ToShortDateString()));
                row.AppendChild(CreateDataCell(task.Done ? "Completed" : "Open"));
                row.AppendChild(CreateDataCell(GetImportanceText(task.Importance)));
                row.AppendChild(CreateDataCell(task.subTaskSet?.Length.ToString() ?? "0"));
                table.AppendChild(row);
            }

            return table;
        }

        /// <summary>
        /// Creates a header cell for a table
        /// </summary>
        private static TableCell CreateHeaderCell(string text)
        {
            var cell = new TableCell();

            // Cell properties with shading
            var cellProps = new TableCellProperties();
            cellProps.AppendChild(new Shading()
            {
                Val = ShadingPatternValues.Clear,
                Color = "auto",
                Fill = "4F81BD"
            });
            cell.AppendChild(cellProps);

            // Cell content
            var para = new Paragraph();
            var run = new Run();
            var runProps = new RunProperties();
            runProps.AppendChild(new Bold());
            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = "FFFFFF" });
            run.AppendChild(runProps);
            run.AppendChild(new Text(text));
            para.AppendChild(run);
            cell.AppendChild(para);

            return cell;
        }

        /// <summary>
        /// Creates a data cell for a table
        /// </summary>
        private static TableCell CreateDataCell(string text)
        {
            var cell = new TableCell();
            var para = new Paragraph();
            var run = new Run();
            run.AppendChild(new Text(text));
            para.AppendChild(run);
            cell.AppendChild(para);
            return cell;
        }

        /// <summary>
        /// Adds detailed information about a task
        /// </summary>
        private static void AddTaskDetails(Body body, MiracleTask task)
        {
            // Page break before task details
            var pageBreakPara = new Paragraph();
            var pageBreakRun = new Run();
            pageBreakRun.AppendChild(new Break() { Type = BreakValues.Page });
            pageBreakPara.AppendChild(pageBreakRun);
            body.AppendChild(pageBreakPara);

            // Task heading
            var headingPara = new Paragraph();
            var headingRun = new Run();
            var headingProps = new RunProperties();
            headingProps.AppendChild(new Bold());
            headingProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
            headingRun.AppendChild(headingProps);
            headingRun.AppendChild(new Text(task.Title));
            headingPara.AppendChild(headingRun);
            body.AppendChild(headingPara);

            // Task details table
            var detailsTable = new Table();
            var tableProps = new TableProperties();
            tableProps.AppendChild(new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });
            detailsTable.AppendChild(tableProps);

            // Add task property rows
            AddDetailRow(detailsTable, "ID", task.TaskID.ToString());
            AddDetailRow(detailsTable, "Created", task.Created.ToString("g"));
            AddDetailRow(detailsTable, "Due", $"{task.Due:g} ({task.DueInDays} days remaining)");
            AddDetailRow(detailsTable, "Priority", GetImportanceText(task.Importance));
            AddDetailRow(detailsTable, "Status", task.Done ? "Completed" : "Open");
            AddDetailRow(detailsTable, "Effort", GetEffortText(task.Effort));

            body.AppendChild(detailsTable);

            // Task notes
            if (!string.IsNullOrWhiteSpace(task.Note))
            {
                // Notes label
                var notesLabelPara = new Paragraph();
                var notesLabelRun = new Run();
                var notesLabelProps = new RunProperties();
                notesLabelProps.AppendChild(new Bold());
                notesLabelRun.AppendChild(notesLabelProps);
                notesLabelRun.AppendChild(new Text("Notes:"));
                notesLabelPara.AppendChild(notesLabelRun);
                body.AppendChild(notesLabelPara);

                // Notes content
                var notesPara = new Paragraph();
                var notesRun = new Run();
                notesRun.AppendChild(new Text(task.Note));
                notesPara.AppendChild(notesRun);
                body.AppendChild(notesPara);
            }

            // Subtasks
            if (task.subTaskSet != null && task.subTaskSet.Length > 0)
            {
                // Subtasks label
                var subtasksLabelPara = new Paragraph();
                var subtasksLabelRun = new Run();
                var subtasksLabelProps = new RunProperties();
                subtasksLabelProps.AppendChild(new Bold());
                subtasksLabelRun.AppendChild(subtasksLabelProps);
                subtasksLabelRun.AppendChild(new Text("Subtasks:"));
                subtasksLabelPara.AppendChild(subtasksLabelRun);
                body.AppendChild(subtasksLabelPara);

                // Subtasks table
                var subtasksTable = new Table();
                var subtasksTableProps = new TableProperties();
                subtasksTableProps.AppendChild(new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });
                var subtasksTableBorders = new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 1 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 1 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 1 },
                    new RightBorder() { Val = BorderValues.Single, Size = 1 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 1 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 1 }
                );
                subtasksTableProps.AppendChild(subtasksTableBorders);
                subtasksTable.AppendChild(subtasksTableProps);

                // Add header row
                var headerRow = new TableRow();
                headerRow.AppendChild(CreateHeaderCell("Status"));
                headerRow.AppendChild(CreateHeaderCell("Title"));
                headerRow.AppendChild(CreateHeaderCell("Created"));
                subtasksTable.AppendChild(headerRow);

                // Add subtask rows
                foreach (var subtask in task.subTaskSet)
                {
                    var row = new TableRow();
                    row.AppendChild(CreateDataCell(subtask.Done ? "?" : "?"));
                    row.AppendChild(CreateDataCell(subtask.Title));
                    row.AppendChild(CreateDataCell(subtask.Created.ToString("g")));
                    subtasksTable.AppendChild(row);
                }

                body.AppendChild(subtasksTable);
            }
        }

        /// <summary>
        /// Adds a detail row to a table
        /// </summary>
        private static void AddDetailRow(Table table, string label, string value)
        {
            var row = new TableRow();

            // Label cell
            var labelCell = new TableCell();
            var labelCellProps = new TableCellProperties();
            labelCellProps.AppendChild(new Shading()
            {
                Val = ShadingPatternValues.Clear,
                Color = "auto",
                Fill = "DEDEDE"
            });
            labelCell.AppendChild(labelCellProps);

            var labelPara = new Paragraph();
            var labelRun = new Run();
            var labelRunProps = new RunProperties();
            labelRunProps.AppendChild(new Bold());
            labelRun.AppendChild(labelRunProps);
            labelRun.AppendChild(new Text(label));
            labelPara.AppendChild(labelRun);
            labelCell.AppendChild(labelPara);

            // Value cell
            var valueCell = new TableCell();
            var valuePara = new Paragraph();
            var valueRun = new Run();
            valueRun.AppendChild(new Text(value));
            valuePara.AppendChild(valueRun);
            valueCell.AppendChild(valuePara);

            // Add cells to row
            row.AppendChild(labelCell);
            row.AppendChild(valueCell);

            // Add row to table
            table.AppendChild(row);
        }

        /// <summary>
        /// Gets a display text for importance level
        /// </summary>
        private static string GetImportanceText(int importance)
        {
            return importance switch
            {
                0 => "Low",
                1 => "Medium",
                2 => "High",
                _ => $"Level {importance}"
            };
        }

        /// <summary>
        /// Gets a display text for effort level
        /// </summary>
        private static string GetEffortText(double effort)
        {
            // For exact values
            if (Math.Abs(effort - Math.Floor(effort)) < 0.001)
            {
                return (int)effort switch
                {
                    0 => "Minimal",
                    1 => "Medium",
                    2 => "Significant",
                    _ => $"{effort:0.#} hours"
                };
            }
            else
            {
                // For decimal values
                return $"{effort:0.#} hours";
            }
        }
    }
}