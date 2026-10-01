using Vero.Shared.Exceptions;

namespace Vero.Shared.Extensions
{
    public static class TaskRetry
    {
        public static async Task<T> GetUntilAsync<T>(Func<Task<T?>> get, Predicate<T> validate, int repeat = 4, int wait = 1200)
        {
            var repeatCount = 0;
            while (true)
            {
                var result = await get();

                if (result is not null && validate(result))
                    return result;

                await Task.Delay(wait);

                if (++repeatCount > repeat)
                    break;
            }

            throw new TaskRetryNotExceptedValueException();
        }

        private sealed class TaskRetryNotExceptedValueException() : AppException("Brak wymaganej wartości!");
    }
}