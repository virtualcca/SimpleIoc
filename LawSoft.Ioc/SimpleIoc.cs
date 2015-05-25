using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using LawSoft.Ioc.Interface;

namespace LawSoft.Ioc
{
    /// <summary>
    /// A very simple IOC container with basic functionality needed to register and resolve
    /// instances. 
    /// </summary>
    public class SimpleIoc : ISimpleIoc
    {
        #region [ Static ]
        private static ISimpleIoc _default;

        /// <summary>
        /// The Ioc default's instance
        /// </summary>
        public static ISimpleIoc Default
        {
            get
            {
                if (_default == null)
                    _default = new SimpleIoc();
                return _default;
            }
        } 
        #endregion


        #region [ Field ]
        private readonly Dictionary<Type, ConstructorInfo> _constructorInfos = new Dictionary<Type, ConstructorInfo>();
        private readonly IDictionary<Type, object> _instanceInfo = new Dictionary<Type, object>();
        private readonly IDictionary<Type, Delegate> _factories = new Dictionary<Type, Delegate>();
        private readonly IDictionary<Type, Type> _typeMapInfos = new Dictionary<Type, Type>();
        private readonly object[] _emptyArguments = new object[0];

        private readonly object _syncLock = new object();
        #endregion [ Field ]

        #region [ Register ]
        /// <summary>
        /// Registers a given instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">The instance can not be null</exception>
        /// <exception cref="InvalidOperationException">This instance type already registered</exception>
        /// <exception cref="ArgumentException">Given type is not instance define type</exception>
        /// <param name="instance">The register instance</param>
        /// <param name="type">The instance must be inherit this type.</param>
        /// <returns></returns>
        public ISimpleIoc Register(object instance, Type type)
        {
            if (instance == null)
                throw new ArgumentNullException("Instance");
            if (!type.IsInstanceOfType(instance))
                throw new ArgumentException("Type");

            lock (_syncLock)
            {
                var classType = type;

                if (_instanceInfo.ContainsKey(classType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "There is already a instance registered for {0}.",
                            classType.FullName));
                }
                if (!_typeMapInfos.ContainsKey(classType))
                {
                    _typeMapInfos.Add(classType, null);
                }
                DoRegister(classType, instance);
            }
            return this;
        }

        /// <summary>
        /// Registers a given instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">The instance can not be null</exception>
        /// <exception cref="InvalidOperationException">This instance type already registered</exception>
        /// <typeparam name="TClass">The register instance.</typeparam>
        public ISimpleIoc Register<TClass>(TClass instance) where TClass : class
        {
            Register(instance, typeof(TClass));
            return this;
        }

        /// <summary>
        /// Registers a given instance for a given type.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        public ISimpleIoc Register<TClass>(Creator<TClass> factory) where TClass : class
        {
            Register(factory, false);
            return this;
        }

        /// <summary>
        /// Registers a given instance for a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public ISimpleIoc Register<TClass>(Creator<TClass> factory, bool createInstanceImmediately)
            where TClass : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            lock (_syncLock)
            {
                var classType = typeof(TClass);

                if (_factories.ContainsKey(classType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "There is already a factory registered for {0}.",
                            classType.FullName));
                }

                if (!_typeMapInfos.ContainsKey(classType))
                {
                    _typeMapInfos.Add(classType, null);
                }

                DoRegister(classType, factory);

                if (createInstanceImmediately)
                {
                    GetInstance<TClass>();
                }
                return this;
            }
        }

        /// <summary>
        /// Registers a given type for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        public ISimpleIoc Register<TInterface, TClass>()
           where TClass : class, TInterface
           where TInterface : class
        {
            Register<TInterface, TClass>(false);
            return this;
        }

        /// <summary>
        /// Registers a given type for a given interface with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public ISimpleIoc Register<TInterface, TClass>(bool createInstanceImmediately)
            where TClass : class, TInterface
            where TInterface : class
        {
            lock (_syncLock)
            {
                var interfaceType = typeof(TInterface);
                var classType = typeof(TClass);

                if (_typeMapInfos.ContainsKey(interfaceType))
                {
                    if (_typeMapInfos[interfaceType] != classType)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "There is already a class registered for {0}.",
                                interfaceType.FullName));
                    }
                    else
                    {
                        return this;
                    }
                }
                else
                {
                    _typeMapInfos.Add(interfaceType, classType);
                    _constructorInfos.Add(classType, GetConstructorInfo(classType));
                }

                Creator<TInterface> factory = MakeInstance<TInterface>;
                DoRegister(interfaceType, factory);

                if (createInstanceImmediately)
                    GetInstance<TInterface>();
                return this;
            }
        }



        #endregion [ Register ]

        #region [ Unregister ]
        /// <summary>
        /// Unregisters a class from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TClass">The class that must be removed.</typeparam>
        public void Unregister<TClass>()
            where TClass : class
        {
            lock (_syncLock)
            {
                var serviceType = typeof(TClass);
                Type resolveTo;

                if (_typeMapInfos.ContainsKey(serviceType))
                {
                    resolveTo = _typeMapInfos[serviceType] ?? serviceType;
                    _typeMapInfos.Remove(serviceType);
                }
                else
                {
                    resolveTo = serviceType;
                }

                if (_instanceInfo.ContainsKey(serviceType))
                {
                    _instanceInfo.Remove(serviceType);
                }

                if (_factories.ContainsKey(serviceType))
                {
                    _factories.Remove(serviceType);
                }

                if (_constructorInfos.ContainsKey(resolveTo))
                {
                    _constructorInfos.Remove(resolveTo);
                }
            }
        } 
        #endregion

        #region [ IsRegistered ]
        /// <summary>
        /// Checks whether the current type is already registered
        /// </summary>
        /// <typeparam name="TClass">The type whether registered</typeparam>
        /// <returns></returns>
        public bool IsRegistered<TClass>()
        {
            var classType = typeof(TClass);
            lock (_syncLock)
            {
                return _typeMapInfos.ContainsKey(classType);
            }
        }

        #endregion

        #region [ ContainsCreated ]
        /// <summary>
        /// Checks instance of a given class is already created in the container.
        /// </summary>
        /// <typeparam name="TClass">The class that is queried.</typeparam>
        /// <returns>True if at least on instance of the class is already created, false otherwise.</returns>
        public bool ContainsCreated<TClass>()
        {
            var type = typeof(TClass);
            if (_instanceInfo.ContainsKey(type))
                return true;
            return false;
        }
        #endregion

        #region [ GetInstance ]

        /// <summary>
        /// Looks up an object within the container, based on the design time Type
        /// </summary>
        /// <typeparam name="TInterface">The interface to lookup</typeparam>
        /// <returns>An instance of TInterface or null if not found</returns>
        public TInterface GetInstance<TInterface>()
        {
            return (TInterface)GetInstance(typeof(TInterface));
        }

        /// <summary>
        /// Looks up an object within the container based on the runtime name of the Type
        /// </summary>
        /// <param name="typeName">The type fullname of the contract to lookup</param>
        /// <returns>An instance of TContract or null if not found</returns>
        public object GetInstance(string typeName)
        {
            foreach (var type in _typeMapInfos.Keys)
            {
                if (type.FullName.Equals(typeName))
                {
                    return GetInstance(type);
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up an object within the container, based on the runtime Type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>An instance of TContract or null if not found</returns>
        public object GetInstance(Type serviceType)
        {
            return DoGetService(serviceType);

        }


        #endregion


        #region [ GetAllInstances ] 
        /// <summary>
        /// Provides a way to get all the created instances available in the
        /// cache. Calling this method auto-creates 
        /// instances for all registered classes.
        /// </summary>
        /// <returns>All the instances of registered.</returns>
        public IEnumerable<object> GetAllInstances()
        {
            var list = new List<object>();
            lock (_syncLock)
            {
                foreach (var type in _typeMapInfos)
                {
                    var instance = DoGetService(type.Key);
                    if (instance != null)
                        list.Add(instance);
                }
            }
            return list;

        }

        /// <summary>
        /// Provides a way to get all the created instance of a given base type available in the
        /// cache. Calling this method auto-creates instances for all registed classes for derived
        /// given class
        /// </summary>
        /// <typeparam name="TBaseClass"></typeparam>
        /// <returns></returns>
        public IEnumerable<TBaseClass> GetAllInstancesOnBase<TBaseClass>() where TBaseClass : class
        {
            var targetType = typeof(TBaseClass);
            var list = new List<TBaseClass>();
            lock (_syncLock)
            {
                foreach (var type in _typeMapInfos.Keys)
                {
                    if (targetType.IsAssignableFrom(type))
                    {
                        list.Add((TBaseClass)GetInstance(type));
                    }
                }
            }
            return list;
        }
        #endregion


        #region [ Private Method ]


        private object DoGetService(Type serviceType, bool cache = true)
        {
            lock (_syncLock)
            {
                if (!_instanceInfo.ContainsKey(serviceType))
                {
                    if (!_typeMapInfos.ContainsKey(serviceType))
                    {
                        //Not found in cache
                        return null;
                    }

                }
                else
                {
                    return _instanceInfo[serviceType];
                }

                object instance = null;

                if (_factories.ContainsKey(serviceType))
                {
                    if (_factories.ContainsKey(serviceType))
                    {
                        instance = _factories[serviceType].DynamicInvoke(null);
                    }
                    else
                    {
                        //Not found in cache
                        return null;
                    }
                }

                if (cache
                    && instance != null)
                {
                    _instanceInfo.Add(serviceType, instance);
                }

                return instance;
            }
        }

        private ConstructorInfo GetConstructorInfo(Type serviceType)
        {
            Type resolveTo;

            if (_typeMapInfos.ContainsKey(serviceType))
            {
                resolveTo = _typeMapInfos[serviceType] ?? serviceType;
            }
            else
            {
                resolveTo = serviceType;
            }

            var constructorInfos = resolveTo.GetConstructors();

            if (constructorInfos.Length > 1)
            {
                return constructorInfos[0];
            }

            if (constructorInfos.Length == 0
                || (constructorInfos.Length == 1
                    && !constructorInfos[0].IsPublic))
            {
                throw new Exception(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot register: No public constructor found in {0}.",
                        resolveTo.Name));
            }

            return constructorInfos[0];
        }

        private void DoRegister<TClass>(Type classType, Creator<TClass> factory)
        {
            if (_factories.ContainsKey(classType))
            {
                // The class is already registered, ignore and continue.
                return;
            }
            else
            {

                _factories.Add(classType, factory);
            }
        }

        private void DoRegister<TClass>(Type classType, TClass instance)
        {
            if (_instanceInfo.ContainsKey(classType))
            {
                // The class is already registered, ignore and continue.
                return;
            }
            else
            {
                _instanceInfo.Add(classType, instance);
            }
        }

        private TClass MakeInstance<TClass>()
        {
            var serviceType = typeof(TClass);

            var constructor = _constructorInfos.ContainsKey(serviceType)
                                  ? _constructorInfos[serviceType]
                                  : GetConstructorInfo(serviceType);

            var parameterInfos = constructor.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return (TClass)constructor.Invoke(_emptyArguments);
            }

            var parameters = new object[parameterInfos.Length];

            foreach (var parameterInfo in parameterInfos)
            {
                parameters[parameterInfo.Position] = DoGetService(parameterInfo.ParameterType);
            }

            return (TClass)constructor.Invoke(parameters);
        }


        #endregion


        /// <summary>
        /// Resets the instance in its original states. It will deletes all the
        /// registrations.
        /// </summary>
        public void Reset()
        {
            _typeMapInfos.Clear();
            _instanceInfo.Clear();
            _constructorInfos.Clear();
            _factories.Clear();
        }
    }
}
