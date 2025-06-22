using MiracleListVueHybridDemo.Dtos;
using System.Text;
using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo.Utils
{
  /// <summary>
  /// Provides formatting methods for MiracleTask objects
  /// </summary>
  public static class TaskFormatter
  {
    /// <summary>
    /// Formats a MiracleTask into a nicely formatted string representation for sharing
    /// </summary>
    /// <param name="task">The task to format</param>
    /// <returns>A formatted string representation of the task</returns>
    public static string FormatForSharing(MiracleTask task)
    {
      if (task == null)
        throw new ArgumentNullException(nameof(task));

      var sb = new StringBuilder();

      // Add a title with emphasis
      sb.AppendLine($"📝 TASK: {task.Title.ToUpper()} 📝");
      sb.AppendLine(new string('-', 40));

      // Format date information
      sb.AppendLine($"📅 Created: {task.Created.ToShortDateString()}");
      sb.AppendLine($"⏰ Due: {task.Due.ToShortDateString()} ({task.DueInDays} days remaining)");
      sb.AppendLine();

      // Format task details
      sb.AppendLine("✨ DETAILS ✨");
      sb.AppendLine($"Priority: {GetImportanceText(task.Importance)}");
      sb.AppendLine($"Status: {(task.Done ? "✓ Completed" : "⭕ Open")}");
      sb.AppendLine($"Effort: {GetEffortText(task.Effort)}");
      sb.AppendLine();

      // Add task notes if present
      if (!string.IsNullOrWhiteSpace(task.Note))
      {
        sb.AppendLine("📌 NOTES 📌");
        sb.AppendLine(task.Note);
        sb.AppendLine();
      }

      // Add subtasks with formatted bullets
      if (task.subTaskSet != null && task.subTaskSet.Length > 0)
      {
        sb.AppendLine("📋 SUBTASKS 📋");
        foreach (var subtask in task.subTaskSet)
        {
          sb.AppendLine($" {(subtask.Done ? "✓" : "□")} {subtask.Title}");
        }
        sb.AppendLine();
      }

      // Add footer
      sb.AppendLine(new string('-', 40));
      sb.AppendLine("Shared from MiracleList");

      return sb.ToString();
    }

    /// <summary>
    /// Helper method to convert importance number to meaningful text
    /// </summary>
    private static string GetImportanceText(int importance)
    {
      return importance switch
      {
        0 => "⚪ Low",
        1 => "🟡 Medium",
        2 => "🔴 High",
        _ => $"⚪ Level {importance}"
      };
    }

    /// <summary>
    /// Helper method to convert effort number to meaningful text
    /// </summary>
    private static string GetEffortText(double effort)
    {
      // For exact values
      if (Math.Abs(effort - Math.Floor(effort)) < 0.001)
      {
        return (int)effort switch
        {
          0 => "⚪ Minimal",
          1 => "🟡 Medium",
          2 => "🔴 Significant",
          _ => $"⚪ {effort:0.#} hours"
        };
      }
      else
      {
        // For decimal values
        return $"⚪ {effort:0.#} hours";
      }
    }
  }
}