// Copyright (c) 2015 Kristian Hellang (https://github.com/khellang/Scrutor)
// Modified by Toni Wenzel (https://github.com/twenzel/Demystify.Everything)

using codeessentials.Extensions.Logging.Demystifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                var instance = matcher.CreateInstance(provider);

                if (instance != null)
                    return instance;
            }

            // No matching constructor found so far, use default creation logic
            return ActivatorUtilities.CreateInstance(provider, instanceType, givenParameter);
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Count > 0;
        }
    }
}
