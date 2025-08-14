using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using DataAICapstone.Models;
using DataAICapstone.Services;
using System.Text.RegularExpressions;

namespace DataAICapstone.Components.Pages
{
    public partial class Chat : ComponentBase
    {
        [Inject] private IChatService ChatService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        private ChatSession? currentSession;
        private string currentMessage = string.Empty;
        private bool isProcessing = false;
        private ElementReference messageInput;

        protected override async Task OnInitializedAsync()
        {
            currentSession = await ChatService.CreateSessionAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await ScrollToBottom();
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(currentMessage) || isProcessing)
                return;

            var message = currentMessage.Trim();
            currentMessage = string.Empty;
            isProcessing = true;

            try
            {
                StateHasChanged();
                await ScrollToBottom();

                var response = await ChatService.SendMessageAsync(message, currentSession?.Id);
                
                // The ChatService will add messages to the session
                // We just need to trigger a UI update
                StateHasChanged();
                await ScrollToBottom();
            }
            catch (Exception)
            {
                // Handle error
                var errorMessage = new ChatMessage
                {
                    Content = "Sorry, I encountered an error processing your message. Please try again.",
                    Role = MessageRole.System,
                    AgentName = "System"
                };
                
                currentSession?.Messages.Add(errorMessage);
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
                await messageInput.FocusAsync();
            }
        }

        private async Task HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
            {
                await SendMessage();
            }
        }

        private async Task ScrollToBottom()
        {
            await JSRuntime.InvokeVoidAsync("scrollToBottom", "chatMessages");
        }

        private static string FormatMessageContent(string content)
        {
            // Basic markdown-like formatting
            content = content.Replace("\n", "<br>");
            
            // Bold text
            content = Regex.Replace(content, @"\*\*(.*?)\*\*", "<strong>$1</strong>");
            
            // Links
            content = Regex.Replace(content, 
                @"\[([^\]]+)\]\(([^)]+)\)", 
                "<a href=\"$2\" target=\"_blank\" rel=\"noopener noreferrer\">$1</a>");
            
            // Code blocks
            content = Regex.Replace(content, @"`([^`]+)`", "<code>$1</code>");
            
            return content;
        }
    }
}
