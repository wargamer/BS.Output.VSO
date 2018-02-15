using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BS.Output.VSO.Models;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation;

namespace BS.Output.VSO.Services
{
    public class VSOClient
    {
        private const string ReproStepsFieldPath = "/fields/Microsoft.VSTS.TCM.ReproSteps";
        private const string TitleFieldPath = "/fields/System.Title";
        private const string IterationFieldPath = "/fields/System.IterationPath";
        private const string FoundInBuildFieldPath = "/fields/Microsoft.VSTS.Build.FoundIn";

        private const string WorkItemType = "Bug";

        private VssConnection _connection;
        private readonly VSOOutput _output;
        private WorkItemTrackingHttpClient _workItemClient;
        private ProjectHttpClient _projectClient;
        private WorkHttpClient _workClient;
        private BuildHttpClient _buildClient;

        public VSOClient(VSOOutput output)
        {
            _output = output;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        /// Attempts to set up a connection to the VSO URL specified by the <see cref="VSOOutput"/>
        /// </summary>
        /// <returns>True if the connection was set up successfully</returns>
        public async Task<bool> Connect()
        {
            try
            {
                var creds = new VssClientCredentials
                {
                    PromptType = CredentialPromptType.PromptIfNeeded
                };
                _connection = new VssConnection(_output.URL, creds);
                _workItemClient = await _connection.GetClientAsync<WorkItemTrackingHttpClient>();
                _projectClient = await _connection.GetClientAsync<ProjectHttpClient>();
                _workClient = await _connection.GetClientAsync<WorkHttpClient>();
                _buildClient = await _connection.GetClientAsync<BuildHttpClient>();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new bugitem with the given <paramref name="details"/> and the <paramref name="pictureOfBug"/> in the repro steps
        /// </summary>
        public async Task CreateBug(BugDetails details, ImageData pictureOfBug)
        {
            string filename = $"{pictureOfBug.Title}.bmp";
            var stream = ImageToStream(pictureOfBug.Image);
            var attachment = await _workItemClient.CreateAttachmentAsync(stream, filename);

            string reproAsHtml = details.ReproSteps.Replace("\n", "<br />");

            string urlEncodedFilename = WebUtility.UrlEncode(filename);
            string url = string.Format($"{_output.URL}workitemtracking/v1.0/"
                                        + $"attachfilehandler.ashx?filenameguid={attachment.Id}&filename={urlEncodedFilename}");

            JsonPatchDocument doc = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Path = TitleFieldPath,
                    Operation = Operation.Add,
                    Value = details.Title
                },
                new JsonPatchOperation
                {
                    Path = ReproStepsFieldPath,
                    Operation = Operation.Add,
                    Value = $"{reproAsHtml}<br /><img src='{url}' />"
                }
            };

            if (!string.IsNullOrWhiteSpace(_output.IterationName))
            {
                doc.Add(new JsonPatchOperation
                {
                    Path = IterationFieldPath,
                    Operation = Operation.Add,
                    Value = _output.IterationName
                });
            }

            if (!string.IsNullOrEmpty(_output.BuildDefinitionName) && !string.IsNullOrEmpty(details.Build))
            {
                doc.Add(new JsonPatchOperation
                {
                    Path = FoundInBuildFieldPath,
                    Operation = Operation.Add,
                    Value = $"{details.Build}"
                });
            }

            await _workItemClient.CreateWorkItemAsync(doc, _output.ProjectName, WorkItemType);
        }

        /// <summary>
        /// Gets all project names available
        /// </summary>
        public async Task<IEnumerable<string>> GetProjects()
        {
            var projects = await _projectClient.GetProjects();
            return projects.Select(p => p.Name);
        }

        /// <summary>
        /// Gets all iteration paths for the project with the given <paramref name="projectName"/>
        /// </summary>
        public async Task<IEnumerable<string>> GetIterations(string projectName)
        {
            var project = await _projectClient.GetProject(projectName);
            if (project == null)
                return new List<string>();

            var iterations = await _workClient.GetTeamIterationsAsync(new TeamContext(project.Name, project.DefaultTeam.Name));
            return iterations.Select(i => i.Path);
        }

        /// <summary>
        /// Gets all the build definitions by name
        /// </summary>
        /// <param name="projectName">Name of the project which the definitions should be linked to</param>
        public async Task<IEnumerable<string>> GetBuildConfigurations(string projectName)
        {
            var buildDefinitions = await _buildClient.GetDefinitionsAsync(project : projectName);
            return buildDefinitions.Select(i => i.Name);
        }

        /// <summary>
        /// Gets all builds associated with the pair of <paramref name="projectName"/> and <paramref name="buildDefinitionName"/>
        /// </summary>
        public async Task<IEnumerable<string>> GetBuilds(string projectName, string buildDefinitionName)
        {
            var buildDefinitions = await _buildClient.GetDefinitionsAsync(project: projectName);
            var buildDefinition = buildDefinitions.FirstOrDefault(b => b.Name == buildDefinitionName);
            if (buildDefinition == null)
                return new List<string>();

            var builds = await _buildClient.GetBuildsAsync(projectName, new List<int>() {buildDefinition.Id});
            return builds.Select(b => b.BuildNumber);
        }

        private static MemoryStream ImageToStream(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Since this output has quite a few dependencies which are not present in the BugShooting folder
            // we'll need to direct the runtime to the folder our copy of BS.Output.VSO is located in
            string filePath = GetPath(args.Name.Split(',')[0]);
            if (!File.Exists(filePath))
                return null;
            var file = Assembly.LoadFile(filePath);
            return file;
        }

        /// <summary>
        /// Gets the path to the <paramref name="dllName"/>, in the same directory as where BS.Output.VSO was loaded from
        /// </summary>
        private static string GetPath(string dllName)
        {
            string path = new Uri(typeof(VSOClient).Assembly.CodeBase).LocalPath;
            return $"{Path.GetDirectoryName(path)}\\{dllName}.dll";
        }
    }
}