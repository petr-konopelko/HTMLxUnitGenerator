namespace HTMLxUnitGenerator.Model
{
    public enum Status
    {
        Unknown, // unknown status that doesn't map to one of the below
        Skipped, //skipped, not-run, notrun, ignored
        Passed, // passed, success, pass
        Inconclusive, // warning, bad, inconclusive, 
        Error, // error,
        Failed, // failed, failure, fail, invalid
    }

    internal static class StatusExtensions
    {
        /// <summary>
        /// Convert a string into enum Status
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static Status ToStatus(this string str)
        {
            string trimmedString = str.Trim().ToLower();

            if (string.IsNullOrEmpty(trimmedString))
            {
                return Status.Unknown;
            }

            switch (trimmedString)
            {
                case "skipped":
                case "ignored":
                case "not-run":
                case "notrun":
                case "notexecuted":
                case "not-executed":
                    return Status.Skipped;

                case "pass":
                case "passed":
                case "success":
                    return Status.Passed;

                case "warning":
                case "bad":
                case "pending":
                case "inconclusive":
                case "notrunnable":
                case "disconnected":
                case "passedbutrunaborted":
                    return Status.Inconclusive;

                case "fail":
                case "failed":
                case "failure":
                case "invalid":
                    return Status.Failed;

                case "error":
                case "aborted":
                case "timeout":
                    return Status.Error;

                default:
                    return Status.Unknown;
            }
        }
    }
}
