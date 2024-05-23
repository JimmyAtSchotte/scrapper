namespace ScrapperApp.SharedKernel;

public class Maybe<T>
{
    private readonly T? _value;

    private Maybe(T value)
    {
        _value = value;
    }
    
    private Maybe()
    {
    }

    public bool TryGetValue(out T output)
    {
        output = _value;

        return output is not null;
    }

    public static Maybe<T> WithValue(T value)
        => new Maybe<T>(value);
    
    public static Maybe<T> WithoutValue()
        => new Maybe<T>();
}