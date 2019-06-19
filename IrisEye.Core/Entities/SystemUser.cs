namespace IrisEye.Core.Entities
{
    public class SystemUser
    {
        public long Id { get; protected set; }
        public string EntityId { get; set; }
        public string Name { get; set; }
        public string[] GithubAliases { get; set; }
        
        public string GitHubAccount { get; set; }
        public string GitHubToken { get; set; }
        
        public Folder SelectedFolder { get; set; }
    }
}