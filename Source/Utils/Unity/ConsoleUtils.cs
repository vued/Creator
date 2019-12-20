namespace Innoactive.Hub.Training.Unity.Utils
{
    public static class ConsoleUtils
    {
        private const string tab = "    ";

        /// <summary>
        /// Returns a string containing tabs. One tab is four spaces.
        /// </summary>
        /// <param name="tabsCount">Amount of tabs.</param>
        /// <returns></returns>
        public static string GetTabs(uint tabsCount = 1)
        {
            string result = "";

            for (uint i = 0; i < tabsCount; i++)
            {
                result += tab;
            }

            return result;
        }
    }
}
