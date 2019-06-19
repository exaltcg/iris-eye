using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace IrisEye.Data.Parsers
{
    public class GithubApiClient
    {

        private readonly RestClient _restClient;
        private readonly string _login;
        private const string BaseUrl = "https://api.github.com";

        public GithubApiClient()
        {
            _restClient = new RestClient(BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("", ""), 
                UserAgent = "Iris"
            };
            _restClient.AddDefaultHeader("Accept", "application/vnd.github.inertia-preview+json");
        }
        public GithubApiClient(string login, string password)
        {
            _login = login;
            _restClient = new RestClient(BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(login, password), 
                UserAgent = "Iris"
            };
            _restClient.AddDefaultHeader("Accept", "application/vnd.github.inertia-preview+json");
        }

        public bool CredentialsAreValid()
        {
            var request = new RestRequest("/repos/mozilla/iris", Method.GET);
            var response = _restClient.Execute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public GitHubPullRequest CreateNewPullRequest(string title, string body, string branch)
        {
            var request = new RestRequest("/repos/mozilla/iris/pulls", Method.POST);
            var newPullRequest = new GitHubNewPullRequest
            {
                title = title,
                body = body,
                head = "mozilla:issue_"+branch,
                @base = "dev"
            };
            request.AddJsonBody(newPullRequest);
            var response = _restClient.Execute(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<GitHubPullRequest>(response.Content);
            }
            var error = JsonConvert.DeserializeObject<GitHubError>(response.Content);
            throw new Exception(error.errors?.FirstOrDefault()?.message);
        }
        
        public IEnumerable<TestInfo> GetTestsInfo()
        {
            var ret = new List<TestInfo>();
            
            var request = new RestRequest("/repos/mozilla/iris/contents/iris/tests", Method.GET);
            request.AddParameter("ref", "dev");


            var allFolders = _restClient.Execute(request).Content;
            var folders = JsonConvert.DeserializeObject<List<GitHubContent>>(allFolders);
            foreach (var folder in folders.Where(p => p.type == "dir"))
            {
                var folderRequest = new RestRequest($@"/repos/mozilla/iris/contents/iris/tests/{folder.name}");
                var jsonFolderContent = _restClient.Execute(folderRequest).Content;
                var tests = JsonConvert.DeserializeObject<List<GitHubContent>>(jsonFolderContent);
                foreach (var test in tests.Where(p => p.type == "file"))
                {
                    var newTestInfo = new TestInfo
                    {
                        TestName = test.name, 
                        SuiteName = folder.name
                    };
                    var commitsRequest = new RestRequest($@"/repos/mozilla/iris/commits?path=/iris/tests/{folder.name}/{test.name}");
                    commitsRequest.AddParameter("sha", "dev");
                    var jsonCommitHistory = _restClient.Execute(commitsRequest).Content;

                    var commits = JsonConvert.DeserializeObject<List<GitHubCommits>>(jsonCommitHistory);
                    newTestInfo.AuthorLogin = commits.FirstOrDefault()?.commit.author.name;
                    newTestInfo.AuthorGitHubId = commits.FirstOrDefault()?.commit.author.email.Split('+')[0];
                                        
                    ret.Add(newTestInfo);
                }
            }
            return ret;
        }

        public bool UserExists(string username)
        {
            var request = new RestRequest("/users/"+username, Method.GET);
            return !_restClient.Execute(request).Content.Contains("Not Found");
        }

        public GitHubProjectDetails GetIssuesForProject(int id)
        {
            var details = new GitHubProjectDetails {GitHubIssues = new List<GitHubIssue>()};
            var listOfIssues = new List<CardForIssue>();
            var projects = GetGitHubProjects();
            var selectedProject = projects.FirstOrDefault(p => p.number == id);
            if (selectedProject==null) throw new Exception("Project with requested id was not found.");
            details.Name = selectedProject.name;
            details.AddedOn = selectedProject.created_at;
            details.Body = selectedProject.body;
            var columns = GetGitHubColumnses(selectedProject);
            foreach (var col in columns)
            {
                var cardsRequest = new RestRequest($@"/projects/columns/{col.id}/cards", Method.GET);

                var cardsResponse = _restClient.Execute(cardsRequest, Method.GET);
                var cardsJson = cardsResponse.Content;
                var cards = JsonConvert.DeserializeObject<List<GitHubCard>>(cardsJson);
                foreach (var card in cards)
                {
                    listOfIssues.Add(new CardForIssue
                    {
                        Issue = card.content_url,
                        Column =  col.name
                    }); 
                }
            }
            
            //process Issues
            foreach (var issue in listOfIssues)
            {
                var issueRequest = new RestRequest(issue.Issue.Replace("https://api.github.com",""), Method.GET);

                var issueJson = _restClient.Execute(issueRequest).Content;
                var finalIssue = JsonConvert.DeserializeObject<GitHubIssue>(issueJson);
                finalIssue.Column = issue.Column;
                details.GitHubIssues.Add(finalIssue);
            }
            
            return details;
        }

        private List<GitHubColumns> GetGitHubColumnses(GitHubProject selectedProject)
        {
            var projectsColumnsRequest = new RestRequest($@"/projects/{selectedProject.id}/columns", Method.GET);

            var columnsJson = _restClient.Execute(projectsColumnsRequest).Content;
            var columns = JsonConvert.DeserializeObject<List<GitHubColumns>>(columnsJson);
            return columns;
        }

        private IEnumerable<GitHubProject> GetGitHubProjects()
        {
            var projectsRequest = new RestRequest("/repos/mozilla/iris/projects", Method.GET);

            var projectsJson = _restClient.Execute(projectsRequest).Content;
            var projects = JsonConvert.DeserializeObject<List<GitHubProject>>(projectsJson);
            return projects;
        }

        public GitHubIssue GetIssue(int issueId)
        {
            var request = new RestRequest("/repos/mozilla/iris/issues/"+issueId, Method.GET);
            var jsonResponse = _restClient.Execute(request).Content;
            var ret = JsonConvert.DeserializeObject<GitHubIssue>(jsonResponse);
            if (ret.title==null) throw new Exception("Request issue id was not found.");
            return ret;
        }

        public GitHubBranch GetBranch(string branchName)
        {
            var request = new RestRequest("/repos/mozilla/iris/branches/"+branchName, Method.GET);
            var jsonResponse = _restClient.Execute(request).Content;
            var ret = JsonConvert.DeserializeObject<GitHubBranch>(jsonResponse);
            return ret;

        }
        public GitHubPullRequest GetPullRequest(int id)
        {
            var request = new RestRequest("/repos/mozilla/iris/pulls/"+id, Method.GET); 
            var jsonResponse = _restClient.Execute(request).Content;
            if (jsonResponse.Contains("Not Found")) throw new Exception("Pull Request was not found.");
            var ret = JsonConvert.DeserializeObject<GitHubPullRequest>(jsonResponse);
            return ret;

        }

        public List<GitHubComment> GetCommentsForIssue(int issueId)
        {
            var request = new RestRequest($@"/repos/mozilla/iris/issues/{issueId}/comments", Method.GET); 
            var jsonResponse = _restClient.Execute(request).Content;
            if (jsonResponse.Contains("documentation_url")) throw new Exception("Issue was not found.");
            var ret = JsonConvert.DeserializeObject<List<GitHubComment>>(jsonResponse);
            return ret;
        }
        
        public List<GitHubReviewComment> GetPullRequestComments(int prId)
        {
            var request = new RestRequest($@"/repos/mozilla/iris/pulls/{prId}/reviews", Method.GET); 
            var jsonResponse = _restClient.Execute(request).Content;
            if (jsonResponse.Contains("documentation_url")) throw new Exception("PR was not found.");
            var ret = JsonConvert.DeserializeObject<List<GitHubReviewComment>>(jsonResponse);
            return ret;
        }

        public void AddAssignees(int id, string[] gitHubLogins)
        {
            var request = new RestRequest($@"/repos/mozilla/iris/issues/{id}/assignees", Method.POST);
            var data = new GitHubAssignees
            {
                assignees = gitHubLogins
            };
            request.AddJsonBody(data);
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Can't add requested assignee");
        }

        public void AddReviewer(int id, string[] gitHubLogins)
        {
            var request = new RestRequest($@"/repos/mozilla/iris/pulls/{id}/requested_reviewers", Method.POST);
            var data = new IrisEye.Core.Models.GitHubReviewers()
            {
                reviewers = gitHubLogins
            };
            request.AddJsonBody(data);
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Can't add requested reviewer");
 
        }

        public void AddComment(int issueId, string message)
        {
            var request = new RestRequest($@"/repos/mozilla/iris/issues/{issueId}/comments", Method.POST);
            var data = new GitHubNewComment()
            {
                body = message
            };
            request.AddJsonBody(data);
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created) throw new Exception("Can't create new GitHub comment");
        }

        public GitHubIssue CreateNewIssue(string title, string message, int projectId)
        {
            var projects = GetGitHubProjects();
            var project = projects.FirstOrDefault(p => p.number == projectId);
            if (project==null) throw new Exception("Selected GitHub project was not found");
            
            var request = new RestRequest($@"/repos/mozilla/iris/issues", Method.POST);
            var data = new GitHubNewIssue()
            {
                title = title,
                body = message,
                assignees = new List<string>(new string[] {_login})
            };
            request.AddJsonBody(data);
            var response = _restClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created) throw new Exception("Can't create new GitHub issue");
            var newIssueInfo = JsonConvert.DeserializeObject<GitHubIssue>(response.Content);
            var columns = GetGitHubColumnses(project);
            var column = columns.FirstOrDefault();
            if (column==null) throw new Exception("Can't find any columns in the GitHub project");
            
            var assignIssueRequest = new RestRequest($@"/projects/columns/{column.id}/cards", Method.POST);
            var assignIssueData = new GitHubNewCard()
            {
                content_type = "Issue",
                content_id = newIssueInfo.id
            };
            assignIssueRequest.AddJsonBody(assignIssueData);
            var assignResponse = _restClient.Execute(assignIssueRequest);
            if (assignResponse.StatusCode != HttpStatusCode.Created) throw new Exception("Can't assign new issue to project in GitHub");
            return newIssueInfo;

        }
    }
}