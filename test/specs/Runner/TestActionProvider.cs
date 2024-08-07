using TDL.Client.Runner;

namespace TDL.Test.Specs.Runner
{
    public class TestActionProvider : IActionProvider
    {
        private string value = null;

        public string Get()
        {
            return value;
        }

        public void Set(string value)
        {
            this.value = value;
        }
    }
}
