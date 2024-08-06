namespace TDL.Client.Runner
{
    public class RunnerAction
    {
        public string ShortName { get; }

        private RunnerAction(string shortName)
        {
            ShortName = shortName;
        }

        public static readonly RunnerAction GetNewRoundDescription = new RunnerAction("new");
        public static readonly RunnerAction DeployToProduction = new RunnerAction("deploy");

        public static readonly RunnerAction[] AllActions =
        {
            GetNewRoundDescription,
            DeployToProduction
        };
    }
}
