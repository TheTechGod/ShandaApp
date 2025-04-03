using ShandaApp.Models.Enums;

namespace ShandaApp.Services
{
    public class AIResponseService
    {
        public string GenerateContextAwareGreeting(string role, bool isPersonalMode, string firstName, AIMood mood, string area, string controller, string action)
        {
            string tone = mood switch
            {
                AIMood.Friendly => "Hey there",
                AIMood.Professional => "Welcome back",
                AIMood.Playful => "Guess who's back?",
                AIMood.Calm => "Hello again",
                _ => "Greetings"
            };

            string baseName = string.IsNullOrWhiteSpace(firstName) ? "User" : firstName;
            string prefix = isPersonalMode ? $"{tone}, {baseName}." : $"{tone}.";

            return $"{prefix} You're in the {controller}/{action} area as a {role}.";
        }
    }
}
