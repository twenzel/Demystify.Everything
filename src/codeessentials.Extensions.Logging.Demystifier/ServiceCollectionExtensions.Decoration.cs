// Copyright (c) 2015 Kristian Hellang (https://github.com/khellang/Scrutor)

using codeessentials.Extensions.Logging.Demystifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This code is borrowed from Kristian Hellang author of https://github.com/khellang/Scrutor
    /// which creates this awesome function
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the specified type <typeparamref name="TDecorator"/>.        
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of the type <typeparamref name="TService"/> has been registered.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
            where TDecorator : TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.DecorateDescriptors(typeof(TService), x => x.Decorate(typeof(TDecorator)));
        }

        private static IServiceCollection DecorateDescriptors(this IServiceCollection services, Type serviceType, Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            if (services.TryDecorateDescriptors(serviceType, decorator))
            {
                return services;
            }

            throw new MissingTypeRegistrationException(serviceType);
        }

        private static ServiceDescriptor Decorate(this ServiceDescriptor descriptor, Type decoratorType)
        {
            return descriptor.WithFactory(provider => provider.CreateInstance(decoratorType, provider.GetInstance(descriptor)));
        }

        private static bool TryDecorateDescriptors(this IServiceCollection services, Type serviceType, Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            if (!services.TryGetDescriptors(serviceType, out var descriptors))
            {
                return false;
            }

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);

                // To avoid reordering descriptors, in case a specific order is expected.
                services.Insert(index, decorator(descriptor));

                services.Remove(descriptor);
            }

            return true;
        }

        private static ServiceDescriptor WithFactory(this ServiceDescriptor descriptor, Func<IServiceProvider, object> factory)
        {
            return ServiceDescriptor.Describe(descriptor.ServiceType, factory, descriptor.Lifetime);
        }

        private static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationType != null)
            {
                return provider.GetServiceOrCreateInstance(descriptor.ImplementationType);
            }

            return descriptor.ImplementationFactory(provider);
        }

        private static object GetServiceOrCreateInstance(this IServiceProvider provider, Type type)
        {            
            return provider.GetService(type) ?? CreateInstanceByBestMatchedConstructor(provider, type);           
        }

        private static object CreateInstance(this IServiceProvider provider, Type instanceType, object decoratedInstance)
        {
            if (decoratedInstance == null)
                return null;

            return CreateInstanceByBestMatchedConstructor(provider, instanceType, decoratedInstance);
        }

        private static object CreateInstanceByBestMatchedConstructor(this IServiceProvider provider, Type instanceType, params object[] givenParameter)
        {
            // get constructors with the most parameters first
            var constructors = instanceType
                    .GetTypeInfo()
                    .DeclaredConstructors
                    .Where(c => !c.IsStatic && c.IsPublic)
                    .Select(constructor => new ConstructorMatcher(constructor, givenParameter)).OrderByDescending(m => m.ParameterCount);


            foreach (var matcher in constructors)
            {
                try
                {
                    return matcher.CreateInstance(provider);
                }
                catch (InvalidOperationException)
                {
                    // constructor parameters could not be fulfilled
                    // go to next constructor
                }
            }

            return ActivatorUtilities.CreateInstance(provider, instanceType, givenParameter);
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }

        private class ConstructorMatcher
        {
            private readonly ConstructorInfo _constructor;

            private readonly ParameterInfo[] _parameters;

            private readonly object[] _parameterValues;

            private readonly bool[] _parameterValuesSet;

            public int ParameterCount { get => _parameters.Length; }

            public ConstructorMatcher(ConstructorInfo constructor)
            {
                _constructor = constructor;
                _parameters = this._constructor.GetParameters();
                _parameterValuesSet = new bool[_parameters.Length];
                _parameterValues = new object[_parameters.Length];
            }

            public ConstructorMatcher(ConstructorInfo constructor, params object[] givenParameters)
                :this(constructor)
            {
                SetGivenParameters(givenParameters);
            }             

            public int SetGivenParameters(params object[] givenParameters)
            {
                int num = 0;
                int result = 0;
                for (int i = 0; i != givenParameters.Length; i++)
                {
                    object obj = givenParameters[i];
                    TypeInfo typeInfo = obj?.GetType().GetTypeInfo();
                    bool flag = false;
                    int num2 = num;
                    while (!flag && num2 != _parameters.Length)
                    {
                        if (!_parameterValuesSet[num2] && this._parameters[num2].ParameterType.GetTypeInfo().IsAssignableFrom(typeInfo))
                        {
                            flag = true;
                            _parameterValuesSet[num2] = true;
                            _parameterValues[num2] = givenParameters[i];
                            if (num == num2)
                            {
                                num++;
                                if (num2 == i)
                                {
                                    result = num2;
                                }
                            }
                        }
                        num2++;
                    }

                    if (!flag)
                    {
                        return -1;
                    }
                }
                return result;
            }

            public object CreateInstance(IServiceProvider provider)
            {
                for (int i = 0; i != _parameters.Length; i++)
                {
                    if (!_parameterValuesSet[i])
                    {
                        object service = provider.GetService(_parameters[i].ParameterType);
                        if (service == null)
                        {
                            if (!TryGetDefaultValue(_parameters[i], out object obj))
                                throw new InvalidOperationException(string.Format("Unable to resolve service for type '{0}' while attempting to activate '{1}'.", this._parameters[i].ParameterType, this._constructor.DeclaringType));
                            
                            _parameterValues[i] = obj;
                        }
                        else
                        {
                            _parameterValues[i] = service;
                        }
                    }
                }
                try
                {
                    return _constructor.Invoke(_parameterValues);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    throw;
                }
            }

            public static bool TryGetDefaultValue(ParameterInfo parameter, out object defaultValue)
            {
                bool flag = true;
                defaultValue = null;
                bool flag2;
                try
                {
                    flag2 = parameter.HasDefaultValue;
                }
                catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
                {
                    flag2 = true;
                    flag = false;
                }
                if (flag2)
                {
                    if (flag)
                    {
                        defaultValue = parameter.DefaultValue;
                    }
                    if (defaultValue == null && parameter.ParameterType.IsValueType)
                    {
                        defaultValue = Activator.CreateInstance(parameter.ParameterType);
                    }
                }
                return flag2;
            }
        }
    }


}
