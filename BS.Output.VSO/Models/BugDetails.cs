namespace BS.Output.VSO.Models
{
    public class BugDetails : IOutputSendOptions
    {
        /// <summary>
        /// List of steps to reproduce
        /// </summary>
        public string ReproSteps { get; set; }

        /// <summary>
        /// Title of the work item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Name of the build which this bugitem is related to
        /// </summary>
        public string Build { get; set; }

        /// <summary>
        /// Return True if all required fields have been provided values for
        /// </summary>
        public bool Validate()
        {
            return !string.IsNullOrEmpty(Title)
                   && !string.IsNullOrEmpty(ReproSteps);
        }
    }
}