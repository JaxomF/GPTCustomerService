namespace GPTCustomerService_Web.Utilities
{
    public enum SkillFileType
    {
        Prompt,
        Config
    }
    public class ReadSkill
    {
        public async static Task<string?> Read(string path, SkillFileType skillFileType)
        {
            switch (skillFileType)
            {
                case SkillFileType.Prompt:
                    return await File.ReadAllTextAsync(System.IO.Path.Combine(path, "skprompt.txt"));

                case SkillFileType.Config:
                    return await File.ReadAllTextAsync(System.IO.Path.Combine(path, "config.json"));

                default:
                    break;
            }
            return default;

        }
    }
}
