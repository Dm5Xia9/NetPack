namespace Netpack.Hosting.Processes.Observer
{
    public class ProcessObserverOptions
    {
        public required string NPOPath { get; set; }
        public required string WorkFolderPath { get; set; }

        public string ProcessesFile => Path.Combine(WorkFolderPath, "processes.json");
    }
}
