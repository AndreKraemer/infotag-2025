/**
 * HybridWebView.js - Placeholder file for MAUI integration
 * 
 * This is a placeholder file that will be replaced at runtime in the MAUI app
 * using the WebResourceRequested event handler.
 * 
 * When running in a browser, these are empty implementations.
 * When running in MAUI, this entire file will be replaced with the actual implementation.
 */
 
window.HybridWebView = {
  // Core methods
  Init: function() {
    console.log("[Browser] HybridWebView.Init called (placeholder)");
  },
  
  SendRawMessage: function(message) {
    console.log("[Browser] HybridWebView.SendRawMessage called with:", message);
  },
  
  InvokeDotNet: function(methodName, params) {
    console.log("[Browser] HybridWebView.InvokeDotNet called:", methodName, params);
    // Return a resolved Promise to prevent errors when called
    return Promise.resolve({ 
      success: false, 
      message: "This is a browser environment. Native methods are not available." 
    });
  },
  
  // Internal methods (used by the MAUI implementation)
  __SendMessageInternal: function(type, message) {
    console.log("[Browser] HybridWebView.__SendMessageInternal called:", type, message);
  },
  
  __InvokeJavaScript: function(taskId, methodName, args) {
    console.log("[Browser] HybridWebView.__InvokeJavaScript called:", taskId, methodName, args);
    return Promise.resolve(null);
  },
  
  __TriggerAsyncCallback: function(taskId, result) {
    console.log("[Browser] HybridWebView.__TriggerAsyncCallback called:", taskId, result);
  },
  
  __TriggerAsyncFailedCallback: function(taskId, error) {
    console.log("[Browser] HybridWebView.__TriggerAsyncFailedCallback called:", taskId, error);
  }
};

// Create a custom event to dispatch messages from HybridWebView
function createHybridWebViewMessageEvent(message) {
  return new CustomEvent('HybridWebViewMessageReceived', {
    detail: { message: message }
  });
}

// For browser testing: allow simulating messages from native side
window.simulateNativeMessage = function(message) {
  console.log("[Browser] Simulating message from native side:", message);
  window.dispatchEvent(createHybridWebViewMessageEvent(message));
};

console.log("[Browser] HybridWebView placeholder loaded - this file will be replaced in MAUI");