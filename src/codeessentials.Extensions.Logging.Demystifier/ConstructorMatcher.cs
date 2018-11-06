using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace codeessentials.Extensions.Logging.Demystifier
{
    /// <summary>
    /// Class to find and execute the best matching constructor.
    /// Uses fiven parameters as well as parameters resolved from IServiceProvider
    /// </summary>
    internal class ConstructorMatcher
    {
        private readonly ConstructorInfo _constructor;
        private readonly ParameterInfo[] _parameters;
        private readonly object[] _parameterValues;
        private readonly bool[] _parameterValuesSet;

        public int ParameterCount { get => _parameters.Length; }
        public bool GivenParametersMatched { get; }

        public ConstructorMatcher(ConstructorInfo constructor)
        {
            _constructor = constructor;
            _parameters = _constructor.GetParameters();
            _parameterValuesSet = new bool[_parameters.Length];
            _parameterValues = new object[_parameters.Length];
        }

        public ConstructorMatcher(ConstructorInfo constructor, params object[] givenParameters)
            : this(constructor)
        {
            GivenParametersMatched = SetGivenParameters(givenParameters);
        }

        /// <summary>
        /// Sets the given parameters to the internal list
        /// </summary>
        public bool SetGivenParameters(params object[] givenParameters)
        {
            int givenParemeterIndex = 0;

            for (int i = 0; i != givenParameters.Length; i++)
            {
                object obj = givenParameters[i];
                TypeInfo typeInfo = obj?.GetType().GetTypeInfo();
                bool parameterMatched = false;
                int constructorParameterIndex = givenParemeterIndex;

                while (!parameterMatched && constructorParameterIndex != _parameters.Length)
                {
                    if (!_parameterValuesSet[constructorParameterIndex] && _parameters[constructorParameterIndex].ParameterType.GetTypeInfo().IsAssignableFrom(typeInfo))
                    {
                        parameterMatched = true;
                        _parameterValuesSet[constructorParameterIndex] = true;
                        _parameterValues[constructorParameterIndex] = givenParameters[i];
                        if (givenParemeterIndex == constructorParameterIndex)
                        {
                            givenParemeterIndex++;
                        }
                    }
                    constructorParameterIndex++;
                }

                if (!parameterMatched)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates an instance using the given parameters and objects resolved from IServiceProvider
        /// </summary>
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
                            return null;

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
            bool hasDefaultValue;

            try
            {
                hasDefaultValue = parameter.HasDefaultValue;
            }
            catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
            {
                hasDefaultValue = true;
                flag = false;
            }

            if (hasDefaultValue)
            {
                if (flag)
                    defaultValue = parameter.DefaultValue;

                if (defaultValue == null && parameter.ParameterType.IsValueType)
                    defaultValue = Activator.CreateInstance(parameter.ParameterType);
            }

            return hasDefaultValue;
        }
    }
}
