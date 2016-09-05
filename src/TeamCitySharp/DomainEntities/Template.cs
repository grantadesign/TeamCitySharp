namespace TeamCitySharp.DomainEntities
{
    public class Template
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Project Project { get; set; }
        public Parameters Parameters { get; set; }
        public ArtifactDependencies ArtifactDependencies { get; set; }
        public SnapshotDependencies SnapshotDependencies { get; set; }
        public VcsRootEntries VcsRootEntries { get; set; }
        public BuildSteps Steps { get; set; }
        public AgentRequirements AgentRequirements { get; set; }
        public BuildFeatures Features { get; set; }
        public BuildTriggers Triggers { get; set; }
        public Properties Settings { get; set; }
    }
}