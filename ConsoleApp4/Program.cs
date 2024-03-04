using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Plugins;

var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion("chatdj", "https://azoaidj.openai.azure.com/", "7f629b03e6524736a64127d31abf5029");
builder.Plugins.AddFromType<MathPlugin>();
builder.Plugins.AddFromType<MathPlanner>();
Kernel kernel = builder.Build();

ChatHistory history = new(); 
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

Console.Write("User > ");
string? userInput;
while ((userInput = Console.ReadLine()) != null)
{
    history.AddUserMessage(userInput);

    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel);

    string fullMessage = "";
    var first = true;
    await foreach (var content in result)
    {
        if (content.Role.HasValue && first)
        {
            Console.Write("Assistant > ");
            first = false;
        }
        Console.Write(content.Content);
        fullMessage += content.Content;
    }
    Console.WriteLine();

    history.AddAssistantMessage(fullMessage);

    Console.Write("User > ");
}