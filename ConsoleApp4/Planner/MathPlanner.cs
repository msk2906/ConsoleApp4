using Microsoft.SemanticKernel;
using System.ComponentModel;

public class MathPlanner
{
    [KernelFunction]
    [Description("Returns back the Math calculation.")]
    [return: Description("Return the calculated answer")]
    public async Task<string> GenerateRequiredStepsAsync(
        Kernel kernel,
        [Description("Number 1")] string number1,
        [Description("Number 2")] string number2
    )
    {
        // Prompt the LLM to generate a list of steps to complete the task
        var result = await kernel.InvokePromptAsync($"""
        I'm going to calculate the {number1} and {number2} on behalf of user.
        """, new() {
            { "Number1", number1 },
            { "Number2", number2 }
        });

        // Return the plan back to the agent
        return result.ToString();
    }
}