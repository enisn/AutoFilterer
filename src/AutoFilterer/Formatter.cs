using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace AutoFilterer;

public class Formatter<T>
{
    internal static readonly ConcurrentDictionary<Expression<Func<T, string>>, Func<T, string>> Cache = new();
        
    private readonly Expression<Func<T, string>> _expression;

    public Formatter(Expression<Func<T, string>> expression)
    {
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public Formatter<TParent> From<TParent>(Expression<Func<TParent, T>> map) => new(
        Expression.Lambda<Func<TParent, string>>(Expression.Invoke(_expression, map.Body),
        map.Parameters.First()));

    public static implicit operator Formatter<T>(Expression<Func<T, string>> expresssion) => new(expresssion);

    public static implicit operator Expression<Func<T, string>>(Formatter<T> formatter) => formatter._expression;

    public string Format(T obj) => Cache.GetOrAdd(_expression, e => e.Compile()).Invoke(obj);
}