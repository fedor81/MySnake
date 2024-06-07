using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MySnake.Tools;

public class RandomMethodSelector
{
    private readonly List<Delegate> _methods = new();
    private readonly Random _random;
    private readonly object _methodsContainer;

    public RandomMethodSelector(Type methodsContainer)
    {
        // TODO
    }

    public RandomMethodSelector(object methodsContainer)
    {
        _random = new Random();
        _methodsContainer = methodsContainer;

        UpdateMethodsList();
    }

    // TODO: Не добавлять повторно методы которые уже есть в List, сделать public
    private void UpdateMethodsList()
    {
        var methods = _methodsContainer.GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.GetCustomAttributes(typeof(SelectableMethodAttribute), false).Length > 0);

        foreach (var method in methods)
        {
            var delegateType = GetDelegateType(method);
            var target = method.IsStatic ? null : _methodsContainer;
            _methods.Add(Delegate.CreateDelegate(delegateType, target, method));
        }
    }

    private static Type GetDelegateType(MethodInfo method)
    {
        var types = method.GetParameters().Select(p => p.ParameterType).ToList();

        if (method.ReturnType == typeof(void))
            return Expression.GetActionType(types.ToArray());

        types.Add(method.ReturnType);
        return Expression.GetFuncType(types.ToArray());
    }

    public object InvokeRandomMethod(params object[] args)
    {
        var randomMethod = GetRandomMethod();

        try
        {
            return randomMethod.DynamicInvoke(args);
        }
        catch (TargetParameterCountException)
        {
            throw new ArgumentException($"Incorrect number of arguments for the method {randomMethod.Method.Name}");
        }
        catch (TargetInvocationException ex)
        {
            throw new InvalidOperationException($"Error when calling the method {randomMethod.Method.Name}", ex);
        }
    }

    public Delegate GetRandomMethod()
    {
        if (_methods.Count == 0)
            throw new InvalidOperationException("There are no methods to call");

        return _methods[_random.Next(_methods.Count)];
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class SelectableMethodAttribute : Attribute
{
}