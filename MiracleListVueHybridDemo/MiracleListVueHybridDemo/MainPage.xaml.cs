using MiracleListVueHybridDemo.Dtos;
using MiracleListVueHybridDemo.Utils;
using System.Text;
using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();
      hybridWebView.SetInvokeJavaScriptTarget(this);
    }

    private void OnSendMessageButtonClicked(object sender, EventArgs e)
    {
      hybridWebView.SendRawMessage(messageText.Text);
    }

    private async void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
      await Dispatcher.DispatchAsync(async () =>
      {
        await DisplayAlertAsync($"Raw Message Received: {e.Message}", e.Message, "OK");
      });
    }

    public async Task<Result> ShareCurrentTask(MiracleTask task)
    {
      try
      {
        if (task == null)
        {
          return new Result { Success = false, Message = "No task provided" };
        }

        // Use the TaskFormatter class instead of the local method
        var taskText = TaskFormatter.FormatForSharing(task);

        await Share.Default.RequestAsync(new ShareTextRequest
        {
          Text = taskText,
          Title = $"Task: {task.Title}"
        });

        return new Result { Success = true, Message = "Task shared successfully" };
      }
      catch (Exception ex)
      {
        return new Result { Success = false, Message = $"Error sharing: {ex.Message}" };
      }
    }

    private async void OnExportCurrentCategory(object sender, EventArgs e)
    {
      try
      {


        // Get the current category from JavaScript
        Result<Category>? result = await hybridWebView.InvokeJavaScriptAsync(
          "ExportCurrentCategory",
          HybridSampleJSContext.Default.ResultCategory);

        if (result?.Success == true && result.Data != null)
        {
          Category category = result.Data;

          // Export the category to a Word document
          string filePath = await WordExporter.ExportCategoryToWordAsync(category);

          // Share the document
          await ShareFile(filePath, category.Name);

          // Show success message
          await Dispatcher.DispatchAsync(async () =>
          {
            await DisplayAlertAsync("Export Successful",
                                   $"Category '{category.Name}' with {category.Tasks?.Length ?? 0} tasks has been exported to Word.",
                                   "OK");
          });
        }
        else
        {
          // Show error message
          await Dispatcher.DispatchAsync(async () =>
          {
            await DisplayAlertAsync("Export Failed",
                                   $"Failed to export category: {result?.Message ?? "Unknown error"}",
                                   "OK");
          });
        }
      }
      catch (Exception ex)
      {
        // Show exception message
        await Dispatcher.DispatchAsync(async () =>
        {
          await DisplayAlertAsync("Export Error",
                                 $"An error occurred while exporting: {ex.Message}",
                                 "OK");
        });
      }
    }

    /// <summary>
    /// Shares a file with the system's share feature
    /// </summary>
    private async Task ShareFile(string filePath, string title)
    {
      try
      {
        var launchResult = await Launcher.OpenAsync(new OpenFileRequest
        {
          File = new ReadOnlyFile(filePath)
        });
        if (!launchResult)
        {
          var shareFile = new ShareFileRequest
          {
            Title = $"MiracleList Category: {title}",
            File = new ShareFile(filePath)
          };

          await Share.Default.RequestAsync(shareFile);
        }
      }


      catch (Exception ex)
      {
        Console.WriteLine($"Error opening file: {ex.Message}");
      }
    }

    private async void OnExportAllTasks(object sender, EventArgs e)
    {
      try
      {

        // Get all categories from JavaScript
        Result<List<Category>>? result = await hybridWebView.InvokeJavaScriptAsync(
          "Export",
          HybridSampleJSContext.Default.ResultListCategory);

        if (result?.Success == true && result.Data != null && result.Data.Count > 0)
        {
          List<Category> categories = result.Data;

          // Export all categories to a Word document
          string filePath = await WordExporter.ExportAllCategoriesToWordAsync(categories);

          // Get some statistics for the success message
          int totalCategories = categories.Count;
          int totalTasks = categories.Sum(c => c.Tasks?.Length ?? 0);
          int totalSubtasks = categories.Sum(c => c.Tasks?.Sum(t => t.subTaskSet?.Length ?? 0) ?? 0);

          // Share the document
          await ShareFile(filePath, "All Categories");

          // Show success message
          await Dispatcher.DispatchAsync(async () =>
          {
            await DisplayAlertAsync("Export Successful", 
                                   $"Exported {totalCategories} categories with {totalTasks} tasks and {totalSubtasks} subtasks to Word.",
                                   "OK");
          });
        }
        else
        {
          // Show error or no data message
          string message = result?.Success == true && (result.Data == null || result.Data.Count == 0)
                           ? "No categories found to export."
                           : $"Failed to export categories: {result?.Message ?? "Unknown error"}";

          await Dispatcher.DispatchAsync(async () =>
          {
            await DisplayAlertAsync("Export Failed", message, "OK");
          });
        }
      }
      catch (Exception ex)
      {
        // Show exception message
        await Dispatcher.DispatchAsync(async () =>
        {
          await DisplayAlertAsync("Export Error", 
                                 $"An error occurred while exporting: {ex.Message}", 
                                 "OK");
        });
      }
    }

    private void OnHybridWebViewWebResourceRequested(object sender, HybridWebViewWebResourceRequestedEventArgs e)
    {

      // NOTES:
      // * This method MUST be synchronous, as it is called from the WebView's thread.
      // * This method MUST return a response (even if it is not yet complete)
      // * The response must be set using the SetResponse method of the event args.

      // Only handle requests for the specific JavaScript file
      if (!e.Uri.ToString().EndsWith("/js/hybridwebview.js", StringComparison.OrdinalIgnoreCase))
        return;

      // Prevent the default behavior of the web view
      e.Handled = true;

      // Set the response with our custom hybridwebview.js content
      e.SetResponse(200, "OK", "application/javascript", GetHybridWebViewScriptStream());
    }
    private Stream GetHybridWebViewScriptStream()
    {
      try
      {
        var jsFileTask = FileSystem.OpenAppPackageFileAsync("Scripts/HybridWebView.js");

        // We need to return a Stream synchronously, so we'll create a copy
        using (var fileStream = jsFileTask.Result)
        {
          var memoryStream = new MemoryStream();
          fileStream.CopyTo(memoryStream);
          memoryStream.Position = 0;
          return memoryStream;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading HybridWebView script: {ex.Message}");

        // Fallback to a basic implementation if the resource is not found
        var basicScript = @"
                window.HybridWebView = {
                    Init: function() { console.log('HybridWebView initialized'); },
                    SendRawMessage: function(msg) { console.log('Message: ' + msg); },
                    InvokeDotNet: function(method, params) { console.log('Called: ' + method); return Promise.resolve(null); }
                };
                console.log('Basic HybridWebView loaded');
            ";

        return new MemoryStream(Encoding.UTF8.GetBytes(basicScript));
      }
    }
  }

  [JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Result<List<Category>>))]
[JsonSerializable(typeof(Result<Category>))]
internal partial class HybridSampleJSContext : JsonSerializerContext
{
  // This type's attributes specify JSON serialization info to preserve type structure
  // for trimmed builds.    
}
}
