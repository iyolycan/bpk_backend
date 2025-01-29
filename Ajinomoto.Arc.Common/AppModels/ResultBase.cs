namespace Ajinomoto.Arc.Common.AppModels
{
    public class ResultBase
    {
        /// <summary>
        /// The Status of the Result
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The friendly message fire out
        /// </summary>
        public string Message { get; set; }
    }

    public class ResultBase<T>
    {
        /// <summary>
        /// The Status of the Result
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The friendly message fire out
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// idreturn
        /// </summary>
        public T Model { get; set; }
    }
}
