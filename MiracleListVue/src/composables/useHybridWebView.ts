import { ref, onMounted, onUnmounted, inject } from 'vue'
import { MiracleListProxy } from '@/services/MiracleListProxyV2'
import { AppState } from '@/services/AppState'
import { useToast, POSITION } from 'vue-toastification'



/**
 * Hybrid WebView integration for MAUI and browser environments
 * Simplified for demo purposes
 */
export function useHybridWebView() {
  const isAvailable = ref(false)
  const proxy = inject('MiracleListProxy') as MiracleListProxy
  const toast = useToast()

  onMounted(() => {
    // 1. Check if we're running in MAUI
    isAvailable.value = typeof window.HybridWebView !== 'undefined';
    console.log("HybridWebView available:", isAvailable.value);

    // Listen for messages from MAUI - ensure global registration
    if (isAvailable.value) {
      // Remove any existing listener to prevent duplicates
      window.removeEventListener('HybridWebViewMessageReceived', handleMessage as EventListener);
      // Add the listener
      window.addEventListener('HybridWebViewMessageReceived', handleMessage as EventListener);
      console.log("HybridWebView message listener registered");
    }

    // Wire up the export functions to the window object
    window.Export = exportAllData;
    window.ExportCurrentCategory = exportCurrentCategory;
  });

  onUnmounted(() => {
    window.removeEventListener('HybridWebViewMessageReceived', handleMessage as EventListener)
  });
  
  

  // 1. DEMO: Share a task (works in both MAUI and browser)
  const shareTask = async (task: any): Promise<{ success: boolean, message?: string }> => {
    // In browser - use Web Share API if available
    if (!isAvailable.value) {
      if (typeof navigator.share === 'function') {
        try {
          await navigator.share({
            title: `Task: ${task.title}`,
            text: `Task: ${task.title}\nDue: ${task.due ? new Date(task.due).toLocaleDateString() : 'No due date'}`
          });
          return { success: true, message: 'Shared via Web Share API' };
        } catch (error) {
          if ((error as Error).name !== 'AbortError') {
            console.error("Web Share API error:", error);
          }
        }
      }

      return { success: false, message: 'Sharing unavailable in this browser' };
    }
    // In MAUI - call native Share method
    try {
      const result = await callDotNetMethod('ShareCurrentTask', task);
      return result || { success: false, message: 'No response from native side' };
    } catch (error) {
      return { success: false, message: String(error) };
    }
  }; 


  // 2. DEMO: Export all categories with tasks and subtasks
  const exportAllData = async (): Promise<{
    success: boolean;
    message: string;
    data?: Array<Category>;
  }> => {
    try {
      if (!AppState.Authenticated) {
        return { success: false, message: 'Not authenticated' };
      }

      // Get all categories
      const categories = await proxy.categorySet(AppState.Token);
      if (!categories || categories.length === 0) {
        return { success: false, message: 'No categories found' };
      }

      // Map each category using the helper function
      const result: Category[] = [];
      
      for (const category of categories) {
        const mappedCategory = await mapCategory(category);
        if (mappedCategory) {
          result.push(mappedCategory);
        }
      }

      return {
        success: true,
        message: 'Data exported successfully',
        data: result
      };
    } catch (error) {
      return { success: false, message: `Export failed: ${error}` };
    }
  };

  // 3. DEMO: Export only the current category
  const exportCurrentCategory = async (): Promise<{
    success: boolean;
    message: string;
    data?: Category;
  }> => {
    try {
      if (!AppState.Authenticated) {
        return { success: false, message: 'Not authenticated' };
      }

      // Get all categories
      const categories = await proxy.categorySet(AppState.Token);
      if (!categories || categories.length === 0) {
        return { success: false, message: 'No categories found' };
      }

      // Use the actual current category from AppState if available
      const currentCategoryID = AppState.CurrentCategoryID.value;
      let currentCategory;

      if (currentCategoryID) {
        // Find the category by ID
        currentCategory = categories.find(c => c.categoryID === currentCategoryID);
      }

      // Fallback to first category if no current category is set
      if (!currentCategory) {
        currentCategory = categories[0];
        console.log('No current category found, using first category as fallback');
      }

      if (!currentCategory?.categoryID) {
        return { success: false, message: 'Current category not found' };
      }

      // Map the current category using the helper function
      const categoryData = await mapCategory(currentCategory);
      
      if (!categoryData) {
        return { success: false, message: 'Failed to process category data' };
      }

      return {
        success: true,
        message: `Category "${currentCategory.name}" exported`,
        data: categoryData
      };
    } catch (error) {
      return { success: false, message: `Export failed: ${error}` };
    }
  };

 

  // Helper function to map a subtask to the export format
  const mapSubTask = (subTask: any, taskID: number): SubTask => ({
    subTaskID: subTask.subTaskID || 0,
    title: subTask.title || '',
    done: subTask.done || false,
    created: subTask.created || new Date().toISOString(),
    taskID: taskID
  });

  // Helper function to map a task to the export format
  const mapTask = (task: any, categoryID: number): Task => ({
    taskID: task.taskID || 0,
    title: task.title || '',
    created: task.created || new Date().toISOString(),
    due: task.due || null,
    importance: task.importance || 0,
    note: task.note || '',
    done: task.done || false,
    effort: task.effort || 0,
    order: task.order || 0,
    dueInDays: task.dueInDays || 0,
    categoryID: categoryID,
    subTaskSet: (task.subTaskSet || []).map((subTask: any) => 
      mapSubTask(subTask, task.taskID || 0)
    )
  });

  // Helper function to map a category to the export format
  const mapCategory = async (category: any): Promise<Category | null> => {
    if (!category.categoryID) return null;
    
    const tasks = await proxy.taskSet(category.categoryID, AppState.Token);
    
    return {
      categoryID: category.categoryID,
      name: category.name || '',
      tasks: tasks.map(task => mapTask(task, category.categoryID!))
    };
  };

   // Simple message handler for native messages
  const handleMessage = (event: CustomEvent) => {
    if (!event?.detail?.message || !isAvailable.value) return;

    const message = event.detail.message;
    console.log('Message from native:', message);

    toast.info(`Message from MAUI: ${message}`, {
      timeout: 5000,
      position: POSITION.TOP_RIGHT
    });
  };

  // Call a .NET method
  const callDotNetMethod = async <T = any>(methodName: string, ...params: any[]): Promise<T | null> => {
    if (!isAvailable.value) return null;

    try {
      return await window.HybridWebView.InvokeDotNet(methodName, params);
    } catch (error) {
      console.error(`Error calling .NET method ${methodName}:`, error);
      return null;
    }
  };
  
  return {
    isAvailable,
    callDotNetMethod,
    shareTask,
    isRunningInMaui: isAvailable,
    // Expose sendMessage as a convenience method
    sendMessage: (message: string) => {
      if (isAvailable.value && window.HybridWebView) {
        try {
          window.HybridWebView.SendRawMessage(message);
          console.log('Sent message to HybridWebView:', message);
          return true;
        } catch (error) {
          console.error('Error sending message to HybridWebView:', error);
          return false;
        }
      }
      console.warn('HybridWebView not available, message not sent:', message);
      return false;
    }
  }
}
// Type definitions for task data structures
type SubTask = {
  subTaskID: number;
  title: string;
  done: boolean;
  created: string | Date;
  taskID: number;
}

type Task = {
  taskID: number;
  title: string;
  created: string | Date;
  due: Date | null;
  importance: number;
  note: string;
  done: boolean;
  effort: number;
  order: number;
  dueInDays: number;
  categoryID: number;
  subTaskSet: SubTask[];
}

type Category = {
  categoryID: number;
  name: string;
  tasks: Task[];
}
// Type definitions for the global window methods
declare global {
  interface Window {
    HybridWebView: any;
    Export: () => Promise<{
      success: boolean;
      message: string;
      data?: Array<Category>;
    }>;
    ExportCurrentCategory: () => Promise<{
      success: boolean;
      message: string;
      data?: Category;
    }>;
  }
}
