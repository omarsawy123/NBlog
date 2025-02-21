namespace Application.Shared
{
    public class Result
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public string? Error { get; }


        protected Result(bool isSuccess, int statusCode, string? error)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Error = error;
        }

        public static Result Success(int statusCode = 200)
        {
            return new Result(true, statusCode, null);
        }

        public static Result Failure(int statusCode, string error)
        {
            return new Result(false, statusCode, error);
        }

        public static Result Failure(int statusCode, IEnumerable<string> error)
        {
            string errors = string.Join(",", error);
            return new Result(false, statusCode, errors);
        }
    }


    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(bool isSuccess, int statusCode, string? error, T value) : base(isSuccess, statusCode, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value, int statusCode)
        {
            return new Result<T>(true, statusCode, null, value);
        }

    }
}
