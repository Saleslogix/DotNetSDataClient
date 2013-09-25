namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// Specifies the types of diagnosis code.
    /// </summary>
    public enum DiagnosisCode
    {
        /// <summary>
        /// Invalid URL syntax.
        /// </summary>
        BadUrlSyntax,

        /// <summary>
        /// Invalid Query Parameter.
        /// </summary>
        BadQueryParameter,

        /// <summary>
        /// Application does not exist.
        /// </summary>
        ApplicationNotFound,

        /// <summary>
        /// Application exists but is not available.
        /// </summary>
        ApplicationUnavailable,

        /// <summary>
        /// Dataset does not exist.
        /// </summary>
        DatasetNotFound,

        /// <summary>
        /// Dataset exists but is not available.
        /// </summary>
        DatasetUnavailable,

        /// <summary>
        /// Contract does not exist.
        /// </summary>
        ContractNotFound,

        /// <summary>
        /// Resource kind does not exist.
        /// </summary>
        ResourceKindNotFound,

        /// <summary>
        /// Invalid syntax for a where condition.
        /// </summary>
        BadWhereSyntax,

        /// <summary>
        /// Application specific diagnosis, detail is in the applicationCode element.
        /// </summary>
        ApplicationDiagnosis
    }
}