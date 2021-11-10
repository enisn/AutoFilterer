#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace AutoFilterer.Attributes
{
    public class CollectionFilterAttribute : FilteringOptionsBaseAttribute
    {
        public CollectionFilterAttribute()
        {
        }

        public CollectionFilterAttribute(CollectionFilterType filterOption)
        {
            this.FilterOption = filterOption;
        }

        public CollectionFilterType FilterOption { get; set; }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            /// Possibilities
            /// * => []
            ///     {} => []
            ///     [] => []
            ///     p => []
            ///
            /// [] => *
            ///     [] => {}
            ///     [] => p

            // * => []
            if (typeof(ICollection).IsAssignableFrom(targetProperty.PropertyType))
            {
                if (value is IFilter filter) // {} => []
                {
                    var type = targetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                    var parameter = Expression.Parameter(type, "a");
                    var innerLambda = Expression.Lambda(filter.BuildExpression(type, body: parameter), parameter);
                    var prop = Expression.Property(expressionBody, targetProperty.Name);
                    var methodInfo = typeof(Enumerable).GetMethods().LastOrDefault(x => x.Name == FilterOption.ToString());
                    var method = methodInfo.MakeGenericMethod(type);

                    return Expression.Call(
                                        method: method,
                                        instance: null,
                                        arguments: new Expression[] { prop, innerLambda }
                        );
                }
                // p => []
                else if (value.GetType().IsPrimitive || value is string)
                {
                    var containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == nameof(Enumerable.Contains)).MakeGenericMethod(type);

                    return Expression.Call(
                                        method: containsMethod,
                                        arguments: new Expression[]
                                        {
                                                Expression.Property(expressionBody, targetProperty.Name),
                                                Expression.Constant(value)
                                        });
                }
                else // [] => []
                {

                }

            }

           
            else if (value is ICollection collection) // [] => *   (  [] => p , [] => {} , [] => []   )
            {
                if (collection.Count == 0)
                    return Expression.Constant(true);

              
                else // [] => *
                {
                    var type = targetProperty.PropertyType;
                    var prop = Expression.Property(expressionBody, targetProperty.Name);

                    // [] => p
                    if (targetProperty.PropertyType.IsPrimitive || targetProperty.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == nameof(Enumerable.Contains)).MakeGenericMethod(type);


                        return Expression.Call(
                                            method: containsMethod,
                                            arguments: new Expression[]
                                            {
                                                Expression.Constant(value),
                                                Expression.Property(expressionBody, targetProperty.Name)
                                            });
                    }
                    else // [] => {}
                    {
                        var methodInfo = typeof(Enumerable).GetMethods().LastOrDefault(x => x.Name == FilterOption.ToString());
                        var method = methodInfo.MakeGenericMethod(type);
                    }
                }



            }
            return expressionBody;
        }
    }
}
